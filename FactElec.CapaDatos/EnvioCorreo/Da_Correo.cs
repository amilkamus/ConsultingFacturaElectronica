using FactElec.CapaEntidad.EnvioCorreo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FactElec.CapaDatos.EnvioCorreo
{
    public class Da_Correo
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["cnx"].ConnectionString;

        public List<En_Correo> CorreosPendientesDeEnvio()
        {
            List<En_Correo> correos = new List<En_Correo>();
            SqlConnection cn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("dbo.usp_ListarCorreosPendientes", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            try
            {
                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    En_Correo correo = new En_Correo
                    {
                        IdComprobante = (long)dr[0],
                        De = (string)dr[1],
                        Para = (string)dr[2],
                        Asunto = (string)dr[3]
                    };
                    correos.Add(correo);
                }

                cn.Close();

                return correos;
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

        public void ActualizarEstadoComprobanteCorreo(En_Correo correo)
        {
            SqlConnection cn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("dbo.usp_ActualizarEstadoComprobanteCorreo", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@IdComprobante", SqlDbType = SqlDbType.BigInt, Value = correo.IdComprobante });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@Estado", SqlDbType = SqlDbType.SmallInt, Value = correo.Estado });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@MensajeProceso", SqlDbType = SqlDbType.VarChar, Size = 2000, Value = correo.MensajeProceso });

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

        public void InsertarComprobanteCorreo(En_Correo correo)
        {
            SqlConnection cn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("dbo.usp_RegistrarComprobanteCorreo", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@IdComprobante", SqlDbType = SqlDbType.BigInt, Value = correo.IdComprobante });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@De", SqlDbType = SqlDbType.VarChar, Size = 1000, Value = correo.De });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@Para", SqlDbType = SqlDbType.VarChar, Size = 1000, Value = correo.Para });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "@Asunto", SqlDbType = SqlDbType.VarChar, Size = 2000, Value = correo.Asunto });

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

    }
}
