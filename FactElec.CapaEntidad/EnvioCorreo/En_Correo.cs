namespace FactElec.CapaEntidad.EnvioCorreo
{
    public class En_Correo
    {
        public long IdComprobante { get; set; }
        public string De { get; set; }
        public string Para { get; set; }
        public string Asunto { get; set; }
        public short Estado { get; set; }
        public string MensajeProceso { get; set; }
    }
}
