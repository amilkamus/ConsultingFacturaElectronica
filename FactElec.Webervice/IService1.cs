using System.Collections.Generic;
using System.ServiceModel;
using FactElec.CapaEntidad.ListarComprobanteElectronicos;
using FactElec.CapaEntidad.ObtenerArchivo;
using FactElec.CapaEntidad.RegistroComprobante;

namespace FactElec.WebService
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        En_Respuesta RegistroComprobante(En_ComprobanteElectronico Comprobante);

        [OperationContract]
        En_SalidaArchivo ObtenerRepresentacionImpresa(long idComprobante);
        
        [OperationContract]
        En_SalidaArchivo ObtenerDocumentoComprobante(long idComprobante);
        
        [OperationContract]
        En_SalidaArchivo ObtenerRespuestaComprobante(long idComprobante);

        [OperationContract]
        List<En_SalidaListarComprobante> ListarComprobanteElectronicos(En_EntradaListarComprobante entrada);
    }

}
