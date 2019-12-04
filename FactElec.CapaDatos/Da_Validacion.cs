using FactElec.CapaEntidad.RegistroComprobante;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.CapaDatos
{
    public class Da_Validacion
    {
        public List<En_Validacion> ListarValidaciones()
        {
            List<En_Validacion> validaciones = new List<En_Validacion>();

            // Comprobante
            AgregarPropiedad(ref validaciones, "En_ComprobanteElectronico", "FechaEmision", Condicion.Mandatorio, TipoDato.Date, 0, "yyyy-MM-dd");
            AgregarPropiedad(ref validaciones, "En_ComprobanteElectronico", "HoraEmision", Condicion.Condicional, TipoDato.Date, 0, "HH:mm:ss");
            AgregarPropiedad(ref validaciones, "En_ComprobanteElectronico", "ImporteTotal", Condicion.Mandatorio, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteElectronico", "Moneda", Condicion.Mandatorio, TipoDato.String, 3, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteElectronico", "SerieNumero", Condicion.Mandatorio, TipoDato.String, 13, "[FB0-9][A-Z0-9]{3}-[0-9]{1,8}$");
            AgregarPropiedad(ref validaciones, "En_ComprobanteElectronico", "TipoComprobante", Condicion.Mandatorio, TipoDato.String, 2, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteElectronico", "TipoOperacion", Condicion.Mandatorio, TipoDato.String, 4, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteElectronico", "TotalCargo", Condicion.Condicional, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteElectronico", "TotalDescuento", Condicion.Condicional, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteElectronico", "TotalImpuesto", Condicion.Condicional, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteElectronico", "TotalPrecioVenta", Condicion.Condicional, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteElectronico", "TotalValorVenta", Condicion.Condicional, TipoDato.Decimal, 0, "");

            // ComprobanteDetalle            
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalle", "Cantidad", Condicion.Mandatorio, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalle", "Codigo", Condicion.Mandatorio, TipoDato.String, 30, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalle", "CodigoSunat", Condicion.Condicional, TipoDato.String, 8, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalle", "CodigoTipoPrecio", Condicion.Mandatorio, TipoDato.String, 2, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalle", "Descripcion", Condicion.Mandatorio, TipoDato.String, 500, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalle", "ImpuestoTotal", Condicion.Mandatorio, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalle", "Item", Condicion.Mandatorio, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalle", "MultiDescripcion", Condicion.Mandatorio, TipoDato.ArrayString, 500, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalle", "Total", Condicion.Mandatorio, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalle", "UnidadMedida", Condicion.Mandatorio, TipoDato.String, 3, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalle", "ValorVentaUnitarioIncIgv", Condicion.Mandatorio, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalle", "ValorVentaUnitario", Condicion.Mandatorio, TipoDato.Decimal, 0, "");

            // ComprobanteDetalleImpuestos
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalleImpuestos", "AfectacionIGV", Condicion.Mandatorio, TipoDato.String, 2, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalleImpuestos", "CodigoInternacionalTributo", Condicion.Mandatorio, TipoDato.String, 4, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalleImpuestos", "CodigoTributo", Condicion.Mandatorio, TipoDato.String, 4, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalleImpuestos", "MontoBase", Condicion.Mandatorio, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalleImpuestos", "MontoTotalImpuesto", Condicion.Mandatorio, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalleImpuestos", "NombreTributo", Condicion.Mandatorio, TipoDato.String, 6, "");
            AgregarPropiedad(ref validaciones, "En_ComprobanteDetalleImpuestos", "Porcentaje", Condicion.Condicional, TipoDato.Decimal, 0, "");

            // DescuentoCargo
            AgregarPropiedad(ref validaciones, "En_DescuentoCargo", "CodigoMotivo", Condicion.Condicional, TipoDato.String, 2, "");
            AgregarPropiedad(ref validaciones, "En_DescuentoCargo", "Factor", Condicion.Condicional, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_DescuentoCargo", "Indicador", Condicion.Condicional, TipoDato.Boolean, 0, "");
            AgregarPropiedad(ref validaciones, "En_DescuentoCargo", "MontoBase", Condicion.Condicional, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_DescuentoCargo", "MontoTotal", Condicion.Condicional, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_DescuentoCargo", "Motivo", Condicion.Condicional, TipoDato.String, 100, "");

            // DocumentoReferenciaDespacho
            AgregarPropiedad(ref validaciones, "En_DocumentoReferencia", "Fecha", Condicion.Condicional, TipoDato.Date, 0, "yyyy-MM-dd");
            AgregarPropiedad(ref validaciones, "En_DocumentoReferencia", "SerieNumero", Condicion.Condicional, TipoDato.String, 30, "[FB0-9][A-Z0-9]{3}-[0-9]{1,8}$");
            AgregarPropiedad(ref validaciones, "En_DocumentoReferencia", "TipoDocumento", Condicion.Condicional, TipoDato.String, 2, "");

            // DocumentoReferenciaNota
            AgregarPropiedad(ref validaciones, "En_DocumentoReferenciaNota", "Fecha", Condicion.Condicional, TipoDato.Date, 0, "yyyy-MM-dd");
            AgregarPropiedad(ref validaciones, "En_DocumentoReferenciaNota", "SerieNumero", Condicion.Condicional, TipoDato.String, 30, "[FB0-9][A-Z0-9]{3}-[0-9]{1,8}$");
            AgregarPropiedad(ref validaciones, "En_DocumentoReferenciaNota", "TipoDocumento", Condicion.Condicional, TipoDato.String, 2, "");

            // DocumentoSustentoNota
            AgregarPropiedad(ref validaciones, "En_DocumentoSustentoNota", "CodigoMotivoAnulacion", Condicion.Condicional, TipoDato.String, 10, "");
            AgregarPropiedad(ref validaciones, "En_DocumentoSustentoNota", "MotivoAnulacion", Condicion.Condicional, TipoDato.String, 500, "");
            AgregarPropiedad(ref validaciones, "En_DocumentoSustentoNota", "SerieNumero", Condicion.Condicional, TipoDato.String, 30, "[FB0-9][A-Z0-9]{3}-[0-9]{1,8}$");
            
            // Emisor
            AgregarPropiedad(ref validaciones, "En_Emisor", "CodigoDomicilioFiscal", Condicion.Mandatorio, TipoDato.String, 4, "");
            AgregarPropiedad(ref validaciones, "En_Emisor", "CodigoPais", Condicion.Condicional, TipoDato.String, 2, "");
            AgregarPropiedad(ref validaciones, "En_Emisor", "CodigoUbigeo", Condicion.Condicional, TipoDato.String, 6, "");
            AgregarPropiedad(ref validaciones, "En_Emisor", "Departamento", Condicion.Condicional, TipoDato.String, 30, "");
            AgregarPropiedad(ref validaciones, "En_Emisor", "Direccion", Condicion.Condicional, TipoDato.String, 200, "");
            AgregarPropiedad(ref validaciones, "En_Emisor", "Distrito", Condicion.Condicional, TipoDato.String, 30, "");
            AgregarPropiedad(ref validaciones, "En_Emisor", "NombreComercial", Condicion.Condicional, TipoDato.String, 1500, "");
            AgregarPropiedad(ref validaciones, "En_Emisor", "NumeroDocumentoIdentidad", Condicion.Mandatorio, TipoDato.String, 20, "");
            AgregarPropiedad(ref validaciones, "En_Emisor", "PaginaWeb", Condicion.Condicional, TipoDato.String, 200, "");
            AgregarPropiedad(ref validaciones, "En_Emisor", "Provincia", Condicion.Condicional, TipoDato.String, 30, "");
            AgregarPropiedad(ref validaciones, "En_Emisor", "RazonSocial", Condicion.Mandatorio, TipoDato.String, 1500, "");
            AgregarPropiedad(ref validaciones, "En_Emisor", "TipoDocumentoIdentidad", Condicion.Mandatorio, TipoDato.String, 1, "");
            AgregarPropiedad(ref validaciones, "En_Emisor", "Urbanizacion", Condicion.Condicional, TipoDato.String, 25, "");

            // Contacto
            AgregarPropiedad(ref validaciones, "En_Contacto", "Correo", Condicion.Condicional, TipoDato.String, 100, "");
            AgregarPropiedad(ref validaciones, "En_Contacto", "Nombre", Condicion.Condicional, TipoDato.String, 100, "");
            AgregarPropiedad(ref validaciones, "En_Contacto", "Telefono", Condicion.Condicional, TipoDato.String, 100, "");

            // Leyenda
            AgregarPropiedad(ref validaciones, "En_Leyenda", "Codigo", Condicion.Condicional, TipoDato.String, 4, "");
            AgregarPropiedad(ref validaciones, "En_Leyenda", "Valor", Condicion.Condicional, TipoDato.String, 200, "");

            // Receptor
            AgregarPropiedad(ref validaciones, "En_Receptor", "CodigoDomicilioFiscal", Condicion.Mandatorio, TipoDato.String, 4, "");
            AgregarPropiedad(ref validaciones, "En_Receptor", "CodigoPais", Condicion.Condicional, TipoDato.String, 2, "");
            AgregarPropiedad(ref validaciones, "En_Receptor", "CodigoUbigeo", Condicion.Condicional, TipoDato.String, 6, "");
            AgregarPropiedad(ref validaciones, "En_Receptor", "Departamento", Condicion.Condicional, TipoDato.String, 30, "");
            AgregarPropiedad(ref validaciones, "En_Receptor", "Direccion", Condicion.Condicional, TipoDato.String, 200, "");
            AgregarPropiedad(ref validaciones, "En_Receptor", "Distrito", Condicion.Condicional, TipoDato.String, 30, "");
            AgregarPropiedad(ref validaciones, "En_Receptor", "NombreComercial", Condicion.Condicional, TipoDato.String, 1500, "");
            AgregarPropiedad(ref validaciones, "En_Receptor", "NumeroDocumentoIdentidad", Condicion.Mandatorio, TipoDato.String, 20, "");
            AgregarPropiedad(ref validaciones, "En_Receptor", "PaginaWeb", Condicion.Condicional, TipoDato.String, 200, "");
            AgregarPropiedad(ref validaciones, "En_Receptor", "Provincia", Condicion.Condicional, TipoDato.String, 30, "");
            AgregarPropiedad(ref validaciones, "En_Receptor", "RazonSocial", Condicion.Mandatorio, TipoDato.String, 1500, "");
            AgregarPropiedad(ref validaciones, "En_Receptor", "TipoDocumentoIdentidad", Condicion.Mandatorio, TipoDato.String, 1, "");
            AgregarPropiedad(ref validaciones, "En_Receptor", "Urbanizacion", Condicion.Condicional, TipoDato.String, 25, "");

            // Gravado
            AgregarPropiedad(ref validaciones, "En_Gravado", "Total", Condicion.Mandatorio, TipoDato.Decimal, 0, "");

            // GrabadoIGV
            AgregarPropiedad(ref validaciones, "En_GrabadoIGV", "MontoBase", Condicion.Condicional, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_GrabadoIGV", "MontoTotalImpuesto", Condicion.Condicional, TipoDato.Decimal, 0, "");
            AgregarPropiedad(ref validaciones, "En_GrabadoIGV", "Porcentaje", Condicion.Condicional, TipoDato.Decimal, 0, "");

            return validaciones;
        }
        private void AgregarPropiedad(ref List<En_Validacion> propiedades, string clase, string propiedad, Condicion condicion, TipoDato tipoDeDato, int tamanioMax, string formato)
        {
            propiedades.Add(new En_Validacion(clase, propiedad, condicion, tipoDeDato, tamanioMax, formato));
        }
    }
}
