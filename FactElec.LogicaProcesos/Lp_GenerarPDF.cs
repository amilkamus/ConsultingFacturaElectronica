using FactElec.CapaDatos.GenerarPDF;
using FactElec.CapaEntidad.GenerarPDF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.LogicaProceso
{
    public class Lp_GenerarPDF
    {
        readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Lp_GenerarPDF));
        public void ProcesarRepresentacionImpresa()
        {
            try
            {
                Da_Comprobante adComprobante = new Da_Comprobante();
                List<En_Archivo> listaComprobante = adComprobante.ComprobantesPendientesGenerarPdf();

                if (listaComprobante.Count > 0)
                {
                    log.InfoFormat("Se inicia la generación de PDFs, cantidad: {0}.", listaComprobante.Count());
                    Task[] taskArray = new Task[listaComprobante.Count];

                    int i = 0;
                    foreach (En_Archivo comprobante in listaComprobante)
                    {
                        En_Archivo comprobanteParam = comprobante;
                        taskArray[i] = Task.Factory.StartNew(() => GenerarPdf(comprobanteParam));
                        i += 1;
                    }
                    Task.WaitAll(taskArray.ToArray());
                    log.InfoFormat("Se ha terminado la generación de PDFs, cantidad: {0}.", listaComprobante.Count());
                }
                else
                {
                    log.Info("No hay PDFs pendientes de generación.");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message.ToString(), ex);
            }
        }
        public void InsertarRepresentacionImpresa(En_Archivo archivo)
        {
            try
            {
                Da_Comprobante adComprobante = new Da_Comprobante();
                adComprobante.InsertarRepresentacionImpresa(archivo);

            }
            catch (Exception ex)
            {
                log.Error(archivo.NombreXML + " " + ex.Message.ToString(), ex);
            }
        }
        public void GenerarPdf(En_Archivo comprobante)
        {
            string archivoXML = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temporal") + /*comprobante.Ruta*/  @"\" + comprobante.NombreXML;
            // if (!Directory.Exists(comprobante.Ruta)) Directory.CreateDirectory(comprobante.Ruta);

            try
            {
                if (!File.Exists(archivoXML))
                {
                    File.WriteAllBytes(archivoXML, comprobante.ArchivoXML);
                }
            }
            catch (Exception ex)
            {

                log.Error(String.Format("{0} Error : ", comprobante.NombreXML, ex.Message.ToString()));
                return;
            }

            if (comprobante.TipoComprobante == "01" || comprobante.TipoComprobante == "03")
            {
                Lp_Invoice oInvoice = new Lp_Invoice();
                oInvoice.GenerarInvoice(comprobante);
            }

            if (comprobante.TipoComprobante == "07")
            {
                Lp_CreditNote oInvoice = new Lp_CreditNote();
                oInvoice.GenerarCreditNote(comprobante);
            }

            if (comprobante.TipoComprobante == "08")
            {
                Lp_DebitNote oInvoice = new Lp_DebitNote();
                oInvoice.GenerarDebitNote(comprobante);
            }
        }
    }
}
