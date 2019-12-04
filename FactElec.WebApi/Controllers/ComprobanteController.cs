using FactElec.CapaEntidad.RegistroComprobante;
using FactElec.LogicaProceso;
using FactElec.LogicaProceso.RegistroComprobante;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

[assembly: FactElec.Log.Configuracion("WebApi")]
namespace FactElec.WebApi.Controllers
{
    public class ComprobanteController : ApiController
    {
        readonly log4net.ILog log = null;
        public ComprobanteController() => log = log4net.LogManager.GetLogger(typeof(ComprobanteController));

        // GET: api/Comprobantes


        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Comprobante/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Comprobante        
        public HttpResponseMessage Post([FromBody]En_ComprobanteElectronico comprobante)
        {
            try
            {
                log.Info("Inicio del proceso.");
                En_Respuesta oRespuesta = null;
                string mensajeRetorno = "";

                Lp_Comprobante lpComprobante = new Lp_Comprobante();
                comprobante.Emisor = lpComprobante.ObtenerEmisor(comprobante.Emisor.NumeroDocumentoIdentidad, ref mensajeRetorno);
                if (comprobante.Emisor == null)
                {
                    oRespuesta = new En_Respuesta
                    {
                        Codigo = "99",
                        Descripcion = mensajeRetorno
                    };
                    log.Info("Fin del proceso");
                    return Request.CreateResponse(HttpStatusCode.Created, oRespuesta);
                }

                bool esValido = true;
                oRespuesta = Lp_Validacion.ComprobanteValido(comprobante, ref esValido);
                
                if (esValido)
                {
                    if (comprobante.TipoComprobante.Trim() == "01" || comprobante.TipoComprobante.Trim() == "03")
                    {
                        Lp_Metodo_Invoice lp = new Lp_Metodo_Invoice();
                        oRespuesta = lp.RegistroComprobante(comprobante);
                    }
                    if (comprobante.TipoComprobante.Trim() == "07")
                    {
                        Lp_Metodo_CreditNote lp = new Lp_Metodo_CreditNote();
                        oRespuesta = lp.RegistroComprobante(comprobante);
                    }
                    if (comprobante.TipoComprobante.Trim() == "08")
                    {
                        Lp_Metodo_DebitNote lp = new Lp_Metodo_DebitNote();
                        oRespuesta = lp.RegistroComprobante(comprobante);
                    }
                }
                log.Info("Fin del proceso");
                return Request.CreateResponse(HttpStatusCode.Created, oRespuesta);
            }
            catch (Exception ex)
            {
                log.Error("Ocurrió un error.", ex);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
            finally
            {
                log.Info("Fin del proceso.");
            }
        }

        // PUT: api/Comprobante/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Comprobante/5
        public void Delete(int id)
        {
        }
    }
}
