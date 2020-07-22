using FactElec.CapaEntidad.RegistroComprobante;
using FactElec.LogicaProceso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FactElec.WebApi.Controllers
{
    public class ProgramadoController : ApiController
    {
        readonly log4net.ILog log = null;
        public ProgramadoController() => log = log4net.LogManager.GetLogger(typeof(ProgramadoController));

        // GET: api/Programado
        public HttpResponseMessage Get()
        {
            try
            {
                string mensajeRetorno = "";
                Lp_Comprobante lpComprobante = new Lp_Comprobante();
                bool resultado = lpComprobante.InsertarProgramacion(ref mensajeRetorno);

                if (resultado)
                {
                    // Enviar comprobantes
                    Lp_EnvioComprobante lpEnvioComprobante = new Lp_EnvioComprobante();
                    lpEnvioComprobante.ProcesarEnviarComprobantes();

                    // Sincronizar comprobantes
                    Lp_SincronizarComprobante lpSincronizarComprobante = new Lp_SincronizarComprobante();
                    lpSincronizarComprobante.ProcesarCDR();

                    // Generar PDF
                    Lp_GenerarPDF lpGenerarPDF = new Lp_GenerarPDF();
                    lpGenerarPDF.ProcesarRepresentacionImpresa();

                    // Enviar correo
                    Lp_EnvioCorreo lpEnvioCorreo = new Lp_EnvioCorreo();
                    lpEnvioCorreo.ProcesarRegistroCorreo();
                    //lpEnvioCorreo.ProcesarEnvioCorreo();

                    lpComprobante.QuitarProgramacion(ref mensajeRetorno);
                }

                En_Respuesta oRespuesta = new En_Respuesta();
                if (resultado) oRespuesta.Codigo = "0";
                else oRespuesta.Codigo = "99";

                oRespuesta.Descripcion = mensajeRetorno;
                return Request.CreateResponse(HttpStatusCode.Created, oRespuesta);
            }
            catch (Exception ex)
            {
                En_Respuesta oRespuesta = new En_Respuesta
                {
                    Codigo = "99",
                    Descripcion = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, oRespuesta);
            }
        }

        // GET: api/Programado/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Programado
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Programado/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Programado/5
        public void Delete(int id)
        {
        }
    }
}
