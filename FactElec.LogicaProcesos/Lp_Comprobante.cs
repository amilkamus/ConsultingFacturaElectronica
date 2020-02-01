using FactElec.CapaDatos;
using FactElec.CapaEntidad.ObtenerRepresentacionImpresa;
using FactElec.CapaEntidad.RegistroComprobante;

namespace FactElec.LogicaProceso
{
    public class Lp_Comprobante
    {
        public En_SalidaObtenerRI ObtenerRepresentacionImpresa(En_EntradaObtenerRI entrada, ref string mensajeRetorno)
        {
            Da_Comprobante daComprobante = new Da_Comprobante();
            return daComprobante.ObtenerRepresentacionImpresa(entrada, ref mensajeRetorno);
        }
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
        public bool QuitarProgramacion(ref string mensajeRetorno)
        {
            Da_Comprobante daComprobante = new Da_Comprobante();
            return daComprobante.QuitarProgramacion(ref mensajeRetorno);
        }
    }
}
