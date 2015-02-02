using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.CommonLibrary
{
    public class DbConnection : IDisposable
    {
        public SqlConnection connection;
        private string connectionString;

        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }


        public DbConnection()
        {
            ConnectionString = Config.SQLConnectionString;
            connection = new SqlConnection(ConnectionString);
            OpenConnection();
        }
        public DbConnection(string newConnectionString)
        {
            ConnectionString = newConnectionString;
            connection = new SqlConnection(ConnectionString);
            OpenConnection();
        }

        public static bool isConnectionAvailable()
        {
            bool result;
            try
            {
                //SqlConnection connection = new SqlConnection(Manager.GetConnectionString(Config.ConnectionString, Config.UserID, Config.Password));
                SqlConnection connection = new SqlConnection(Config.SQLConnectionString);
                connection.Open();
                result = true;
                connection.Close();
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public bool OpenConnection()
        {
            try
            {
                if (this.connection.State == ConnectionState.Closed)
                    connection.Open();
            }
            catch (Exception err)
            {
                FileLogger.Write("Error In DbConnection.OpenConnection() ", err);
                return false;
            }
            return true;
        }

        public bool CloseConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public SqlDataReader GetDataReader(string query)
        {
            return GetDataReader(query, CommandType.Text);
        }

        public SqlDataReader GetDataReaderFromStoreProcedure(string procedureName)
        {
            return GetDataReader(procedureName, CommandType.StoredProcedure);
        }

        public SqlDataReader GetDataReader(string query, CommandType type)
        {
            SqlDataReader dr;
            SqlCommand command;
            try
            {
                command = new SqlCommand(query, connection);
                command.CommandType = type;
                dr = command.ExecuteReader();
                return dr;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in DbConnection.ReturnDataReader()", err);
                return null;
            }
        }

        public Object ReturnScalar(string query)
        {
            SqlCommand aCommand;
            try
            {
                aCommand = new SqlCommand(query, connection);
                aCommand.CommandType = CommandType.Text;
                return aCommand.ExecuteScalar();
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Connection.ReturnScalar()", err);
                return null;
            }
        }

        public DataTable ReturnDataTable(string query)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(query, connection);
            da.Fill(ds);
            return ds.Tables[0];
        }

        public DataTable ReturnDataTable(SqlCommand cmd)
        {
            DataSet ds = new DataSet();
            cmd.Connection = connection;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            return ds.Tables[0];
        }

        public DataSet ReturnDataSet(string query)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(query, connection);
                da.Fill(ds);
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in DataBaseClass.ReturnDataSet(query)", err);
            }
            return ds;
        }

        public int Excute(string query)
        {
            int rowsAffected = 0;
            try
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandType = CommandType.Text;
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in DbConnection.Excute(" + query + ")", err);
            }
            return rowsAffected;
        }

        public static DataTable ChangeDataReader(SqlDataReader dr)
        {
            DataTable dt = new DataTable();
            try
            {
                if (dr.HasRows)
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        dt.Columns.Add(dr.GetSchemaTable().Rows[i][0].ToString(), dr.GetFieldType(i));
                    }
                }
                while (dr.Read())
                {
                    try
                    {
                        DataRow drow = dt.NewRow();
                        for (int j = 0; j < dr.FieldCount; j++)
                        {
                            drow[j] = dr.GetValue(j).ToString();
                        }
                        dt.Rows.Add(drow);
                    }
                    catch (Exception err)
                    {
                        FileLogger.Write("Error in DbConnection.ChangeDataReader(SqlDataReader).While", err);
                    }
                }
                dr.Close();

            }
            catch (Exception err)
            {
                FileLogger.Write("Error in DbConnection.ChangeDataReader(SqlDataReader)", err);
            }
            return dt;
        }


        public static bool IsAvailable(string connectionString)
        {
            try
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                connection.Close();
            }
            catch (SqlException)
            {
                return false;
            }

            return true;
        }


        public SqlCommand CreateCommand()
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandTimeout = 0;
            return cmd;
        }

        public void Dispose()
        {
            CloseConnection();
            connection.Dispose();
        }
    }
}
