using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.CapaEntidad.ObtenerRepresentacionImpresa
{
    public class En_SalidaObtenerRI
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string NombreArchivo { get; set; }
        public byte[] ContenidoArchivo { get; set; }
    }
}
