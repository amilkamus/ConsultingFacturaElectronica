using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.CapaEntidad.ListarComprobanteElectronicos
{
    public class En_EntradaListarComprobante
    {
        public int Estado { get; set; }
        public string NumeroDocumentoIdentidadEmisor { get; set; }
        public string NumeroDocumentoIdentidadReceptor { get; set; }
        public string FechaInicial { get; set; }
        public string FechaFinal { get; set; }
        public string TipoComprobante { get; set; }
        public string SerieNumero { get; set; }
    }
}
