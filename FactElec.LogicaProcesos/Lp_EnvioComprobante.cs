using FactElec.CapaDatos;
using FactElec.CapaDatos.EnvioComprobante;
using FactElec.CapaEntidad.EnvioComprobante;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FactElec.LogicaProceso
{
    public class Lp_EnvioComprobante
    {
        readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Lp_EnvioComprobante));
        public void ProcesarEnviarComprobantes()
        {
            try
            {
                Da_Comprobante adComprobante = new Da_Comprobante();
                List<En_Comprobante> comprobantes = adComprobante.ComprobantesPendientesDeEnvio();

                if (comprobantes.Count > 0)
                {
                    log.InfoFormat("Se inicia el envío de comprobantes, cantidad: {0}.", comprobantes.Count());

                    Task task = Task.Factory.StartNew(() =>
                    {
                        foreach (En_Comprobante comprobante in comprobantes)
                        {
                            En_Comprobante comprobanteParam = comprobante;
                            EnviarComprobante(comprobanteParam);
                        }
                    });

                    task.Wait();

                    log.InfoFormat("Se ha terminado el envío de los comprobantes, cantidad: {0}.", comprobantes.Count());
                }
                else
                {
                    log.Info("No hay comprobantes pendientes de envío.");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message.ToString(), ex);
            }
        }

        public void EnviarComprobante(En_Comprobante comprobante)
        {
            long idComprobante = comprobante.IdComprobante;
            Da_Archivo adArchivo = new Da_Archivo();
            Da_Comprobante adComprobante = new Da_Comprobante();
            En_Archivo archivo = adArchivo.ObtenerArchivoComprobante(idComprobante);
            string nombre = archivo.Nombre;
            byte[] contenido = archivo.Contenido;
            string carpetaTemporal = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temporal"); //adArchivo.ObtenerRutaTemporal("TempENVIO");
            string carpetaArchivo = Path.GetFileNameWithoutExtension(nombre);
            string rutaCarpetaXML = Path.Combine(carpetaTemporal, carpetaArchivo);
            string rutaArchivoXML = Path.Combine(rutaCarpetaXML, nombre);
            string rutaArchivoZIP = string.Concat(rutaCarpetaXML, ".zip");
            string nombreArchivoZIP = Path.GetFileName(rutaArchivoZIP);
            string nombreArchivoZipResponse = string.Concat("R-", nombreArchivoZIP);
            string rutaZipResponse = Path.Combine(carpetaTemporal, nombreArchivoZipResponse);

            CrearCarpeta(rutaCarpetaXML);
            CrearArchivo(rutaArchivoXML, contenido);
            Comprimir(rutaArchivoZIP, rutaCarpetaXML);
            EliminarCarpeta(rutaCarpetaXML);

            ServicePointManager.ServerCertificateValidationCallback = (snder, cert, chain, error) => true;

            byte[] archivoZip = File.ReadAllBytes(rutaArchivoZIP);
            wsSUNAT.sendBillRequest sendBill = new wsSUNAT.sendBillRequest();
            wsSUNAT.billServiceClient billService = new wsSUNAT.billServiceClient();

            try
            {
                billService.Open();
                byte[] archivoResponse = billService.sendBill(nombreArchivoZIP, archivoZip, "");
                adComprobante.InsertarCdrPendiente(idComprobante, archivoResponse);
                billService.Close();
                log.Info(string.Format("El comprobante {0}-{1} de la empresa emisora con ruc: {2} se procesó correctamente.",
                    comprobante.TipoComprobante, comprobante.SerieNumero, comprobante.RucEmisor));
            }
            catch (FaultException ex)
            {
                if (billService.State == CommunicationState.Opened)
                {
                    billService.Close();
                }
                string codigo = ex.Code.Name.ToLower().Replace("client.", "");
                string mensaje = ex.Message.ToString();
                int reintento = adComprobante.QuitarPendienteEnvio(idComprobante, codigo);
                string mensajeReintento = (reintento == 1) ? "Se dejará de reintentar el envío de éste comprobante." : "";

                // crear xml de excepción
                StringBuilder xmlExcepcion = new StringBuilder();
                xmlExcepcion.AppendLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>");
                xmlExcepcion.AppendLine("<excepcion>");
                xmlExcepcion.AppendFormat("<codigo>{0}</codigo>", codigo);
                xmlExcepcion.AppendFormat("<mensaje><![CDATA[{0}]]></mensaje>",mensaje);
                xmlExcepcion.AppendLine("</excepcion>");

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlExcepcion.ToString());

                string archivoExcepcion = Path.Combine(carpetaTemporal, string.Format("EXP-{0}-{1}{2}.xml", comprobante.RucEmisor, comprobante.TipoComprobante, comprobante.SerieNumero));
                doc.Save(archivoExcepcion);

                adComprobante.InsertarCdrPendiente(idComprobante, File.ReadAllBytes(archivoExcepcion));
                EliminarArchivo(archivoExcepcion);

                log.Error(string.Format("El comprobante {0}-{1} de la empresa emisora con ruc: {2} obtuvo el código de error \"{3}\" con mensaje \"{4}\". {5}",
                    comprobante.TipoComprobante, comprobante.SerieNumero, comprobante.RucEmisor, codigo, mensaje, mensajeReintento));
            }
            catch (Exception ex)
            {
                if (billService.State == CommunicationState.Opened)
                {
                    billService.Close();
                }
                log.Error(string.Format("El comprobante {0}-{1} de la empresa emisora con ruc: {2} obtuvo el error \"{3}\"",
                    comprobante.TipoComprobante, comprobante.SerieNumero, comprobante.RucEmisor, ex.Message.ToString()));
            }

            EliminarArchivo(rutaZipResponse);
            EliminarArchivo(rutaArchivoZIP);
        }

        private void CrearCarpeta(string rutaCarpeta)
        {
            if (Directory.Exists(rutaCarpeta)) Directory.Delete(rutaCarpeta);
            Directory.CreateDirectory(rutaCarpeta);
        }

        private void EliminarCarpeta(string rutaCarpeta)
        {
            if (Directory.Exists(rutaCarpeta)) Directory.Delete(rutaCarpeta, true);
        }

        private void CrearArchivo(string rutaArchivo, byte[] contenido)
        {
            if (File.Exists(rutaArchivo)) File.Delete(rutaArchivo);
            File.WriteAllBytes(rutaArchivo, contenido);
        }

        private void EliminarArchivo(string rutaArchivo)
        {
            if (File.Exists(rutaArchivo)) File.Delete(rutaArchivo);
        }

        private void Comprimir(string rutaArchivoZip, string rutaCarpetaXML)
        {
            FileStream fsOut = File.Create(rutaArchivoZip);
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);
            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression
            int folderOffset = rutaCarpetaXML.Length + (rutaCarpetaXML.EndsWith("\\") ? 0 : 1);

            ProcesoCompresion(rutaCarpetaXML, zipStream, folderOffset);

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        private void ProcesoCompresion(string path, ZipOutputStream zipStream, int folderOffset)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string filename in files)
            {
                FileInfo fi = new FileInfo(filename);
                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity
                newEntry.Size = fi.Length;
                zipStream.PutNextEntry(newEntry);
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                ProcesoCompresion(folder, zipStream, folderOffset);
            }
        }

    }
}
