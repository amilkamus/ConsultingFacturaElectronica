using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.CapaEntidad.RegistroComprobante
{
    public class En_DescuentoCargo
    {
        public bool Indicador { get; set; }
        public string CodigoMotivo { get; set; }

        public string Motivo { get; set; }
        public decimal MontoBase { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal Factor { get; set; }
    }
}
