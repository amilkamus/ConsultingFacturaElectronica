using FactElec.CapaEntidad.RegistroComprobante;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactElec.CapaDatos
{
    public class Da_Comprobante
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["cnx"].ConnectionString;
        readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Da_Comprobante));

        public En_Emisor ObtenerEmisor(string numeroDocumentoIdentidad, ref string mensajeRetorno)
        {
            SqlConnection cn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("dbo.usp_ObtenerEmisor", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@numeroDocumentoIdentidad", SqlDbType = SqlDbType.VarChar, Size = 20, Value = numeroDocumentoIdentidad });

            try
            {
                En_Emisor emisor = null;

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (dr.Read())
                {
                    emisor = new En_Emisor
                    {
                        CodigoDomicilioFiscal = (dr.IsDBNull(dr.GetOrdinal("CodigoDomicilioFiscal"))) ? "" : dr.GetString(dr.GetOrdinal("CodigoDomicilioFiscal")),
                        CodigoPais = (dr.IsDBNull(dr.GetOrdinal("CodigoPais"))) ? "" : dr.GetString(dr.GetOrdinal("CodigoPais")),
                        CodigoUbigeo = (dr.IsDBNull(dr.GetOrdinal("CodigoUbigeo"))) ? "" : dr.GetString(dr.GetOrdinal("CodigoUbigeo")),
                        Contacto = new En_Contacto
                        {
                            Correo = (dr.IsDBNull(dr.GetOrdinal("ContactoCorreo"))) ? "" : dr.GetString(dr.GetOrdinal("ContactoCorreo")),
                            Nombre = (dr.IsDBNull(dr.GetOrdinal("ContactoNombre"))) ? "" : dr.GetString(dr.GetOrdinal("ContactoNombre")),
                            Telefono = (dr.IsDBNull(dr.GetOrdinal("ContactoTelefono"))) ? "" : dr.GetString(dr.GetOrdinal("ContactoTelefono"))
                        },
                        Departamento = (dr.IsDBNull(dr.GetOrdinal("Departamento"))) ? "" : dr.GetString(dr.GetOrdinal("Departamento")),
                        Direccion = (dr.IsDBNull(dr.GetOrdinal("Direccion"))) ? "" : dr.GetString(dr.GetOrdinal("Direccion")),
                        Distrito = (dr.IsDBNull(dr.GetOrdinal("Distrito"))) ? "" : dr.GetString(dr.GetOrdinal("Distrito")),
                        NombreComercial = (dr.IsDBNull(dr.GetOrdinal("NombreComercial"))) ? "" : dr.GetString(dr.GetOrdinal("NombreComercial")),
                        NumeroDocumentoIdentidad = (dr.IsDBNull(dr.GetOrdinal("NumeroDocumentoIdentidad"))) ? "" : dr.GetString(dr.GetOrdinal("NumeroDocumentoIdentidad")),
                        PaginaWeb = (dr.IsDBNull(dr.GetOrdinal("PaginaWeb"))) ? "" : dr.GetString(dr.GetOrdinal("PaginaWeb")),
                        Provincia = (dr.IsDBNull(dr.GetOrdinal("Provincia"))) ? "" : dr.GetString(dr.GetOrdinal("Provincia")),
                        RazonSocial = (dr.IsDBNull(dr.GetOrdinal("RazonSocial"))) ? "" : dr.GetString(dr.GetOrdinal("RazonSocial")),
                        TipoDocumentoIdentidad = (dr.IsDBNull(dr.GetOrdinal("TipoDocumentoIdentidad"))) ? "" : dr.GetString(dr.GetOrdinal("TipoDocumentoIdentidad")),
                        Urbanizacion = (dr.IsDBNull(dr.GetOrdinal("Urbanizacion"))) ? "" : dr.GetString(dr.GetOrdinal("Urbanizacion")),
                    };
                }

                cn.Close();
                return emisor;
            }
            catch (SqlException ex)
            {
                if (cn.State == ConnectionState.Open) { cn.Close(); }
                mensajeRetorno = ex.Message.ToString();
                return null;
            }
            catch (Exception ex)
            {
                if (cn.State == ConnectionState.Open) { cn.Close(); }
                mensajeRetorno = "Ocurrió un error al registrar el comprobante, revisar el log.";
                log.Error(mensajeRetorno, ex);
                return null;
            }
        }

        public bool InsertarComprobante(En_ComprobanteElectronico comprobante, string nombreXML, byte[] archivoXML, string codigoHASH, string firma, ref string mensajeRetorno)
        {
            StringBuilder firmaQR = new StringBuilder();
            string[] serieNumero = comprobante.SerieNumero.Split('-');

            firmaQR.Append(comprobante.Emisor.NumeroDocumentoIdentidad).Append('|');
            firmaQR.Append(comprobante.TipoComprobante).Append('|');
            firmaQR.Append(serieNumero[0]).Append('|');
            firmaQR.Append(serieNumero[1]).Append('|');
            firmaQR.Append(comprobante.TotalImpuesto.ToString()).Append('|');
            firmaQR.Append(comprobante.ImporteTotal).Append('|');
            firmaQR.Append(comprobante.FechaEmision).Append('|');
            firmaQR.Append(comprobante.Receptor.TipoDocumentoIdentidad).Append('|');
            firmaQR.Append(comprobante.Receptor.NumeroDocumentoIdentidad).Append('|');
            firmaQR.Append(codigoHASH).Append('|');
            firmaQR.Append(firma);

            SqlConnection cn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("dbo.usp_InsertarComprobante", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@NroDocumentoIdentidadEmisor", SqlDbType = SqlDbType.VarChar, Size = 30, Value = comprobante.Emisor.NumeroDocumentoIdentidad });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@TipoDocumentoIdentidad", SqlDbType = SqlDbType.VarChar, Size = 2, Value = comprobante.Receptor.TipoDocumentoIdentidad });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@NroDocumentoIdentidad", SqlDbType = SqlDbType.VarChar, Size = 30, Value = comprobante.Receptor.NumeroDocumentoIdentidad });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@RazonSocial", SqlDbType = SqlDbType.VarChar, Size = 500, Value = comprobante.Receptor.RazonSocial });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@TipoComprobante", SqlDbType = SqlDbType.VarChar, Size = 2, Value = comprobante.TipoComprobante });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@SerieNumero", SqlDbType = SqlDbType.VarChar, Size = 13, Value = comprobante.SerieNumero });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@FechaEmison", SqlDbType = SqlDbType.VarChar, Size = 10, Value = comprobante.FechaEmision });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@CorreoElectronico", SqlDbType = SqlDbType.VarChar, Size = 500, Value = comprobante.Receptor.Contacto.Correo });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@TotalImpuesto", SqlDbType = SqlDbType.Decimal, Value = comprobante.TotalImpuesto });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@TotalValorVenta", SqlDbType = SqlDbType.Decimal, Value = comprobante.TotalValorVenta });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@TotalPrecioVenta", SqlDbType = SqlDbType.Decimal, Value = comprobante.TotalPrecioVenta });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@TotalDescuento", SqlDbType = SqlDbType.Decimal, Value = comprobante.TotalDescuento });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@TotalCargo", SqlDbType = SqlDbType.Decimal, Value = comprobante.TotalCargo });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@ImporteTotal", SqlDbType = SqlDbType.Decimal, Value = comprobante.ImporteTotal });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@FechaVencimiento", SqlDbType = SqlDbType.VarChar, Size = 50, Value = comprobante.FechaVencimiento ?? "" });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@HoraEmision", SqlDbType = SqlDbType.VarChar, Size = 8, Value = comprobante.HoraEmision });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@Moneda", SqlDbType = SqlDbType.VarChar, Size = 5, Value = comprobante.Moneda });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@TipoOperacion", SqlDbType = SqlDbType.VarChar, Size = 4, Value = comprobante.TipoOperacion });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@NombreXML", SqlDbType = SqlDbType.VarChar, Size = 50, Value = nombreXML });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@ArchivoXML", SqlDbType = SqlDbType.VarBinary, Value = archivoXML });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@CodigoHash", SqlDbType = SqlDbType.VarChar, Value = codigoHASH });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@CodigoQR", SqlDbType = SqlDbType.VarChar, Value = firmaQR.ToString() });
            if (comprobante.MontoTotales != null && comprobante.MontoTotales.Gravado != null)
            {
                En_GrabadoIGV gravadoIGV = comprobante.MontoTotales.Gravado.GravadoIGV;
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@CodigoTributo1001", SqlDbType = SqlDbType.VarChar, Size = 4, Value = "1001" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@MontoOperaciones1001", SqlDbType = SqlDbType.Decimal, Value = gravadoIGV.MontoBase });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@MontoTotalImpuesto1001", SqlDbType = SqlDbType.Decimal, Value = gravadoIGV.MontoTotalImpuesto });
            }

            if (comprobante.DocumentoReferenciaNota != null && comprobante.DocumentoReferenciaNota.Count > 0)
            {
                DataTable tablaReferenciados = ToDataTable(comprobante.DocumentoReferenciaNota);
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@ComprobanteReferenciaNota", SqlDbType = SqlDbType.Structured, Value = tablaReferenciados });
            }
            if (comprobante.DocumentoSustentoNota != null)
            {
                En_DocumentoSustentoNota sustentoNota = comprobante.DocumentoSustentoNota;
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@SerieNumeroDocumentoSustento", SqlDbType = SqlDbType.VarChar, Size = 30, Value = sustentoNota.SerieNumero });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@CodigoAnulacionDocumentoSustento", SqlDbType = SqlDbType.VarChar, Size = 10, Value = sustentoNota.CodigoMotivoAnulacion });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@MotivoAnulacionDocumentoSustento", SqlDbType = SqlDbType.VarChar, Size = 500, Value = sustentoNota.MotivoAnulacion });
            }
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@TextoDetraccion", SqlDbType = SqlDbType.VarChar, Value = comprobante.TextoDetraccion });
            try
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
                mensajeRetorno = "Se registró el comprobante satisfactoriamente.";
                log.Info(mensajeRetorno);
                return true;
            }
            catch (SqlException ex)
            {
                if (cn.State == ConnectionState.Open) { cn.Close(); }
                mensajeRetorno = ex.Message.ToString();
                return false;
            }
            catch (Exception ex)
            {
                if (cn.State == ConnectionState.Open) { cn.Close(); }
                mensajeRetorno = "Ocurrió un error al registrar el comprobante, revisar el log.";
                log.Error(mensajeRetorno, ex);
                return false;
            }
        }

        public DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public bool InsertarProgramacion(ref string mensajeRetorno)
        {
            SqlConnection cn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("dbo.usp_InsertarProgramacion", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            try
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
                mensajeRetorno = "Se registró la programación satisfactoriamente.";
                return true;
            }
            catch (SqlException ex)
            {
                if (cn.State == ConnectionState.Open) { cn.Close(); }
                mensajeRetorno = ex.Message.ToString();
                return false;
            }
            catch (Exception ex)
            {
                if (cn.State == ConnectionState.Open) { cn.Close(); }
                mensajeRetorno = "Ocurrió un error al registrar el comprobante, excepción: " + ex.Message.ToString();
                return false;
            }
        }
    }
}
