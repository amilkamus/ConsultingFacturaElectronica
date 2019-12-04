using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.CapaEntidad.RegistroComprobante
{
    public class En_Respuesta
    {
        public En_Respuesta() { }
        public En_Respuesta(string codigo, string descripcion)
        {
            Codigo = codigo;
            Descripcion = descripcion;
        }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }
}
