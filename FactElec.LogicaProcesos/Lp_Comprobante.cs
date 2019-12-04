using FactElec.CapaDatos;
using FactElec.CapaEntidad.RegistroComprobante;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.LogicaProceso
{
    public class Lp_Comprobante
    {
        public En_Emisor ObtenerEmisor(string numeroDocumentoIdentidad, ref string mensajeRetorno)
        {
            Da_Comprobante daComprobante = new Da_Comprobante();
            return daComprobante.ObtenerEmisor(numeroDocumentoIdentidad, ref mensajeRetorno);
        }
        public bool InsertarComprobante(En_ComprobanteElectronico comprobante, string nombreXML, byte[] archivoXML, string codigoHASH, string firma, ref string mensajeRetorno)
        {
            Da_Comprobante daComprobante = new Da_Comprobante();
            return daComprobante.InsertarComprobante(comprobante, nombreXML, archivoXML, codigoHASH, firma, ref mensajeRetorno);
        }
        public bool InsertarProgramacion(ref string mensajeRetorno)
        {
            Da_Comprobante daComprobante = new Da_Comprobante();
            return daComprobante.InsertarProgramacion(ref mensajeRetorno);
        }
    }
}
