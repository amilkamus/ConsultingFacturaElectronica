namespace FactElec.CapaEntidad.ListarComprobanteElectronicos
{
    public class En_SalidaListarComprobante
    {
        public long IdComprobante { get; set; }
        public string NumeroDocumentoIdentidad { get; set; }
        public string RazonSocial { get; set; }
        public string TipoComprobante { get; set; }
        public string SerieNumero { get; set; }
        public string FechaEmision { get; set; }
        public string Moneda { get; set; }
        public decimal TotalPrecioVenta { get; set; }
        public string Estado { get; set; }
        public string DescripcionEstado { get; set; }
        public int IdEstado { get; set; }
        public string ComprobanteReferenciado { get; set; }
        public decimal TotalImpuesto { get; set; }
        public decimal TotalValorVenta { get; set; }
        public decimal TotalDescuento { get; set; }
    }
}