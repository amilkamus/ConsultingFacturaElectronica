using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.CapaEntidad.RegistroComprobante
{
    public class En_ComprobanteDetalleImpuestos
    {
        public decimal MontoBase { get; set; }
        public decimal MontoTotalImpuesto { get; set; }
       
        public string  AfectacionIGV { get; set; }

        public decimal Porcentaje { get; set; }

        public string CodigoTributo { get; set; }

        public string NombreTributo { get; set; }

        public string CodigoInternacionalTributo { get; set; }
    }
}
