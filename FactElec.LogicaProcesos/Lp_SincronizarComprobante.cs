using System;
using System.Collections.Generic;
using System.Linq;
using FactElec.CapaEntidad.SincronizarComprobante;
using System.Threading.Tasks;
using FactElec.CapaDatos;
using System.IO;

namespace FactElec.LogicaProceso
{
    public class Lp_SincronizarComprobante
    {
        readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Lp_SincronizarComprobante));
        public List<En_Archivo> ObtenerRespuestaPendiente()
        {
            Da_Comprobante oDatos = new Da_Comprobante();
            return oDatos.ObtenerRespuestaPendiente();
        }
        public string RutaTemporalCdr(string codigo)
        {
            Da_Comprobante oDatos = new Da_Comprobante();
            return oDatos.RutaTemporalCdr(codigo);
        }

        public void ProcesarCDR()
        {
            List<En_Archivo> listaRespuesta = new List<En_Archivo>();
            listaRespuesta = ObtenerRespuestaPendiente();
            if (listaRespuesta.Count > 0)
            {
                log.InfoFormat("Se inicia la sincronización de CDRs, cantidad: {0}.", listaRespuesta.Count);

                Task task = Task.Factory.StartNew(() =>
                {
                    foreach (En_Archivo archivo in listaRespuesta)
                    {
                        if (archivo != null)
                        {
                            ExtraerCDR(archivo.IdComprobante, archivo.Archivo);
                        }
                    }
                });

                task.Wait();

                log.InfoFormat("Se ha terminado la sincronización de CDRs, cantidad: {0}.", listaRespuesta.Count);
            }
            else
            {
                log.Info("No hay CDRs pendientes de sincronización.");
            }
        }

        public Boolean ExtraerCDR(long Idcomprobante, byte[] archivoRespuesta)
        {

            log.Info("Extraer CDR" + Idcomprobante.ToString());
            En_Respuesta oRespuesta = new En_Respuesta();
            Lp_Utilitario oUtilitario = new Lp_Utilitario();

            string nombreArchivoRespuesta = String.Format("{0}{1}{2}{3}{4}{5}.zip", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond, Idcomprobante);

            try
            {
                string rutaTemporal = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temporal"); // RutaTemporalCdr("TempCDR");

                //if (!Directory.Exists(rutaTemporal)) Directory.CreateDirectory(rutaTemporal);

                File.WriteAllBytes(rutaTemporal + @"\" + nombreArchivoRespuesta, archivoRespuesta);

                string nombreArchivoDescomprimido = oUtilitario.Descomprimir(rutaTemporal, nombreArchivoRespuesta);
                oRespuesta = oUtilitario.LeerRespuestaXml(nombreArchivoDescomprimido);
                oRespuesta.Idcomprobante = Idcomprobante;
                oRespuesta.Archivo = archivoRespuesta;
                //guardar en base de datos
                Da_Comprobante oDatos = new Da_Comprobante();
                oDatos.RegistrarRespuestaSunat(oRespuesta);
                string archivoEliminar = rutaTemporal + @"\" + nombreArchivoDescomprimido;
                if (File.Exists(archivoEliminar))
                {
                    File.Delete(archivoEliminar);
                }
            }
            catch // (Exception ex)
            {
                //throw ex.Message;
            }
            return true;
        }
    }
}
