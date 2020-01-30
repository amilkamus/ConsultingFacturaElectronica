using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.CapaEntidad.SincronizarComprobante
{
    public class En_Archivo
    {
        public long IdComprobante { get; set; }
        public byte[] Archivo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
