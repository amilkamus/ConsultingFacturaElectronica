using FactElec.CapaDatos.EnvioCorreo;
using FactElec.CapaEntidad.EnvioCorreo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FactElec.LogicaProceso
{
    public class Lp_EnvioCorreo
    {
        readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Lp_EnvioCorreo));
        public void ProcesarRegistroCorreo()
        {
            try
            {
                Da_Comprobante daComprobante = new Da_Comprobante();
                List<En_Comprobante> comprobantes = daComprobante.ComprobantesPendientesDeEnvio();

                if (comprobantes.Count > 0)
                {
                    log.InfoFormat("Se inicia el registro de correos, cantidad: {0}.", comprobantes.Count());
                    Task[] taskArray = new Task[comprobantes.Count];

                    int i = 0;
                    foreach (En_Comprobante comprobante in comprobantes)
                    {
                        En_Comprobante comprobanteParam = comprobante;
                        taskArray[i] = Task.Factory.StartNew(() => RegistrarCorreo(comprobanteParam));
                        i += 1;
                    }
                    Task.WaitAll(taskArray.ToArray());
                    log.InfoFormat("Se ha terminado el registro de los correos, cantidad: {0}.", comprobantes.Count());
                }
                else
                {
                    log.Info("No hay correos pendientes de generar.");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message.ToString(), ex);
            }
        }

        public void ProcesarEnvioCorreo()
        {
            try
            {
                Da_Correo daCorreo = new Da_Correo();
                List<En_Correo> correos = daCorreo.CorreosPendientesDeEnvio();
                Da_Comprobante daComprobante = new Da_Comprobante();
                string carpetaTemporal = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temporal"); // daComprobante.ObtenerRutaTemporal("TempCorreo");
                CrearCarpeta(carpetaTemporal);

                if (correos.Count > 0)
                {
                    log.InfoFormat("Se inicia el envío de correos, cantidad: {0}.", correos.Count());
                    Task[] taskArray = new Task[correos.Count];

                    int i = 0;
                    foreach (En_Correo correo in correos)
                    {
                        En_Correo correoParam = correo;
                        taskArray[i] = Task.Factory.StartNew(() => EnviarCorreo(carpetaTemporal, correoParam));
                        i += 1;
                    }
                    Task.WaitAll(taskArray.ToArray());
                    log.InfoFormat("Se ha terminado el envío de los correos, cantidad: {0}.", correos.Count());
                }
                else
                {
                    log.Info("No hay correos pendientes de enviar.");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message.ToString(), ex);
            }
        }

        private void EnviarCorreo(string carpetaTemporal, En_Correo correo)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            En_Correo enCorreo = null;
            Da_Correo daCorreo = new Da_Correo();

            try
            {
                Da_Archivo daArchivo = new Da_Archivo();
                En_Archivo archivo = daArchivo.ObtenerArchivoComprobante(correo.IdComprobante);
                string rutaXML = Path.Combine(carpetaTemporal, archivo.NombreXML);
                string rutaPDF = Path.Combine(carpetaTemporal, archivo.NombrePDF);
                string correoEmisor = ConfigurationManager.AppSettings["correoEmisor"];
                int puerto = int.Parse(ConfigurationManager.AppSettings["puerto"]);
                string host = ConfigurationManager.AppSettings["host"];
                string usuario = ConfigurationManager.AppSettings["usuario"];
                string clave = ConfigurationManager.AppSettings["clave"];
                string tipoComprobante = "";
                switch (archivo.TipoComprobante)
                {
                    case "01": tipoComprobante = "Factura"; break;
                    case "03": tipoComprobante = "Boleta"; break;
                    case "07": tipoComprobante = "Nota de crédito"; break;
                    case "08": tipoComprobante = "Nota de débito"; break;
                }
                string rutaPlantilla = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlantillaCorreo.txt");
                string cuerpoCorreo = File.ReadAllText(rutaPlantilla);
                cuerpoCorreo = string.Format(cuerpoCorreo, archivo.RazonSocial.ToUpper(),
                    tipoComprobante, archivo.SerieNumero, archivo.FechaEmision);

                CrearArchivo(rutaXML, archivo.ArchivoXML);
                CrearArchivo(rutaPDF, archivo.ArchivoPDF);

                message.From = new MailAddress(correoEmisor);
                message.To.Add(new MailAddress(correo.Para));
                message.Subject = correo.Asunto;
                message.IsBodyHtml = true;
                message.Body = cuerpoCorreo;
                message.Attachments.Add(new Attachment(rutaXML));
                message.Attachments.Add(new Attachment(rutaPDF));
                smtp.Port = puerto;
                smtp.Host = host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(usuario, clave);

                try
                {
                    smtp.Send(message);
                    message.Dispose();
                    smtp.Dispose();
                    enCorreo = new En_Correo()
                    {
                        IdComprobante = correo.IdComprobante,
                        Estado = 2,
                        MensajeProceso = "Correo enviado satisfactoriamente."
                    };
                    daCorreo.ActualizarEstadoComprobanteCorreo(enCorreo);
                    log.InfoFormat("Se envió el correo para el comprobante {0}-{1}.", archivo.TipoComprobante, archivo.SerieNumero);
                }
                catch (Exception ex)
                {
                    message.Dispose();
                    smtp.Dispose();
                    enCorreo = new En_Correo()
                    {
                        IdComprobante = correo.IdComprobante,
                        Estado = 3,
                        MensajeProceso = "Ocurrió un error en el envío. Detalle: " + ex.Message.ToString()
                    };
                    daCorreo.ActualizarEstadoComprobanteCorreo(enCorreo);
                    log.InfoFormat("Ocurrió un error al enviar el comprobante {0}-{1}.", archivo.TipoComprobante, archivo.SerieNumero);
                    throw;
                }

                EliminarArchivo(rutaXML);
                EliminarArchivo(rutaPDF);
            }
            catch (Exception ex)
            {
                string mensajeError = ex.Message.ToString();
                log.Error(mensajeError, ex);
            }
        }

        private void RegistrarCorreo(En_Comprobante comprobante)
        {
            try
            {
                string correoEmisor = ConfigurationManager.AppSettings["correoEmisor"];
                string tipoComprobante = "";
                switch (comprobante.TipoComprobante)
                {
                    case "01": tipoComprobante = "Factura"; break;
                    case "03": tipoComprobante = "Boleta"; break;
                    case "07": tipoComprobante = "Nota de crédito"; break;
                    case "08": tipoComprobante = "Nota de débito"; break;
                }
                string asunto = string.Format(ConfigurationManager.AppSettings["asunto"], tipoComprobante, comprobante.SerieNumero);
                Da_Correo daCorreo = new Da_Correo();
                En_Correo enCorreo = new En_Correo
                {
                    De = correoEmisor,
                    Para = comprobante.CorreoElectronico,
                    IdComprobante = comprobante.IdComprobante,
                    Asunto = asunto
                };

                daCorreo.InsertarComprobanteCorreo(enCorreo);
                log.InfoFormat("Se registró el correo para el comprobante {0}-{1} de la empresa emisora con ruc: {2}.",
                    comprobante.TipoComprobante, comprobante.SerieNumero, comprobante.RucEmisor);
            }
            catch (Exception ex)
            {
                string mensajeError = string.Format("Ocurrió un error en el registro del correo para el comprobante {0}-{1} de la empresa emisora con ruc: {2}.",
                    comprobante.TipoComprobante, comprobante.SerieNumero, comprobante.RucEmisor);
                log.Error(mensajeError, ex);
            }
        }

        private void CrearCarpeta(string rutaCarpeta)
        {
            if (!Directory.Exists(rutaCarpeta))
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
    }
}
