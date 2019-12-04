using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.CapaEntidad.RegistroComprobante
{
    public class En_ComprobanteDetalle
    {
        public decimal Item { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public decimal Total { get; set; }
        public decimal ValorVentaUnitarioIncIgv { get; set; }

        public string CodigoTipoPrecio { get; set; }

        public decimal ImpuestoTotal { get; set; }
        public decimal ValorVentaUnitario { get; set; }
        public List<En_ComprobanteDetalleImpuestos> ComprobanteDetalleImpuestos { get; set; }

        public string Descripcion { get; set; }
        public List<string> MultiDescripcion { get; set; }

        public string Codigo { get; set; }

        public string CodigoSunat { get; set; }

    }
}
