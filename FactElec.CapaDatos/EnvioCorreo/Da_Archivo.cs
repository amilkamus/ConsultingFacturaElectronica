using FactElec.CapaEntidad.EnvioCorreo;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FactElec.CapaDatos.EnvioCorreo
{
    public class Da_Archivo
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["cnx"].ConnectionString;

        public En_Archivo ObtenerArchivoComprobante(long idComprobante)
        {
            En_Archivo archivo = null;
            SqlConnection cn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("dbo.usp_ObtenerPDFyXMLporComprobante", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@IdComprobante", SqlDbType = SqlDbType.BigInt, Value = idComprobante });

            try
            {
                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    archivo = new En_Archivo
                    {
                        IdComprobante = (long)dr[0],
                        TipoComprobante = (string)dr[1],
                        SerieNumero = (string)dr[2],
                        NombreXML = (string)dr[3],
                        ArchivoXML = (byte[])dr[4],
                        NombrePDF = (string)dr[5],
                        ArchivoPDF = (byte[])dr[6],
                        FechaEmision = (string)dr[7],
                        RazonSocial = (string)dr[8]
                    };
                }

                cn.Close();

                return archivo;
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
