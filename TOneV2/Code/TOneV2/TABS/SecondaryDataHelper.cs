using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TABS
{
    public partial class SecondaryDataHelper
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("LCR Filtering Rules");
        /// <summary>
        /// Get a connection for the default TABS database.
        /// </summary>
        /// <returns>A connection to the TABS database</returns>
        public static IDbConnection GetOpenConnection(bool execInitCommand)
        {
            IDbConnection connection = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings["SecondaryConnetionString"].ToString());
            connection.Open();
            //return connection;            
            //IDbConnection connection = DataConfiguration.Default.SessionFactory.ConnectionProvider.GetConnection();
            if (execInitCommand)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        @"set ANSI_NULLS ON 
                  set ANSI_PADDING ON 
                  set ANSI_WARNINGS ON 
                  set ARITHABORT ON 
                  set CONCAT_NULL_YIELDS_NULL ON 
                  set QUOTED_IDENTIFIER ON 
                  set NUMERIC_ROUNDABORT OFF";
                    command.ExecuteNonQuery();
                }
            }
            return connection;
        }
        // protected static int CommandTimeOut_User { get { return Helper.CurrentUser[Helper.UserOption.RouteOptionCount.ToString()]; } }
        public static IDbConnection GetOpenConnection()
        {
            return GetOpenConnection(true);
        }

        /// <summary>
        /// Create prameters for the specified command and parameter values supplied.
        /// The parameters will be named @P1, @P2, .... respectively.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameterValues"></param>
        public static void CreateCommandParameters(IDbCommand command, object[] parameterValues)
        {
            if (parameterValues != null)
            {
                int index = 1;
                foreach (object paramValue in parameterValues)
                {
                    IDbDataParameter param = command.CreateParameter();
                    param.ParameterName = "@P" + index;
                    if (paramValue == null)
                        param.Value = DBNull.Value;
                    else
                        param.Value = paramValue;
                    command.Parameters.Add(param);
                    index++;
                }
            }
        }

        /// <summary>
        /// Execute a query and return an auto-connection-close reader.
        /// The query can have parameters (@P1, @P2, ....) defined in the sql and values should be passed 
        /// in the parameterValues array
        /// </summary>
        /// <param name="sql">The SQL defining the sql-command to execute and return a reader</param>
        /// <param name="parameterValues">The parameter values. Should match exactly the number 
        /// of parameters defined in the query.</param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(string sql, params object[] parameterValues)
        {
            IDbConnection connection = GetOpenConnection();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = sql;
            CreateCommandParameters(command, parameterValues);
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }


        /// <summary>
        /// Execute a single row / column query and return its result as an object
        /// The query can have parameters (@P1, @P2, ....) defined in the sql and values should be passed 
        /// in the parameterValues array
        /// </summary>
        /// <param name="sql">The SQL</param>
        /// <param name="parameterValues">Parameters if any</param>
        /// <returns></returns>
        public static object ExecuteScalar(string sql, params object[] parameterValues)
        {
            object value = null;
            using (IDbConnection connection = GetOpenConnection())
            {
                IDbCommand command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandTimeout = 120;
                CreateCommandParameters(command, parameterValues);
                value = command.ExecuteScalar();
            }
            return value;
        }

        /// <summary>
        /// Fill a datatable from the given SQL statement and parameters
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static void GetDataTable(DataTable dataTable, string sql, params object[] parameterValues)
        {
            using (IDbConnection connection = GetOpenConnection())
            {
                IDbCommand command = connection.CreateCommand();
                System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand)command);
                command.CommandText = sql;
                command.CommandTimeout = 600;
                CreateCommandParameters(command, parameterValues);
                da.Fill(dataTable);
                command.Dispose();
                da.Dispose();
                connection.Close();
            }
        }

        /// <summary>
        /// Get a datatable from the given SQL statement and parameters
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, params object[] parameterValues)
        {
            DataTable dataTable = new DataTable();
            GetDataTable(dataTable, sql, parameterValues);
            return dataTable;
        }

        /// <summary>
        /// Get a dataset from the given SQL statement and parameters
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string sql, params object[] parameterValues)
        {
            DataSet dataSet = new DataSet();
            using (IDbConnection connection = GetOpenConnection())
            {
                IDbCommand command = connection.CreateCommand();
                System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand)command);
                command.CommandText = sql;
                command.CommandTimeout = 0;
                CreateCommandParameters(command, parameterValues);
                da.Fill(dataSet);
            }
            return dataSet;
        }
        public static DataSet GetDataSet(string sql, int Commandtimeout, params object[] parameterValues)
        {
            DataSet dataSet = new DataSet();
            using (IDbConnection connection = GetOpenConnection())
            {
                IDbCommand command = connection.CreateCommand();
                System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand)command);
                command.CommandText = sql;
                command.CommandTimeout = Commandtimeout;
                CreateCommandParameters(command, parameterValues);
                da.Fill(dataSet);
            }
            return dataSet;
        }
        /// <summary>
        /// Execute a non-reader Query; Update/Insert/Delete?
        /// </summary>
        /// <param name="sql">The sql that defines the command to execute. You can specify parameters (@P1, @P2, ...)</param>
        /// <param name="parameterValues">Null or the values of the parameters defined in the SQL</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteNonQuery(string sql, params object[] parameterValues)
        {
            int rowsAffected = 0;
            using (IDbConnection connection = GetOpenConnection())
            {
                IDbCommand command = connection.CreateCommand();
                command.CommandTimeout = 0;
                command.CommandText = sql;
                CreateCommandParameters(command, parameterValues);
                rowsAffected = command.ExecuteNonQuery();
            }
            return rowsAffected;
        }
        public static int ExecuteNonQuery(string sql, IDbConnection connection, NHibernate.ITransaction transaction, params object[] parameterValues)
        {
            int rowsAffected = 0;
            IDbCommand command = connection.CreateCommand();
            command.CommandTimeout = 0;
            command.CommandText = sql;
            transaction.Enlist(command);
            //command.Transaction = (IDbTransaction)transaction;
            CreateCommandParameters(command, parameterValues);
            rowsAffected = command.ExecuteNonQuery();

            return rowsAffected;
        }

        /// <summary>
        /// Create and add a parameter to the command 
        /// </summary>
        /// <returns>The added parameter</returns>
        public static IDbDataParameter AddParameter(IDbCommand command, string name, object value)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = name;
            param.Value = value;
            command.Parameters.Add(param);
            return param;
        }

        /// <summary>
        /// Create and add a parameter to the command 
        /// </summary>
        /// <returns>The added parameter</returns>
        public static IDbDataParameter AddParameter(IDbCommand command, string name, object value, DbType dbType)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = name;
            param.DbType = dbType;
            param.Value = value;
            command.Parameters.Add(param);
            return param;
        }

        /// <summary>
        /// filling the billing statistics 
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>       
        /// <param name="ex"></param>
        /// <returns></returns>
    }
}
