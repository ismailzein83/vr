using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TABS.BusinessEntities
{
    public class LogBO
    {
        static string connectionString;

        protected static SqlConnection _Connection;
        protected static SqlConnection Connection
        {
            get
            {
                if (_Connection == null)
                {
                    lock (TABS.ObjectAssembler.SyncRoot)
                    {
                        if (connectionString == null)
                        {
                            //Helper.Log.Info("Connection String Initialized for Log Viewer");
                            connectionString = TABS.Components.AdoNetAppender.GlobalConnectionString;
                        }
                    }
                    _Connection = new SqlConnection(connectionString);
                }
                return _Connection;
            }
        }

        public static System.Data.SqlClient.SqlConnection GetLogConnection()
        {
            return new System.Data.SqlClient.SqlConnection(TABS.Components.AdoNetAppender.GlobalConnectionString);
        }

        public static DataTable GetCodePreparationLog(string date)
        {
            DataTable dataTable = new DataTable();

            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.Append(@"
                SET ANSI_NULLS ON; 
                SET ANSI_PADDING ON; 
                SET ANSI_WARNINGS ON; 
                SET ARITHABORT ON; 
                SET CONCAT_NULL_YIELDS_NULL ON; 
                SET QUOTED_IDENTIFIER ON; 
                SET NUMERIC_ROUNDABORT OFF;  
            ");

            sql.AppendFormat(@"SELECT TOP 200 ID, Date, Thread, Context, Level, Logger, Message, Exception FROM [LOG] WITH (NOLOCK, INDEX(IX_LogDate)) WHERE Logger like '{0}' ", "Code preperation procedure");
            sql.AppendFormat(" AND [Date] >= '{0}' ", date);

            sql.Append(" ORDER BY ID DESC ");

            using (SqlDataAdapter da = new SqlDataAdapter(sql.ToString(), GetLogConnection()))
            {
                da.Fill(dataTable);
            }

            return dataTable;

        }

        public static DataTable GetRepricingLog()
        {
            DataTable dataTable = new DataTable();

            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.Append(@"
          SET ANSI_NULLS ON; 
          SET ANSI_PADDING ON; 
          SET ANSI_WARNINGS ON; 
          SET ARITHABORT ON; 
          SET CONCAT_NULL_YIELDS_NULL ON; 
          SET QUOTED_IDENTIFIER ON; 
          SET NUMERIC_ROUNDABORT OFF;  
        ");

            if (TABS.Components.Engine.IsRepricingRunning)
            {
                sql.AppendFormat(@"SELECT TOP 200 ID, Date, Thread, Context, Level, Logger, Message, Exception FROM [LOG] WITH (NOLOCK, INDEX(IX_LogDate)) WHERE Logger like '{0}' ", TABS.Components.Engine.RepricingLoggerName);
                sql.AppendFormat(" AND [Date] >= '{0}' ", TABS.Components.Engine.RepricingParameters.Started.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                sql.AppendFormat(@"SELECT TOP 200 ID, Date, Thread, Context, Level, Logger, Message, Exception FROM [LOG] WITH (NOLOCK, INDEX(IX_LogDate)) WHERE Logger like '{0}' ", TABS.Components.Engine.RepricingLoggerName);
                sql.AppendFormat(" AND [Date] >= '{0}' ", DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            sql.Append(" ORDER BY ID DESC ");

            using (SqlDataAdapter da = new SqlDataAdapter(sql.ToString(), GetLogConnection()))
            {
                da.Fill(dataTable);
            }

            return dataTable;
        }

        public static DataTable GetLogs(int logCount, DateTime? fromDate, DateTime? toDate, string logLevel, string message, string logger, string order)
        {
            DataTable dataTable = new DataTable();

        System.Text.StringBuilder sql = new System.Text.StringBuilder();
        sql.Append(@"
              SET ANSI_NULLS ON; 
              SET ANSI_PADDING ON; 
              SET ANSI_WARNINGS ON; 
              SET ARITHABORT ON; 
              SET CONCAT_NULL_YIELDS_NULL ON; 
              SET QUOTED_IDENTIFIER ON; 
              SET NUMERIC_ROUNDABORT OFF;  
        ");

        sql.Append(@"SELECT TOP " + logCount + @" ID,Date,Thread,Context,Level,Logger,Message,Exception FROM [LOG] WITH (NOLOCK, INDEX(IX_LogDate)) WHERE 1=1 ");// Context='" + Server.MachineName + "' ");

        if (fromDate.HasValue) sql.AppendFormat(@" AND Date >= '{0}'", ((DateTime)fromDate).ToString("yyyy-MM-dd HH:mm:ss"));
        if (toDate.HasValue) sql.AppendFormat(@" AND Date <= '{0}'", ((DateTime)toDate).ToString("yyyy-MM-dd HH:mm:ss"));
        if (!string.IsNullOrEmpty(logLevel)) sql.AppendFormat(" AND Level LIKE '{0}'", logLevel);
        if (!string.IsNullOrEmpty(message)) sql.AppendFormat(" AND Message LIKE '{0}'", message);
        if (!string.IsNullOrEmpty(logger)) sql.AppendFormat("AND Logger like '{0}'", logger);
        if (!TABS.SystemParameter.WindowsEventLogger.BooleanValue.Value) sql.Append("AND Logger NOT LIKE 'Windows_Events'");
        if (!string.IsNullOrEmpty(order)) sql.AppendFormat(" ORDER BY {0} DESC", order);


        using (SqlDataAdapter da = new SqlDataAdapter(sql.ToString(), Connection))
        {
            da.Fill(dataTable);
        }

        //foreach (DataRow row in dataTable.Rows)
        //{
        //    if (!string.IsNullOrEmpty(row["Message"].ToString()))
        //    {
        //        row["Message"] = Helper.EscapeHtmlGtLt(row["Message"].ToString());
        //    }
        //}

        return dataTable;
        }
    }
}
