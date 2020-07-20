using FactElec.CapaEntidad.ListarComprobanteElectronicos;
using FactElec.CapaEntidad.ObtenerArchivo;
using FactElec.CapaEntidad.RegistroComprobante;
using FactElec.LogicaProceso;
using FactElec.LogicaProceso.RegistroComprobante;
using System.Collections.Generic;

[assembly: FactElec.Log.Configuracion("WCFService")]
namespace FactElec.WebService
{
    public class Service1 : IService1
    {
        readonly log4net.ILog log = null;
        public Service1() => log = log4net.LogManager.GetLogger(typeof(Service1));

        public En_SalidaArchivo ObtenerDocumentoComprobante(long idComprobante)
        {
            log.Info("Inicio del proceso.");
            string mensajeRetorno = "";
            En_SalidaArchivo salida = new Lp_Comprobante().ObtenerDocumentoComprobante(idComprobante, ref mensajeRetorno);

            if (salida != null)
            {
                salida.Codigo = "0";
                salida.Descripcion = "Se realizó el proceso correctamente.";
            }
            else
            {
                salida = new En_SalidaArchivo();
                salida.Codigo = "1";

                if (string.IsNullOrEmpty(mensajeRetorno))
                {
                    salida.Descripcion = "No se encontró el documento XML en la base de datos";
                    log.Error(string.Format("{0} - {1}", salida.Codigo, salida.Descripcion));
                }
                else
                {
                    salida.Descripcion = mensajeRetorno;
                    log.Error(string.Format("{0} - {1}", salida.Codigo, salida.Descripcion));
                }
            }

            log.Info("Fin del proceso.");
            return salida;
        }

        public En_SalidaArchivo ObtenerRepresentacionImpresa(long idComprobante)
        {
            log.Info("Inicio del proceso.");
            string mensajeRetorno = "";
            En_SalidaArchivo salida = new Lp_Comprobante().ObtenerRepresentacionImpresa(idComprobante, ref mensajeRetorno);

            if (salida != null)
            {
                salida.Codigo = "0";
                salida.Descripcion = "Se realizó el proceso correctamente.";
            }
            else
            {
                salida = new En_SalidaArchivo();
                salida.Codigo = "1";

                if (string.IsNullOrEmpty(mensajeRetorno))
                {
                    salida.Descripcion = "No se encontró la representación impresa en la base de datos";
                    log.Error(string.Format("{0} - {1}", salida.Codigo, salida.Descripcion));
                }
                else
                {
                    salida.Descripcion = mensajeRetorno;
                    log.Error(string.Format("{0} - {1}", salida.Codigo, salida.Descripcion));
                }
            }

            log.Info("Fin del proceso.");
            return salida;
        }

        public En_SalidaArchivo ObtenerRespuestaComprobante(long idComprobante)
        {
            log.Info("Inicio del proceso.");
            string mensajeRetorno = "";
            En_SalidaArchivo salida = new Lp_Comprobante().ObtenerRespuestaComprobante(idComprobante, ref mensajeRetorno);

            if (salida != null)
            {
                salida.Codigo = "0";
                salida.Descripcion = "Se realizó el proceso correctamente.";
            }
            else
            {
                salida = new En_SalidaArchivo();
                salida.Codigo = "1";

                if (string.IsNullOrEmpty(mensajeRetorno))
                {
                    salida.Descripcion = "No se encontró la respuesta en la base de datos";
                    log.Error(string.Format("{0} - {1}", salida.Codigo, salida.Descripcion));
                }
                else
                {
                    salida.Descripcion = mensajeRetorno;
                    log.Error(string.Format("{0} - {1}", salida.Codigo, salida.Descripcion));
                }
            }

            log.Info("Fin del proceso.");
            return salida;
        }

        public List<En_SalidaListarComprobante> ListarComprobanteElectronicos(En_EntradaListarComprobante entrada)
        {
            log.Info("Inicio del proceso.");
            List<En_SalidaListarComprobante> comprobantes = new Lp_Comprobante().ListarComprobanteElectronicos(entrada);

            if (comprobantes == null || comprobantes.Count == 0)
            {
                log.ErrorFormat("No se encontró ningún comprobante para la empresa: {1}.", comprobantes.Count, entrada.NumeroDocumentoIdentidadEmisor);
            }
            else
            {
                log.InfoFormat("Se encontraron {0} comprobantes para la empresa: {1}.", comprobantes.Count, entrada.NumeroDocumentoIdentidadEmisor);
            }

            log.Info("Fin del proceso.");
            return comprobantes;
        }

        public En_Respuesta RegistroComprobante(En_ComprobanteElectronico Comprobante)
        {
            log.Info("Inicio del proceso.");
            En_Respuesta oRespuesta = null;
            string mensajeRetorno = "";

            Lp_Comprobante lpComprobante = new Lp_Comprobante();
            Comprobante.Emisor = lpComprobante.ObtenerEmisor(Comprobante.Emisor.NumeroDocumentoIdentidad, ref mensajeRetorno);
            if (Comprobante.Emisor == null)
            {
                oRespuesta = new En_Respuesta
                {
                    Codigo = "99",
                    Descripcion = mensajeRetorno
                };
                log.Info("Fin del proceso");
                return oRespuesta;
            }

            bool esValido = true;
            oRespuesta = Lp_Validacion.ComprobanteValido(Comprobante, ref esValido);

            if (esValido)
            {
                if (Comprobante.TipoComprobante.Trim() == "01" || Comprobante.TipoComprobante.Trim() == "03")
                {
                    Lp_Metodo_Invoice lp = new Lp_Metodo_Invoice();
                    oRespuesta = lp.RegistroComprobante(Comprobante);
                }
                if (Comprobante.TipoComprobante.Trim() == "07")
                {
                    Lp_Metodo_CreditNote lp = new Lp_Metodo_CreditNote();
                    oRespuesta = lp.RegistroComprobante(Comprobante);
                }
                if (Comprobante.TipoComprobante.Trim() == "08")
                {
                    Lp_Metodo_DebitNote lp = new Lp_Metodo_DebitNote();
                    oRespuesta = lp.RegistroComprobante(Comprobante);
                }
            }

            log.Info("Fin del proceso");
            return oRespuesta;
        }
    }
}
