using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.CapaEntidad.RegistroComprobante
{
    public class En_ComprobanteElectronico
    {
        public string SerieNumero { get; set; }
        public string FechaEmision { get; set; }
        public string FechaVencimiento { get; set; }
        public string HoraEmision { get; set; }
        public string Moneda { get; set; }
        public string TipoOperacion { get; set; }
        public string TipoComprobante { get; set; }
        public decimal TotalImpuesto { get; set; }
        public decimal TotalValorVenta { get; set; }
        public decimal TotalPrecioVenta { get; set; }
        public decimal ImporteTotal { get; set; }
        public En_Emisor Emisor = new En_Emisor();
        public En_Receptor Receptor = new En_Receptor();
        public En_MontosTotales MontoTotales = new En_MontosTotales();
        public List<En_DescuentoCargo> DescuentoCargo = new List<En_DescuentoCargo>();
        public decimal TotalCargo { get; set; }
        public decimal TotalDescuento { get; set; }
        public List<En_ComprobanteDetalle> ComprobanteDetalle = new List<En_ComprobanteDetalle>();
        public List<En_Leyenda> Leyenda { get; set; }
        public List<En_DocumentoReferencia> DocumentoReferenciaDespacho = new List<En_DocumentoReferencia>();
        public En_DocumentoSustentoNota DocumentoSustentoNota = new En_DocumentoSustentoNota();
        public List<En_DocumentoReferenciaNota> DocumentoReferenciaNota = new List<En_DocumentoReferenciaNota>();
        public string TextoDetraccion { get; set; }
    }
}
