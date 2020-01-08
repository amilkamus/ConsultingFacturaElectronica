using FactElec.CapaEntidad.ObtenerRepresentacionImpresa;
using FactElec.CapaEntidad.RegistroComprobante;
using FactElec.LogicaProceso;
using FactElec.LogicaProceso.RegistroComprobante;

[assembly: FactElec.Log.Configuracion("WCFService")]
namespace FactElec.WebService
{
    public class Service1 : IService1
    {
        readonly log4net.ILog log = null;
        public Service1() => log = log4net.LogManager.GetLogger(typeof(Service1));

        public En_SalidaObtenerRI ObtenerRepresentacionImpresa(En_EntradaObtenerRI entrada)
        {
            log.Info("Inicio del proceso.");
            string mensajeRetorno = "";
            En_SalidaObtenerRI salida;

            bool esValido = true;
            salida = Lp_Validacion.ValidarObtenerRepresentacionImpresa(entrada, ref esValido);

            if (esValido)
            {
                salida = new Lp_Comprobante().ObtenerRepresentacionImpresa(entrada, ref mensajeRetorno);

                if (salida != null)
                {
                    salida.Codigo = "0";
                    salida.Descripcion = "Se realizó el proceso correctamente.";
                }
                else
                {
                    if (string.IsNullOrEmpty(mensajeRetorno))
                    {
                        salida = new En_SalidaObtenerRI();
                        salida.Codigo = "6";
                        salida.Descripcion = "No se encontró el comprobante en la base de datos";
                        log.Error(string.Format("{0} - {1}", salida.Codigo, salida.Descripcion));
                    }
                    else
                    {
                        salida.Codigo = "7";
                        salida.Descripcion = mensajeRetorno;
                        log.Error(string.Format("{0} - {1}", salida.Codigo, salida.Descripcion));
                    }
                }
            }
            log.Info("Fin del proceso.");
            return salida;
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
