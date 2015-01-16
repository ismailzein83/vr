using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using NHibernate.Criterion;

namespace TABS
{
    public partial class DataHelper
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("LCR Filtering Rules");
        /// <summary>
        /// Get a connection for the default TABS database.
        /// </summary>
        /// <returns>A connection to the TABS database</returns>
        public static IDbConnection GetOpenConnection(bool execInitCommand)
        {
            IDbConnection connection = new System.Data.SqlClient.SqlConnection(DataConfiguration.Default.Properties["connection.connection_string"].ToString());
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

        public static DataTable GetDataTableCustomPaging(string sql,ParameterGenerator[] parameters, List<object> outputParams, int RecordCount)
        {
            using (IDbConnection connection = GetOpenConnection())
            {
                DataSet dataSet = new DataSet();
                IDbCommand command = connection.CreateCommand();
                System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand)command);
                command.CommandText = sql;
                command.CommandTimeout = 600;
                command.CommandType = CommandType.StoredProcedure;
                GenerateParameters(parameters.ToList<ParameterGenerator>(), command);
                da.Fill(dataSet);
                if (RecordCount > 0)
                {
                    DataTable dtParamerters = dataSet.Tables[0];
                    for (int i = 0; i < RecordCount; i++)
                        outputParams.Add(dtParamerters.Rows[0][i]);
                }
                command.Dispose();
                da.Dispose();
                connection.Close();
                return dataSet.Tables[1];
            }
        }

        public static DataSet GetDataSetCustomPaging(string sql, ParameterGenerator[] parameters, List<object> outputParams, int RecordCount)
        {
            using (IDbConnection connection = GetOpenConnection())
            {
                DataSet dataSet = new DataSet();
                IDbCommand command = connection.CreateCommand();
                System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand)command);
                command.CommandText = sql;
                command.CommandTimeout = 600;
                command.CommandType = CommandType.StoredProcedure;
                GenerateParameters(parameters.ToList<ParameterGenerator>(), command);
                da.Fill(dataSet);
                if (RecordCount > 0)
                {
                    DataTable dtParamerters = dataSet.Tables[0];
                    for (int i = 0; i < RecordCount; i++)
                        outputParams.Add(dtParamerters.Rows[0][i]);
                }
                command.Dispose();
                da.Dispose();
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// Get a connection string for the default TABS database.
        /// </summary>
        /// <returns>A connection string to the TABS database</returns>
        public static string GetOpenedConnectionString(string Key)
        {
            return DataConfiguration.Default.Properties[Key].ToString();
        }

        public static IDbConnection GetOpenConnection()
        {
            return GetOpenConnection(true);
        }

        public class ParameterGenerator
        {
            public string Name { get; set; }
            public object Value { get; set; }
            public ParameterDirection Direction { get; set; }
        }

        public static void GenerateParameters(List<ParameterGenerator> Parameters, IDbCommand command)
        {

            foreach (ParameterGenerator key in Parameters)
            {
                IDbDataParameter p = command.CreateParameter();
                p.ParameterName = key.Name;
                p.Direction = key.Direction;
                p.Value = key.Value == null ? DBNull.Value : key.Value;
                command.Parameters.Add(p);
            }
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
        public static System.Data.IDataReader ExecuteReader(string sql)
        {
            IDbConnection connection = GetOpenConnection();
            if (connection.State == ConnectionState.Closed) connection.Open();
            System.Data.IDbCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.Connection = connection;
            System.Data.IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return reader;
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

        public static DataTable GetDataTable(string sql, ParameterGenerator[] parameters, out Dictionary<string, object> outputParams)
        {
            using (IDbConnection connection = GetOpenConnection())
            {
                DataTable dataTable = new DataTable();
                IDbCommand command = connection.CreateCommand();
                System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand)command);
                command.CommandText = sql;
                command.CommandTimeout = 600;
                command.CommandType = CommandType.StoredProcedure;
                GenerateParameters(parameters.ToList<ParameterGenerator>(), command);
                da.Fill(dataTable);
                command.ExecuteNonQuery();
                outputParams = new Dictionary<string, object>();
                foreach (IDbDataParameter param in command.Parameters)
                {
                    if (param.Direction == ParameterDirection.Output)
                    {
                        outputParams[param.ParameterName] = param.Value;
                    }
                }
                command.Dispose();
                da.Dispose();
                connection.Close();
                return dataTable;

            }

        }

        public static DataSet GetDataSet(string sql, ParameterGenerator[] parameters, out Dictionary<string, object> outputParams)
        {
            DataSet dataSet = new DataSet();
            using (IDbConnection connection = GetOpenConnection())
            {
                IDbCommand command = connection.CreateCommand();
                System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand)command);
                command.CommandText = sql;
                command.CommandTimeout = 0;
                command.CommandType = CommandType.StoredProcedure;
                GenerateParameters(parameters.ToList<ParameterGenerator>(), command);
                da.Fill(dataSet);
                command.ExecuteNonQuery();
                outputParams = new Dictionary<string, object>();
                foreach (IDbDataParameter param in command.Parameters)
                {
                    if (param.Direction == ParameterDirection.Output)
                        outputParams[param.ParameterName] = param.Value;
                }
                command.Dispose();
                dataSet.Dispose();
                connection.Close();

            }

            return dataSet;
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
        public static bool BuildBillingStats(DateTime Day, out Exception ex)
        {
            IDbConnection connection = GetOpenConnection();
            IDbCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "bp_BuildBillingStats";

            IDbDataParameter paramDay = command.CreateParameter();
            paramDay.ParameterName = "@Day";
            paramDay.Value = Day;
            command.Parameters.Add(paramDay);


            try
            {
                command.ExecuteNonQuery();
                ex = null;
                return true;
            }
            catch (Exception e)
            {
                ex = e;
                return false;
            }

        }
        /// <summary>
        /// Producing a new invoice for a selected Customer between FromDate and ToDate
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public static bool SupplierInvoiceIssue(string SupplierID, CustomTimeZoneInfo timeInfo, DateTime FromDate, DateTime ToDate, DateTime IssueDate, DateTime DueDate, int UserID, out Exception ex)
        {
            short timeshift = (short)timeInfo.BaseUtcOffset;
            IDbConnection connection = GetOpenConnection();
            IDbCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = timeshift == 0 ? "bp_CreateSupplierInvoiceZeroShift" : "bp_CreateSupplierInvoice";
            command.CommandTimeout = 0;

            IDbDataParameter paramSupplierID = command.CreateParameter();
            paramSupplierID.ParameterName = "@SupplierID";
            paramSupplierID.Value = SupplierID;
            command.Parameters.Add(paramSupplierID);

            if (timeshift != 0)
            {
                IDbDataParameter paramGmtTimeShift = command.CreateParameter();
                paramGmtTimeShift.ParameterName = "@GMTShifttime";
                paramGmtTimeShift.Value = timeInfo.BaseUtcOffset;
                command.Parameters.Add(paramGmtTimeShift);
            }

            IDbDataParameter paramGmtTimeInfo = command.CreateParameter();
            paramGmtTimeInfo.ParameterName = "@TimeZoneInfo";
            paramGmtTimeInfo.Value = timeInfo.DisplayName;
            command.Parameters.Add(paramGmtTimeInfo);

            IDbDataParameter paramFromDate = command.CreateParameter();
            paramFromDate.ParameterName = "@FromDate";
            paramFromDate.Value = FromDate;
            command.Parameters.Add(paramFromDate);

            IDbDataParameter paramToDate = command.CreateParameter();
            paramToDate.ParameterName = "@ToDate";
            paramToDate.Value = ToDate;
            command.Parameters.Add(paramToDate);

            IDbDataParameter paramIssueDate = command.CreateParameter();
            paramIssueDate.ParameterName = "@IssueDate";
            paramIssueDate.Value = IssueDate;
            command.Parameters.Add(paramIssueDate);

            IDbDataParameter paramDueDate = command.CreateParameter();
            paramDueDate.ParameterName = "@DueDate";
            paramDueDate.Value = DueDate;
            command.Parameters.Add(paramDueDate);


            IDbDataParameter paramUserID = command.CreateParameter();
            paramUserID.ParameterName = "@UserID";
            paramUserID.Value = UserID;
            command.Parameters.Add(paramUserID);

            IDbDataParameter param = DataHelper.AddParameter(command, "@InvoiceID", 1);
            param.Direction = ParameterDirection.Output;

            try
            {
                command.ExecuteNonQuery();

                int i = 0;
                int.TryParse(param.Value.ToString(), out i);
                var issuedInvoice = ObjectAssembler.Get<Billing_Invoice>(i);
                ex = issuedInvoice != null ? null : new Exception("No billing data");
                return issuedInvoice != null;
            }
            catch (Exception e)
            {
                ex = e;
                return false;
            }
        }
        /// <summary>
        /// Producing a new invoice for a selected Customer between FromDate and ToDate
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>

        public static Billing_Invoice InvoiceIssue(string CustomerID, DateTime FromDate, DateTime ToDate, DateTime IssueDate, DateTime DueDate, CustomTimeZoneInfo info, int UserID, out Exception ex)
        {
            return InvoiceIssue(CustomerID, FromDate, ToDate, IssueDate, DueDate, info, UserID, null, out ex);
        }
        public static Billing_Invoice InvoiceIssue(string CustomerID, DateTime FromDate, DateTime ToDate, DateTime IssueDate, DateTime DueDate, CustomTimeZoneInfo info, int UserID, string serial, out Exception ex)
        {
            Billing_Invoice issuedInvoice = null;
            short timeshift = (short)info.BaseUtcOffset;
            // No more than one invoice allowed to be issued at a time
            lock (typeof(DataHelper))
            {
                IDbConnection connection = GetOpenConnection();
                IDbCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = timeshift == 0 ? "bp_CreateInvoice" : "bp_CreateCustomerInvoice";
                command.CommandTimeout = 0;

                IDbDataParameter paramCustomerID = command.CreateParameter();
                paramCustomerID.ParameterName = "@CustomerID";
                paramCustomerID.Value = CustomerID;
                command.Parameters.Add(paramCustomerID);

                IDbDataParameter paramFromDate = command.CreateParameter();
                paramFromDate.ParameterName = "@FromDate";
                paramFromDate.Value = FromDate;
                command.Parameters.Add(paramFromDate);


                IDbDataParameter paramToDate = command.CreateParameter();
                paramToDate.ParameterName = "@ToDate";
                paramToDate.Value = ToDate;
                command.Parameters.Add(paramToDate);

                IDbDataParameter paramIssueDate = command.CreateParameter();
                paramIssueDate.ParameterName = "@IssueDate";
                paramIssueDate.Value = IssueDate;
                command.Parameters.Add(paramIssueDate);

                IDbDataParameter paramDueDate = command.CreateParameter();
                paramDueDate.ParameterName = "@DueDate";
                paramDueDate.Value = DueDate;
                command.Parameters.Add(paramDueDate);


                IDbDataParameter paramTimeInfo = command.CreateParameter();
                paramTimeInfo.ParameterName = "@TimeZoneInfo";
                paramTimeInfo.Value = info.DisplayName;
                command.Parameters.Add(paramTimeInfo);

                IDbDataParameter paramCustomerMaskID = command.CreateParameter();
                paramCustomerMaskID.ParameterName = "@CustomerMaskID";
                paramCustomerMaskID.Value = TABS.CarrierAccount.All[CustomerID].CustomerMask;
                command.Parameters.Add(paramCustomerMaskID);

                if (timeshift != 0)
                {
                    IDbDataParameter paramTimeShift = command.CreateParameter();
                    paramTimeShift.ParameterName = "@GMTShifttime";
                    paramTimeShift.Value = timeshift;
                    command.Parameters.Add(paramTimeShift);
                }

                IDbDataParameter paramUserID = command.CreateParameter();
                paramUserID.ParameterName = "@UserID";
                paramUserID.Value = UserID;
                command.Parameters.Add(paramUserID);



                Billing_Invoice invoice = new Billing_Invoice();
                invoice.Customer = CarrierAccount.All[CustomerID];
                invoice.BeginDate = FromDate;
                invoice.EndDate = ToDate;
                invoice.IssueDate = IssueDate;

                string Serial = string.IsNullOrEmpty(serial) ? SpecialSystemParameters.InvoiceSerialGenerator.GenerateSerial_New(invoice) : serial;
                IDbDataParameter paramSerial = command.CreateParameter();
                paramSerial.ParameterName = "@Serial";
                paramSerial.Value = Serial;
                command.Parameters.Add(paramSerial);

                IDbDataParameter param = DataHelper.AddParameter(command, "@InvoiceID", 1);
                param.Direction = ParameterDirection.Output;

                try
                {
                    command.ExecuteNonQuery();
                    int i = 0;
                    int.TryParse(param.Value.ToString(), out i);
                    issuedInvoice = ObjectAssembler.Get<Billing_Invoice>(i);

                    if (issuedInvoice != null)
                    {
                        issuedInvoice.VatValue = invoice.Customer.CarrierProfile.VAT;
                        TABS.ObjectAssembler.SaveOrUpdate(issuedInvoice, out ex);
                    }

                    ex = issuedInvoice != null ? null : new Exception("No billing data");
                }
                catch (Exception e)
                {
                    ex = e;
                    issuedInvoice = null;
                }
            }
            return issuedInvoice;
        }

        /// <summary>
        /// Producing a new disputed invoice for a selected Customer between FromDate and ToDate
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>

        public static Billing_Invoice DisputeInvoiceIssue(string CustomerID, DateTime FromDate, DateTime ToDate, DateTime IssueDate, DateTime DueDate, CustomTimeZoneInfo info, int UserID, out Exception ex)
        {
            return DisputeInvoiceIssue(CustomerID, FromDate, ToDate, IssueDate, DueDate, info, UserID, null, out ex);
        }
        public static Billing_Invoice DisputeInvoiceIssue(string CustomerID, DateTime FromDate, DateTime ToDate, DateTime IssueDate, DateTime DueDate, CustomTimeZoneInfo info, int UserID, string serial, out Exception ex)
        {
            Billing_Invoice issuedInvoice = null;
            short timeshift = (short)info.BaseUtcOffset;
            // No more than one invoice allowed to be issued at a time
            lock (typeof(DataHelper))
            {
                IDbConnection connection = GetOpenConnection();
                IDbCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = timeshift == 0 ? "bp_CreateInvoiceDispute" : "bp_CreateCustomerInvoiceDispute";
                command.CommandTimeout = 0;

                IDbDataParameter paramCustomerID = command.CreateParameter();
                paramCustomerID.ParameterName = "@CustomerID";
                paramCustomerID.Value = CustomerID;
                command.Parameters.Add(paramCustomerID);

                IDbDataParameter paramFromDate = command.CreateParameter();
                paramFromDate.ParameterName = "@FromDate";
                paramFromDate.Value = FromDate;
                command.Parameters.Add(paramFromDate);


                IDbDataParameter paramToDate = command.CreateParameter();
                paramToDate.ParameterName = "@ToDate";
                paramToDate.Value = ToDate;
                command.Parameters.Add(paramToDate);

                IDbDataParameter paramIssueDate = command.CreateParameter();
                paramIssueDate.ParameterName = "@IssueDate";
                paramIssueDate.Value = IssueDate;
                command.Parameters.Add(paramIssueDate);

                IDbDataParameter paramDueDate = command.CreateParameter();
                paramDueDate.ParameterName = "@DueDate";
                paramDueDate.Value = DueDate;
                command.Parameters.Add(paramDueDate);


                IDbDataParameter paramTimeInfo = command.CreateParameter();
                paramTimeInfo.ParameterName = "@TimeZoneInfo";
                paramTimeInfo.Value = info.DisplayName;
                command.Parameters.Add(paramTimeInfo);

                IDbDataParameter paramCustomerMaskID = command.CreateParameter();
                paramCustomerMaskID.ParameterName = "@CustomerMaskID";
                paramCustomerMaskID.Value = TABS.CarrierAccount.All[CustomerID].CustomerMask;
                command.Parameters.Add(paramCustomerMaskID);

                if (timeshift != 0)
                {
                    IDbDataParameter paramTimeShift = command.CreateParameter();
                    paramTimeShift.ParameterName = "@GMTShifttime";
                    paramTimeShift.Value = timeshift;
                    command.Parameters.Add(paramTimeShift);
                }

                IDbDataParameter paramUserID = command.CreateParameter();
                paramUserID.ParameterName = "@UserID";
                paramUserID.Value = UserID;
                command.Parameters.Add(paramUserID);



                Billing_Invoice invoice = new Billing_Invoice();
                invoice.Customer = CarrierAccount.All[CustomerID];
                invoice.BeginDate = FromDate;
                invoice.EndDate = ToDate;
                invoice.IssueDate = IssueDate;

                string Serial = string.IsNullOrEmpty(serial) ? SpecialSystemParameters.InvoiceSerialGenerator.GenerateSerial_New(invoice) : serial;
                IDbDataParameter paramSerial = command.CreateParameter();
                paramSerial.ParameterName = "@Serial";
                paramSerial.Value = Serial;
                command.Parameters.Add(paramSerial);

                IDbDataParameter param = DataHelper.AddParameter(command, "@InvoiceID", 1);
                param.Direction = ParameterDirection.Output;

                try
                {
                    command.ExecuteNonQuery();
                    int i = 0;
                    int.TryParse(param.Value.ToString(), out i);
                    issuedInvoice = ObjectAssembler.Get<Billing_Invoice>(i);

                    if (issuedInvoice != null)
                    {
                        issuedInvoice.VatValue = invoice.Customer.CarrierProfile.VAT;
                        TABS.ObjectAssembler.SaveOrUpdate(issuedInvoice, out ex);
                    }

                    ex = issuedInvoice != null ? null : new Exception("No billing data");
                }
                catch (Exception e)
                {
                    ex = e;
                    issuedInvoice = null;
                }
            }
            return issuedInvoice;
        }


        public static Billing_Invoice PreviewCustomerInvoice(string CustomerID, DateTime FromDate, DateTime ToDate, DateTime IssueDate, DateTime DueDate, CustomTimeZoneInfo info, out Exception ex)
        {
            Billing_Invoice Invoice = null;

            short timeshift = (short)info.BaseUtcOffset;

            string sql = @"EXEC bp_PreviewCustomerInvoice
	                                    @CustomerID = @P1,
	                                    @GMTShifttime = @P2,
	                                    @TimeZoneInfo = @P3,
	                                    @FromDate = @P4,
	                                    @ToDate = @P5,
	                                    @IssueDate = @P6,
	                                    @DueDate = @P7";

            bool nodata = false;

            try
            {
                DataSet invoiceSet = GetDataSet(sql, CustomerID, timeshift, info.DisplayName, FromDate, ToDate, IssueDate, DueDate);

                nodata = invoiceSet == null || invoiceSet.Tables.Count == 0;

                DataTable InvoiceTable = invoiceSet.Tables[0];
                DataTable InvoiceDetail = invoiceSet.Tables[1];

                Invoice = new TABS.Billing_Invoice();

                Invoice.Supplier = TABS.CarrierAccount.SYSTEM;
                Invoice.Customer = TABS.CarrierAccount.All[CustomerID];

                Invoice.Amount = (decimal)InvoiceTable.Rows[0]["Amount"];
                Invoice.NumberOfCalls = (int)InvoiceTable.Rows[0]["NumberOfCalls"];
                Invoice.Duration = (decimal)InvoiceTable.Rows[0]["Duration"];
                Invoice.BeginDate = (DateTime)InvoiceTable.Rows[0]["BeginDate"];
                Invoice.EndDate = (DateTime)InvoiceTable.Rows[0]["EndDate"];
                Invoice.IssueDate = (DateTime)InvoiceTable.Rows[0]["IssueDate"];
                Invoice.DueDate = (DateTime)InvoiceTable.Rows[0]["DueDate"];
                var currencySymbol = InvoiceTable.Rows[0]["CurrencyID"].ToString();
                Invoice.Currency = TABS.Currency.All.ContainsKey(currencySymbol) ? TABS.Currency.All[currencySymbol] : TABS.ObjectAssembler.Get<TABS.Currency>(currencySymbol);


                List<TABS.Billing_Invoice_Detail> details = new List<TABS.Billing_Invoice_Detail>(InvoiceDetail.Rows.Count);

                foreach (DataRow row in InvoiceDetail.Rows)
                {
                    TABS.Billing_Invoice_Detail detail = new TABS.Billing_Invoice_Detail();
                    detail.Destination = row["Destination"].ToString();
                    detail.FromDate = DateTime.Parse(row["FromDate"].ToString());
                    detail.TillDate = DateTime.Parse(row["TillDate"].ToString());
                    detail.NumberOfCalls = row["NumberOfCalls"] == DBNull.Value ? 0 : int.Parse(row["NumberOfCalls"].ToString());
                    detail.Duration = row["Duration"] == DBNull.Value ? 0m : decimal.Parse(row["Duration"].ToString());
                    detail.Rate = row["Rate"] == DBNull.Value ? 0m : decimal.Parse(row["Rate"].ToString());
                    detail.RateType = row["RateType"] == DBNull.Value ? TABS.ToDRateType.Normal : (TABS.ToDRateType)(byte.Parse(row["RateType"].ToString()));
                    detail.Amount = row["Amount"] == DBNull.Value ? 0m : (decimal)float.Parse(row["Amount"].ToString());
                    var currencySymbolDetail = row["CurrencyID"].ToString();
                    detail.Currency = TABS.Currency.All.ContainsKey(currencySymbolDetail) ? TABS.Currency.All[currencySymbolDetail] : TABS.ObjectAssembler.Get<TABS.Currency>(currencySymbolDetail);
                    detail.Billing_Invoice = Invoice;

                    details.Add(detail);
                }

                Invoice.Billing_Invoice_Details = details.OrderBy(d => d.Destination).ToList();
                ex = null;
            }
            catch (Exception e)
            {
                ex = nodata ? null : e;
            }

            return Invoice;
        }
        public static Billing_Invoice PreviewCustomerInvoice(string CustomerID, DateTime FromDate, DateTime ToDate, DateTime IssueDate, DateTime DueDate, CustomTimeZoneInfo info, out Exception ex, int commandtimeout)
        {
            Billing_Invoice Invoice = null;

            short timeshift = (short)info.BaseUtcOffset;
            string sql = "";
            //if(timeshift>0)
            sql = @"EXEC bp_PreviewCustomerInvoice
	                                    @CustomerID = @P1,
	                                    @GMTShifttime = @P2,
	                                    @TimeZoneInfo = @P3,
	                                    @FromDate = @P4,
	                                    @ToDate = @P5,
	                                    @IssueDate = @P6,
	                                    @DueDate = @P7";
            //            else
            //                sql = @"EXEC bp_PreviewInvoice
            //	                                    @CustomerID = @P1,
            //	                                    @FromDate = @P4,
            //	                                    @ToDate = @P5";




            bool nodata = false;

            try
            {
                DataSet invoiceSet = null;
                //if(timeshift>0)
                invoiceSet = GetDataSet(sql, commandtimeout, CustomerID, timeshift, info.DisplayName, FromDate, ToDate, IssueDate, DueDate);
                //else
                //    invoiceSet = GetDataSet(sql, commandtimeout, CustomerID,FromDate, ToDate);

                nodata = invoiceSet == null || invoiceSet.Tables.Count == 0;

                DataTable InvoiceTable = invoiceSet.Tables[0];
                DataTable InvoiceDetail = invoiceSet.Tables[1];

                Invoice = new TABS.Billing_Invoice();

                Invoice.Supplier = TABS.CarrierAccount.SYSTEM;
                Invoice.Customer = TABS.CarrierAccount.All[CustomerID];

                Invoice.Amount = (decimal)InvoiceTable.Rows[0]["Amount"];
                Invoice.NumberOfCalls = (int)InvoiceTable.Rows[0]["NumberOfCalls"];
                Invoice.Duration = (decimal)InvoiceTable.Rows[0]["Duration"];
                Invoice.BeginDate = (DateTime)InvoiceTable.Rows[0]["BeginDate"];
                Invoice.EndDate = (DateTime)InvoiceTable.Rows[0]["EndDate"];
                Invoice.IssueDate = (DateTime)InvoiceTable.Rows[0]["IssueDate"];
                Invoice.DueDate = (DateTime)InvoiceTable.Rows[0]["DueDate"];
                var currencySymbol = InvoiceTable.Rows[0]["CurrencyID"].ToString();
                Invoice.Currency = TABS.Currency.All.ContainsKey(currencySymbol) ? TABS.Currency.All[currencySymbol] : TABS.ObjectAssembler.Get<TABS.Currency>(currencySymbol);


                List<TABS.Billing_Invoice_Detail> details = new List<TABS.Billing_Invoice_Detail>(InvoiceDetail.Rows.Count);

                foreach (DataRow row in InvoiceDetail.Rows)
                {
                    TABS.Billing_Invoice_Detail detail = new TABS.Billing_Invoice_Detail();
                    detail.Destination = row["Destination"].ToString();
                    detail.FromDate = DateTime.Parse(row["FromDate"].ToString());
                    detail.TillDate = DateTime.Parse(row["TillDate"].ToString());
                    detail.NumberOfCalls = row["NumberOfCalls"] == DBNull.Value ? 0 : int.Parse(row["NumberOfCalls"].ToString());
                    detail.Duration = row["Duration"] == DBNull.Value ? 0m : decimal.Parse(row["Duration"].ToString());
                    detail.Rate = row["Rate"] == DBNull.Value ? 0m : decimal.Parse(row["Rate"].ToString());
                    detail.RateType = row["RateType"] == DBNull.Value ? TABS.ToDRateType.Normal : (TABS.ToDRateType)(byte.Parse(row["RateType"].ToString()));
                    detail.Amount = row["Amount"] == DBNull.Value ? 0m : (decimal)float.Parse(row["Amount"].ToString());
                    var currencySymbolDetail = row["CurrencyID"].ToString();
                    detail.Currency = TABS.Currency.All.ContainsKey(currencySymbolDetail) ? TABS.Currency.All[currencySymbolDetail] : TABS.ObjectAssembler.Get<TABS.Currency>(currencySymbolDetail);
                    detail.Billing_Invoice = Invoice;

                    details.Add(detail);
                }

                Invoice.Billing_Invoice_Details = details.OrderBy(d => d.Destination).ToList();
                ex = null;
            }
            catch (Exception e)
            {
                ex = nodata ? null : e;
            }

            return Invoice;
        }

        public static Billing_Invoice PreviewCustomerDisputedInvoice(string CustomerID, DateTime FromDate, DateTime ToDate, DateTime IssueDate, DateTime DueDate, CustomTimeZoneInfo info, out Exception ex, int commandtimeout)
        {
            Billing_Invoice Invoice = null;

            short timeshift = (short)info.BaseUtcOffset;
            string sql = "";
            sql = @"EXEC bp_PreviewDisputeCustomerInvoice
	                                    @CustomerID = @P1,
	                                    @GMTShifttime = @P2,
	                                    @TimeZoneInfo = @P3,
	                                    @FromDate = @P4,
	                                    @ToDate = @P5,
	                                    @IssueDate = @P6,
	                                    @DueDate = @P7";

            bool nodata = false;

            try
            {
                DataSet invoiceSet = null;
                invoiceSet = GetDataSet(sql, commandtimeout, CustomerID, timeshift, info.DisplayName, FromDate, ToDate, IssueDate, DueDate);
                nodata = invoiceSet == null || invoiceSet.Tables.Count == 0;

                DataTable InvoiceTable = invoiceSet.Tables[0];
                DataTable InvoiceDetail = invoiceSet.Tables[1];

                Invoice = new TABS.Billing_Invoice();

                Invoice.Supplier = TABS.CarrierAccount.SYSTEM;
                Invoice.Customer = TABS.CarrierAccount.All[CustomerID];

                Invoice.Amount = (decimal)InvoiceTable.Rows[0]["Amount"];
                Invoice.NumberOfCalls = (int)InvoiceTable.Rows[0]["NumberOfCalls"];
                Invoice.Duration = (decimal)InvoiceTable.Rows[0]["Duration"];
                Invoice.BeginDate = (DateTime)InvoiceTable.Rows[0]["BeginDate"];
                Invoice.EndDate = (DateTime)InvoiceTable.Rows[0]["EndDate"];
                Invoice.IssueDate = (DateTime)InvoiceTable.Rows[0]["IssueDate"];
                Invoice.DueDate = (DateTime)InvoiceTable.Rows[0]["DueDate"];
                var currencySymbol = InvoiceTable.Rows[0]["CurrencyID"].ToString();
                Invoice.Currency = TABS.Currency.All.ContainsKey(currencySymbol) ? TABS.Currency.All[currencySymbol] : TABS.ObjectAssembler.Get<TABS.Currency>(currencySymbol);


                List<TABS.Billing_Invoice_Detail> details = new List<TABS.Billing_Invoice_Detail>(InvoiceDetail.Rows.Count);

                foreach (DataRow row in InvoiceDetail.Rows)
                {
                    TABS.Billing_Invoice_Detail detail = new TABS.Billing_Invoice_Detail();
                    detail.Destination = row["Destination"].ToString();
                    detail.FromDate = DateTime.Parse(row["FromDate"].ToString());
                    detail.TillDate = DateTime.Parse(row["TillDate"].ToString());
                    detail.NumberOfCalls = row["NumberOfCalls"] == DBNull.Value ? 0 : int.Parse(row["NumberOfCalls"].ToString());
                    detail.Duration = row["Duration"] == DBNull.Value ? 0m : decimal.Parse(row["Duration"].ToString());
                    detail.Rate = row["Rate"] == DBNull.Value ? 0m : decimal.Parse(row["Rate"].ToString());
                    detail.RateType = row["RateType"] == DBNull.Value ? TABS.ToDRateType.Normal : (TABS.ToDRateType)(byte.Parse(row["RateType"].ToString()));
                    detail.Amount = row["Amount"] == DBNull.Value ? 0m : (decimal)float.Parse(row["Amount"].ToString());
                    var currencySymbolDetail = row["CurrencyID"].ToString();
                    detail.Currency = TABS.Currency.All.ContainsKey(currencySymbolDetail) ? TABS.Currency.All[currencySymbolDetail] : TABS.ObjectAssembler.Get<TABS.Currency>(currencySymbolDetail);
                    detail.Billing_Invoice = Invoice;

                    details.Add(detail);
                }

                Invoice.Billing_Invoice_Details = details.OrderBy(d => d.Destination).ToList();
                ex = null;
            }
            catch (Exception e)
            {
                ex = nodata ? null : e;
            }

            return Invoice;
        }

        public static Dictionary<int, DTO.DTO_ZoneRate> GetSupplyRatesWithTraffic(string codeFilter, string zoneNameFilter, short ServicesFlag, string CustomerID, double ExcludedRate, DateTime StatsFrom, DateTime StatsTill, SupplierRatePolicy ratePolicy)
        {
            var rates = GetSupplyRates(codeFilter, zoneNameFilter, ServicesFlag, CustomerID, ExcludedRate, ratePolicy, null);


            if (Math.Abs(StatsFrom.Subtract(StatsTill).TotalSeconds) > 0)
            {
                //Updating the stats 
                List<Stats> stats = new List<Stats>();
                string SQL = @"SELECT * FROM GetSupplierZoneStats(@P1,@P2,@P3)";


                using (IDataReader statsReader = TABS.DataHelper.ExecuteReader(SQL, StatsFrom, StatsTill, null))
                {
                    while (statsReader.Read())
                    {
                        int index = -1;
                        Stats stat = new Stats();
                        index++; if (!statsReader.IsDBNull(index)) stat.SupplierID = statsReader.GetString(index);
                        index++; if (!statsReader.IsDBNull(index)) stat.SupplierZoneID = statsReader.GetInt32(index);
                        index++; if (!statsReader.IsDBNull(index)) stat.DurationInMinutes = (double?)statsReader.GetDecimal(index);
                        index++; if (!statsReader.IsDBNull(index)) stat.ASR = (double?)statsReader.GetDecimal(index);
                        index++; if (!statsReader.IsDBNull(index)) stat.ACD = (double?)statsReader.GetDecimal(index);

                        stats.Add(stat);
                    }
                }

                Dictionary<string, Stats> statsDic = new Dictionary<string, Stats>();
                foreach (var stat in stats)
                    statsDic[string.Concat(stat.SupplierID, ",", stat.SupplierZoneID)] = stat;

                foreach (var rate in rates.Values)
                {
                    foreach (var supplyRate in rate.SupplierRates)
                    {
                        Stats stat = null;
                        string key = string.Concat(supplyRate.Supplier.CarrierAccountID, ",", rate.OurZone.ZoneID);
                        if (statsDic.ContainsKey(key))
                            stat = statsDic[key];

                        if (stat != null)
                        {
                            supplyRate.ASR = stat.ASR;
                            supplyRate.ACD = stat.ACD;
                            supplyRate.Duration = stat.DurationInMinutes;
                        }
                    }
                }
            }
            //updating stats

            foreach (DTO.DTO_ZoneRate zoneRate in rates.Values)
                zoneRate.SupplierRates.Sort();


            return rates;
        }

        public static Dictionary<int, DTO.DTO_ZoneRate> GetSupplyRates(string codeFilter, string zoneNameFilter, short ServicesFlag, string CustomerID, double ExcludedRate, SupplierRatePolicy ratePolicy, string suppliers)
        {
            return GetSupplyRates(codeFilter, zoneNameFilter, ServicesFlag, CustomerID, ExcludedRate, null, null, ratePolicy, suppliers);
        }

        public static Dictionary<string, DTO.DTO_CodeRate> GetSupplyRatesByCode(string codeFilter, string zoneNameFilter, short ServicesFlag, string suppliers)
        {
            // This is a dictionary based on the Zone ID
            Dictionary<string, DTO.DTO_CodeRate> rates = new Dictionary<string, DTO.DTO_CodeRate>();
            Dictionary<int, Zone> zones = new Dictionary<int, Zone>();

            // The no-zone Rate!
            DTO.DTO_CodeRate noZoneRate = new TABS.DTO.DTO_CodeRate();
            noZoneRate.ServicesFlag = ServicesFlag;
            noZoneRate.SupplierRates = new List<TABS.DTO.DTO_SupplyRate>();

            IDbConnection connection = GetOpenConnection();
            IDbCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "bp_GetCodeLCR";

            // Add parameters
            if (codeFilter != null && codeFilter.Length > 0) AddParameter(command, "@codeFilter", codeFilter);
            if (zoneNameFilter != null && zoneNameFilter.Length > 0) AddParameter(command, "@zoneNameFilter", zoneNameFilter);
            AddParameter(command, "@ServicesFlagFilter", ServicesFlag);
            if (!string.IsNullOrEmpty(suppliers)) AddParameter(command, "@SupplierIDs", suppliers);
            // Get a reader
            IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                int index = -1;
                index++; string code = reader.GetString(index);
                index++; int OurZoneID = reader.GetInt32(index);
                index++; string ourZoneName = reader.GetString(index);
                index++; double? OurRate = 0; if (!reader.IsDBNull(index)) OurRate = reader.GetFloat(index);

                index++; string Supplier = reader.GetString(index);
                index++; int SupplierZoneID = reader.GetInt32(index);
                index++; string SupplierZoneName = reader.GetString(index);
                index++; double? SupplierRate = null; if (!reader.IsDBNull(index)) SupplierRate = reader.GetFloat(index);
                index++; short SupplierServicesFlag = 0; if (!reader.IsDBNull(index)) SupplierServicesFlag = reader.GetInt16(index);

                DTO.DTO_CodeRate codeRate = noZoneRate;
                TABS.Zone ownZone = null;
                if (!zones.TryGetValue(OurZoneID, out ownZone))
                {
                    ownZone = new Zone();
                    ownZone.Name = ourZoneName;
                    ownZone.Supplier = CarrierAccount.SYSTEM;
                }

                if (!rates.TryGetValue(code, out codeRate))
                {
                    codeRate = new TABS.DTO.DTO_CodeRate();
                    codeRate.OurCode = code;
                    codeRate.OurZone = ownZone;
                    codeRate.SupplierRates = new List<DTO.DTO_SupplyRate>();
                    codeRate.Normal = OurRate;
                    rates.Add(code, codeRate);
                }

                TABS.CarrierAccount SupplierAccount = CarrierAccount.All[Supplier];

                if (SupplierAccount.ActivationStatus != ActivationStatus.Inactive)
                {
                    DTO.DTO_SupplyRate supplyRate = new TABS.DTO.DTO_SupplyRate();

                    supplyRate.Supplier = CarrierAccount.All[Supplier];
                    supplyRate.Normal = SupplierRate;
                    supplyRate.ServicesFlag = SupplierServicesFlag;

                    Zone supplierZone = null;
                    if (!zones.TryGetValue(SupplierZoneID, out supplierZone))
                    {
                        supplierZone = new Zone();
                        supplierZone.ZoneID = SupplierZoneID;
                        supplierZone.Name = SupplierZoneName;
                        zones.Add(SupplierZoneID, supplierZone);
                    }
                    supplyRate.Zone = supplierZone;

                    codeRate.SupplierRates.Add(supplyRate);
                }
            }
            reader.Close();

            return rates;
        }
        /// <summary>
        /// Build a Supply Rate List based on the passed filters and return the list of DTOs.
        /// </summary>
        /// <param name="codeFilter">Filter for the Code</param>
        /// <param name="ZoneNameFilter">Filter for the zone name</param>
        /// <param name="ServicesFlag">The ServicesFlag (Combination of services) for which to retrieve the supply</param>
        /// <returns>A List of DTOs</returns>
        public static Dictionary<int, DTO.DTO_ZoneRate> GetSupplyRates
            (string codeFilter,
            string zoneNameFilter,
            short ServicesFlag,
            string CustomerID,
            double ExcludedRate,
            string Mode,
            int? Days,
            TABS.SupplierRatePolicy SupplierRatePolicy,
            string suppliers)
        {
            // This is a dictionary based on the Zone ID
            Dictionary<int, DTO.DTO_ZoneRate> rates = new Dictionary<int, DTO.DTO_ZoneRate>();
            Dictionary<int, Zone> supplierZones = new Dictionary<int, Zone>();

            // The no-zone Rate!
            DTO.DTO_ZoneRate noZoneRate = new TABS.DTO.DTO_ZoneRate();
            noZoneRate.ServicesFlag = ServicesFlag;
            noZoneRate.SupplierRates = new List<TABS.DTO.DTO_SupplyRate>();

            IDbConnection connection = GetOpenConnection();
            IDbCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "bp_GetZoneRates";

            // Add parameters
            if (codeFilter != null && codeFilter.Length > 0) AddParameter(command, "@codeFilter", codeFilter);
            if (zoneNameFilter != null && zoneNameFilter.Length > 0) AddParameter(command, "@zoneNameFilter", zoneNameFilter);
            if (CustomerID != null && CustomerID.Length > 0) AddParameter(command, "@CustomerID", CustomerID);
            if (ExcludedRate != 0) AddParameter(command, "@ExcludedRate", ExcludedRate);
            AddParameter(command, "@ServicesFlag", ServicesFlag);
            AddParameter(command, "@CurrencyID", CustomerID != null ? TABS.CarrierAccount.All[CustomerID].CarrierProfile.Currency.Symbol : Currency.Main.Symbol);
            if (!string.IsNullOrEmpty(suppliers)) AddParameter(command, "@SupplierIDs", suppliers);

            //with stats 
            if (Days != null) AddParameter(command, "@Days", Days);
            if (Mode != null) AddParameter(command, "@Mode", Mode);

            // Get a reader
            IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                int index = -1;
                index++; int? OurZoneID = null; if (!reader.IsDBNull(index)) OurZoneID = reader.GetInt32(index);
                index++; // skip the zoneName
                index++; double? OurRate = null; if (!reader.IsDBNull(index)) OurRate = reader.GetFloat(index);
                index++; double? OurOffPeakRate = null; if (!reader.IsDBNull(index)) OurOffPeakRate = reader.GetFloat(index);
                index++; double? OurWeekendRate = null; if (!reader.IsDBNull(index)) OurWeekendRate = reader.GetFloat(index);
                index++; short OurServicesFlag = 0; if (!reader.IsDBNull(index)) OurServicesFlag = reader.GetInt16(index);


                index++; string Supplier = reader.GetString(index);
                index++; int SupplierZoneID = reader.GetInt32(index);
                index++; string SupplierZoneName = reader.GetString(index);
                index++; double? SupplierRate = null; if (!reader.IsDBNull(index)) SupplierRate = reader.GetFloat(index);
                index++; double? SupplierOffPeakRate = null; if (!reader.IsDBNull(index)) SupplierOffPeakRate = reader.GetFloat(index);
                index++; double? SupplierWeekendRate = null; if (!reader.IsDBNull(index)) SupplierWeekendRate = reader.GetFloat(index);
                index++; short SupplierServicesFlag = 0; if (!reader.IsDBNull(index)) SupplierServicesFlag = reader.GetInt16(index);
                index++; DateTime? endeffectivedate = null; if (!reader.IsDBNull(index)) endeffectivedate = reader.GetDateTime(index);

                DTO.DTO_ZoneRate zoneRate = noZoneRate;
                if (OurZoneID != null && Zone.OwnZones.ContainsKey(OurZoneID.Value))
                {
                    if (!rates.TryGetValue(OurZoneID.Value, out zoneRate))
                    {
                        zoneRate = new TABS.DTO.DTO_ZoneRate();
                        zoneRate.ServicesFlag = OurServicesFlag;
                        zoneRate.OurZone = Zone.OwnZones[OurZoneID.Value];
                        zoneRate.SupplierRates = new List<DTO.DTO_SupplyRate>();
                        zoneRate.Normal = OurRate;
                        zoneRate.OffPeak = OurOffPeakRate;
                        zoneRate.Weekend = OurWeekendRate;
                        rates.Add(OurZoneID.Value, zoneRate);
                    }
                }

                TABS.CarrierAccount SupplierAccount = CarrierAccount.All[Supplier];

                if (SupplierAccount.ActivationStatus != ActivationStatus.Inactive)
                {
                    DTO.DTO_SupplyRate supplyRate = new TABS.DTO.DTO_SupplyRate();

                    supplyRate.Supplier = SupplierAccount;
                    supplyRate.Normal = SupplierRate;
                    supplyRate.OffPeak = SupplierOffPeakRate;
                    supplyRate.Weekend = SupplierWeekendRate;
                    supplyRate.ServicesFlag = SupplierServicesFlag;
                    supplyRate.EndEffectiveDate = endeffectivedate;

                    Zone supplierZone = null;
                    if (!supplierZones.TryGetValue(SupplierZoneID, out supplierZone))
                    {
                        supplierZone = new Zone();
                        supplierZone.Supplier = SupplierAccount;
                        supplierZone.ZoneID = SupplierZoneID;
                        supplierZone.Name = SupplierZoneName;
                        supplierZones.Add(SupplierZoneID, supplierZone);
                    }
                    supplyRate.Zone = supplierZone;

                    // check if supplier already has another zone/rate
                    DTO.DTO_SupplyRate otherFound = null;
                    int indexFound = 0;
                    switch (SupplierRatePolicy)
                    {
                        // Higest rate from suppliers 
                        case SupplierRatePolicy.Highest_Rate:
                            foreach (DTO.DTO_SupplyRate otherRate in zoneRate.SupplierRates)
                            {
                                if (otherRate.Supplier == supplyRate.Supplier)
                                {
                                    otherFound = otherRate;
                                    break;
                                }
                                indexFound++;
                            }

                            if (otherFound != null)
                            {
                                if (otherFound.Normal.Value < supplyRate.Normal.Value)
                                    zoneRate.SupplierRates[indexFound] = supplyRate;
                            }
                            else
                                zoneRate.SupplierRates.Add(supplyRate);
                            break;
                        // Lowest Rate Per Supplier 
                        case SupplierRatePolicy.Lowest_Rate:
                            indexFound = 0;
                            foreach (DTO.DTO_SupplyRate otherRate in zoneRate.SupplierRates)
                            {
                                if (otherRate.Supplier == supplyRate.Supplier)
                                {
                                    otherFound = otherRate;
                                    break;
                                }
                                indexFound++;
                            }

                            if (otherFound != null)
                            {
                                if (otherFound.Normal.Value > supplyRate.Normal.Value)
                                    zoneRate.SupplierRates[indexFound] = supplyRate;
                            }
                            else
                                zoneRate.SupplierRates.Add(supplyRate);
                            break;
                        case SupplierRatePolicy.None:
                            zoneRate.SupplierRates.Add(supplyRate);
                            break;
                    }

                    zoneRate.SupplierRates.Sort();
                    //if (highestRatePerSupplier)
                    //{
                    //    var other = zoneRate.SupplierRates.Where(sr => sr.Supplier.Equals(SupplierAccount)).FirstOrDefault();
                    //    if (other != null)
                    //    {

                    //        if (other.Normal < supplyRate.Normal)
                    //        {
                    //            zoneRate.SupplierRates.Remove(other);
                    //            zoneRate.SupplierRates.Add(supplyRate);
                    //        }
                    //    }
                    //    else
                    //        zoneRate.SupplierRates.Add(supplyRate);
                    //}
                    //else
                    //    zoneRate.SupplierRates.Add(supplyRate);





                }
            }
            reader.Close();

            return rates;
        }

        /// <summary>
        /// Build a Supply Rate List based on the passed filters and return the list of DTOs.
        /// </summary>
        /// <param name="codeFilter">Filter for the Code</param>
        /// <param name="ZoneNameFilter">Filter for the zone name</param>
        /// <param name="ServicesFlag">The ServicesFlag (Combination of services) for which to retrieve the supply</param>
        /// <returns>A List of DTOs</returns>
        public static Dictionary<int, DTO.DTO_ZoneRate> GetSaleRates(string codeFilter, string zoneNameFilter, short ServicesFlag)
        {
            // This is a dictionary based on the Zone ID
            Dictionary<int, DTO.DTO_ZoneRate> rates = new Dictionary<int, DTO.DTO_ZoneRate>();
            Dictionary<int, Zone> ourZones = new Dictionary<int, Zone>();

            // The no-zone Rate!
            DTO.DTO_ZoneRate noZoneRate = new TABS.DTO.DTO_ZoneRate();
            noZoneRate.ServicesFlag = ServicesFlag;
            noZoneRate.SaleRates = new List<TABS.DTO.DTO_SaleRate>();

            IDbConnection connection = GetOpenConnection();
            IDbCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "bp_GetSaleZoneRates";

            // Add parameters
            if (codeFilter != null && codeFilter.Length > 0) AddParameter(command, "@codeFilter", codeFilter);
            if (zoneNameFilter != null && zoneNameFilter.Length > 0) AddParameter(command, "@zoneNameFilter", zoneNameFilter);
            AddParameter(command, "@ServicesFlag", ServicesFlag);

            // Get a reader
            IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                int index = -1;
                index++; string customerID = reader.GetString(index);
                index++; int? OurZoneID = null; if (!reader.IsDBNull(index)) OurZoneID = reader.GetInt32(index);
                index++; double? OurRate = null; if (!reader.IsDBNull(index)) OurRate = reader.GetFloat(index);
                index++; double? OurOffPeakRate = null; if (!reader.IsDBNull(index)) OurOffPeakRate = reader.GetFloat(index);
                index++; double? OurWeekendRate = null; if (!reader.IsDBNull(index)) OurWeekendRate = reader.GetFloat(index);
                index++; short OurServicesFlag = 0; if (!reader.IsDBNull(index)) OurServicesFlag = reader.GetInt16(index);

                DTO.DTO_ZoneRate zoneRate = noZoneRate;
                if (OurZoneID != null && Zone.OwnZones.ContainsKey(OurZoneID.Value))
                {
                    if (!rates.TryGetValue(OurZoneID.Value, out zoneRate))
                    {
                        zoneRate = new TABS.DTO.DTO_ZoneRate();
                        zoneRate.OurZone = Zone.OwnZones[OurZoneID.Value];
                        zoneRate.SaleRates = new List<TABS.DTO.DTO_SaleRate>();
                        rates.Add(OurZoneID.Value, zoneRate);
                    }
                }

                TABS.CarrierAccount customerAccount = CarrierAccount.All.ContainsKey(customerID) ? CarrierAccount.All[customerID] : ObjectAssembler.Get<CarrierAccount>(customerID);

                if (customerAccount.ActivationStatus != ActivationStatus.Inactive)
                {
                    DTO.DTO_SaleRate saleRate = new DTO.DTO_SaleRate();

                    saleRate.Customer = CarrierAccount.All[customerID];
                    saleRate.Normal = OurRate;
                    saleRate.OffPeak = OurOffPeakRate;
                    saleRate.Weekend = OurWeekendRate;
                    saleRate.ServicesFlag = OurServicesFlag;
                    saleRate.Zone = zoneRate.OurZone;
                    zoneRate.SaleRates.Add(saleRate);
                }
            }
            reader.Close();

            return rates;
        }

        /// <summary>
        /// Get rate  plan for a customer or a policy 
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static RatePlan GetRatePlan(CarrierAccount customer, Currency currency)
        {
            using (IDbConnection connection = GetOpenConnection())
            {
                IDbCommand command = connection.CreateCommand();
                command.CommandText = "bp_CreateRatePlan";
                command.CommandType = CommandType.StoredProcedure;
                AddParameter(command, "@CustomerID", customer.CarrierAccountID);
                AddParameter(command, "@Currency", currency.Symbol);
                IDbDataParameter ratePlanID = AddParameter(command, "@RatePlanID", -1);
                ratePlanID.Direction = ParameterDirection.Output;
                command.ExecuteNonQuery();
                return ObjectAssembler.Get<RatePlan>((int)ratePlanID.Value);
            }
        }

        /// <summary>
        /// Fix the rate changes flag for rates in a pricelist (after being saved!)
        /// </summary>
        /// <param name="priceList"></param>
        public static void FixRateChanges(PriceList priceList)
        {
            ExecuteNonQuery("EXEC bp_FixRateChanges @PriceListID=@P1", priceList.ID);
        }

        public static double GetCurrencyExchangeFactor(Currency from, Currency to)
        {
            if (from.Equals(to)) return 1;
            else return to.LastRate / from.LastRate;
        }

        //public static List<TABS.Rate> GetRatesAfterZoneBlock(
        //    List<TABS.Rate> supplyRates
        //       , TABS.CarrierAccount customer)
        //{

        //    var effectiveBlocks = TABS.RouteBlock.SupplierZoneBlocks
        //        .Where(r =>
        //            r.Customer == null
        //            || r.Customer.Equals(customer));


        //    List<TABS.Rate> tobeExcluded = new List<Rate>();
        //    // group by supplier and zone
        //    var groupedRates = supplyRates.GroupBy(r => new { r.Zone.CodeGroup, r.Supplier });

        //    foreach (var group in groupedRates)
        //    {
        //        if (group.Any(g => effectiveBlocks.Any(r => r.Supplier.Equals(g.Supplier) && r.Zone.Equals(g.Zone))))
        //            tobeExcluded.AddRange(group);
        //    }

        //    return supplyRates.Except(tobeExcluded).ToList();
        //}

        public static HashSet<TABS.Rate> GetMarketPriceRates(TABS.DTO.DTO_ZoneRate zoneRate, Currency currency, List<TABS.Rate> supplyRates)
        {
            HashSet<TABS.Rate> result = new HashSet<Rate>();

            foreach (var supplyRatesBysupplier in supplyRates.GroupBy(s => s.Supplier))
            {
                List<TABS.Rate> toExclude = new List<Rate>();

                string key = string.Concat(zoneRate.OurZone.ZoneID, "_", zoneRate.ServicesFlag.ToString());
                if (SaleZoneMarketPrice.ZoneServiceAll.ContainsKey(key))
                {
                    foreach (var item in supplyRatesBysupplier)
                    {
                        double currencyExchangeFactor = GetCurrencyExchangeFactor(item.PriceList.Currency, currency);
                        if (SaleZoneMarketPrice.ZoneServiceAll[key].FromRate > Math.Round(item.Value.Value * (decimal)currencyExchangeFactor, int.Parse(TABS.SystemParameter.RatesDigitsAfterDot.NumericValue.Value.ToString("#")))
                                    || SaleZoneMarketPrice.ZoneServiceAll[key].ToRate < Math.Round(item.Value.Value * (decimal)currencyExchangeFactor, int.Parse(TABS.SystemParameter.RatesDigitsAfterDot.NumericValue.Value.ToString("#"))))
                            toExclude.Add(item);
                    }
                }

                foreach (var item in supplyRatesBysupplier.Except(toExclude))
                    result.Add(item);

            }

            return result;
        }

        public static HashSet<TABS.DTO.DTO_SupplyRate> GetMarketPriceRates(Zone zone, short services, Currency currency, List<TABS.DTO.DTO_SupplyRate> supplyRates)
        {
            HashSet<TABS.DTO.DTO_SupplyRate> result = new HashSet<TABS.DTO.DTO_SupplyRate>();
            List<TABS.DTO.DTO_SupplyRate> toExclude = new List<TABS.DTO.DTO_SupplyRate>();
            foreach (var supplyRatesBysupplier in supplyRates.GroupBy(s => s.Supplier))
            {
                toExclude.Clear();
                string key = string.Concat(zone.ZoneID, "_", services.ToString());
                if (SaleZoneMarketPrice.ZoneServiceAll.ContainsKey(key))
                {
                    foreach (var item in supplyRatesBysupplier)
                    {
                        double currencyExchangeFactor = GetCurrencyExchangeFactor(item.Rate.PriceList.Currency, currency);
                        if (SaleZoneMarketPrice.ZoneServiceAll[key].FromRate > Math.Round(item.Rate.Value.Value * (decimal)currencyExchangeFactor, int.Parse(TABS.SystemParameter.RatesDigitsAfterDot.NumericValue.Value.ToString("#")))
                                    || SaleZoneMarketPrice.ZoneServiceAll[key].ToRate < Math.Round(item.Rate.Value.Value * (decimal)currencyExchangeFactor, int.Parse(TABS.SystemParameter.RatesDigitsAfterDot.NumericValue.Value.ToString("#"))))
                            toExclude.Add(item);
                    }
                }

                foreach (var item in supplyRatesBysupplier.Except(toExclude))
                    result.Add(item);


            }
            toExclude = null;
            return result;
        }
        public static void GetMarketPriceRates(Zone zone, short services, Currency currency, List<TABS.DTO.DTO_SupplyRate> supplyRates, out List<TABS.DTO.DTO_SupplyRate> SupplyRatesMP)
        {
            //HashSet<TABS.DTO.DTO_SupplyRate> result = new HashSet<TABS.DTO.DTO_SupplyRate>();
            SupplyRatesMP = null;
            List<TABS.DTO.DTO_SupplyRate> toExclude = new List<TABS.DTO.DTO_SupplyRate>();
            List<TABS.DTO.DTO_SupplyRate> toExcludes = new List<TABS.DTO.DTO_SupplyRate>();
            foreach (var supplyRatesBysupplier in supplyRates.GroupBy(s => s.Supplier))
            {
                toExclude.Clear();
                string key = string.Concat(zone.ZoneID, "_", services.ToString());
                if (SaleZoneMarketPrice.ZoneServiceAll.ContainsKey(key))
                {
                    foreach (var item in supplyRatesBysupplier)
                    {
                        double currencyExchangeFactor = GetCurrencyExchangeFactor(item.Rate.PriceList.Currency, currency);
                        if (SaleZoneMarketPrice.ZoneServiceAll[key].FromRate > Math.Round(item.Rate.Value.Value * (decimal)currencyExchangeFactor, int.Parse(TABS.SystemParameter.RatesDigitsAfterDot.NumericValue.Value.ToString("#")))
                                    || SaleZoneMarketPrice.ZoneServiceAll[key].ToRate < Math.Round(item.Rate.Value.Value * (decimal)currencyExchangeFactor, int.Parse(TABS.SystemParameter.RatesDigitsAfterDot.NumericValue.Value.ToString("#"))))
                            toExclude.Add(item);
                    }
                }

                //foreach (var item in supplyRatesBysupplier.Except(toExclude))
                //    result.Add(item);
                toExcludes.AddRange(toExclude);

            }
            toExclude = null;
            SupplyRatesMP = supplyRates.Except(toExcludes).ToList();
            toExcludes = null;
        }
        public static void GetMarketPriceRates(Zone zone, short services, Currency currency, List<TABS.Rate> supplyRates, out List<TABS.Rate> SupplyRatesMP)
        {
            //HashSet<TABS.DTO.DTO_SupplyRate> result = new HashSet<TABS.DTO.DTO_SupplyRate>();
            SupplyRatesMP = null;
            List<TABS.Rate> toExclude = new List<TABS.Rate>();
            List<TABS.Rate> toExcludes = new List<TABS.Rate>();
            foreach (var supplyRatesBysupplier in supplyRates.GroupBy(s => s.Supplier))
            {
                toExclude.Clear();
                string key = string.Concat(zone.ZoneID, "_", services.ToString());
                if (SaleZoneMarketPrice.ZoneServiceAll.ContainsKey(key))
                {
                    foreach (var item in supplyRatesBysupplier)
                    {
                        double currencyExchangeFactor = GetCurrencyExchangeFactor(item.PriceList.Currency, currency);
                        if (SaleZoneMarketPrice.ZoneServiceAll[key].FromRate > Math.Round(item.Value.Value * (decimal)currencyExchangeFactor, int.Parse(TABS.SystemParameter.RatesDigitsAfterDot.NumericValue.Value.ToString("#")))
                                    || SaleZoneMarketPrice.ZoneServiceAll[key].ToRate < Math.Round(item.Value.Value * (decimal)currencyExchangeFactor, int.Parse(TABS.SystemParameter.RatesDigitsAfterDot.NumericValue.Value.ToString("#"))))
                            toExclude.Add(item);
                    }
                }

                //foreach (var item in supplyRatesBysupplier.Except(toExclude))
                //    result.Add(item);
                toExcludes.AddRange(toExclude);

            }
            toExclude = null;
            SupplyRatesMP = supplyRates.Except(toExcludes).ToList();
            toExcludes = null;
        }

        /// <summary>
        /// filter the rates when all other supplier zones are blocked  and the landline only exist in list 
        /// </summary>
        /// <param name="saleZone"></param>
        /// <param name="supplyRates"></param>
        /// <returns></returns>
        public static List<TABS.Rate> GetFavorateRates(
            TABS.CarrierAccount customer,
            TABS.RouteBlock[] supplierZoneBlocks,
            TABS.Zone saleZone,
            HashSet<TABS.Rate> supplyRates
            , bool IsCurrent
            , List<string> Warnings)
        {
            List<TABS.Rate> result = new List<Rate>();
            if (supplyRates == null) return result;

            if (customer != null)
                supplierZoneBlocks = supplierZoneBlocks.Where(z => z.Customer == null || (z.Customer != null && z.Customer == customer)).ToArray();

            bool IsIncludedBlockedSupplierZones = (bool.Parse(TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.Include_Blocked_Zones_In_ZoneRates].Value.ToString()) == true) ? true : false;
            var loadedCodes = IsCurrent ? TABS.Components.RoutePool.CurrentZoneCodes : TABS.Components.RoutePool.FutureZoneCodes;

            foreach (var supplyRatesBysupplier in supplyRates.GroupBy(s => s.Supplier))
            {
                if (!loadedCodes.ContainsKey(saleZone.ZoneID))
                {
                    string message = string.Format("The loaded Code info doesn't contains sale zone {0}", saleZone.Name);
                    log.ErrorFormat(message);
                    Warnings.Add(message);
                }

                // check if one of the cost zone in the code info
                foreach (var zone in supplyRatesBysupplier.Select(s => s.Zone))
                {
                    if (!loadedCodes.ContainsKey(zone.ZoneID))
                    {

                        string message = string.Format("The loaded Code info doesn't contains cost zone {0}", zone.Name);
                        log.ErrorFormat(message);
                        Warnings.Add(message);
                    }
                }

                try
                {
                    var firstCode = loadedCodes[saleZone.ZoneID].OrderBy(c => c.Code).First().Code;
                    // check if main break (landline ---> do nothing)
                    if (firstCode == saleZone.CodeGroup.Code)
                    {
                        result.AddRange(supplyRatesBysupplier
                            .Where(r => loadedCodes[r.ZoneID].Any(c => c.Code == c.CodeGroup)));
                        continue;
                    }

                    bool isLandlineExistsInSupplyRates = supplyRatesBysupplier
                        .Any(sr => loadedCodes[sr.ZoneID].Any(c => sr.Zone.CodeGroup.Code == c.Code));

                    bool OnlyLandlineMatch =
                        isLandlineExistsInSupplyRates ?
                        supplyRatesBysupplier
                        .Where(sr => loadedCodes[sr.ZoneID].Any(c => sr.Zone.CodeGroup.Code == c.Code))
                        .Count() == 1
                        && supplyRatesBysupplier.Count() == 1
                        : false;

                    if (OnlyLandlineMatch)
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;
                    }

                    if (!isLandlineExistsInSupplyRates)
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;
                    }

                    var landlineRate = supplyRatesBysupplier
                        .First(r => loadedCodes[r.ZoneID].Any(c => c.Code == r.Zone.CodeGroup.Code));


                    Dictionary<TABS.Rate, bool> supplyRatesBlocks = new Dictionary<Rate, bool>();

                    foreach (var rate in supplyRatesBysupplier)
                        supplyRatesBlocks[rate] = supplierZoneBlocks.Any(b => b.Zone.Equals(rate.Zone));

                    bool IsAllBlockedButLandline = supplyRatesBlocks.Where(s => !s.Key.Zone.Equals(landlineRate.Zone)).All(s => s.Value);

                    if (IsAllBlockedButLandline)
                        continue;
                    else
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;

                    }
                }

                catch (Exception ex)
                {
                    string message = string.Format("Error while filtering LCR for supplier {0}, please check log for more information", supplyRatesBysupplier.First().Supplier);
                    log.Error(message, ex);
                    Warnings.Add(message);
                }
            }
            if (IsIncludedBlockedSupplierZones == false)
                foreach (RouteBlock r in supplierZoneBlocks)
                {//if blocked zones from supplier not included
                    result.Remove(result.Where(z => z.ZoneID == r.Zone.ZoneID).FirstOrDefault());
                }
            return result.ToList();

        }

        public static List<TABS.Rate> GetFavorateRates(
            TABS.CarrierAccount customer,
            TABS.RouteBlock[] supplierZoneBlocks,
            TABS.Zone saleZone,
            HashSet<TABS.Rate> supplyRates
            , bool IsCurrent
            , List<string> Warnings, bool IncludeBlockedSupplierZones)
        {


            DateTime starttime = DateTime.Now;


            List<TABS.Rate> result = new List<Rate>();
            if (supplyRates == null) return result;

            if (customer != null)
                supplierZoneBlocks = supplierZoneBlocks.Where(z => z.Customer == null || (z.Customer != null && z.Customer == customer)).ToArray();

            // bool IsIncludedBlockedSupplierZones = (bool.Parse(TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.Include_Blocked_Zones_In_ZoneRates].Value.ToString()) == true) ? true : false;
            //            var loadedCodes = IsCurrent ? TABS.Components.RoutePool.CurrentZoneCodes.ToDictionary(k => k.Key, v => v.Value.Where(c => c.Code.Contains(saleZone.CodeGroup.Code)).ToList()) : TABS.Components.RoutePool.FutureZoneCodes.ToDictionary(k => k.Key, v => v.Value.Where(c => c.Code.Contains(saleZone.CodeGroup.Code)).ToList());
            var loadedCodes = IsCurrent ? TABS.Components.RoutePool.CurrentZoneCodes : TABS.Components.RoutePool.FutureZoneCodes;


            foreach (var supplyRatesBysupplier in supplyRates.GroupBy(s => s.Supplier))
            {
                if (!loadedCodes.ContainsKey(saleZone.ZoneID))
                {
                    string message = string.Format("The loaded Code info doesn't contains sale zone {0}", saleZone.Name);
                    log.ErrorFormat(message);
                    Warnings.Add(message);
                }

                // check if one of the cost zone in the code info
                foreach (var zone in supplyRatesBysupplier.Select(s => s.Zone))
                {
                    if (!loadedCodes.ContainsKey(zone.ZoneID))
                    {

                        string message = string.Format("The loaded Code info doesn't contains cost zone {0}", zone.Name);
                        log.ErrorFormat(message);
                        Warnings.Add(message);
                    }
                }

                try
                {
                    var firstCode = loadedCodes[saleZone.ZoneID].OrderBy(c => c.Code).First().Code;
                    // check if main break (landline ---> do nothing)
                    if (firstCode == saleZone.CodeGroup.Code)
                    {
                        result.AddRange(supplyRatesBysupplier
                            .Where(r => loadedCodes[r.ZoneID].Any(c => c.Code == c.CodeGroup)));
                        continue;
                    }

                    bool isLandlineExistsInSupplyRates = supplyRatesBysupplier
                        .Any(sr => loadedCodes[sr.ZoneID].Any(c => sr.Zone.CodeGroup.Code == c.Code));

                    bool OnlyLandlineMatch =
                        isLandlineExistsInSupplyRates ?
                        supplyRatesBysupplier
                        .Where(sr => loadedCodes[sr.ZoneID].Any(c => sr.Zone.CodeGroup.Code == c.Code))
                        .Count() == 1
                        && supplyRatesBysupplier.Count() == 1
                        : false;

                    if (OnlyLandlineMatch)
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;
                    }

                    if (!isLandlineExistsInSupplyRates)
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;
                    }

                    var landlineRate = supplyRatesBysupplier
                        .First(r => loadedCodes[r.ZoneID].Any(c => c.Code == r.Zone.CodeGroup.Code));


                    Dictionary<TABS.Rate, bool> supplyRatesBlocks = new Dictionary<Rate, bool>();

                    foreach (var rate in supplyRatesBysupplier)
                        supplyRatesBlocks[rate] = supplierZoneBlocks.Any(b => b.Zone.Equals(rate.Zone));

                    bool IsAllBlockedButLandline = supplyRatesBlocks.Where(s => !s.Key.Zone.Equals(landlineRate.Zone)).All(s => s.Value);

                    if (IsAllBlockedButLandline)
                        continue;
                    else
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;

                    }
                }

                catch (Exception ex)
                {
                    string message = string.Format("Error while filtering LCR for supplier {0}, please check log for more information", supplyRatesBysupplier.First().Supplier);
                    log.Error(message, ex);
                    Warnings.Add(message);
                }
            }
            // List<TABS.RouteBlock> iraq= supplierZoneBlocks.Where(z => z.Zone.Name == "Iraq-Baghdad").ToList();
            if (IncludeBlockedSupplierZones == false)
                foreach (RouteBlock r in supplierZoneBlocks)
                {//if blocked zones from supplier not included
                    result.Remove(result.Where(z => z.ZoneID == r.Zone.ZoneID).FirstOrDefault());
                }
            //DateTime Endtime = DateTime.Now;
            //string diff = Endtime.Subtract(starttime).ToString();
            return result.ToList();

        }

        public static List<DTO.DTO_SupplyRate> GetFavorateRates(
          TABS.CarrierAccount customer,
          TABS.RouteBlock[] supplierZoneBlocks,
          TABS.Zone saleZone,
          HashSet<TABS.DTO.DTO_SupplyRate> supplyRates
          , bool IsCurrent
          , List<string> Warnings)
        {


            List<TABS.DTO.DTO_SupplyRate> result = new List<DTO.DTO_SupplyRate>();
            if (supplyRates == null) return new List<DTO.DTO_SupplyRate>();

            var loadedCodes = IsCurrent ? TABS.Components.RoutePool.CurrentZoneCodes : TABS.Components.RoutePool.FutureZoneCodes;

            foreach (var supplyRatesBysupplier in supplyRates.GroupBy(s => s.Supplier))
            {
                // check if sale zone in the code info 
                if (!loadedCodes.ContainsKey(saleZone.ZoneID))
                {
                    string message = string.Format("The loaded Code info doesn't contains sale zone {0}", saleZone.Name);
                    log.ErrorFormat(message);
                    Warnings.Add(message);
                }

                // check if one of the cost zone in the code info
                foreach (var zone in supplyRatesBysupplier.Select(s => s.Zone))
                {
                    if (!loadedCodes.ContainsKey(zone.ZoneID))
                    {

                        string message = string.Format("The loaded Code info doesn't contains cost zone {0},Supplier {1}", zone.Name, zone.Supplier);
                        log.ErrorFormat(message);
                        Warnings.Add(message);
                    }
                }

                try
                {
                    var firstCode = loadedCodes[saleZone.ZoneID].OrderBy(c => c.Code).First().Code;
                    // check if main break (landline ---> do nothing)
                    if (saleZone.CodeGroup == null)
                    {
                        saleZone.CodeGroup.Code = "999999";
                        log.Error(string.Format("Zone {0} has null CodeGroup value", saleZone.Name));
                    }
                    if (firstCode == saleZone.CodeGroup.Code)
                    {

                        result.AddRange(supplyRatesBysupplier
                            .Where(r => loadedCodes[r.Zone.ZoneID].Any(c => c.Code == c.CodeGroup)));

                        continue;
                    }

                    bool isLandlineExistsInSupplyRates = false;

                    isLandlineExistsInSupplyRates = supplyRatesBysupplier
                        .Any(sr => loadedCodes[sr.Zone.ZoneID]
                        .Any(c => sr.Zone.CodeGroup.Code == c.Code));


                    bool OnlyLandlineMatch =
                        isLandlineExistsInSupplyRates ?
                        supplyRatesBysupplier
                        .Where(sr => loadedCodes[sr.Zone.ZoneID].Any(c => sr.Zone.CodeGroup.Code == c.Code))
                        .Count() == 1
                        && supplyRatesBysupplier.Count() == 1
                        : false;

                    if (OnlyLandlineMatch)
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;
                    }

                    if (!isLandlineExistsInSupplyRates)
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;
                    }

                    var landlineRate = supplyRatesBysupplier
                        .First(r => loadedCodes[r.Zone.ZoneID].Any(c => c.Code == r.Zone.CodeGroup.Code));


                    Dictionary<TABS.Rate, bool> supplyRatesBlocks = new Dictionary<Rate, bool>();

                    foreach (var rate in supplyRatesBysupplier)
                        supplyRatesBlocks[rate.Rate] = supplierZoneBlocks.Any(b => b.Zone.Equals(rate.Zone));

                    var bb = supplyRatesBlocks.Where(s => !s.Key.Zone.Equals(landlineRate.Zone)).ToList();
                    bool IsAllBlockedButLandline = bb.All(s => s.Value);

                    if (IsAllBlockedButLandline)
                        continue;
                    else
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;

                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Error while filtering LCR, please check log for more information");
                    log.Error(message, ex);
                    Warnings.Add(message);
                }
            }

            loadedCodes = null;
            return result.ToList();


        }
        public static void GetFavorateRates(
         TABS.CarrierAccount customer,
         TABS.RouteBlock[] supplierZoneBlocks,
         TABS.Zone saleZone,
         List<TABS.DTO.DTO_SupplyRate> supplyRates
         , bool IsCurrent
         , List<string> Warnings, out List<DTO.DTO_SupplyRate> FavorateRates)
        {

            FavorateRates = null;
            //List<DTO.DTO_SupplyRate > result= new List<DTO.DTO_SupplyRate>();
            if (supplyRates == null)
            { FavorateRates = new List<DTO.DTO_SupplyRate>(); return; }


            var loadedCodes = IsCurrent ? TABS.Components.RoutePool.CurrentZoneCodes : TABS.Components.RoutePool.FutureZoneCodes;
            //FavorateRates = supplyRates.ToList();
            //return;
            foreach (var supplyRatesBysupplier in supplyRates.GroupBy(s => s.Supplier))
            {
                // check if sale zone in the code info 
                if (!loadedCodes.ContainsKey(saleZone.ZoneID))
                {
                    string message = string.Format("The loaded Code info doesn't contains sale zone {0}", saleZone.Name);
                    log.ErrorFormat(message);
                    Warnings.Add(message);
                }

                // check if one of the cost zone in the code info
                foreach (var zone in supplyRatesBysupplier.Select(s => s.Zone))
                {
                    if (!loadedCodes.ContainsKey(zone.ZoneID))
                    {

                        string message = string.Format("The loaded Code info doesn't contains cost zone {0},Supplier {1}", zone.Name, zone.Supplier);
                        log.ErrorFormat(message);
                        Warnings.Add(message);
                    }
                }

                try
                {
                    var firstCode = loadedCodes[saleZone.ZoneID].OrderBy(c => c.Code).First().Code;
                    // check if main break (landline ---> do nothing)
                    if (saleZone.CodeGroup == null)
                    {
                        saleZone.CodeGroup.Code = "999999";
                        log.Error(string.Format("Zone {0} has null CodeGroup value", saleZone.Name));
                    }
                    if (firstCode == saleZone.CodeGroup.Code)
                    {


                        supplyRates = supplyRates.Except(supplyRatesBysupplier
                           .Where(r => !loadedCodes[r.Zone.ZoneID].Any(c => c.Code == c.CodeGroup))).ToList();
                        // result.AddRange(supplyRatesBysupplier
                        //   .Where(r => loadedCodes[r.Zone.ZoneID].Any(c => c.Code == c.CodeGroup)));
                        continue;
                    }

                    bool isLandlineExistsInSupplyRates = false;

                    isLandlineExistsInSupplyRates = supplyRatesBysupplier
                        .Any(sr => loadedCodes[sr.Zone.ZoneID]
                        .Any(c => sr.Zone.CodeGroup.Code == c.Code));


                    bool OnlyLandlineMatch =
                        isLandlineExistsInSupplyRates ?
                        supplyRatesBysupplier
                        .Where(sr => loadedCodes[sr.Zone.ZoneID].Any(c => sr.Zone.CodeGroup.Code == c.Code))
                        .Count() == 1
                        && supplyRatesBysupplier.Count() == 1
                        : false;

                    if (OnlyLandlineMatch)
                    {
                        // result.AddRange(supplyRatesBysupplier);
                        continue;
                    }

                    if (!isLandlineExistsInSupplyRates)
                    {
                        // result.AddRange(supplyRatesBysupplier);
                        continue;
                    }

                    var landlineRate = supplyRatesBysupplier
                        .First(r => loadedCodes[r.Zone.ZoneID].Any(c => c.Code == r.Zone.CodeGroup.Code));


                    Dictionary<TABS.Rate, bool> supplyRatesBlocks = new Dictionary<Rate, bool>();

                    foreach (var rate in supplyRatesBysupplier)
                        supplyRatesBlocks[rate.Rate] = supplierZoneBlocks.Any(b => b.Zone.Equals(rate.Zone));

                    var bb = supplyRatesBlocks.Where(s => !s.Key.Zone.Equals(landlineRate.Zone)).ToList();
                    bool IsAllBlockedButLandline = bb.All(s => s.Value);

                    if (IsAllBlockedButLandline)
                        continue;
                    else
                    {
                        //  result.AddRange(supplyRatesBysupplier);
                        continue;

                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Error while filtering LCR, please check log for more information");
                    log.Error(message, ex);
                    Warnings.Add(message);
                }
            }
            FavorateRates = supplyRates.ToList();
            //if (result != null)
            //    FavorateRates.AddRange(result);
            // result=null;


        }

        public static List<TABS.Rate> GetFavorateRates(
          TABS.CarrierAccount customer,
          TABS.RouteBlock[] supplierZoneBlocks,
          TABS.Zone saleZone,
          List<TABS.Rate> supplyRates
          , bool IsCurrent)
        {


            List<TABS.Rate> result = new List<TABS.Rate>();
            if (supplyRates == null) return new List<TABS.Rate>();

            var loadedCodes = IsCurrent ? TABS.Components.RoutePool.CurrentZoneCodes : TABS.Components.RoutePool.FutureZoneCodes;

            foreach (var supplyRatesBysupplier in supplyRates.GroupBy(s => s.Supplier))
            {
                // check if sale zone in the code info 
                //if (!loadedCodes.ContainsKey(saleZone.ZoneID))
                //{
                //    string message = string.Format("The loaded Code info doesn't contains sale zone {0}", saleZone.Name);
                //    log.ErrorFormat(message);
                //    Warnings.Add(message);
                //}

                //// check if one of the cost zone in the code info
                //foreach (var zone in supplyRatesBysupplier.Select(s => s.Zone))
                //{
                //    if (!loadedCodes.ContainsKey(zone.ZoneID))
                //    {

                //        string message = string.Format("The loaded Code info doesn't contains cost zone {0},Supplier {1}", zone.Name, zone.Supplier);
                //        log.ErrorFormat(message);
                //        Warnings.Add(message);
                //    }
                //}

                try
                {
                    var firstCode = loadedCodes[saleZone.ZoneID].OrderBy(c => c.Code).First().Code;
                    // check if main break (landline ---> do nothing)
                    if (saleZone.CodeGroup == null)
                    {
                        saleZone.CodeGroup.Code = "999999";
                        log.Error(string.Format("Zone {0} has null CodeGroup value", saleZone.Name));
                    }
                    if (firstCode == saleZone.CodeGroup.Code)
                    {

                        result.AddRange(supplyRatesBysupplier
                            .Where(r => loadedCodes[r.Zone.ZoneID].Any(c => c.Code == c.CodeGroup)));

                        continue;
                    }

                    bool isLandlineExistsInSupplyRates = false;

                    isLandlineExistsInSupplyRates = supplyRatesBysupplier
                        .Any(sr => loadedCodes[sr.Zone.ZoneID]
                        .Any(c => sr.Zone.CodeGroup.Code == c.Code));


                    bool OnlyLandlineMatch =
                        isLandlineExistsInSupplyRates ?
                        supplyRatesBysupplier
                        .Where(sr => loadedCodes[sr.Zone.ZoneID].Any(c => sr.Zone.CodeGroup.Code == c.Code))
                        .Count() == 1
                        && supplyRatesBysupplier.Count() == 1
                        : false;

                    if (OnlyLandlineMatch)
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;
                    }

                    if (!isLandlineExistsInSupplyRates)
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;
                    }

                    var landlineRate = supplyRatesBysupplier
                        .First(r => loadedCodes[r.Zone.ZoneID].Any(c => c.Code == r.Zone.CodeGroup.Code));


                    Dictionary<TABS.Rate, bool> supplyRatesBlocks = new Dictionary<Rate, bool>();

                    foreach (var rate in supplyRatesBysupplier)
                        supplyRatesBlocks[rate] = supplierZoneBlocks.Any(b => b.Zone.Equals(rate.Zone));

                    var bb = supplyRatesBlocks.Where(s => !s.Key.Zone.Equals(landlineRate.Zone)).ToList();
                    bool IsAllBlockedButLandline = bb.All(s => s.Value);

                    if (IsAllBlockedButLandline)
                        continue;
                    else
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;

                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Error while filtering LCR, please check log for more information");
                    log.Error(message, ex);

                }
            }

            loadedCodes = null;
            return result.ToList();


        }

        public static List<TABS.Rate> GetFavorateRatesByZones(
        TABS.RouteBlock[] supplierZoneBlocks,
        TABS.Zone saleZone,
        List<TABS.Rate> supplyRates)
        {

            bool IsCodeGroup = (saleZone.EffectiveCodes.Count == 1 && saleZone.EffectiveCodes[0].Value == saleZone.CodeGroup.Code);
            List<TABS.Rate> result = new List<TABS.Rate>();
            if (supplyRates == null) return new List<TABS.Rate>();
            foreach (var supplyRatesBysupplier in supplyRates.GroupBy(s => s.Supplier.CarrierAccountID))
            {
                try
                {
                    // check if main break (landline ---> do nothing)
                    if (saleZone.CodeGroup == null)
                    {
                        saleZone.CodeGroup.Code = "999999";
                        log.Error(string.Format("Zone {0} has null CodeGroup value", saleZone.Name));
                    }
                    if (IsCodeGroup == true)//saleZone.IsCodeGroup==true
                    {

                        //if (!supplyRatesBysupplier.Any(r => r.Zone.IsHaveMatchingCodeGroup == false))
                        //    result.AddRange(supplyRatesBysupplier);
                        result.AddRange(supplyRatesBysupplier
                           .Where(r => r.Zone.IsHaveMatchingCodeGroup == true));

                        continue;
                    }

                    bool isLandlineExistsInSupplyRates = false;

                    isLandlineExistsInSupplyRates = supplyRatesBysupplier
                        .Any(sr => sr.Zone.IsCodeGroup);


                    bool OnlyLandlineMatch =
                        isLandlineExistsInSupplyRates ?
                        supplyRatesBysupplier
                        .Where(sr => sr.Zone.IsCodeGroup)
                        .Count() == 1
                        && supplyRatesBysupplier.Count() == 1
                        : false;

                    if (OnlyLandlineMatch)
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;
                    }

                    if (!isLandlineExistsInSupplyRates)
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;
                    }

                    var landlineRate = supplyRatesBysupplier
                        .First(r => r.Zone.IsCodeGroup);


                    Dictionary<TABS.Rate, bool> supplyRatesBlocks = new Dictionary<Rate, bool>();

                    foreach (var rate in supplyRatesBysupplier)
                        supplyRatesBlocks[rate] = supplierZoneBlocks.Any(b => b.Zone.Equals(rate.Zone));

                    var bb = supplyRatesBlocks.Where(s => !s.Key.Zone.Equals(landlineRate.Zone)).ToList();
                    bool IsAllBlockedButLandline = bb.All(s => s.Value);

                    if (IsAllBlockedButLandline)
                        continue;
                    else
                    {
                        result.AddRange(supplyRatesBysupplier);
                        continue;

                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Error while filtering LCR, please check log for more information");
                    log.Error(message, ex);

                }
            }

            return result.ToList();
        }

        private static TABS.Components.RoutePool GetRoutePool(CarrierAccount Customer, bool IsCurrent, bool IncludeLosses)
        {
            TABS.Components.RoutePool ratePool = null;
            bool isexactmatch = (bool)TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_ExactMatchProcessing].Value;
            if (isexactmatch == false)
            {
                if (IsCurrent == true)
                    ratePool = TABS.Components.RoutePool.Current;
                else
                    ratePool = TABS.Components.RoutePool.Future;
            }
            else
            {
                if (IsCurrent == true)
                    ratePool = TABS.Components.RoutePool.CurrentPool(Customer, IncludeLosses);
                if (IsCurrent == false)
                    ratePool = TABS.Components.RoutePool.FuturePool(Customer, IncludeLosses);
            }
            return ratePool;
        }
        public static List<TABS.DTO.DTO_SupplyRate> GetAllSupplyRatesbyZones(CarrierAccount Customer, bool IsCurrent, bool IncludeLossesExactMatch, TABS.Zone Zone, short ServicesFlag, TABS.SupplierRatePolicy Policy, Currency Currency, int TopSuppierRates, bool OverrideIncludeZoneBlock)
        {
            HashSet<TABS.Rate> supplyRates = null;
            List<TABS.DTO.DTO_SupplyRate> ValidSupplyRates = null;

            //var ratePool = (this.ISFutureRatePlan == false) ? TABS.Components.RoutePool.Current : TABS.Components.RoutePool.Future;
            var ratePool = GetRoutePool(Customer, IsCurrent, IncludeLossesExactMatch);
            var supplierZoneBlocks = TABS.RouteBlock.SupplierZoneBlocksList(ratePool.BaseDate == DateTime.Now).Where(s => s.Customer == null || s.Customer.Equals(Customer)).ToArray();

            var blokcsByZoneSupplier = supplierZoneBlocks.Distinct(new TABS.DataHelper.ISupplierBlockComparer()).ToDictionary(s => string.Concat(s.Zone.ZoneID, s.Supplier.CarrierAccountID));
            if (ratePool.SupplyRatesBySaleZone.TryGetValue(Zone, out supplyRates))
            {

                List<TABS.Rate> sortedRates = new List<TABS.Rate>();
                List<TABS.Rate> items = new List<TABS.Rate>();
                short minServiceFlag = ServicesFlag;// ServicesFlag;
                List<TABS.Rate> all = null;
                switch (Policy)
                {
                    case TABS.SupplierRatePolicy.None:
                        all = supplyRates.Where(r => !r.Supplier.CarrierProfile.Equals(Customer.CarrierProfile) && ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                        if (OverrideIncludeZoneBlock)
                            all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                        items = all.OrderBy(r => r.Value.Value * decimal.Parse(TABS.DataHelper.GetCurrencyExchangeFactor(r.PriceList.Currency, Currency).ToString())).Take(TopSuppierRates).ToList();
                        sortedRates.AddRange(items);

                        break;
                    case TABS.SupplierRatePolicy.Highest_Rate:

                        TABS.DataHelper.GetMarketPriceRates(Zone, ServicesFlag, Currency, supplyRates.Where(i => !i.Supplier.CarrierProfile.Equals(Customer.CarrierProfile)).ToList(), out all);
                        all = TABS.DataHelper.GetFavorateRatesByZones(supplierZoneBlocks, Zone, all);
                        all = all.Where(
                        r => ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                        if (OverrideIncludeZoneBlock)
                            all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                        foreach (var group in all.GroupBy(s => s.SupplierID))
                        {

                            var Max = group.Max(i => i.Value.Value);
                            items.Add(group.FirstOrDefault(i => i.Value.Value.Equals(Max)));
                        }
                        items = items.OrderBy(r => r.Value.Value * decimal.Parse(TABS.DataHelper.GetCurrencyExchangeFactor(r.PriceList.Currency, Currency).ToString())).Take(TopSuppierRates).ToList();
                        sortedRates.AddRange(items);

                        break;
                    case TABS.SupplierRatePolicy.Lowest_Rate:
                        all = supplyRates.Where(
                        r => !r.Supplier.CarrierProfile.Equals(Customer.CarrierProfile) && ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                        if (OverrideIncludeZoneBlock)
                            all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                        foreach (var group in all.GroupBy(s => s.SupplierID))
                        {
                            var min = group.Min(i => i.Value.Value);
                            items.Add(group.FirstOrDefault(i => i.Value.Value.Equals(min)));
                        }
                        items = items.OrderBy(r => r.Value.Value * decimal.Parse(TABS.DataHelper.GetCurrencyExchangeFactor(r.PriceList.Currency, Currency).ToString())).Take(TopSuppierRates).ToList();
                        sortedRates.AddRange(items);
                        break;
                }
                items = null;

                List<TABS.Rate> tobeExcluded = new List<TABS.Rate>();
                ValidSupplyRates = new List<TABS.DTO.DTO_SupplyRate>();
                foreach (TABS.Rate supplierRate in sortedRates)//FavorateRates
                {
                    var currencyExchangeFactor = TABS.DataHelper.GetCurrencyExchangeFactor(supplierRate.PriceList.Currency, Currency);

                    TABS.DTO.DTO_SupplyRate supplyRate = new TABS.DTO.DTO_SupplyRate();
                    supplyRate.Supplier = supplierRate.Supplier;
                    supplyRate.Rate = supplierRate;
                    supplyRate.Zone = supplierRate.Zone;
                    var key = string.Concat(supplierRate.ZoneID, supplierRate.SupplierID);
                    supplyRate.IsBlockAffected = blokcsByZoneSupplier.ContainsKey(key);

                    supplyRate.Normal = (double)supplierRate.Value * currencyExchangeFactor;
                    supplyRate.OffPeak = (double?)supplierRate.OffPeakRate * currencyExchangeFactor;
                    supplyRate.Weekend = (double?)supplierRate.WeekendRate * currencyExchangeFactor;
                    supplyRate.ServicesFlag = supplierRate.ServicesFlag;
                    supplierRate.BeginEffectiveDate = supplierRate.BeginEffectiveDate;
                    supplyRate.EndEffectiveDate = supplierRate.EndEffectiveDate;
                    supplyRate.ACD = 0;
                    supplyRate.ASR = 0;
                    supplyRate.Duration = 0;

                    // check if supplier already has another zone/rate
                    TABS.DTO.DTO_SupplyRate otherFound = null;
                    int indexFound = 0;

                    ValidSupplyRates.Add(supplyRate);
                }
            }
            return ValidSupplyRates;
        }

        public class ISupplierBlockComparer : IEqualityComparer<TABS.RouteBlock>
        {

            public bool Equals(TABS.RouteBlock x, TABS.RouteBlock y)
            {
                return x.Zone.ZoneID == y.Zone.ZoneID && x.Supplier.CarrierAccountID == y.Supplier.CarrierAccountID;
            }

            public int GetHashCode(TABS.RouteBlock obj)
            {
                return string.Concat(obj.Zone.ZoneID, obj.Supplier.CarrierAccountID).GetHashCode();
            }

        }

        public static List<TABS.DTO.DTO_SupplyRate> GetAllSupplyRatesbyZonesTargetAnalysis(CarrierAccount Customer, bool IsCurrent, bool IncludeLossesExactMatch, TABS.Zone Zone, short ServicesFlag, TABS.SupplierRatePolicy Policy, Currency Currency, int TopSuppierRates, bool OverrideIncludeZoneBlock, TABS.CarrierAccount Supplier)
        {
            HashSet<TABS.Rate> supplyRates = null;
            List<TABS.DTO.DTO_SupplyRate> ValidSupplyRates = null;

            //var ratePool = (this.ISFutureRatePlan == false) ? TABS.Components.RoutePool.Current : TABS.Components.RoutePool.Future;
            var ratePool = GetRoutePool(Customer, IsCurrent, IncludeLossesExactMatch);
            var supplierZoneBlocks = TABS.RouteBlock.SupplierZoneBlocksList(ratePool.BaseDate == DateTime.Now).Where(s => s.Customer == null || s.Customer.Equals(Customer)).ToArray();

            var blokcsByZoneSupplier = supplierZoneBlocks.Distinct(new TABS.DataHelper.ISupplierBlockComparer()).ToDictionary(s => string.Concat(s.Zone.ZoneID, s.Supplier.CarrierAccountID));
            if (ratePool.SupplyRatesBySaleZone.TryGetValue(Zone, out supplyRates))
            {

                List<TABS.Rate> sortedRates = new List<TABS.Rate>();
                List<TABS.Rate> items = new List<TABS.Rate>();
                short minServiceFlag = ServicesFlag;// ServicesFlag;
                List<TABS.Rate> all = null;

                TABS.DataHelper.GetMarketPriceRates(Zone, ServicesFlag, Currency, supplyRates.Where(i => !i.Supplier.CarrierProfile.Equals(Customer.CarrierProfile)).ToList(), out all);
                all = TABS.DataHelper.GetFavorateRatesByZones(supplierZoneBlocks, Zone, all);

                switch (Policy)
                {
                    //case TABS.SupplierRatePolicy.None:
                    //    all = supplyRates.Where(r => !r.Supplier.CarrierProfile.Equals(Customer.CarrierProfile) && ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                    //    if (OverrideIncludeZoneBlock)
                    //        all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                    //    items = all.OrderBy(r => r.Value.Value * decimal.Parse(TABS.DataHelper.GetCurrencyExchangeFactor(r.PriceList.Currency, Currency).ToString())).Take(TopSuppierRates).ToList();
                    //    sortedRates.AddRange(items);
                    //    break;

                    case TABS.SupplierRatePolicy.Highest_Rate:
                        //TABS.DataHelper.GetMarketPriceRates(Zone, ServicesFlag, Currency, supplyRates.Where(i => !i.Supplier.CarrierProfile.Equals(Customer.CarrierProfile)).ToList(), out all);
                        //all = TABS.DataHelper.GetFavorateRatesByZones(supplierZoneBlocks, Zone, all);
                        all = all.Where(
                        r => ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                        if (OverrideIncludeZoneBlock)
                            all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                        foreach (var group in all.GroupBy(s => s.SupplierID))
                        {
                            var Max = group.Max(i => i.Value.Value);
                            items.Add(group.FirstOrDefault(i => i.Value.Value.Equals(Max)));
                        }
                        if (Supplier != null)
                            all = all.Where(r => r.Supplier.CarrierAccountID == Supplier.CarrierAccountID).ToList();
                        items = items.OrderBy(r => r.Value.Value * decimal.Parse(TABS.DataHelper.GetCurrencyExchangeFactor(r.PriceList.Currency, Currency).ToString())).Take(TopSuppierRates).ToList();
                        sortedRates.AddRange(items);
                        break;

                    case SupplierRatePolicy.None:
                    case TABS.SupplierRatePolicy.Lowest_Rate:
                        all = all.Where(
                        r => !r.Supplier.CarrierProfile.Equals(Customer.CarrierProfile) && ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                        if (OverrideIncludeZoneBlock)
                            all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                        foreach (var group in all.GroupBy(s => s.SupplierID))
                        {
                            var min = group.Min(i => i.Value.Value);
                            items.Add(group.FirstOrDefault(i => i.Value.Value.Equals(min)));
                        }
                        if (Supplier != null)
                            all = all.Where(r => r.Supplier.CarrierAccountID == Supplier.CarrierAccountID).ToList();
                        items = items.OrderBy(r => r.Value.Value * decimal.Parse(TABS.DataHelper.GetCurrencyExchangeFactor(r.PriceList.Currency, Currency).ToString())).Take(TopSuppierRates).ToList();
                        sortedRates.AddRange(items);
                        break;
                }
                items = null;

                List<TABS.Rate> tobeExcluded = new List<TABS.Rate>();
                ValidSupplyRates = new List<TABS.DTO.DTO_SupplyRate>();
                foreach (TABS.Rate supplierRate in sortedRates)//FavorateRates
                {
                    var currencyExchangeFactor = TABS.DataHelper.GetCurrencyExchangeFactor(supplierRate.PriceList.Currency, Currency);

                    TABS.DTO.DTO_SupplyRate supplyRate = new TABS.DTO.DTO_SupplyRate();
                    supplyRate.Supplier = supplierRate.Supplier;
                    supplyRate.Rate = supplierRate;
                    supplyRate.Zone = supplierRate.Zone;
                    var key = string.Concat(supplierRate.ZoneID, supplierRate.SupplierID);
                    supplyRate.IsBlockAffected = blokcsByZoneSupplier.ContainsKey(key);

                    supplyRate.Normal = (double)supplierRate.Value * currencyExchangeFactor;
                    supplyRate.OffPeak = (double?)supplierRate.OffPeakRate * currencyExchangeFactor;
                    supplyRate.Weekend = (double?)supplierRate.WeekendRate * currencyExchangeFactor;
                    supplyRate.ServicesFlag = supplierRate.ServicesFlag;
                    supplierRate.BeginEffectiveDate = supplierRate.BeginEffectiveDate;
                    supplyRate.EndEffectiveDate = supplierRate.EndEffectiveDate;
                    supplyRate.ACD = 0;
                    supplyRate.ASR = 0;
                    supplyRate.Duration = 0;

                    // check if supplier already has another zone/rate
                    TABS.DTO.DTO_SupplyRate otherFound = null;
                    int indexFound = 0;

                    ValidSupplyRates.Add(supplyRate);
                }
            }
            return ValidSupplyRates;
        }

        public static Dictionary<int, DTO.DTO_ZoneRate> GetPlaningRatesWithSupply(CarrierAccount customer, Currency currency, bool IsStatsPeriodDefined, int? Days, DateTime StatsFrom, DateTime StatsTill, TABS.SupplierRatePolicy SupplierRatePolicy, TABS.Components.RoutePool ratePool, bool IsFuture)
        {
            //Get Supplier rates per sale zone with Quality statistic if requested for a certain period
            // This is a dictionary based on the Zone ID
            Dictionary<int, DTO.DTO_ZoneRate> rates = new Dictionary<int, DTO.DTO_ZoneRate>();

            var supplierZoneBlocks = TABS.RouteBlock.SupplierZoneBlocksList(IsFuture).Where(s => s.Customer == null || s.Customer.Equals(customer)).ToArray();

            var blokcsByZoneSupplier = supplierZoneBlocks.Distinct(new ISupplierBlockComparer()).ToDictionary(s => string.Concat(s.Zone.ZoneID, s.Supplier.CarrierAccountID));
            // get Rates from Current Sale
            var saleRates = ObjectAssembler.CurrentSession.CreateQuery(@"
                    FROM Rate R 
                    WHERE R.PriceList.Customer = :customer AND R.EndEffectiveDate IS NULL
                    ORDER BY R.Zone.Name
                ")
                .SetParameter("customer", customer)
                .List<TABS.Rate>();


            // Fill in sale rates
            foreach (var rate in saleRates)
            {
                var currencyExchangeFactor = GetCurrencyExchangeFactor(rate.PriceList.Currency, currency);
                var zoneRate = new TABS.DTO.DTO_ZoneRate();
                zoneRate.ServicesFlag = rate.ServicesFlag;
                zoneRate.OurZone = rate.Zone;
                zoneRate.SupplierRates = new List<DTO.DTO_SupplyRate>();
                zoneRate.Normal = (double)rate.Value * currencyExchangeFactor;
                zoneRate.OffPeak = rate.OffPeakRate.HasValue ? (double?)rate.OffPeakRate * currencyExchangeFactor : null;
                zoneRate.Weekend = rate.WeekendRate.HasValue ? (double?)rate.WeekendRate * currencyExchangeFactor : null;
                rates[rate.ZoneID] = zoneRate;
            }
            //  list = list.Where(l => (l.EndEffectiveDate > (PricelistEffectiveDate.SelectedDate.Value) && l.EndEffectiveDate != null && l.BeginEffectiveDate <= PricelistEffectiveDate.SelectedDate.Value) || (l.BeginEffectiveDate <= PricelistEffectiveDate.SelectedDate.Value && l.EndEffectiveDate == null)).Select(l => l).ToList();
            //else
            //    list = list.Where(l => (l.EndEffectiveDate > (PricelistEffectiveDate.SelectedDate.Value.AddDays(IncreaseEffectiveDate)) && l.EndEffectiveDate != null) || (l.EndEffectiveDate == null)).Select(l => l).ToList();
            // get existing planning Rates

            var planningRates = ObjectAssembler.CurrentSession.CreateQuery(@"FROM PlaningRate PR WHERE PR.RatePlan.Customer = :customer ORDER BY PR.Zone.Name")
                .SetParameter("customer", customer)
                .List<TABS.PlaningRate>();

            // Fill in planning rates
            foreach (TABS.PlaningRate rate in planningRates)
            {
                var currencyExchangeFactor = GetCurrencyExchangeFactor(rate.RatePlan.Currency, currency);
                var zoneRate = new TABS.DTO.DTO_ZoneRate();
                zoneRate.ServicesFlag = rate.ServicesFlag;
                zoneRate.OurZone = TABS.Zone.OwnZones.ContainsKey(rate.Zone.ZoneID) ? TABS.Zone.OwnZones[rate.ZoneID] : rate.Zone;
                zoneRate.SupplierRates = new List<DTO.DTO_SupplyRate>();
                zoneRate.Normal = (double)rate.Value * currencyExchangeFactor;
                zoneRate.OffPeak = rate.OffPeakRate.HasValue ? (double?)rate.OffPeakRate * currencyExchangeFactor : null;
                zoneRate.Weekend = rate.WeekendRate.HasValue ? (double?)rate.WeekendRate * currencyExchangeFactor : null;
                rates[rate.ZoneID] = zoneRate;

            }
            var policyHigh = SupplierRatePolicy != SupplierRatePolicy.Highest_Rate;
            // Now join Supply Rates!
            foreach (TABS.DTO.DTO_ZoneRate zoneRate in rates.Values)
            {
                HashSet<Rate> supplyRates = null;
                if (ratePool.SupplyRatesBySaleZone.TryGetValue(zoneRate.OurZone, out supplyRates))
                {
                    //supplyRates.RemoveWhere(r => r.Supplier.CarrierProfile.ProfileID.Equals(customer.CarrierProfile.ProfileID));



                    HashSet<TABS.Rate> sortedRates = new HashSet<Rate>();// supplyRates;// new HashSet<Rate>();

                    foreach (var item in supplyRates)
                    {
                        if (!item.Supplier.CarrierProfile.Equals(customer.CarrierProfile))
                            sortedRates.Add(item);
                    }


                    var FavorateRates = sortedRates.ToList();

                    List<TABS.Rate> tobeExcluded = new List<TABS.Rate>();

                    //foreach (var rate in FavorateRates)
                    //{
                    //    if (blokcsByZoneSupplier.ContainsKey(string.Concat(rate.ZoneID, rate.SupplierID)))
                    //        tobeExcluded.Add(rate);
                    //}

                    //FavorateRates = FavorateRates.Except(tobeExcluded).ToList();
                    //if (FavorateRates.Count > 1)
                    //    FavorateRates.Sort();

                    foreach (TABS.Rate supplierRate in FavorateRates)
                    {

                        //if (policyHigh
                        //    &&
                        //    !TABS.SystemParameter.Include_Blocked_Zones_In_ZoneRates.BooleanValue.Value
                        //    &&
                        //    blokcsByZoneSupplier.ContainsKey(string.Concat(supplierRate.ZoneID, supplierRate.SupplierID))
                        //    )
                        //    continue;

                        var currencyExchangeFactor = GetCurrencyExchangeFactor(supplierRate.PriceList.Currency, currency);

                        DTO.DTO_SupplyRate supplyRate = new TABS.DTO.DTO_SupplyRate();
                        supplyRate.Supplier = supplierRate.Supplier;
                        supplyRate.Rate = supplierRate;
                        supplyRate.Zone = supplierRate.Zone;
                        var key = string.Concat(supplierRate.ZoneID, supplierRate.SupplierID);
                        supplyRate.IsBlockAffected = blokcsByZoneSupplier.ContainsKey(key);

                        supplyRate.Normal = (double)supplierRate.Value * currencyExchangeFactor;
                        supplyRate.OffPeak = (double?)supplierRate.OffPeakRate * currencyExchangeFactor;
                        supplyRate.Weekend = (double?)supplierRate.WeekendRate * currencyExchangeFactor;
                        supplyRate.ServicesFlag = supplierRate.ServicesFlag;
                        supplierRate.BeginEffectiveDate = supplierRate.BeginEffectiveDate;
                        supplyRate.EndEffectiveDate = supplierRate.EndEffectiveDate;
                        supplyRate.ACD = 0;
                        supplyRate.ASR = 0;
                        supplyRate.Duration = 0;

                        // check if supplier already has another zone/rate
                        DTO.DTO_SupplyRate otherFound = null;
                        int indexFound = 0;

                        zoneRate.SupplierRates.Add(supplyRate);

                        //switch (SupplierRatePolicy)
                        //{
                        //    // Higest rate from suppliers 
                        //    case SupplierRatePolicy.Highest_Rate:
                        //        foreach (DTO.DTO_SupplyRate otherRate in zoneRate.SupplierRates)
                        //        {
                        //            if (otherRate.Supplier.Equals(supplyRate.Supplier))
                        //            {
                        //                otherFound = otherRate;
                        //                break;
                        //            }
                        //            indexFound++;
                        //        }

                        //        if (otherFound != null)
                        //        {
                        //            if (otherFound.Normal.Value < supplyRate.Normal.Value)
                        //                zoneRate.SupplierRates[indexFound] = supplyRate;
                        //        }
                        //        else
                        //            zoneRate.SupplierRates.Add(supplyRate);
                        //        break;
                        //    // Lowest Rate Per Supplier 
                        //    case SupplierRatePolicy.Lowest_Rate:
                        //        indexFound = 0;
                        //        foreach (DTO.DTO_SupplyRate otherRate in zoneRate.SupplierRates)
                        //        {
                        //            if (otherRate.Supplier.Equals(supplyRate.Supplier))
                        //            {
                        //                otherFound = otherRate;
                        //                break;
                        //            }
                        //            indexFound++;
                        //        }

                        //        if (otherFound != null)
                        //        {
                        //            if (otherFound.Normal.Value > supplyRate.Normal.Value)
                        //                zoneRate.SupplierRates[indexFound] = supplyRate;
                        //        }
                        //        else
                        //            zoneRate.SupplierRates.Add(supplyRate);
                        //        break;
                        //    case SupplierRatePolicy.None:
                        //        zoneRate.SupplierRates.Add(supplyRate);
                        //        break;
                        //}


                    }
                }
            }

            // Sort Supply Rates and Init Stats
            foreach (DTO.DTO_ZoneRate zoneRate in rates.Values)
            {
                zoneRate.SupplierRates.Sort();
                zoneRate.ACD = 0;
                zoneRate.ASR = 0;
                zoneRate.Duration = 0;
            }

            // Update Stats if necessary
            UpdateZoneRatesSupplierStats(IsStatsPeriodDefined, Days, ref StatsFrom, ref StatsTill, rates);
            //calculated in TQI page 
            // Return result
            return rates;
        }

        public static Dictionary<int, DTO.DTO_ZoneRate> GetPlaningRatesWithSupply(CarrierAccount customer, Currency currency, bool IsStatsPeriodDefined, int? Days, DateTime StatsFrom, DateTime StatsTill, TABS.SupplierRatePolicy SupplierRatePolicy, TABS.Components.RoutePool ratePool, bool IsFuture, int SuppllierTopRates, short ServicesFlag, bool IncludeZoneBlock, TABS.Currency SelectedCurrency)
        {
            //Get Supplier rates per sale zone with Quality statistic if requested for a certain period
            // This is a dictionary based on the Zone ID
            Dictionary<int, DTO.DTO_ZoneRate> rates = new Dictionary<int, DTO.DTO_ZoneRate>();

            var supplierZoneBlocks = TABS.RouteBlock.SupplierZoneBlocksList(IsFuture).Where(s => s.Customer == null || s.Customer.Equals(customer)).ToArray();

            var blokcsByZoneSupplier = supplierZoneBlocks.Distinct(new ISupplierBlockComparer()).ToDictionary(s => string.Concat(s.Zone.ZoneID, s.Supplier.CarrierAccountID));

            // Removed By Sari On 2013-09-04 to change the customer rate collection from Nhibernate to ADO
            // get Rates from Current Sale
            //            var saleRates = ObjectAssembler.CurrentSession.CreateQuery(@"
            //                    FROM Rate R 
            //                    WHERE R.PriceList.Customer = :customer AND R.EndEffectiveDate IS NULL
            //                    ORDER BY R.Zone.Name
            //                ")
            //                .SetParameter("customer", customer)
            //                .List<TABS.Rate>();

            var saleRates = GetOurRates(customer.CarrierAccountID, false, DateTime.Now, true);
            // Fill in sale rates
            foreach (TABS.Rate rate in saleRates)
            {
                var currencyExchangeFactor = GetCurrencyExchangeFactor(rate.PriceList.Currency, currency);
                var zoneRate = new TABS.DTO.DTO_ZoneRate();
                zoneRate.ServicesFlag = rate.ServicesFlag;
                zoneRate.OurZone = rate.Zone;
                zoneRate.SupplierRates = new List<DTO.DTO_SupplyRate>();
                zoneRate.Normal = (double)rate.Value * currencyExchangeFactor;
                zoneRate.OffPeak = rate.OffPeakRate.HasValue ? (double?)rate.OffPeakRate * currencyExchangeFactor : null;
                zoneRate.Weekend = rate.WeekendRate.HasValue ? (double?)rate.WeekendRate * currencyExchangeFactor : null;
                rates[rate.ZoneID] = zoneRate;
            }
            //  list = list.Where(l => (l.EndEffectiveDate > (PricelistEffectiveDate.SelectedDate.Value) && l.EndEffectiveDate != null && l.BeginEffectiveDate <= PricelistEffectiveDate.SelectedDate.Value) || (l.BeginEffectiveDate <= PricelistEffectiveDate.SelectedDate.Value && l.EndEffectiveDate == null)).Select(l => l).ToList();
            //else
            //    list = list.Where(l => (l.EndEffectiveDate > (PricelistEffectiveDate.SelectedDate.Value.AddDays(IncreaseEffectiveDate)) && l.EndEffectiveDate != null) || (l.EndEffectiveDate == null)).Select(l => l).ToList();
            // get existing planning Rates

            var planningRates = ObjectAssembler.CurrentSession.CreateQuery(@"FROM PlaningRate PR WHERE PR.RatePlan.Customer = :customer ORDER BY PR.Zone.Name")
                .SetParameter("customer", customer)
                .List<TABS.PlaningRate>();

            // Fill in planning rates
            foreach (TABS.PlaningRate rate in planningRates)
            {
                var currencyExchangeFactor = GetCurrencyExchangeFactor(rate.RatePlan.Currency, currency);
                var zoneRate = new TABS.DTO.DTO_ZoneRate();
                zoneRate.ServicesFlag = rate.ServicesFlag;
                zoneRate.OurZone = TABS.Zone.OwnZones.ContainsKey(rate.Zone.ZoneID) ? TABS.Zone.OwnZones[rate.ZoneID] : rate.Zone;
                zoneRate.SupplierRates = new List<DTO.DTO_SupplyRate>();
                zoneRate.Normal = (double)rate.Value * currencyExchangeFactor;
                zoneRate.OffPeak = rate.OffPeakRate.HasValue ? (double?)rate.OffPeakRate * currencyExchangeFactor : null;
                zoneRate.Weekend = rate.WeekendRate.HasValue ? (double?)rate.WeekendRate * currencyExchangeFactor : null;
                rates[rate.ZoneID] = zoneRate;

            }
            var policyHigh = SupplierRatePolicy != SupplierRatePolicy.Highest_Rate;
            // Now join Supply Rates!
            foreach (TABS.DTO.DTO_ZoneRate zoneRate in rates.Values)
            {
                HashSet<Rate> supplyRates = null;
                if (ratePool.SupplyRatesBySaleZone.TryGetValue(zoneRate.OurZone, out supplyRates))
                {
                    //supplyRates.RemoveWhere(r => r.Supplier.CarrierProfile.ProfileID.Equals(customer.CarrierProfile.ProfileID));
                    //HashSet<TABS.Rate> sortedRates = new HashSet<Rate>();
                    List<TABS.Rate> sortedRates = new List<TABS.Rate>();
                    //newly added
                    List<TABS.Rate> items = new List<Rate>();
                    short minServiceFlag = zoneRate.ServicesFlag;// ServicesFlag;
                    List<TABS.Rate> all = null;
                    switch (SupplierRatePolicy)
                    {
                        case SupplierRatePolicy.None:
                            all = supplyRates.Where(r => !r.Supplier.CarrierProfile.Equals(customer.CarrierProfile) && ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                            if (IncludeZoneBlock == false)
                                all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                            items = all.OrderBy(r => r.Value.Value * decimal.Parse(GetCurrencyExchangeFactor(r.PriceList.Currency, currency).ToString())).Take(SuppllierTopRates).ToList();
                            sortedRates.AddRange(items);

                            break;
                        case SupplierRatePolicy.Highest_Rate:

                            TABS.DataHelper.GetMarketPriceRates(zoneRate.OurZone, ServicesFlag, SelectedCurrency, supplyRates.Where(i => !i.Supplier.CarrierProfile.Equals(customer.CarrierProfile)).ToList(), out all);
                            all = TABS.DataHelper.GetFavorateRatesByZones(supplierZoneBlocks, zoneRate.OurZone, all);
                            all = all.Where(
                            r => ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                            if (IncludeZoneBlock == false)
                                all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                            foreach (var group in all.GroupBy(s => s.SupplierID))
                            {

                                var Max = group.Max(i => i.Value.Value);
                                items.Add(group.FirstOrDefault(i => i.Value.Value.Equals(Max)));
                            }
                            items = items.OrderBy(r => r.Value.Value * decimal.Parse(GetCurrencyExchangeFactor(r.PriceList.Currency, currency).ToString())).Take(SuppllierTopRates).ToList();
                            sortedRates.AddRange(items);

                            break;
                        case SupplierRatePolicy.Lowest_Rate:
                            all = supplyRates.Where(
                            r => !r.Supplier.CarrierProfile.Equals(customer.CarrierProfile) && ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                            if (IncludeZoneBlock == false)
                                all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                            foreach (var group in all.GroupBy(s => s.SupplierID))
                            {
                                var min = group.Min(i => i.Value.Value);
                                items.Add(group.FirstOrDefault(i => i.Value.Value.Equals(min)));
                            }
                            items = items.OrderBy(r => r.Value.Value * decimal.Parse(GetCurrencyExchangeFactor(r.PriceList.Currency, currency).ToString())).Take(SuppllierTopRates).ToList();
                            sortedRates.AddRange(items);
                            break;
                    }
                    items = null;
                    //foreach (var item in supplyRates.ToList())
                    //{
                    //    if (!item.Supplier.CarrierProfile.Equals(customer.CarrierProfile))
                    //        sortedRates.Add(item);
                    //}


                    //List<TABS.Rate> FavorateRates = sortedRates.ToList();

                    List<TABS.Rate> tobeExcluded = new List<TABS.Rate>();
                    foreach (TABS.Rate supplierRate in sortedRates)//FavorateRates
                    {



                        var currencyExchangeFactor = GetCurrencyExchangeFactor(supplierRate.PriceList.Currency, currency);

                        DTO.DTO_SupplyRate supplyRate = new TABS.DTO.DTO_SupplyRate();
                        supplyRate.Supplier = supplierRate.Supplier;
                        supplyRate.Rate = supplierRate;
                        supplyRate.Zone = supplierRate.Zone;
                        var key = string.Concat(supplierRate.ZoneID, supplierRate.SupplierID);
                        supplyRate.IsBlockAffected = blokcsByZoneSupplier.ContainsKey(key);

                        supplyRate.Normal = (double)supplierRate.Value * currencyExchangeFactor;
                        supplyRate.OffPeak = (double?)supplierRate.OffPeakRate * currencyExchangeFactor;
                        supplyRate.Weekend = (double?)supplierRate.WeekendRate * currencyExchangeFactor;
                        supplyRate.ServicesFlag = supplierRate.ServicesFlag;
                        supplierRate.BeginEffectiveDate = supplierRate.BeginEffectiveDate;
                        supplyRate.EndEffectiveDate = supplierRate.EndEffectiveDate;
                        supplyRate.ACD = 0;
                        supplyRate.ASR = 0;
                        supplyRate.Duration = 0;

                        // check if supplier already has another zone/rate
                        DTO.DTO_SupplyRate otherFound = null;
                        int indexFound = 0;

                        zoneRate.SupplierRates.Add(supplyRate);


                    }
                }
            }

            // Sort Supply Rates and Init Stats
            foreach (DTO.DTO_ZoneRate zoneRate in rates.Values)
            {
                zoneRate.SupplierRates.Sort();
                zoneRate.ACD = 0;
                zoneRate.ASR = 0;
                zoneRate.Duration = 0;
            }

            // Update Stats if necessary
            UpdateZoneRatesSupplierStats(IsStatsPeriodDefined, Days, ref StatsFrom, ref StatsTill, rates);

            // Return result
            return rates;
        }

        public static Dictionary<int, DTO.DTO_ZoneRate> GetPlaningRatesWithSupply(CarrierAccount customer, Currency currency, bool IsStatsPeriodDefined, int? Days, DateTime StatsFrom, DateTime StatsTill, TABS.SupplierRatePolicy SupplierRatePolicy, TABS.Components.RoutePool ratePool, bool IsFuture, int SuppllierTopRates, short ServicesFlag, bool IncludeZoneBlock, TABS.Currency SelectedCurrency, List<TABS.Rate> CustomrRate)
        {
            //Get Supplier rates per sale zone with Quality statistic if requested for a certain period
            // This is a dictionary based on the Zone ID
            Dictionary<int, DTO.DTO_ZoneRate> rates = new Dictionary<int, DTO.DTO_ZoneRate>();

            var supplierZoneBlocks = TABS.RouteBlock.SupplierZoneBlocksList(IsFuture).Where(s => s.Customer == null || s.Customer.Equals(customer)).ToArray();

            var blokcsByZoneSupplier = supplierZoneBlocks.Distinct(new ISupplierBlockComparer()).ToDictionary(s => string.Concat(s.Zone.ZoneID, s.Supplier.CarrierAccountID));

            // Removed By Sari On 2013-09-04 to change the customer rate collection from Nhibernate to ADO
            // get Rates from Current Sale
            //            var saleRates = ObjectAssembler.CurrentSession.CreateQuery(@"
            //                    FROM Rate R 
            //                    WHERE R.PriceList.Customer = :customer AND R.EndEffectiveDate IS NULL
            //                    ORDER BY R.Zone.Name
            //                ")
            //                .SetParameter("customer", customer)
            //                .List<TABS.Rate>();
            //var saleRates = GetOurRates(customer.CarrierAccountID, false, DateTime.Now, true);

            // Changed By Sari On 2013-09-10 customer rate sent as parameter to the function as they have already prepared
            var saleRates = CustomrRate;

            // Fill in sale rates
            foreach (TABS.Rate rate in saleRates)
            {
                var currencyExchangeFactor = GetCurrencyExchangeFactor(rate.PriceList.Currency, currency);
                var zoneRate = new TABS.DTO.DTO_ZoneRate();
                zoneRate.ServicesFlag = rate.ServicesFlag;
                zoneRate.OurZone = rate.Zone;
                zoneRate.OurRate = rate;
                zoneRate.SupplierRates = new List<DTO.DTO_SupplyRate>();
                zoneRate.Normal = (double)rate.Value * currencyExchangeFactor;
                zoneRate.OffPeak = rate.OffPeakRate.HasValue ? (double?)rate.OffPeakRate * currencyExchangeFactor : null;
                zoneRate.Weekend = rate.WeekendRate.HasValue ? (double?)rate.WeekendRate * currencyExchangeFactor : null;
                rates[rate.ZoneID] = zoneRate;
            }
            //  list = list.Where(l => (l.EndEffectiveDate > (PricelistEffectiveDate.SelectedDate.Value) && l.EndEffectiveDate != null && l.BeginEffectiveDate <= PricelistEffectiveDate.SelectedDate.Value) || (l.BeginEffectiveDate <= PricelistEffectiveDate.SelectedDate.Value && l.EndEffectiveDate == null)).Select(l => l).ToList();
            //else
            //    list = list.Where(l => (l.EndEffectiveDate > (PricelistEffectiveDate.SelectedDate.Value.AddDays(IncreaseEffectiveDate)) && l.EndEffectiveDate != null) || (l.EndEffectiveDate == null)).Select(l => l).ToList();
            // get existing planning Rates

            var planningRates = ObjectAssembler.CurrentSession.CreateQuery(@"FROM PlaningRate PR WHERE PR.RatePlan.Customer = :customer ORDER BY PR.Zone.Name")
                .SetParameter("customer", customer)
                .List<TABS.PlaningRate>();

            // Fill in planning rates
            foreach (TABS.PlaningRate rate in planningRates)
            {
                var currencyExchangeFactor = GetCurrencyExchangeFactor(rate.RatePlan.Currency, currency);
                var zoneRate = new TABS.DTO.DTO_ZoneRate();
                zoneRate.ServicesFlag = rate.ServicesFlag;
                zoneRate.OurZone = TABS.Zone.OwnZones.ContainsKey(rate.Zone.ZoneID) ? TABS.Zone.OwnZones[rate.ZoneID] : rate.Zone;
                zoneRate.OurRate = new Rate { ServicesFlag = rate.ServicesFlag, Value = rate.Value, BeginEffectiveDate = rate.BeginEffectiveDate, Zone = rate.Zone };
                zoneRate.SupplierRates = new List<DTO.DTO_SupplyRate>();
                zoneRate.Normal = (double)rate.Value * currencyExchangeFactor;
                zoneRate.OffPeak = rate.OffPeakRate.HasValue ? (double?)rate.OffPeakRate * currencyExchangeFactor : null;
                zoneRate.Weekend = rate.WeekendRate.HasValue ? (double?)rate.WeekendRate * currencyExchangeFactor : null;
                rates[rate.ZoneID] = zoneRate;

            }
            var policyHigh = SupplierRatePolicy != SupplierRatePolicy.Highest_Rate;
            // Now join Supply Rates!
            foreach (TABS.DTO.DTO_ZoneRate zoneRate in rates.Values)
            {
                HashSet<Rate> supplyRates = null;
                if (ratePool.SupplyRatesBySaleZone.TryGetValue(zoneRate.OurZone, out supplyRates))
                {
                    //supplyRates.RemoveWhere(r => r.Supplier.CarrierProfile.ProfileID.Equals(customer.CarrierProfile.ProfileID));
                    //HashSet<TABS.Rate> sortedRates = new HashSet<Rate>();
                    List<TABS.Rate> sortedRates = new List<TABS.Rate>();
                    //newly added
                    List<TABS.Rate> items = new List<Rate>();
                    short minServiceFlag = zoneRate.ServicesFlag;// ServicesFlag;
                    List<TABS.Rate> all = null;
                    switch (SupplierRatePolicy)
                    {
                        case SupplierRatePolicy.None:
                            all = supplyRates.Where(r => !r.Supplier.CarrierProfile.Equals(customer.CarrierProfile) && ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                            if (IncludeZoneBlock == false)
                                all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                            items = all.OrderBy(r => r.Value.Value * decimal.Parse(GetCurrencyExchangeFactor(r.PriceList.Currency, currency).ToString())).Take(SuppllierTopRates).ToList();
                            sortedRates.AddRange(items);

                            break;
                        case SupplierRatePolicy.Highest_Rate:
                            //ServicesFlag--must be pass the customerrate service
                            TABS.DataHelper.GetMarketPriceRates(zoneRate.OurZone, zoneRate.OurRate.ServicesFlag, SelectedCurrency, supplyRates.Where(i => !i.Supplier.CarrierProfile.Equals(customer.CarrierProfile)).ToList(), out all);
                            all = TABS.DataHelper.GetFavorateRatesByZones(supplierZoneBlocks, zoneRate.OurZone, all);
                            all = all.Where(
                            r => ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                            if (IncludeZoneBlock == false)
                                all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                            foreach (var group in all.GroupBy(s => s.SupplierID))
                            {

                                var Max = group.Max(i => i.Value.Value);
                                items.Add(group.FirstOrDefault(i => i.Value.Value.Equals(Max)));
                            }
                            items = items.OrderBy(r => r.Value.Value * decimal.Parse(GetCurrencyExchangeFactor(r.PriceList.Currency, currency).ToString())).Take(SuppllierTopRates).ToList();
                            sortedRates.AddRange(items);

                            break;
                        case SupplierRatePolicy.Lowest_Rate:
                            all = supplyRates.Where(
                            r => !r.Supplier.CarrierProfile.Equals(customer.CarrierProfile) && ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                            if (IncludeZoneBlock == false)
                                all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                            foreach (var group in all.GroupBy(s => s.SupplierID))
                            {
                                var min = group.Min(i => i.Value.Value);
                                items.Add(group.FirstOrDefault(i => i.Value.Value.Equals(min)));
                            }
                            items = items.OrderBy(r => r.Value.Value * decimal.Parse(GetCurrencyExchangeFactor(r.PriceList.Currency, currency).ToString())).Take(SuppllierTopRates).ToList();
                            sortedRates.AddRange(items);
                            break;
                    }
                    items = null;
                    //foreach (var item in supplyRates.ToList())
                    //{
                    //    if (!item.Supplier.CarrierProfile.Equals(customer.CarrierProfile))
                    //        sortedRates.Add(item);
                    //}


                    //List<TABS.Rate> FavorateRates = sortedRates.ToList();

                    List<TABS.Rate> tobeExcluded = new List<TABS.Rate>();
                    foreach (TABS.Rate supplierRate in sortedRates)//FavorateRates
                    {



                        var currencyExchangeFactor = GetCurrencyExchangeFactor(supplierRate.PriceList.Currency, currency);

                        DTO.DTO_SupplyRate supplyRate = new TABS.DTO.DTO_SupplyRate();
                        supplyRate.Supplier = supplierRate.Supplier;
                        supplyRate.Rate = supplierRate;
                        supplyRate.Zone = supplierRate.Zone;
                        var key = string.Concat(supplierRate.ZoneID, supplierRate.SupplierID);
                        supplyRate.IsBlockAffected = blokcsByZoneSupplier.ContainsKey(key);

                        supplyRate.Normal = (double)supplierRate.Value * currencyExchangeFactor;
                        supplyRate.OffPeak = (double?)supplierRate.OffPeakRate * currencyExchangeFactor;
                        supplyRate.Weekend = (double?)supplierRate.WeekendRate * currencyExchangeFactor;
                        supplyRate.ServicesFlag = supplierRate.ServicesFlag;
                        supplierRate.BeginEffectiveDate = supplierRate.BeginEffectiveDate;
                        supplyRate.EndEffectiveDate = supplierRate.EndEffectiveDate;
                        supplyRate.ACD = 0;
                        supplyRate.ASR = 0;
                        supplyRate.Duration = 0;

                        // check if supplier already has another zone/rate
                        DTO.DTO_SupplyRate otherFound = null;
                        int indexFound = 0;

                        zoneRate.SupplierRates.Add(supplyRate);


                    }
                }
            }

            // Sort Supply Rates and Init Stats
            foreach (DTO.DTO_ZoneRate zoneRate in rates.Values)
            {
                zoneRate.SupplierRates.Sort();
                zoneRate.ACD = 0;
                zoneRate.ASR = 0;
                zoneRate.Duration = 0;
            }

            // Update Stats if necessary
            UpdateZoneRatesSupplierStats(IsStatsPeriodDefined, Days, ref StatsFrom, ref StatsTill, rates);

            // Return result
            return rates;
        }
        public static List<TABS.Rate> GetOurRates(string Customerid, bool AllCustomers, DateTime when, bool iscurrent)
        {
            List<TABS.Rate> Rates = new List<TABS.Rate>();
            System.Data.SqlClient.SqlConnection connection = (System.Data.SqlClient.SqlConnection)GetOpenedConnection();// TABS.DataHelper.GetOpenConnection();
            using (connection)
            {
                string CurrentandFutureCondition = string.Format("(r.EndEffectiveDate is null or (r.EndEffectiveDate > '{0}' and r.BeginEffectiveDate<r.EndEffectiveDate))", when.ToString("yyyy-MM-dd"));//"yyyy-MM-dd hh:mm:ss
                if (iscurrent == false)
                    CurrentandFutureCondition = string.Format("r.BeginEffectiveDate > '{0}' and (r.EndEffectiveDate is null or (r.EndEffectiveDate > '{0}' and r.BeginEffectiveDate<r.EndEffectiveDate))", when.ToString("yyyy-MM-dd"));

                string sql = "";
                if (AllCustomers)
                    sql = string.Format(@"Select z.name,getdate() as BeginEffectiveDate,r.EndEffectiveDate,r.Zoneid,min(rate),r.pricelistid,r.ServicesFlag,C.lastrate,r.RateID
                                from Rate r  With(nolock) inner join zone z With(nolock)  on r.zoneid=z.zoneid
                                inner join pricelist P With(nolock) on P.PriceListId=r.PriceListId
                                inner join Currency C With(nolock) on P.Currencyid=C.Currencyid
                                Where P.PriceListId=r.PriceListId and P.SupplierId='SYS' and
                                {0}
                                group by r.Zoneid,z.name,r.Rate,r.pricelistid,r.ServicesFlag,C.lastrate,r.EndEffectiveDate,r.RateID", CurrentandFutureCondition);
                else
                {
                    sql = string.Format(@"Select z.name,r.BeginEffectiveDate,r.EndEffectiveDate,r.Zoneid,r.rate,r.OffPeakRate,r.WeekendRate,r.pricelistid,r.ServicesFlag,C.lastrate,r.RateID
                                from Rate r  With(nolock) inner join zone z With(nolock)  on r.zoneid=z.zoneid
                                inner join pricelist P With(nolock) on P.PriceListId=r.PriceListId
                                inner join Currency C With(nolock) on P.Currencyid=C.Currencyid
                                 Where P.PriceListId=r.PriceListId and P.CustomerId='{1}' and
                                {0}      
                                --group by r.Zoneid,z.name,r.BeginEffectiveDate,r.Rate,r.pricelistid,r.ServicesFlag,C.lastrate,r.EndEffectiveDate", CurrentandFutureCondition, Customerid);//when.ToString("yyyy-MM-dd hh:mm:ss")
                }
                System.Data.IDbCommand command = connection.CreateCommand();
                command.CommandTimeout = 60;
                command.CommandText = sql;
                command.Connection = connection;
                System.Data.IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                int index = 0;
                string ZoneName = "";
                TABS.Zone zone = null;
                DateTime? zoneBED = null;
                while (reader.Read())
                {
                    index = 0;
                    TABS.Rate r = new TABS.Rate();
                    TABS.PriceList pricelist = new TABS.PriceList();
                    TABS.Currency currency = new TABS.Currency();
                    r.PriceList = pricelist;
                    r.PriceList.Currency = currency;
                    string DBZoneName = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                    if (!reader.IsDBNull(index))
                        zoneBED = reader.GetDateTime(index);
                    else
                        zoneBED = null;
                    r.BeginEffectiveDate = zoneBED;

                    index++;
                    if (!reader.IsDBNull(index))
                        r.EndEffectiveDate = reader.GetDateTime(index);
                    else
                        r.EndEffectiveDate = null;
                    index++;
                    if (DBZoneName != ZoneName)
                    {
                        zone = new TABS.Zone();
                        ZoneName = DBZoneName;
                        zone.CodeGroup = new TABS.CodeGroup();
                        zone.EffectiveCodes = new List<TABS.Code>();
                        zone.Supplier = new TABS.CarrierAccount();
                        zone.Supplier.CarrierAccountID = "SYS";
                        zone.BeginEffectiveDate = zoneBED;
                    }
                    r.Zone = zone;
                    r.Zone.Name = DBZoneName;
                    //r.PriceList.ID = !reader.IsDBNull(index) ? reader.GetInt32(index) : -1; index++;
                    r.Zone.ZoneID = !reader.IsDBNull(index) ? reader.GetInt32(index) : -1; index++;
                    r.Value = !reader.IsDBNull(index) ? reader.GetDecimal(index) : 0; index++;
                    r.OffPeakRate = !reader.IsDBNull(index) ? reader.GetDecimal(index) : 0; index++;
                    r.WeekendRate = !reader.IsDBNull(index) ? reader.GetDecimal(index) : 0; index++;

                    r.Change = TABS.Change.None;// reader.GetInt16(index); 
                    r.PriceList.ID = !reader.IsDBNull(index) ? reader.GetInt32(index) : -1; index++;
                    r.ServicesFlag = reader.GetInt16(index); index++;
                    r.PriceList.Currency.LastRate = !reader.IsDBNull(index) ? float.Parse(reader[index].ToString()) : -1; index++;
                    r.ID = reader.GetInt64(index); index++;
                    Rates.Add(r);
                }
            }
            return Rates;

        }
        public static IDbConnection GetOpenedConnection()
        {
            IDbConnection connection = new System.Data.SqlClient.SqlConnection(TABS.DataConfiguration.Default.Properties["connection.connection_string"].ToString());
            connection.Open();
            return connection;
        }

        protected static void GetStatsPeriod(bool isStatsFromTo, int days, DateTime statsFrom, DateTime statsTill)
        {
            DateTime TrafficStatsFrom = DateTime.Today;
            DateTime TrafficStatsTo = DateTime.Today;

            bool IsStatsPeriodDefined = isStatsFromTo;
            double Days = days;

            if (Days > 0 && !IsStatsPeriodDefined)
            {
                DateTime Now = DateTime.Now;
                TrafficStatsFrom = Now.AddDays(-1.0 * Days);
                TrafficStatsTo = Now;
            }

            if (IsStatsPeriodDefined)
            {
                TrafficStatsFrom = statsFrom;
                TrafficStatsTo = statsTill;
            }
        }

        protected static bool IsnewCustomer(string customerID)
        {
            var sql = string.Format("Select count(*) from pricelist with(nolock) where customerid ='{0}'", customerID);

            return (int)TABS.DataHelper.ExecuteScalar(sql) == 0;
        }
        protected static Dictionary<TABS.Zone, List<TABS.Rate>> GetRates(TABS.CarrierAccount customer, DateTime when)
        {
            Dictionary<TABS.Zone, List<TABS.Rate>> currentRates = new Dictionary<TABS.Zone, List<TABS.Rate>>();

            IList<TABS.Rate> rates = TABS.DataConfiguration.CurrentSession
                       .CreateQuery(string.Format(@"FROM Rate R 
                          WHERE     
                                R.PriceList.Supplier = :Supplier 
                            AND R.PriceList.Customer = :Customer
                            AND ((R.BeginEffectiveDate <= :when AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate > :when)) OR R.BeginEffectiveDate > :when)
                          ORDER BY 
                                R.BeginEffectiveDate ASC"))
                        .SetParameter("Supplier", TABS.CarrierAccount.SYSTEM)
                        .SetParameter("Customer", customer)
                        .SetParameter("when", when)
                        .List<TABS.Rate>();

            List<TABS.Rate> zoneRates;
            foreach (TABS.Rate rate in rates)
            {
                if (!currentRates.TryGetValue(rate.Zone, out zoneRates))
                {
                    zoneRates = new List<TABS.Rate>();
                    currentRates[rate.Zone] = zoneRates;
                }

                if (Nullable.Compare<DateTime>(rate.BeginEffectiveDate, rate.EndEffectiveDate) != 0)
                    zoneRates.Add(rate);
            }
            return currentRates;
        }
        public static IList<TABS.ToDConsideration> GetEffectiveTodConsideration(TABS.CarrierAccount customer, TABS.Zone zone, DateTime whenEffective)
        {
            NHibernate.ICriteria criteria =
                TABS.ObjectAssembler.CurrentSession.CreateCriteria(typeof(TABS.ToDConsideration))
                        .Add(Expression.Le("BeginEffectiveDate", whenEffective))
                       .Add(Expression.Or(
                        Expression.Ge("EndEffectiveDate", whenEffective),
                        new NullExpression("EndEffectiveDate")
                    )
                )
                .AddOrder(new Order("BeginEffectiveDate", false));
            if (customer != null) criteria = criteria.Add(Expression.Eq("Customer", customer));
            criteria = criteria.Add(Expression.Eq("Supplier", TABS.CarrierAccount.SYSTEM));
            if (zone != null) criteria.Add(Expression.Eq("Zone", zone));
            return criteria.List<TABS.ToDConsideration>();
        }
       
        public static List<TABS.DTO.DTO_EditablePlanningRate> FromRatePlan(TABS.RatePlan ratePlan, TABS.CarrierAccount SelectedCustomer, TABS.Currency SelectedCurrency, TABS.Components.RoutePool ratePool, bool IsStatsPeriodDefined, DateTime StatsFrom, DateTime StatsTill, int? days, DateTime BaseDate, int showAllRates, int rateCount, TABS.SupplierRatePolicy policy, bool IsFuture, Dictionary<TABS.Zone, List<TABS.Rate>> customerRates)
        {
            // get the rates as a list for the corresponding  rate plan 
            List<TABS.DTO.DTO_EditablePlanningRate> list = new List<TABS.DTO.DTO_EditablePlanningRate>(ratePlan.Rates.Count);

            //Dictionary<TABS.Zone, List<TABS.Rate>> customerRates = CustomrRate;


            //Dictionary<TABS.Zone, List<TABS.Rate>> customerRates = GetRates(SelectedCustomer, BaseDate);

            Dictionary<int, TABS.DTO.DTO_ZoneRate> planingrates = new Dictionary<int, TABS.DTO.DTO_ZoneRate>();

            //GetStatsPeriod(IsStatsPeriodDefined, (days != null ? (int)days : 0), statsFrom, statsTill);
            short? service = IsnewCustomer(SelectedCustomer.CarrierAccountID) ? SelectedCustomer.ServicesFlag : (short?)null;
            //planingrates =  TABS.DataHelper.GetPlaningRatesWithSupply(SelectedCustomer.CarrierAccountID, SelectedCurrency, null, 1000, service, IsStatsPeriodDefined, (int?)RadDays.Value.Value, StatsFrom.SelectedDate.Value, StatsTill.SelectedDate.Value, policy);

            planingrates = TABS.DataHelper.GetPlaningRatesWithSupply(SelectedCustomer, SelectedCurrency, IsStatsPeriodDefined, (int?)days, StatsFrom, StatsTill, policy, ratePool, IsFuture);

            // Get pending rates for this customer
            var pendingRates = TABS.DataConfiguration.CurrentSession.CreateQuery("FROM Rate R WHERE R.BeginEffectiveDate > :when AND R.EndEffectiveDate IS NULL AND R.PriceList.Customer = :customer")
                .SetParameter("when", DateTime.Now)
                .SetParameter("customer", SelectedCustomer)
                .List<TABS.Rate>()
                .ToDictionary(r => r.ZoneID);

            foreach (TABS.DTO.DTO_ZoneRate rate in planingrates.Values)
            {
                if (ratePlan.Rates.ContainsKey(rate.OurZone))
                {
                    TABS.PlaningRate pr = (TABS.PlaningRate)ratePlan.Rates[rate.OurZone];
                    TABS.DTO.DTO_EditablePlanningRate newRate = new TABS.DTO.DTO_EditablePlanningRate(pr
                        , SelectedCurrency
                        , customerRates.ContainsKey(pr.Zone) ? customerRates[pr.Zone] : new List<TABS.Rate>()
                        , ratePool.BaseDate);
                    TABS.Rate pendingRate = null;
                    // If a pending rate is found
                    if (pendingRates.TryGetValue(newRate.ZoneID, out pendingRate))
                    {
                        newRate.PendingRate = pendingRate;
                        // newRate.Value = pendingRate.Value;
                        // newRate.OffPeakRate = pendingRate.OffPeakRate;
                        // newRate.WeekendRate = pendingRate.WeekendRate;
                        // var factor = (decimal)(newRate.Curreny.LastRate / pendingRate.PriceListBase.Currency.LastRate);
                        // newRate.Value = decimal.Parse((newRate.Value.Value * factor).ToString("0.00000"));
                        // newRate.OffPeakRate = decimal.Parse(((newRate.OffPeakRate ?? 0) * factor).ToString("0.00000"));
                        // newRate.WeekendRate = decimal.Parse(((newRate.WeekendRate ?? 0) * factor).ToString("0.00000"));
                    }

                    newRate.ZoneStatsDays = (int?)days;
                    newRate.Policy = policy;
                    newRate.AllSupplyRates = rate.SupplierRates;
                    newRate.ValidSupplyRates = rate.SupplierRates;

                    if (showAllRates == 1)
                        newRate.RateCount = rateCount;
                    else
                        newRate.RateCount = newRate.ValidSupplyRates.Count;
                    list.Add(newRate);
                }
            }

            //// non supplied zones ( no service supplied ...)
            var noSuppliedPlanningZones = ratePlan.Rates.Keys.Except(list.Select(r => r.Zone));

            List<TABS.PlaningRate> newPlaningRates = new List<TABS.PlaningRate>();
            foreach (TABS.Zone zone in noSuppliedPlanningZones)
            {
                var rate = ratePlan.Rates[zone];
                //creating a new planing rate for a specific zone
                TABS.PlaningRate newPlaningRate = new TABS.PlaningRate();
                newPlaningRate.RatePlan = ratePlan;
                newPlaningRate.BeginEffectiveDate = rate.BeginEffectiveDate;
                newPlaningRate.EndEffectiveDate = rate.EndEffectiveDate;
                newPlaningRate.Zone = zone;
                newPlaningRate.ServicesFlag = rate.ServicesFlag;
                newPlaningRate.Value = rate.Value;
                newPlaningRate.OffPeakRate = rate.OffPeakRate;
                newPlaningRate.WeekendRate = rate.WeekendRate;
                newPlaningRates.Add(newPlaningRate);
            }

            // add it to the collection 
            foreach (TABS.PlaningRate rate in newPlaningRates)
            {
                TABS.DTO.DTO_EditablePlanningRate editable = new TABS.DTO.DTO_EditablePlanningRate(rate
                    , SelectedCurrency
                    , customerRates.ContainsKey(rate.Zone) ? customerRates[rate.Zone] : new List<TABS.Rate>()
                    , ratePool.BaseDate);
                editable.NewRate = 0;
                editable.NewOffPeakRate = 0;
                editable.NewOffPeakRate = 0;
                editable.RateCount = rateCount;
                editable.ZoneStatsDays = (int?)days;
                editable.AllSupplyRates = new List<TABS.DTO.DTO_SupplyRate>();
                // If a pending rate is found
                TABS.Rate pendingRate = null;
                if (pendingRates.TryGetValue(editable.ZoneID, out pendingRate))
                {
                    editable.PendingRate = pendingRate;
                }
                list.Add(editable);
            }

            var todList = GetEffectiveTodConsideration(SelectedCustomer, null, BaseDate);

            Dictionary<Zone, ToDConsideration> EffectiveTODs = new Dictionary<Zone, ToDConsideration>();
            foreach (var item in todList)
                EffectiveTODs[item.Zone] = item;

            foreach (var item in list)
            {
                item.HasTOD = EffectiveTODs.ContainsKey(item.Zone);
                item.Checked = item.NewRate != 0;
                try
                {
                    item.Notes = ratePlan.Rates[item.Zone].Notes;
                }
                catch { }
            }
            return list;
        }

        public static Dictionary<int, DTO.DTO_ZoneRate> GetPlaningRatesWithSupply(string CustomerID, Currency currency, int? ZoneID, double ExcludedRate, short? ServicesFlag, bool IsStatsPeriodDefined, int? Days, DateTime StatsFrom, DateTime StatsTill, TABS.SupplierRatePolicy SupplierRatePolicy)
        {
            var zones = ObjectAssembler.GetCurrentAndFutureZones(DateTime.Now);

            // This is a dictionary based on the Zone ID
            Dictionary<int, DTO.DTO_ZoneRate> rates = new Dictionary<int, DTO.DTO_ZoneRate>();
            Dictionary<int, Zone> supplierZones = new Dictionary<int, Zone>();

            // The no-zone Rate!
            DTO.DTO_ZoneRate noZoneRate = new TABS.DTO.DTO_ZoneRate();

            if (ServicesFlag != null) noZoneRate.ServicesFlag = ServicesFlag.Value;
            noZoneRate.SupplierRates = new List<TABS.DTO.DTO_SupplyRate>();

            IDbConnection connection = GetOpenConnection();
            IDbCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "bp_GetPlaningRatesWithSupply";

            // Add parameters
            if (CustomerID != null && CustomerID.Length > 0) AddParameter(command, "@CustomerID", CustomerID);
            if (ZoneID != null) AddParameter(command, "@ZoneID", ZoneID);
            if (ExcludedRate != 0) AddParameter(command, "@ExcludedRate", ExcludedRate);
            if (ServicesFlag != null) AddParameter(command, "@ServicesFlag", ServicesFlag);

            //currency ..
            AddParameter(command, "@CurrencyID", currency.Symbol);

            // Get a reader
            IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                int index = -1;
                index++; int? OurZoneID = null; if (!reader.IsDBNull(index)) OurZoneID = reader.GetInt32(index);
                index++; // skip the zoneName
                index++; double? OurRate = null; if (!reader.IsDBNull(index)) OurRate = reader.GetFloat(index);
                index++; double? OurOffPeakRate = null; if (!reader.IsDBNull(index)) OurOffPeakRate = reader.GetFloat(index);
                index++; double? OurWeekendRate = null; if (!reader.IsDBNull(index)) OurWeekendRate = reader.GetFloat(index);
                index++; short OurServicesFlag = 0; if (!reader.IsDBNull(index)) OurServicesFlag = reader.GetInt16(index);


                index++; string Supplier = (!reader.IsDBNull(index)) ? reader.GetString(index) : null;
                index++; int SupplierZoneID = (!reader.IsDBNull(index)) ? reader.GetInt32(index) : 0;
                index++; string SupplierZoneName = (!reader.IsDBNull(index)) ? reader.GetString(index) : null;
                index++; double? SupplierRate = null; if (!reader.IsDBNull(index)) SupplierRate = reader.GetFloat(index);
                index++; double? SupplierOffPeakRate = null; if (!reader.IsDBNull(index)) SupplierOffPeakRate = reader.GetFloat(index);
                index++; double? SupplierWeekendRate = null; if (!reader.IsDBNull(index)) SupplierWeekendRate = reader.GetFloat(index);
                index++; short SupplierServicesFlag = 0; if (!reader.IsDBNull(index)) SupplierServicesFlag = reader.GetInt16(index);
                index++; DateTime? EndEffectiveDate = null; if (!reader.IsDBNull(index)) EndEffectiveDate = reader.GetDateTime(index);
                index++; double? Duration = 0; if (!reader.IsDBNull(index)) Duration = (double)reader.GetDecimal(index);
                index++; double? ASR = 0; if (!reader.IsDBNull(index)) ASR = (double)reader.GetDecimal(index);
                index++; double? ACD = 0; if (!reader.IsDBNull(index)) ACD = (double)reader.GetDecimal(index);

                DTO.DTO_ZoneRate zoneRate = noZoneRate;
                if (OurZoneID != null && zones.ContainsKey(OurZoneID.Value))
                {
                    if (!rates.TryGetValue(OurZoneID.Value, out zoneRate))
                    {
                        zoneRate = new TABS.DTO.DTO_ZoneRate();
                        zoneRate.ServicesFlag = OurServicesFlag;
                        zoneRate.OurZone = zones[OurZoneID.Value];
                        zoneRate.SupplierRates = new List<DTO.DTO_SupplyRate>();
                        zoneRate.Normal = OurRate;/// LastRate;
                        zoneRate.OffPeak = OurOffPeakRate;// / LastRate;
                        zoneRate.Weekend = OurWeekendRate;// / LastRate;
                        rates.Add(OurZoneID.Value, zoneRate);
                    }
                }

                if (Supplier != null)
                {
                    DTO.DTO_SupplyRate supplyRate = new TABS.DTO.DTO_SupplyRate();
                    supplyRate.Supplier = CarrierAccount.All[Supplier];
                    supplyRate.Normal = SupplierRate;
                    supplyRate.OffPeak = SupplierOffPeakRate;
                    supplyRate.Weekend = SupplierWeekendRate;
                    supplyRate.ServicesFlag = SupplierServicesFlag;
                    supplyRate.EndEffectiveDate = EndEffectiveDate;
                    supplyRate.Duration = Duration;
                    supplyRate.ACD = ACD;
                    supplyRate.ASR = ASR;

                    Zone supplierZone = null;
                    if (!supplierZones.TryGetValue(SupplierZoneID, out supplierZone))
                    {
                        supplierZone = new Zone();
                        supplierZone.ZoneID = SupplierZoneID;
                        supplierZone.Name = SupplierZoneName;
                        supplierZones.Add(SupplierZoneID, supplierZone);
                    }
                    supplyRate.Zone = supplierZone;


                    // check if supplier already has another zone/rate
                    DTO.DTO_SupplyRate otherFound = null;
                    int indexFound = 0;
                    switch (SupplierRatePolicy)
                    {
                        // Higest rate from suppliers 
                        case SupplierRatePolicy.Highest_Rate:
                            foreach (DTO.DTO_SupplyRate otherRate in zoneRate.SupplierRates)
                            {
                                if (otherRate.Supplier == supplyRate.Supplier)
                                {
                                    otherFound = otherRate;
                                    break;
                                }
                                indexFound++;
                            }

                            if (otherFound != null)
                            {
                                if (otherFound.Normal.Value < supplyRate.Normal.Value)
                                    zoneRate.SupplierRates[indexFound] = supplyRate;
                            }
                            else
                                zoneRate.SupplierRates.Add(supplyRate);
                            break;
                        // Lowest Rate Per Supplier 
                        case SupplierRatePolicy.Lowest_Rate:
                            indexFound = 0;
                            foreach (DTO.DTO_SupplyRate otherRate in zoneRate.SupplierRates)
                            {
                                if (otherRate.Supplier == supplyRate.Supplier)
                                {
                                    otherFound = otherRate;
                                    break;
                                }
                                indexFound++;
                            }

                            if (otherFound != null)
                            {
                                if (otherFound.Normal.Value > supplyRate.Normal.Value)
                                    zoneRate.SupplierRates[indexFound] = supplyRate;
                            }
                            else
                                zoneRate.SupplierRates.Add(supplyRate);
                            break;
                        case SupplierRatePolicy.None:
                            zoneRate.SupplierRates.Add(supplyRate);
                            break;
                    }
                }
            }
            reader.Close();

            UpdateZoneRatesSupplierStats(IsStatsPeriodDefined, Days, ref StatsFrom, ref StatsTill, rates);

            foreach (DTO.DTO_ZoneRate zoneRate in rates.Values)
                zoneRate.SupplierRates.Sort();


            return rates;
        }



        private static void UpdateZoneRatesSupplierStats(bool IsStatsPeriodDefined, int? Days, ref DateTime StatsFrom, ref DateTime StatsTill, Dictionary<int, DTO.DTO_ZoneRate> rates)
        {
            DateTime TrafficStatsFrom = DateTime.Today;
            DateTime TrafficStatsTo = DateTime.Today;

            if (Days != null && Days > 0 && !IsStatsPeriodDefined)
            {
                DateTime Now = DateTime.Now;
                TrafficStatsFrom = Now.AddDays(Days != null ? -(double)Days : 0);
                TrafficStatsTo = Now;
            }

            if (IsStatsPeriodDefined)
            {
                TrafficStatsFrom = StatsFrom;
                TrafficStatsTo = StatsTill.AddDays(1).AddMilliseconds(-5);
            }

            if (Math.Abs(TrafficStatsFrom.Subtract(TrafficStatsTo).TotalSeconds) > 0)
            {
                //Updating the stats 
                List<Stats> stats = new List<Stats>();
                string SQL = string.Format(@"EXEC Sp_GetSupplierZoneStats
	                                    @FromDate = '{0:yyyy-MM-dd}',
	                                    @ToDate = '{1:yyyy-MM-dd HH:mm:ss}'", TrafficStatsFrom, TrafficStatsTo);

                DateTime Now = DateTime.Now;

                DateTime statsDateTime = Now.AddDays(Days != null ? -(double)Days : 0);

                using (IDataReader statsReader = TABS.DataHelper.ExecuteReader(SQL))
                {
                    while (statsReader.Read())
                    {
                        int index = -1;
                        Stats stat = new Stats();
                        index++; if (!statsReader.IsDBNull(index)) stat.SupplierID = statsReader.GetString(index);
                        index++; if (!statsReader.IsDBNull(index)) stat.OurZoneID = statsReader.GetInt32(index);
                        index++; if (!statsReader.IsDBNull(index)) stat.SupplierZoneID = statsReader.GetInt32(index);
                        index++; if (!statsReader.IsDBNull(index)) stat.DurationInMinutes = (double?)statsReader.GetDecimal(index);
                        index++; if (!statsReader.IsDBNull(index)) stat.ASR = (double?)statsReader.GetDecimal(index);
                        index++; if (!statsReader.IsDBNull(index)) stat.ACD = (double?)statsReader.GetDecimal(index);

                        stats.Add(stat);
                    }
                }

                Dictionary<string, Stats> statsDic = new Dictionary<string, Stats>();
                foreach (var stat in stats)
                    statsDic[string.Concat(stat.SupplierID, ",", stat.SupplierZoneID, ",", stat.OurZoneID)] = stat;

                foreach (var rate in rates.Values)
                {
                    foreach (var supplyRate in rate.SupplierRates)
                    {
                        Stats stat = null;
                        string key = string.Concat(supplyRate.Supplier.CarrierAccountID, ",", supplyRate.Zone.ZoneID, ",", rate.OurZone.ZoneID);
                        if (statsDic.ContainsKey(key))
                            stat = statsDic[key];

                        if (stat != null)
                        {
                            supplyRate.ASR = stat.ASR;
                            supplyRate.ACD = stat.ACD;
                            supplyRate.Duration = stat.DurationInMinutes;
                        }
                    }
                }
            }
            //updating stats
        }
        public static void UpdateZoneRatesSupplierStatsForZone(bool IsStatsPeriodDefined, int? Days, DateTime StatsFrom, DateTime StatsTill, int ZoneId, List<DTO.DTO_SupplyRate> rates)//Dictionary<int, DTO.DTO_ZoneRate> rates
        {
            DateTime TrafficStatsFrom = DateTime.Today;
            DateTime TrafficStatsTo = DateTime.Today;

            if (Days != null && Days > 0 && !IsStatsPeriodDefined)
            {
                DateTime Now = DateTime.Now;
                TrafficStatsFrom = Now.AddDays(Days != null ? -(double)Days : 0);
                TrafficStatsTo = Now;
            }

            if (IsStatsPeriodDefined)
            {
                TrafficStatsFrom = StatsFrom;
                TrafficStatsTo = StatsTill.AddDays(1).AddMilliseconds(-5);
            }

            if (Math.Abs(TrafficStatsFrom.Subtract(TrafficStatsTo).TotalSeconds) > 0)
            {
                //Updating the stats 
                List<Stats> stats = new List<Stats>();
                string SQL = string.Format(@"EXEC Sp_GetSupplierZoneStatsByZone
	                                    @FromDate = '{0:yyyy-MM-dd}',
	                                    @ToDate = '{1:yyyy-MM-dd HH:mm:ss}',
                                        @ZoneId='{2}'", TrafficStatsFrom, TrafficStatsTo, ZoneId);

                DateTime Now = DateTime.Now;

                DateTime statsDateTime = Now.AddDays(Days != null ? -(double)Days : 0);

                using (IDataReader statsReader = TABS.DataHelper.ExecuteReader(SQL))
                {
                    while (statsReader.Read())
                    {
                        int index = -1;
                        Stats stat = new Stats();
                        index++; if (!statsReader.IsDBNull(index)) stat.SupplierID = statsReader.GetString(index);
                        index++; if (!statsReader.IsDBNull(index)) stat.OurZoneID = statsReader.GetInt32(index);
                        index++; if (!statsReader.IsDBNull(index)) stat.SupplierZoneID = statsReader.GetInt32(index);
                        index++; if (!statsReader.IsDBNull(index)) stat.DurationInMinutes = (double?)statsReader.GetDecimal(index);
                        index++; if (!statsReader.IsDBNull(index)) stat.ASR = (double?)statsReader.GetDecimal(index);
                        index++; if (!statsReader.IsDBNull(index)) stat.ACD = (double?)statsReader.GetDecimal(index);

                        stats.Add(stat);
                    }
                }

                Dictionary<string, Stats> statsDic = new Dictionary<string, Stats>();
                foreach (var stat in stats)
                    statsDic[string.Concat(stat.SupplierID, ",", stat.SupplierZoneID, ",", stat.OurZoneID)] = stat;

                foreach (var supplyRate in rates)//rates.Values
                {
                    //foreach (var supplyRate in rate.SupplierRates)
                    //{
                    Stats stat = null;
                    string key = string.Concat(supplyRate.Supplier.CarrierAccountID, ",", supplyRate.Zone.ZoneID, ",", ZoneId);//rate.OurZone.ZoneID
                    if (statsDic.ContainsKey(key))
                        stat = statsDic[key];

                    if (stat != null)
                    {
                        supplyRate.ASR = stat.ASR;
                        supplyRate.ACD = stat.ACD;
                        supplyRate.Duration = stat.DurationInMinutes;
                    }
                    // }
                }
            }
            //updating stats
        }
        public static string SafeGetString(IDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            else
                return string.Empty;
        }

        public static Dictionary<int, string> GetZoneNamesByID(List<int> IDs)
        {
            string query = string.Format("SELECT ZoneID,Name FROM [Zone] WHERE ZoneID IN ({0})",string.Join(",",IDs.Select(id => id.ToString()).ToArray()));

            var reader = ExecuteReader(query);

            Dictionary<int, string> result = new Dictionary<int, string>();

            if (reader != null)
            {
                while (reader.Read())
                {
                    result.Add(reader.GetInt32(0), SafeGetString(reader, 1));
                }
            }

            return result;
        }

        public static List<DTO.DTO_ZoneCode> GetFutureZoneCodesNew(DateTime when,int From,int To,string SortExpression,string FilterExpression,out int RecordCount)
        {
            RecordCount = 0;


            FilterExpression = FilterExpression.Replace("[Code_ID]", "C.ID");
            FilterExpression = FilterExpression.Replace("[Code_Value]", "C.Code");
            FilterExpression = FilterExpression.Replace("[Code_BED]", "C.BeginEffectiveDate");
            FilterExpression = FilterExpression.Replace("[Code_EED]", "C.EndEffectiveDate");
            FilterExpression = FilterExpression.Replace("[Zone_Name]", "Z.Name");
            FilterExpression = FilterExpression.Replace("[Zone_BED]", "Z.BeginEffectiveDate");
            FilterExpression = FilterExpression.Replace("[Zone_EED]", "Z.EndEffectiveDate");
            FilterExpression = FilterExpression.Replace("[Zone_IsEffective]", "Z.IsEffective");
            FilterExpression = FilterExpression.Replace("[Code_IsEffective]", "C.IsEffective");

            Dictionary<Zone, List<Code>> abc = new Dictionary<Zone, List<Code>>();

            string query = @" With TEMP as (SELECT * FROM Zone Z with (Nolock,index(IX_Zone_SupplierID))
                            WHERE Z.SupplierID = 'SYS')

                            
                                SELECT  C.ID AS Code_ID
                                        ,C.Code AS Code_Value
                                        ,C.ZoneID AS ZoneID
                                        ,C.BeginEffectiveDate AS Code_BED
                                        ,C.EndEffectiveDate AS Code_EED
                                        ,Z.Name AS Zone_Name
                                        ,Z.CodeGroup
                                        ,Z.ServicesFlag
                                        ,Z.IsMobile
                                        ,Z.IsProper
                                        ,Z.IsSold
                                        ,Z.BeginEffectiveDate AS Zone_BED
                                        ,Z.EndEffectiveDate  AS Zone_EED
                                        ,Z.IsEffective AS Zone_IsEffective
                                        ,C.IsEffective AS Code_IsEffective
                                INTO #TEMP
                                FROM Code C with (Nolock,index(IX_Code_ZoneID))
                                INNER JOIN TEMP Z on (C.ZoneID = Z.ZoneID)
                                Where (C.EndEffectiveDate > @P3 OR C.EndEffectiveDate IS NULL) 
                                AND C.BeginEffectiveDate > @P3
                                AND {0};

                            SELECT COUNT(1) FROM #TEMP;
                            
                            WITH RESULT  AS
							(SELECT *,ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumberz FROM #TEMP)

                        SELECT * From RESULT
                        WHERE RowNumberz BETWEEN @P1 AND @P2";

            IDataReader reader = TABS.DataHelper.ExecuteReader(string.Format(query,FilterExpression,SortExpression),From,To, when);

            Dictionary<int, Zone> zonesByID = new Dictionary<int, Zone>();
            Dictionary<int, List<Code>> codesByZoneID = new Dictionary<int, List<Code>>();
            List<DTO.DTO_ZoneCode> results = new List<TABS.DTO.DTO_ZoneCode>();

            if (reader != null)
            {
                if (reader.Read())
                    RecordCount = int.Parse(reader[0].ToString());

                reader.NextResult();

                while (reader.Read())
                {
                    TABS.Code code = new Code(); int i = 0;
                    code.ID = reader.GetInt64(i); i++;
                    code.Value = DataHelper.SafeGetString(reader, i); i++;

                    int zoneid = reader.GetInt32(i); i++;

                    code.BeginEffectiveDate = reader.GetDateTime(i); i++;
                    code.EndEffectiveDate = reader.IsDBNull(i) ? new DateTime?() : reader.GetDateTime(i); i++;


                    Zone zone = new Zone();
                    zone.ZoneID = zoneid;
                    zone.Name = DataHelper.SafeGetString(reader, i); i++;
                    List<Code> codes;

                    if (abc.TryGetValue(zone, out codes))
                    {
                        code.Zone = zone;
                        //  abc[zone].Add(code);
                        codes.Add(code);
                        continue;
                    }

                    string codegroup = DataHelper.SafeGetString(reader, i).Trim(); i++;
                    zone.CodeGroup = string.IsNullOrEmpty(codegroup) ? CodeGroup.None : CodeGroup.All[codegroup];


                    zone.Supplier = TABS.CarrierAccount.SYSTEM; //i++; //Carrier AccountField ,Z.SupplierID



                    if (!reader.IsDBNull(i)) //Service Flag 
                        zone.ServicesFlag = reader.GetInt16(i); i++;


                    if (!reader.IsDBNull(i))
                        zone.IsMobile = reader.GetValue(i).ToString().ToUpper().Equals("N") ? false : true; i++;

                    if (!reader.IsDBNull(i))
                        zone.IsProper = reader.GetValue(i).ToString().ToUpper().Equals("N") ? false : true; i++;

                    if (!reader.IsDBNull(i))
                        zone.IsSold = reader.GetValue(i).ToString().ToUpper().Equals("N") ? false : true; i++;

                    zone.BeginEffectiveDate = reader.GetDateTime(i); i++;

                    zone.EndEffectiveDate = reader.IsDBNull(i) ? new DateTime?() : reader.GetDateTime(i); i++;

                    code.Zone = zone;
                    abc.Add(zone, new List<Code>() { code });
                }
            }

            var allCodes = abc.Values.SelectMany(c => c).ToList();

            //if (allCodes != null)
            return DTO.DTO_ZoneCode.Get(allCodes);


        }

        public static List<TABS.Zone> GetEffectiveZonesNew(DateTime when, int From, int To, string SortExpression, string FilterExpression, out int RecordCount)
        {
            RecordCount = 0;

            string newquery = @"                  SELECT Z.[ZoneID]
                                                        ,Z.[CodeGroup]
                                                        ,Z.[Name]
                                                        ,Z.[ServicesFlag]
                                                        ,Z.[IsMobile]
                                                        ,Z.[IsProper]
                                                        ,Z.[IsSold]
                                                        ,Z.[BeginEffectiveDate]
                                                        ,Z.[EndEffectiveDate]
                                                        INTO #TEMP
                                                    FROM ZONE Z with (Nolock,index(IX_Zone_SupplierID,IX_Zone_Dates)) 
                                                    WHERE Z.Supplierid='SYS' 
                                                    AND (Z.EndEffectiveDate > @P3 OR Z.EndEffectiveDate Is NULL) 
                                                    AND Z.BeginEffectiveDate <= @P3
                                                    AND {0};
                                                
                                                SELECT COUNT(1) FROM #TEMP;
                                                    
												WITH RESULT  AS
												(SELECT 
												*
												,ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumberz 
												FROM #TEMP)
												
												
                                                SELECT * From RESULT
                                                WHERE RowNumberz BETWEEN @P1 AND @P2;";

            List<Zone> result = new List<Zone>();

            IDataReader reader = TABS.DataHelper.ExecuteReader(string.Format(newquery, FilterExpression, SortExpression), From, To, when);

            if (reader != null)
            {
                if (reader.Read())
                    RecordCount = int.Parse(reader[0].ToString());

                reader.NextResult();

                int iZoneId = reader.GetOrdinal("ZoneID");
                int iCodeGroup = reader.GetOrdinal("CodeGroup");
                int iName = reader.GetOrdinal("Name");
                int iServicesFlag = reader.GetOrdinal("ServicesFlag");
                int iIsMobile = reader.GetOrdinal("IsMobile");
                int iIsProper = reader.GetOrdinal("IsProper");
                int iIsSold = reader.GetOrdinal("IsSold");
                int iZoneBED = reader.GetOrdinal("BeginEffectiveDate");
                int iZoneEED = reader.GetOrdinal("EndEffectiveDate");

                while (reader.Read())
                {

                    Zone z = new Zone();

                    z.ZoneID = reader.GetInt32(iZoneId);

                    z.Name = DataHelper.SafeGetString(reader, iName);

                    z.Supplier = TABS.CarrierAccount.SYSTEM;

                    if (!reader.IsDBNull(iServicesFlag))
                        z.ServicesFlag = reader.GetInt16(iServicesFlag);

                    if (!reader.IsDBNull(iIsMobile))
                        z.IsMobile = reader.GetValue(iIsMobile).ToString().ToUpper().Equals("N") ? false : true;

                    if (!reader.IsDBNull(iIsProper))
                        z.IsProper = reader.GetValue(iIsProper).ToString().ToUpper().Equals("N") ? false : true;

                    if (!reader.IsDBNull(iIsSold))
                        z.IsSold = reader.GetValue(iIsSold).ToString().ToUpper().Equals("N") ? false : true;

                    z.BeginEffectiveDate = reader.GetDateTime(iZoneBED);

                    z.EndEffectiveDate = reader.IsDBNull(iZoneEED) ? new DateTime?() : reader.GetDateTime(iZoneEED);

                    result.Add(z);

                }
            }
            return result;
        }

        public static List<DTO.DTO_ZoneCode> GetEffectiveZonesAndCodesNew(DateTime when, int From, int To, string SortExpression, string FilterExpression, out int RecordCount)
        {
            RecordCount = 0;



            FilterExpression = FilterExpression.Replace("[Code_ID]", "C.ID");
            FilterExpression = FilterExpression.Replace("[Code_Value]", "C.Code");
            FilterExpression = FilterExpression.Replace("[Code_BED]", "C.BeginEffectiveDate");
            FilterExpression = FilterExpression.Replace("[Code_EED]", "C.EndEffectiveDate");
            FilterExpression = FilterExpression.Replace("[Zone_Name]", "Z.Name");
            FilterExpression = FilterExpression.Replace("[Zone_BED]", "Z.BeginEffectiveDate");
            FilterExpression = FilterExpression.Replace("[Zone_EED]", "Z.EndEffectiveDate");

            string newquery = @"WITH Temp AS (SELECT * FROM ZONE Z with (Nolock,index(IX_Zone_SupplierID,IX_Zone_Dates)) 
                                                WHERE Z.Supplierid='SYS' 
                                                AND (Z.EndEffectiveDate > @P3 OR Z.EndEffectiveDate Is NULL) 
                                                AND Z.BeginEffectiveDate <= @P3)
                             

                                 SELECT  C.ID AS Code_ID
                                        ,C.Code AS Code_Value
                                        ,C.ZoneID AS ZoneID
                                        ,C.BeginEffectiveDate AS Code_BED
                                        ,C.EndEffectiveDate AS Code_EED
                                        ,Z.Name AS Zone_Name
                                        ,Z.CodeGroup
                                        ,Z.ServicesFlag
                                        ,Z.IsMobile
                                        ,Z.IsProper
                                        ,Z.IsSold
                                        ,Z.BeginEffectiveDate AS Zone_BED
                                        ,Z.EndEffectiveDate  AS Zone_EED
                                INTO #TEMP
                                FROM Code C with (Nolock,index(IX_Code_ZoneID))
                                INNER JOIN TEMP Z on (C.ZoneID = Z.ZoneID)
                                Where (C.EndEffectiveDate > @P3 OR C.EndEffectiveDate IS NULL) 
                                AND C.BeginEffectiveDate <= @P3
                                AND {0};

                            SELECT COUNT(1) FROM #TEMP;
                            
                            WITH RESULT  AS
							(SELECT *,ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumberz FROM #TEMP)

                        SELECT * From RESULT
                        WHERE RowNumberz BETWEEN @P1 AND @P2";

            List<DTO.DTO_ZoneCode> result = new List<TABS.DTO.DTO_ZoneCode>();
            Dictionary<int, Zone> Zones = new Dictionary<int, Zone>();

            IDataReader reader = TABS.DataHelper.ExecuteReader(string.Format(newquery, FilterExpression, SortExpression), From, To, when); 
            if (reader != null)
            {
                if (reader.Read())
                    RecordCount = int.Parse(reader[0].ToString());

                reader.NextResult();

                int iZoneId = reader.GetOrdinal("ZoneID");
                int iCodeGroup = reader.GetOrdinal("CodeGroup");
                int iName = reader.GetOrdinal("Zone_Name");
                int iServicesFlag = reader.GetOrdinal("ServicesFlag");
                int iIsMobile = reader.GetOrdinal("IsMobile");
                int iIsProper = reader.GetOrdinal("IsProper");
                int iIsSold = reader.GetOrdinal("IsSold");
                int iZoneBED = reader.GetOrdinal("Zone_BED");
                int iZoneEED = reader.GetOrdinal("Zone_EED");
                int iCodeID = reader.GetOrdinal("Code_ID");
                int iCodeValue = reader.GetOrdinal("Code_Value");
                int iCodeBED = reader.GetOrdinal("Code_BED");
                int iCodeEED = reader.GetOrdinal("Code_EED");

                while (reader.Read())
                {

                    //if (RecordCount == 0)
                    //    RecordCount = reader.GetInt32(iTotal);

                    int ZoneID = reader.GetInt32(iZoneId);
                    Zone z;
                    if (!Zones.TryGetValue(ZoneID, out z))
                    {
                        z = new Zone();

                        z.ZoneID = ZoneID;

                        z.Name = DataHelper.SafeGetString(reader, iName);

                        z.Supplier = TABS.CarrierAccount.SYSTEM;

                        if (!reader.IsDBNull(iServicesFlag))
                            z.ServicesFlag = reader.GetInt16(iServicesFlag);

                        if (!reader.IsDBNull(iIsMobile))
                            z.IsMobile = reader.GetValue(iIsMobile).ToString().ToUpper().Equals("N") ? false : true; 

                        if (!reader.IsDBNull(iIsProper))
                            z.IsProper = reader.GetValue(iIsProper).ToString().ToUpper().Equals("N") ? false : true;

                        if (!reader.IsDBNull(iIsSold))
                            z.IsSold = reader.GetValue(iIsSold).ToString().ToUpper().Equals("N") ? false : true;

                        z.BeginEffectiveDate = reader.GetDateTime(iZoneBED);

                        z.EndEffectiveDate = reader.IsDBNull(iZoneEED) ? new DateTime?() : reader.GetDateTime(iZoneEED);

                        Zones.Add(ZoneID, z);
                    }

                    Code c = new Code();
                    c.Zone = z;
                    c.ID = reader.GetInt64(iCodeID);
                    c.Value = DataHelper.SafeGetString(reader, iCodeValue); 
                    c.BeginEffectiveDate = reader.GetDateTime(iCodeBED);
                    c.EndEffectiveDate = reader.IsDBNull(iCodeEED) ? new DateTime?() : reader.GetDateTime(iCodeEED);

                    DTO.DTO_ZoneCode zc = new TABS.DTO.DTO_ZoneCode(c);
                    result.Add(zc); 
                }
            }

            return result;
        }

        public static List<DTO.DTO_ZoneCode> GetZoneCodesWithEEDNew(DateTime when, int From, int To, string SortExpression, string FilterExpression, out int RecordCount)
        {
            RecordCount = 0;

            FilterExpression = FilterExpression.Replace("[Code_ID]", "C.ID");
            FilterExpression = FilterExpression.Replace("[Code_Value]", "C.Code");
            FilterExpression = FilterExpression.Replace("[Code_BED]", "C.BeginEffectiveDate");
            FilterExpression = FilterExpression.Replace("[Code_EED]", "C.EndEffectiveDate");
            FilterExpression = FilterExpression.Replace("[Zone_Name]", "Z.Name");
            FilterExpression = FilterExpression.Replace("[Zone_BED]", "Z.BeginEffectiveDate");
            FilterExpression = FilterExpression.Replace("[Zone_EED]", "Z.EndEffectiveDate");
            FilterExpression = FilterExpression.Replace("[Zone_IsEffective]", "Z.IsEffective");
            FilterExpression = FilterExpression.Replace("[Code_IsEffective]", "C.IsEffective");


            string query = @" With TEMP as (SELECT * FROM Zone Z with (Nolock,index(IX_Zone_SupplierID))
                            WHERE Z.SupplierID = 'SYS')

                                 SELECT  C.ID AS Code_ID
                                        ,C.Code AS Code_Value
                                        ,C.ZoneID AS ZoneID
                                        ,C.BeginEffectiveDate AS Code_BED
                                        ,C.EndEffectiveDate AS Code_EED
                                        ,Z.Name AS Zone_Name
                                        ,Z.CodeGroup
                                        ,Z.ServicesFlag
                                        ,Z.IsMobile
                                        ,Z.IsProper
                                        ,Z.IsSold
                                        ,Z.BeginEffectiveDate AS Zone_BED
                                        ,Z.EndEffectiveDate  AS Zone_EED
                                        ,Z.IsEffective AS Zone_IsEffective
                                        ,C.IsEffective AS Code_IsEffective
                                INTO #TEMP
                                FROM Code C with (Nolock,index(IX_Code_ZoneID))
                                INNER JOIN TEMP Z on (C.ZoneID = Z.ZoneID)
                                Where (C.EndEffectiveDate IS NOT NULL AND C.EndEffectiveDate > @P3) 
                                AND {0};

                            SELECT COUNT(1) FROM #TEMP;
                            
                            WITH RESULT  AS
							(SELECT *,ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumberz FROM #TEMP)

                        SELECT * From RESULT
                        WHERE RowNumberz BETWEEN @P1 AND @P2";

            List<DTO.DTO_ZoneCode> result = new List<TABS.DTO.DTO_ZoneCode>();
            Dictionary<int, Zone> Zones = new Dictionary<int, Zone>();

            IDataReader reader = TABS.DataHelper.ExecuteReader(string.Format(query, FilterExpression, SortExpression), From, To, when);
            if (reader != null)
            {
                if (reader.Read())
                    RecordCount = int.Parse(reader[0].ToString());

                reader.NextResult();

                int iZoneId = reader.GetOrdinal("ZoneID");
                int iCodeGroup = reader.GetOrdinal("CodeGroup");
                int iName = reader.GetOrdinal("Zone_Name");
                int iServicesFlag = reader.GetOrdinal("ServicesFlag");
                int iIsMobile = reader.GetOrdinal("IsMobile");
                int iIsProper = reader.GetOrdinal("IsProper");
                int iIsSold = reader.GetOrdinal("IsSold");
                int iZoneBED = reader.GetOrdinal("Zone_BED");
                int iZoneEED = reader.GetOrdinal("Zone_EED");
                int iCodeID = reader.GetOrdinal("Code_ID");
                int iCodeValue = reader.GetOrdinal("Code_Value");
                int iCodeBED = reader.GetOrdinal("Code_BED");
                int iCodeEED = reader.GetOrdinal("Code_EED");

                while (reader.Read())
                {

                    //if (RecordCount == 0)
                    //    RecordCount = reader.GetInt32(iTotal);

                    int ZoneID = reader.GetInt32(iZoneId);
                    Zone z;
                    if (!Zones.TryGetValue(ZoneID, out z))
                    {
                        z = new Zone();

                        z.ZoneID = ZoneID;

                        z.Name = DataHelper.SafeGetString(reader, iName);

                        z.Supplier = TABS.CarrierAccount.SYSTEM;

                        if (!reader.IsDBNull(iServicesFlag))
                            z.ServicesFlag = reader.GetInt16(iServicesFlag);

                        if (!reader.IsDBNull(iIsMobile))
                            z.IsMobile = reader.GetValue(iIsMobile).ToString().ToUpper().Equals("N") ? false : true;

                        if (!reader.IsDBNull(iIsProper))
                            z.IsProper = reader.GetValue(iIsProper).ToString().ToUpper().Equals("N") ? false : true;

                        if (!reader.IsDBNull(iIsSold))
                            z.IsSold = reader.GetValue(iIsSold).ToString().ToUpper().Equals("N") ? false : true;

                        z.BeginEffectiveDate = reader.GetDateTime(iZoneBED);

                        z.EndEffectiveDate = reader.IsDBNull(iZoneEED) ? new DateTime?() : reader.GetDateTime(iZoneEED);

                        Zones.Add(ZoneID, z);
                    }

                    Code c = new Code();
                    c.Zone = z;
                    c.ID = reader.GetInt64(iCodeID);
                    c.Value = DataHelper.SafeGetString(reader, iCodeValue);
                    c.BeginEffectiveDate = reader.GetDateTime(iCodeBED);
                    c.EndEffectiveDate = reader.IsDBNull(iCodeEED) ? new DateTime?() : reader.GetDateTime(iCodeEED);

                    DTO.DTO_ZoneCode zc = new TABS.DTO.DTO_ZoneCode(c);
                    result.Add(zc);
                }
            }

            return result;
        }

        public static IEnumerable<RouteOverride> LoadRouteOverrides()
        {
            List<RouteOverride> routeOverrides = new List<RouteOverride>();
            Dictionary<int, SecurityEssentials.User> users = new Dictionary<int, SecurityEssentials.User>();
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                using (var reader = ExecuteReader(@"SELECT CustomerID, Code, IncludeSubCodes,ExcludedCodes, OurZoneID, RouteOptions, BlockedSuppliers, BeginEffectiveDate, EndEffectiveDate, UserID, [Updated] FROM RouteOverride where IsEffective = 'Y'
                                                AND EXISTS(SELECT CarrierAccountID From CarrierAccount WHERE CarrierAccount.IsDeleted = 'N'
                                                AND CarrierAccount.ActivationStatus > 0
                                                AND CarrierAccount.CarrierAccountID = RouteOverRide.CustomerID)"))
                {
                    while (reader.Read())
                    {
                        int index = -1;
                        index++; CarrierAccount customer = null;
                        if (!reader.IsDBNull(index))
                        {
                            if (CarrierAccount.All.ContainsKey(reader.GetString(index)))
                                customer = CarrierAccount.All[reader.GetString(index)];
                            else
                                customer = ObjectAssembler.Get<CarrierAccount>(reader.GetString(index));
                        }

                        index++; string code = reader.IsDBNull(index) ? null : reader.GetString(index);
                        index++; bool includeSubCodes = "Y".Equals(reader[index]);
                        index++; string excludedCodes = reader.IsDBNull(index) ? null : reader.GetString(index);
                        index++; Zone ourZone = null; if (!reader.IsDBNull(index)) Zone.OwnZones.TryGetValue(reader.GetInt32(index), out ourZone);
                        index++; string routeOptions = reader.IsDBNull(index) ? null : reader.GetString(index);
                        index++; string blockedSuppliers = reader.IsDBNull(index) ? null : reader.GetString(index);
                        index++; DateTime bed = reader.GetDateTime(index);
                        index++; DateTime? eed = reader.IsDBNull(index) ? null : (DateTime?)reader.GetDateTime(index);
                        index++; int userID = reader.IsDBNull(index) ? 0 : reader.GetInt32(index);
                        index++; DateTime updated = reader.IsDBNull(index) ? DateTime.MinValue : reader.GetDateTime(index);
                        SecurityEssentials.User user = null;
                        if (userID > 0 && !users.TryGetValue(userID, out user))
                        {
                            user = new SecurityEssentials.User();
                            session.Load(user, userID);
                            users[userID] = user;
                        }
                        if (customer != null && ourZone != null)
                        {
                            RouteOverride ro = new RouteOverride(customer, code, includeSubCodes, ourZone, routeOptions, blockedSuppliers, bed, eed, updated, user);
                            ro.IsTransiant = false;
                            ro.ExcludedCodes = excludedCodes;
                            routeOverrides.Add(ro);
                        }
                    }
                }
            }
            return routeOverrides;
        }

        #region Repricing Service QUEUE

        public static bool AddRePricingQueue(TABS.Components.CdrRepricingParameters parameters)
        {
            try
            {
                DateTime _From = parameters.From;
                DateTime _To = parameters.Till;
                object _CustomerID = parameters.Customer != null ? parameters.Customer.CarrierAccountID : null;
                object _SupplierID = parameters.Supplier != null ? parameters.Supplier.CarrierAccountID : null;
                char _GenerateTrafficStats = parameters.GenerateTrafficStats ? 'Y' : 'N';
                int _UserID = parameters.User.ID;
                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
                conn.ConnectionString = DataConfiguration.Default.Properties["connection.connection_string"].ToString();
                SqlCommand comm = new SqlCommand(string.Format(@"insert into RePricingQueue(QueueDate,[From],[To],CustomerID,SupplierID,[BatchSize],DailyChunks,GenerateTrafficStats,[User])
                                                                values(Convert(datetime,'{0}',103),Convert(datetime,'{1}',103),Convert(datetime,'{2}',103),'{3}','{4}',{5},{6},'{7}',{8})", DateTime.Now, parameters.From, parameters.Till, _CustomerID == null ? DBNull.Value : _CustomerID, _SupplierID == null ? DBNull.Value : _SupplierID, parameters.BatchSize, parameters.DailyChunks, _GenerateTrafficStats, _UserID), conn);
                comm.Connection = conn;
                conn.Open();
                int rowAffected = comm.ExecuteNonQuery();
                conn.Close();
                if (rowAffected > 0) return true;
                return false;
            }
            catch (Exception EX)
            {
                throw new Exception(string.Format("ERROR:Failed to Add RePricing Queue: {0}", EX));
                //log.Info(string.Format("ERROR:Failed to Update RePricing Queue ID={0}: {1}", QueueID, EX.Message));
                //Logger.SaveUserInfo(string.Format("ERROR:Failed to Update RePricing Queue ID={0}: {1}", QueueID, EX.Message));
            }
            return false;
        }

        public static int GetCurrentRunningRePricingQueues(out int WaitingInQueue)
        {
            //
            object CurrentQueueID = 0;
            object _WaitingInQueue = 0;
            try
            {


                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
                conn.ConnectionString = DataConfiguration.Default.Properties["connection.connection_string"].ToString();// System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                SqlCommand comm = new SqlCommand(@"select count(1) from RePricingQueue where CurrentProcessing='Y' AND IsFinished='N'", conn);
                SqlCommand comm1 = new SqlCommand(@"select Count(1) from RePricingQueue where CurrentProcessing='N' AND IsFinished='N'", conn);
                comm.Connection = conn;
                conn.Open();
                CurrentQueueID = comm.ExecuteScalar();
                _WaitingInQueue = comm1.ExecuteScalar();
                conn.Close();
                WaitingInQueue = int.Parse(_WaitingInQueue.ToString());
                return int.Parse(CurrentQueueID.ToString());

            }
            catch (Exception EX)
            {
                throw new Exception("ERROR:Failed to get RePricing Queue: {0}", EX);
            }
            WaitingInQueue = int.Parse(_WaitingInQueue.ToString());
            return int.Parse(CurrentQueueID.ToString());
        }

        #endregion

        #region Repricing Get Exchange Rate For bug 2774

        public static Dictionary<string, Currency> GetCurrencies()
        {
            Dictionary<string, Currency> diccurrencies = new Dictionary<string, Currency>();
            try
            {
                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
                conn.ConnectionString = DataConfiguration.Default.Properties["connection.connection_string"].ToString();
                SqlCommand comm = new SqlCommand(@"select CurrencyID,Name,IsMainCurrency,IsVisible,LastRate,LastUpdated,UserID,timestamp from Currency  WITH (NOLOCK)", conn);
                comm.Connection = conn;
                conn.Open();
                SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                int index = -1;

                while (reader.Read())
                {
                    Currency C = new Currency();
                    index = -1;
                    index++; C.Symbol = reader[index].ToString();
                    index++; C.Name = reader.IsDBNull(index) ? "" : reader[index].ToString();
                    index++; C.IsMainCurrency = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                    index++; //C.IsVisible = char.Parse(reader[index].ToString()) == 'Y' ? true : false;
                    index++; //C.LastRate = reader.IsDBNull(index) ? 0 : (float)float.Parse(reader[index].ToString());
                    index++; //C.LastUpdated = reader.IsDBNull(index) ? null : (DateTime?)DateTime.Parse(reader[index].ToString());
                    index++; index++; C.User = null;
                    diccurrencies[C.Symbol] = C;
                    C = null;
                }
                comm.Connection.Dispose(); conn.Close();
                reader.Dispose(); reader = null;
                comm.Dispose(); comm = null;
                return diccurrencies;
            }
            catch (Exception EX)
            {
                throw new Exception("DataHelper->GetCurrencies():- " + EX.Message);
            }
        }

        public static List<CurrencyExchangeRate> GetExchangeRates()
        {
            Dictionary<string, Currency> _Currencies = GetCurrencies();
            List<CurrencyExchangeRate> results = new List<CurrencyExchangeRate>();
            DateTime FromDate = new DateTime(1995, 1, 1);
            DateTime ToDate = new DateTime(DateTime.Now.Year + 50, 1, 1);
            try
            {
                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
                conn.ConnectionString = DataConfiguration.Default.Properties["connection.connection_string"].ToString();
                SqlCommand comm = new SqlCommand(@"select CurrencyExchangeRateID,CurrencyID,Rate,ExchangeDate from CurrencyExchangeRate C with (NOLOCK) where C.ExchangeDate >= '" + FromDate.ToString("yyyy-MM-dd") + "' and C.ExchangeDate<= '" + ToDate.ToString("yyyy-MM-dd") + "' ", conn);
                comm.Connection = conn;
                conn.Open();
                SqlDataReader reader = comm.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                int index = -1;
                int count = 0;
                while (reader.Read())
                {
                    CurrencyExchangeRate CurrExch = new CurrencyExchangeRate();
                    index = -1;
                    index++; CurrExch.CurrencyExchangeRateID = int.Parse(reader[index].ToString());
                    index++; CurrExch.Currency = _Currencies.ContainsKey(reader[index].ToString()) ? _Currencies[reader[index].ToString()] : null;
                    index++; CurrExch.Rate = reader.IsDBNull(index) ? 0 : float.Parse(reader[index].ToString());
                    index++; CurrExch.ExchangeDate = DateTime.Parse(reader[index].ToString());
                    if (CurrExch.Currency != null)
                        results.Add(CurrExch);
                    count++;
                    CurrExch = null;
                }
                reader.Dispose(); reader = null; comm.Connection.Dispose();
                conn.Close(); comm.Dispose(); comm = null;
                _Currencies = null;
                return results;
            }
            catch (Exception EX)
            {
                throw new Exception("DataHelper->GetExchangeRates():- " + EX.Message);
            }
        }

        #endregion
        protected class Stats
        {
            public string SupplierID { get; set; }
            public int? SupplierZoneID { get; set; }
            public int? OurZoneID { get; set; }
            public double? ASR { get; set; }
            public double? ACD { get; set; }
            public double? DurationInMinutes { get; set; }  


        }
    }
}