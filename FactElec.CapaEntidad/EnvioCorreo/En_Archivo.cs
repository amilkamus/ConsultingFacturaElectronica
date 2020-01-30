namespace FactElec.CapaEntidad.EnvioCorreo
{
    public class En_Archivo
    {
        public long IdComprobante { get; set; }
        public string TipoComprobante { get; set; }
        public string SerieNumero { get; set; }
        public string NombreXML { get; set; }
        public byte[] ArchivoXML { get; set; }
        public string NombrePDF { get; set; }
        public byte[] ArchivoPDF { get; set; }
        public string FechaEmision { get; set; }
        public string RazonSocial { get; set; }
    }
}
