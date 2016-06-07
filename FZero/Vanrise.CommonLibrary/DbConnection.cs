using System;
using System.Data;
using System.Data.SqlClient;

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
    }
}
