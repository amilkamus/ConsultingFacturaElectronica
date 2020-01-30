using FactElec.CapaEntidad.EnvioCorreo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FactElec.CapaDatos.EnvioCorreo
{
    public class Da_Comprobante
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

        public List<En_Comprobante> ComprobantesPendientesDeEnvio()
        {
            List<En_Comprobante> comprobantes = new List<En_Comprobante>();
            SqlConnection cn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("dbo.usp_ListarComprobantesEnvioCorreo", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            try
            {
                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    En_Comprobante comprobante = new En_Comprobante
                    {
                        RucEmisor = (string)(dr[0]),
                        IdComprobante = (long)dr[1],
                        TipoComprobante = (string)dr[2],
                        SerieNumero = (string)dr[3],
                        CorreoElectronico = (string)dr[4]
                    };
                    comprobantes.Add(comprobante);
                }

                cn.Close();

                return comprobantes;
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
