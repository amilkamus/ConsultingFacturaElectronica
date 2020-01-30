using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.CapaEntidad.GenerarPDF
{
    public class En_Archivo
    {
        public long IdComprobante { get; set; }
        public string NombreXML { get; set; }

        public string NombrePdf { get; set; }

        public string TipoComprobante { get; set; }
        public byte[] ArchivoXML { get; set; }

        public byte[] ArchivoPdf { get; set; }

        //public string Ruta { get; set; }

        public string Qr { get; set; }

        public string TextoDetraccion { get; set; }

    }
}
