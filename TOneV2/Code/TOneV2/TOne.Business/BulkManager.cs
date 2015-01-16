using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Configuration;
using System.Data.SqlClient;

namespace TOne.Business
{    
    public class BulkManager
    {
        #region Constants

        public const string MAIN_TABLE_NAME = "Billing_CDR_Main";
        public const string INVALID_TABLE_NAME = "Billing_CDR_Invalid";
        public const string SALE_TABLE_NAME = "Billing_CDR_Sale";
        public const string COST_TABLE_NAME = "Billing_CDR_Cost";
        public const string TRAFFICSTATS_TABLE_NAME = "TrafficStats";
        public const string DAILYTRAFFICSTATS_TABLE_NAME = "TrafficStatsDaily";

        #endregion

        #region Singleton

        static BulkManager _instance;
        public static BulkManager Instance
        {
            get
            {
                return _instance;
            }
        }

        static BulkManager()
        {
            _instance = new BulkManager();
        }

        #endregion

        string _connectionString;
        private BulkManager()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["CDRTargetDBConnString"].ConnectionString;
        }

        Dictionary<string, Queue<AsyncBulkInsert>> _asyncBulkInsertQueues = new Dictionary<string, Queue<AsyncBulkInsert>>();


        #region temp

        //public void WriteAsync(DataTable data, EventWaitHandle waitHandle)
        //{
        //    WriteAsync(data.TableName, data, waitHandle);        
        //}

        //public void WriteAsync(string tableName, DataTable data, EventWaitHandle waitHandle)
        //{
        //    if (data.Rows.Count == 0)
        //    {
        //        waitHandle.Set();
        //        return;
        //    }
        //    AsyncBulkInsert asyncBulkInsert = new AsyncBulkInsert
        //    {
        //        TableName = tableName,
        //        Data = data,
        //        WaitHandle = waitHandle
        //    };
        //    lock (this)
        //    {
        //        Queue<AsyncBulkInsert> queue;
        //        if (!_asyncBulkInsertQueues.TryGetValue(tableName, out queue))
        //        {
        //            queue = new Queue<AsyncBulkInsert>();
        //            _asyncBulkInsertQueues.Add(tableName, queue);
        //            Thread thread = new Thread(new ParameterizedThreadStart(ProcessAsyncBulks));
        //            thread.Start(tableName);
        //        }
        //        queue.Enqueue(asyncBulkInsert);
        //    }
        //}

        //void ProcessAsyncBulks(object prmTableName)
        //{
        //    string tableName = prmTableName as string;
        //    Queue<AsyncBulkInsert> queue = null;
        //    lock (this)
        //    {
        //        queue = _asyncBulkInsertQueues[tableName];
        //    }

        //    while (true)
        //    {
        //        AsyncBulkInsert asyncBulkInsert = null;
        //        lock (this)
        //        {
        //            if (queue.Count > 0)
        //                asyncBulkInsert = queue.Dequeue();
        //        }
        //        if (asyncBulkInsert != null)
        //        {
        //            try
        //            {
        //                Write(asyncBulkInsert.TableName, asyncBulkInsert.Data);
        //                Console.WriteLine();
        //                Console.WriteLine(String.Format("Insert to table '{0}' completed", asyncBulkInsert.TableName));
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(String.Format("Error occurred while inserting data to to table '{0}'. {1}", asyncBulkInsert.TableName, ex.Message));
        //                throw;
        //            }
        //            finally
        //            {
        //                asyncBulkInsert.WaitHandle.Set();
        //            }
        //        }
        //        lock (this)
        //        {
        //            if (queue.Count == 0)
        //                return;
        //        }
        //    }
        //}

        #endregion

        static Dictionary<string, bool> s_tableNamesStatus = new Dictionary<string, bool>();
        public static void ExecuteActionWithTableLock(string tableName, Action action)
        {
            bool isTaken = false;
            do
            {
                lock (s_tableNamesStatus)
                {
                    bool isLockedByOther;
                    if (!s_tableNamesStatus.TryGetValue(tableName, out isLockedByOther))
                    {
                        isLockedByOther = false;
                        s_tableNamesStatus.Add(tableName, isLockedByOther);
                    }
                    if (!isLockedByOther)
                    {
                        isTaken = true;
                        s_tableNamesStatus[tableName] = true;
                    }
                }
                if (isTaken)
                {
                    action();
                    s_tableNamesStatus[tableName] = false;
                }
                else
                    Thread.Sleep(1000);
            }
            while (!isTaken);
        }

        public void Write(string tableName, DataTable table)
        {
            if (table.Rows.Count == 0)
                return;
            //ExecuteActionWithTableLock(tableName,
            //    () =>
            //    {
            DateTime start = DateTime.Now;
            var options = (tableName == "TrafficStats" ? SqlBulkCopyOptions.KeepNulls : SqlBulkCopyOptions.KeepIdentity);// |
            //  SqlBulkCopyOptions.TableLock |
            //   SqlBulkCopyOptions.UseInternalTransaction;
            using (var conn = GetOpenConnection())
            {
            //    conn.Open();
                //using (var transaction = conn.BeginTransaction())
                //{
                    //using (
            SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, options, null);//)
                    //{
                        bulkCopy.DestinationTableName = tableName;
                        foreach (DataColumn column in table.Columns)
                            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                        bulkCopy.BulkCopyTimeout = 10 * 60; // 10 minutes
                        bulkCopy.BatchSize = 10000;

                        bulkCopy.WriteToServer(table);

                        //transaction.Commit();
                    //}
                        conn.Close();
                //}

                        //Console.WriteLine("{0}: {1} records inserted to table {2} in {3}", DateTime.Now, table.Rows.Count, tableName, (DateTime.Now - start));
            }
            table.Dispose();
            //});
        }

        SqlConnection GetOpenConnection()
        {
            SqlConnection connection = new System.Data.SqlClient.SqlConnection(_connectionString);
            connection.Open();
            //return connection;            
            //IDbConnection connection = DataConfiguration.Default.SessionFactory.ConnectionProvider.GetConnection();
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

            return connection;
        }
       

        #region private Classes

        private class AsyncBulkInsert
        {
            public string TableName { get; set; }
            public DataTable Data { get; set; }
            public EventWaitHandle WaitHandle { get; set; }
        }

        #endregion
    }   

}
