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
        // GET: api/Programado
        public HttpResponseMessage Get()
        {
            string mensajeRetorno = "";
            Lp_Comprobante lpComprobante = new Lp_Comprobante();
            bool resultado = lpComprobante.InsertarProgramacion(ref mensajeRetorno);

            En_Respuesta oRespuesta = new En_Respuesta();
            if (resultado) oRespuesta.Codigo = "0";
            else oRespuesta.Codigo = "99";

            oRespuesta.Descripcion = mensajeRetorno;
            return Request.CreateResponse(HttpStatusCode.Created, oRespuesta);
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
