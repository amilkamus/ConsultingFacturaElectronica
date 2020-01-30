using FactElec.CapaEntidad.EnvioComprobante;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FactElec.CapaDatos.EnvioComprobante
{
    public class Da_Archivo
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["cnx"].ConnectionString;

        public string ObtenerRutaTemporal(string codigo)
        {

            string ruta = "";
            SqlConnection cn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("dbo.usp_ListaConfiguracion", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.VarChar, Size = 10, Value = codigo });

            try
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable tablaconfiguracion = new DataTable();
                da.Fill(tablaconfiguracion);
                ruta = tablaconfiguracion.Rows[0][1].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ruta;
        }

        public En_Archivo ObtenerArchivoComprobante(long idComprobante)
        {
            En_Archivo archivo = null;
            SqlConnection cn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("dbo.usp_ObtenerArchivoComprobante", cn)
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
                    archivo = new En_Archivo();
                    archivo.IdComprobante = (long)dr[0];
                    archivo.Nombre = (string)(dr[1]);
                    archivo.Contenido = (byte[])dr[2];
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
