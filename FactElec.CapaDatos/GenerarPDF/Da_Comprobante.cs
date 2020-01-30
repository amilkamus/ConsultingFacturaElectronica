using FactElec.CapaEntidad.GenerarPDF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FactElec.CapaDatos.GenerarPDF
{
    public class Da_Comprobante
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["cnx"].ConnectionString;

        public void InsertarRepresentacionImpresa(En_Archivo archivo)
        {
            SqlConnection cn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("sp_RepresentacionImpresaInsert", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@IdComprobante", SqlDbType = SqlDbType.BigInt, Value = archivo.IdComprobante });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@NombreRepresentacionImpresa", SqlDbType = SqlDbType.VarChar, Value = archivo.NombrePdf });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@Archivo", SqlDbType = SqlDbType.VarBinary, Value = archivo.ArchivoPdf });

            try
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            catch (SqlException ex)
            {
                if (cn.State == ConnectionState.Open) { cn.Close(); }
                throw ex;
            }
            catch (Exception ex)
            {
                if (cn.State == ConnectionState.Open) { cn.Close(); }
                throw ex;
            }
        }

        public List<En_Archivo> ComprobantesPendientesGenerarPdf()
        {
            List<En_Archivo> listaComprobantes = new List<En_Archivo>();
            SqlConnection cn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("sp_RepresentacionImpresaPendiente", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            try
            {
                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    En_Archivo archivo = new En_Archivo();
                    archivo.IdComprobante = (long)(dr[0]);
                    archivo.TipoComprobante = (string)dr[1];
                    archivo.ArchivoXML = (byte[])dr[2];
                    archivo.NombreXML = (string)dr[3];
                    archivo.Qr = (string)dr[4];
                    //archivo.Ruta = (string)dr[5];
                    archivo.TextoDetraccion = (string)dr[6];
                    listaComprobantes.Add(archivo);
                }

                cn.Close();

                return listaComprobantes;
            }
            catch (SqlException ex)
            {
                if (cn.State == ConnectionState.Open) { cn.Close(); }
                throw ex;
            }
            catch (Exception ex)
            {
                if (cn.State == ConnectionState.Open) { cn.Close(); }
                throw ex;
            }
        }
    }
}
