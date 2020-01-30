using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace FactElec.CapaDatos
{
    public class SqlHelper
    {

        #region " - GetConnection - "

        static string cadenaSql = ConfigurationManager.ConnectionStrings["conexSql"].ConnectionString;
        public static string GetConnectionString()
        {
            //EntidadLog oENConfigXml = ad_Comun.CargarConfiguracion();

            //if (oENConfigXml == null)
            //{
            //    return string.Empty;
            //}
            var connectionString = cadenaSql; //string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Connect TimeOut=500000;", oENConfigXml.Servidor, oENConfigXml.BaseDatos, oENConfigXml.Usuario, oENConfigXml.Contrasenia);

            return connectionString;
        }

        public static SqlConnection GetConnection()
        {
            try
            {
                var cnn = new SqlConnection(GetConnectionString());
                return cnn;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    throw new Exception("902" + "|" + "No se pudo conectar a la BD");
                }
                else
                {
                    throw;
                }

            }

        }

        #endregion

        #region " - ExecuteNonQuery - "

        public static int ExecuteNonQuery(string spName, List<SqlParameter> parametros)
        {
            var cn = GetConnection();

            var cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parametros.ToArray());

            try
            {
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception x)
            {
                throw new Exception(x.Message);
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                    cn.Dispose();
                }
                cmd.Dispose();
            }
        }

        public static int ExecuteNonQuery(string spName, List<SqlParameter> parametros, string paramSalida)
        {
            var cn = GetConnection();

            var cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parametros.ToArray());

            try
            {
                cn.Open();
                cmd.ExecuteNonQuery();

                return Convert.ToInt32(cmd.Parameters[paramSalida].Value);

            }
            catch (Exception x)
            {
                throw new Exception(x.Message);
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                    cn.Dispose();
                }
                cmd.Dispose();
            }
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, List<SqlParameter> parametros)
        {
            var cmd = new SqlCommand();
            cmd.Connection = transaction.Connection;
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = transaction;
            cmd.Parameters.AddRange(parametros.ToArray());

            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception x)
            {
                throw new Exception(x.Message);
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, List<SqlParameter> parametros, string paramSalida)
        {
            var cmd = new SqlCommand();
            cmd.Connection = transaction.Connection;
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = transaction;
            cmd.Parameters.AddRange(parametros.ToArray());

            try
            {
                cmd.ExecuteNonQuery();

                return Convert.ToInt32(cmd.Parameters[paramSalida].Value);
            }
            catch (Exception x)
            {
                throw new Exception(x.Message);
            }
            finally
            {
                cmd.Dispose();
            }
        }

        #endregion

        #region " - ExecuteScalar - "

        public static string ExecuteScalar(string spName, List<SqlParameter> parametros)
        {
            try
            {
                using (SqlConnection cn = GetConnection())
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(spName, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parametros.ToArray());
                        var rpta = cmd.ExecuteScalar();
                        return (rpta == null) ? string.Empty : rpta.ToString();
                    }
                }
            }
            catch //(Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region " - DataTable - "

        public static DataTable FillDataTable(string spName, List<SqlParameter> parametros)
        {
            var cn = GetConnection();

            var cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parametros.ToArray());

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();

            try
            {
                cn.Open();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                    cn.Dispose();
                }
                cmd.Dispose();
                da.Dispose();
            }
        }

        public static DataTable FillDataTable(SqlTransaction transaction, string spName, List<SqlParameter> parametros)
        {
            var cmd = new SqlCommand();
            cmd.Connection = transaction.Connection;
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = transaction;
            cmd.Parameters.AddRange(parametros.ToArray());

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();

            try
            {
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cmd.Dispose();
                da.Dispose();
            }
        }

        public static DataSet FillDataSet(string spName, List<SqlParameter> parametros)
        {
            var cn = GetConnection();

            var cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parametros.ToArray());

            var da = new SqlDataAdapter(cmd);
            var ds = new DataSet();

            try
            {
                cn.Open();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                    cn.Dispose();
                }
                cmd.Dispose();
                da.Dispose();
            }

        }

        #endregion

        #region " - ExecuteReader - "

        public static SqlDataReader ExecuteReader(string spName, List<SqlParameter> parametros)
        {
            var cn = GetConnection();

            var cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parametros.ToArray());

            SqlDataReader dr;

            try
            {
                cn.Open();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cmd.Dispose();
            }

            return dr;
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, List<SqlParameter> parametros)
        {
            var cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parametros.ToArray());

            SqlDataReader dr;

            try
            {
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cmd.Dispose();
            }

            return dr;
        }

        #endregion

        #region " - Metodos privados - "

        public static List<T> MapReaderToObject<T>(SqlDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (HasColumn(ref dr, prop.Name))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        protected static bool HasColumn(ref SqlDataReader reader, string columnName)
        {
            for (int i = 0; i <= reader.FieldCount - 1; i++)
            {
                if (reader.GetName(i).Equals(columnName))
                {
                    return !Convert.IsDBNull(reader[columnName]);
                }
            }

            return false;
        }

        public static List<T> ExecuteList<T>(string spName, List<SqlParameter> parametros)
        {
            SqlDataReader dr = null;
            var listaARegresar = new List<T>();
            try
            {
                var cn = GetConnection();

                var cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parametros.ToArray());

                cn.Open();
                dr = cmd.ExecuteReader();

                // Se invoca el método para obtener la lista de propiedades del tipo de objeto indicado
                // enviándole como parámetro una instancia del tipo genérico
                object objecto = Activator.CreateInstance<T>();

                // Obtiene una lista de campos que han hecho match
                var camposMatch = GetPropertyMatch(objecto, dr);

                // Se itera el darareader
                // y se leen los campos que han hecho match
                while (dr.Read())
                {
                    // Se crea una nueva instanacia en cada iteración
                    objecto = Activator.CreateInstance<T>();
                    foreach (string s in camposMatch)
                    {
                        PropertyInfo p = objecto.GetType().GetProperty(s);

                        if (dr.IsDBNull(dr.GetOrdinal(s))) continue;

                        switch (p.PropertyType.Name.ToLower())
                        {
                            case "boolean":
                                p.SetValue(objecto, dr.GetBoolean(dr.GetOrdinal(s)), null);
                                break;
                            case "byte":
                                p.SetValue(objecto, dr.GetByte(dr.GetOrdinal(s)), null);
                                break;
                            case "char":
                                p.SetValue(objecto, dr.GetString(dr.GetOrdinal(s)), null);
                                break;
                            case "decimal":
                                p.SetValue(objecto, dr.GetDecimal(dr.GetOrdinal(s)), null);
                                break;
                            case "double":
                                p.SetValue(objecto, dr.GetDouble(dr.GetOrdinal(s)), null);
                                break;
                            case "single":
                                p.SetValue(objecto, dr.GetFloat(dr.GetOrdinal(s)), null);
                                break;
                            case "int32":
                                p.SetValue(objecto, dr.GetInt32(dr.GetOrdinal(s)), null);
                                break;
                            case "int64":
                                p.SetValue(objecto, dr.GetInt64(dr.GetOrdinal(s)), null);
                                break;
                            case "sbyte":
                                p.SetValue(objecto, dr.GetByte(dr.GetOrdinal(s)), null);
                                break;
                            case "int16":
                                p.SetValue(objecto, dr.GetInt16(dr.GetOrdinal(s)), null);
                                break;
                            case "uint32":
                                p.SetValue(objecto, dr.GetInt32(dr.GetOrdinal(s)), null);
                                break;
                            case "uint64":
                                p.SetValue(objecto, dr.GetInt64(dr.GetOrdinal(s)), null);
                                break;
                            case "uint16":
                                p.SetValue(objecto, dr.GetInt16(dr.GetOrdinal(s)), null);
                                break;
                            case "string":
                                p.SetValue(objecto, dr[s].ToString(), null);
                                break;
                            case "datetime":
                                p.SetValue(objecto, dr.GetDateTime(dr.GetOrdinal(s)), null);
                                break;
                            case "timespan":
                                p.SetValue(objecto, dr.GetDateTime(dr.GetOrdinal(s)), null);
                                break;
                        }
                    }
                    listaARegresar.Add((T)objecto);
                }

                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                    cn.Dispose();
                }
                cmd.Dispose();
                dr.Close();
                dr.Dispose();


            }
            catch //(Exception ex)
            {
                //throw;
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                    dr.Close();
            }

            return listaARegresar;
        }

        private static List<string> GetPropertyMatch(object objecto, SqlDataReader dr)
        {
            List<string> lProp = GetGenericPropertyList(objecto);

            // Obtener una lista de nombres de campos del datareader
            List<string> nombrescamposdr = new List<string>();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                nombrescamposdr.Add(dr.GetName(i));
            }

            // En este punto, ya tenemos las 2 listas, la lista de propiedades del objeto Genérico
            // y la lista de campos que ha devuelto la ejecución de la consuta
            // Ahora, hay que hacer un match, para saber cuáles serán leídas del datareader
            List<string> camposMatch = new List<string>();
            foreach (string s in lProp)
            {
                foreach (string s1 in nombrescamposdr)
                {
                    if (s.ToLower() == s1.ToLower())
                    {
                        camposMatch.Add(s);
                        break;
                    }
                }
            }
            return camposMatch;
        }

        private static List<string> GetGenericPropertyList(object target)
        {
            List<string> result = new List<string>();

            foreach (MemberInfo mi in target.GetType().GetMembers())
            {
                if (mi.MemberType == MemberTypes.Property)
                {
                    PropertyInfo pi = mi as PropertyInfo;
                    if (pi != null)
                    {
                        result.Add(pi.Name);
                    }
                }
            }
            return result;
        }

        #endregion
    }
}
