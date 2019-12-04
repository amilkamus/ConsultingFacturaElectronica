using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.CapaEntidad.RegistroComprobante
{
    public class En_Emisor
    {
        public string TipoDocumentoIdentidad { get; set; }
        public string NumeroDocumentoIdentidad { get; set; }
        public string PaginaWeb { get; set; }
        public string NombreComercial { get; set; }
        public string RazonSocial { get; set; }
        public string Distrito { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string CodigoUbigeo { get; set; }
        public string CodigoDomicilioFiscal { get; set; }
        public string Urbanizacion { get; set; }
        public string Direccion { get; set; }
        public string CodigoPais { get; set; }
        public En_Contacto Contacto { get; set; }
    }
}
