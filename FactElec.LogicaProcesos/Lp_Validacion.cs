using FactElec.CapaDatos;
using FactElec.CapaEntidad.RegistroComprobante;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FactElec.LogicaProceso
{
    public class Lp_Validacion
    {
        public static En_Respuesta ComprobanteValido(En_ComprobanteElectronico comprobante, ref bool esValido)
        {
            En_Respuesta respuesta = new En_Respuesta();
            try
            {
                List<En_Validacion> validaciones = new Da_Validacion().ListarValidaciones();
                respuesta = ValidarEntidad(validaciones, comprobante, ref esValido);
                if (!esValido) return respuesta;

                if (comprobante.ComprobanteDetalle != null)
                {
                    foreach (En_ComprobanteDetalle detalle in comprobante.ComprobanteDetalle)
                    {
                        respuesta = ValidarEntidad(validaciones, detalle, ref esValido);
                        if (!esValido) return respuesta;

                        if (detalle.ComprobanteDetalleImpuestos != null)
                        {
                            foreach (En_ComprobanteDetalleImpuestos detalleImpuesto in detalle.ComprobanteDetalleImpuestos)
                            {
                                respuesta = ValidarEntidad(validaciones, detalleImpuesto, ref esValido);
                                if (!esValido) return respuesta;
                            }
                        }
                    }
                }

                if (comprobante.DescuentoCargo != null)
                {
                    foreach (En_DescuentoCargo descuentoCargo in comprobante.DescuentoCargo)
                    {
                        respuesta = ValidarEntidad(validaciones, descuentoCargo, ref esValido);
                        if (!esValido) return respuesta;
                    }
                }

                if (comprobante.DocumentoReferenciaDespacho != null)
                {
                    foreach (En_DocumentoReferencia documentoReferenciaDespacho in comprobante.DocumentoReferenciaDespacho)
                    {
                        respuesta = ValidarEntidad(validaciones, documentoReferenciaDespacho, ref esValido);
                        if (!esValido) return respuesta;
                    }
                }

                if (comprobante.DocumentoReferenciaNota != null)
                {
                    foreach (En_DocumentoReferenciaNota documentoReferenciaNota in comprobante.DocumentoReferenciaNota)
                    {
                        respuesta = ValidarEntidad(validaciones, documentoReferenciaNota, ref esValido);
                        if (!esValido) return respuesta;
                    }
                }

                if (comprobante.DocumentoSustentoNota != null)
                {
                    respuesta = ValidarEntidad(validaciones, comprobante.DocumentoSustentoNota, ref esValido);
                    if (!esValido) return respuesta;
                }

                if (comprobante.Emisor != null)
                {
                    respuesta = ValidarEntidad(validaciones, comprobante.Emisor, ref esValido);
                    if (!esValido) return respuesta;

                    if (comprobante.Emisor.Contacto != null)
                    {
                        respuesta = ValidarEntidad(validaciones, comprobante.Emisor.Contacto, ref esValido);
                        if (!esValido) return respuesta;
                    }
                }

                if (comprobante.Receptor != null)
                {
                    respuesta = ValidarEntidad(validaciones, comprobante.Receptor, ref esValido);
                    if (!esValido) return respuesta;

                    if (comprobante.Receptor.Contacto != null)
                    {
                        respuesta = ValidarEntidad(validaciones, comprobante.Receptor.Contacto, ref esValido);
                        if (!esValido) return respuesta;
                    }
                }

                if (comprobante.Leyenda != null)
                {
                    foreach (var leyenda in comprobante.Leyenda)
                    {
                        respuesta = ValidarEntidad(validaciones, leyenda, ref esValido);
                        if (!esValido) return respuesta;
                    }
                }

                if (comprobante.MontoTotales != null && comprobante.MontoTotales.Gravado != null)
                {
                    respuesta = ValidarEntidad(validaciones, comprobante.MontoTotales.Gravado, ref esValido);
                    if (!esValido) return respuesta;

                    if (comprobante.MontoTotales.Gravado.GravadoIGV != null)
                    {
                        respuesta = ValidarEntidad(validaciones, comprobante.MontoTotales.Gravado.GravadoIGV, ref esValido);
                        if (!esValido) return respuesta;
                    }
                }
            }
            catch (Exception ex)
            {
                esValido = false;
                respuesta.Codigo = "99";
                respuesta.Descripcion = ex.Message.ToString();
            }
            return respuesta;
        }

        private static En_Respuesta ValidarEntidad<T>(List<En_Validacion> validaciones, T entidad, ref bool esValido)
        {
            PropertyInfo[] propiedades = entidad.GetType().GetProperties();
            string nombreEntidad = entidad.GetType().Name;
            string nombrePropiedad = "";
            List<En_Validacion> validacionesFiltradas = (from v in validaciones
                                                         where v.Clase == nombreEntidad
                                                         select v).ToList();
            try
            {
                foreach (En_Validacion validacion in validacionesFiltradas)
                {
                    PropertyInfo propiedad = (from p in propiedades
                                              where p.Name == validacion.Propiedad
                                              select p).FirstOrDefault();

                    if (propiedad != null)
                    {
                        nombrePropiedad = propiedad.Name;
                        object valor = entidad.GetType().GetProperty(nombrePropiedad).GetValue(entidad, null);
                        switch (validacion.TipoDeDato)
                        {
                            case TipoDato.Boolean: break;
                            case TipoDato.Date:
                                {
                                    string valorString = (string)valor;
                                    if (!EsTextoVacio(validacion.Formato))
                                    {
                                        DateTime dt = new DateTime();
                                        CultureInfo ci = new CultureInfo("es-PE");
                                        if (!DateTime.TryParseExact(valorString, validacion.Formato, ci, DateTimeStyles.None, out dt))
                                        {
                                            esValido = false;
                                            return Respuesta("01", $"La propiedad {nombrePropiedad} de la entidad {nombreEntidad} no cumple con el formato \"{validacion.Formato}\".");
                                        }
                                    }
                                    break;
                                }
                            case TipoDato.Decimal:
                                {
                                    if (valor != null)
                                    {
                                        decimal valorDecimal = (decimal)valor;
                                        if (validacion.Condicion == Condicion.Mandatorio && valorDecimal <= 0)
                                        {
                                            esValido = false;
                                            return Respuesta("02", $"La propiedad {nombrePropiedad} de la entidad {nombreEntidad} es mandatoria por lo tanto debe ser mayor que cero.");
                                        }
                                        if (valorDecimal < 0)
                                        {
                                            esValido = false;
                                            return Respuesta("02", $"La propiedad {nombrePropiedad} de la entidad {nombreEntidad} no debe ser menor que cero.");
                                        }
                                    }
                                    break;
                                }
                            case TipoDato.String:
                                {
                                    string valorString = (string)valor;
                                    if (validacion.Condicion == Condicion.Mandatorio && EsTextoVacio(valorString))
                                    {
                                        esValido = false;
                                        return Respuesta("01", $"La propiedad {nombrePropiedad} de la entidad {nombreEntidad} es mandatoria.");
                                    }
                                    if (!EsTextoVacio(valorString))
                                    {
                                        if (valorString.Length > validacion.TamanioMax)
                                        {
                                            esValido = false;
                                            return Respuesta("01", $"La propiedad {nombrePropiedad} de la entidad {nombreEntidad} no debe se mayor de {validacion.TamanioMax} caracteres.");
                                        }
                                        if (!EsTextoVacio(validacion.Formato))
                                        {
                                            Regex regex = new Regex(validacion.Formato);
                                            Match match = regex.Match(valorString);
                                            if (!match.Success)
                                            {
                                                esValido = false;
                                                return Respuesta("01", $"La propiedad {nombrePropiedad} de la entidad {nombreEntidad} no cumple con el formato \"{validacion.Formato}\".");
                                            }
                                        }
                                    }
                                    break;
                                };
                            case TipoDato.ArrayString:
                                {
                                    List<string> valorArray = (List<string>)valor;
                                    if (validacion.Condicion == Condicion.Mandatorio)
                                    {
                                        if (valorArray == null || valorArray.Count == 0)
                                        {
                                            esValido = false;
                                            return Respuesta("01", $"La propiedad {nombrePropiedad} de la entidad {nombreEntidad} es mandatoria.");
                                        }
                                        foreach (string texto in valorArray)
                                        {
                                            if (EsTextoVacio(texto))
                                            {
                                                esValido = false;
                                                return Respuesta("01", $"La propiedad {nombrePropiedad} de la entidad {nombreEntidad} contiene un elemento vacío.");
                                            }
                                            if (texto.Length > validacion.TamanioMax)
                                            {
                                                esValido = false;
                                                return Respuesta("01", $"La propiedad {nombrePropiedad} de la entidad {nombreEntidad} contiene un elemento que excede los {validacion.TamanioMax} caracteres.");
                                            }
                                            if (!EsTextoVacio(validacion.Formato))
                                            {
                                                Regex regex = new Regex(validacion.Formato);
                                                Match match = regex.Match(texto);
                                                if (!match.Success)
                                                {
                                                    esValido = false;
                                                    return Respuesta("01", $"La propiedad {nombrePropiedad} de la entidad {nombreEntidad} contiene un elemento que no cumple con el formato \"{validacion.Formato}\".");
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                            default: throw new ArgumentException("No se ha definido el tipo de dato.");
                        }
                    }
                }
                esValido = true;
                return Respuesta("0", $"Entidad {nombreEntidad} validada correctamente.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocurrió un error en la propiedad {nombrePropiedad} de la entidad {nombreEntidad}, mensaje del error: {ex.Message.ToString()}");
            }
        }

        private static En_Respuesta Respuesta(string codigo, string descripcion) => new En_Respuesta(codigo, descripcion);

        private static bool EsTextoVacio(string texto)
        {
            if (string.IsNullOrEmpty(texto) || string.IsNullOrWhiteSpace(texto))
                return true;
            else
                return false;
        }
    }
}
