using FactElec.CapaDatos;
using FactElec.CapaEntidad.ListarComprobanteElectronicos;
using FactElec.CapaEntidad.ObtenerArchivo;
using FactElec.CapaEntidad.RegistroComprobante;
using System.Collections.Generic;

namespace FactElec.LogicaProceso
{
    public class Lp_Comprobante
    {
        public En_SalidaArchivo ObtenerRepresentacionImpresa(long idComprobante, ref string mensajeRetorno)
        {
            Da_Comprobante daComprobante = new Da_Comprobante();
            return daComprobante.ObtenerRepresentacionImpresa(idComprobante, ref mensajeRetorno);
        }

        public En_SalidaArchivo ObtenerDocumentoComprobante(long idComprobante, ref string mensajeRetorno)
        {
            Da_Comprobante daComprobante = new Da_Comprobante();
            return daComprobante.ObtenerDocumentoComprobante(idComprobante, ref mensajeRetorno);
        }

        public En_SalidaArchivo ObtenerRespuestaComprobante(long idComprobante, ref string mensajeRetorno)
        {
            Da_Comprobante daComprobante = new Da_Comprobante();
            return daComprobante.ObtenerRespuestaComprobante(idComprobante, ref mensajeRetorno);
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

        public List<En_SalidaListarComprobante> ListarComprobanteElectronicos(En_EntradaListarComprobante entrada)
        {
            Da_Comprobante daComprobante = new Da_Comprobante();
            return daComprobante.ListarComprobanteElectronicos(entrada);
        }
    }
}
