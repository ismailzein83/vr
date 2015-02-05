using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data.SQL
{
    public class QueueDataManager : BaseSQLDataManager, IQueueDataManager
    {
        public QueueDataManager()
            : base(ConfigurationManager.AppSettings["QueueingDBConnStringKey"] ?? "QueueingDBConnString")
        {
        }

//        public void EnqueueItem(string queueName, byte[] item)
//        {
//            string serviceName = String.Format("{0}Service", queueName);
//            ExecuteNonQuerySP("SendBrokerMessage", "RequestService", serviceName, "AsyncContract", "AsyncRequest", item);
//        }

//        public void DequeueItem(string queueName, TimeSpan waitTime, Action<byte[]> onItemReady)
//        {
//            queueName = String.Format("{0}Queue", queueName);
//            ExecuteReaderText(String.Format(query_Dequeue, queueName),
//                (reader) =>
//                {
//                    while (reader.Read())
//                    {
//                        Guid conversationHandle = (Guid)reader["conversation_handle"];
//                        string messageType = reader["message_type_name"] as string;
//                        if (messageType == "http://schemas.microsoft.com/SQL/ServiceBroker/EndDialog")
//                        {
//                            ExecuteNonQueryText(query_EndConversion, (cmd) =>
//                            {
//                                cmd.Parameters.Add(new SqlParameter("@ConversationHandle", conversationHandle));
//                            });
//                        }
//                        else
//                            onItemReady((byte[])reader["message_body"]);
//                    }
//                },
//                (cmd) =>
//                {
//                     cmd.Parameters.Add(new SqlParameter("@count", 1));
//                     cmd.Parameters.Add(new SqlParameter("@timeout", waitTime == TimeSpan.MaxValue ? -1 : (int)waitTime.TotalMilliseconds));
//                     cmd.CommandTimeout = 0; //honor the RECEIVE timeout, whatever it is.
//                });
//        }

//        const string query_Dequeue = @"
//            waitfor( 
//                RECEIVE top (@count) conversation_handle,service_name,message_type_name,message_body,message_sequence_number 
//                FROM [{0}] 
//                    ), timeout @timeout";

//        const string query_EndConversion = "END CONVERSATION @ConversationHandle;";


        ////public void EnqueueItem(string queueName, byte[] item)
        ////{            
        ////    ExecuteNonQueryText(String.Format(query_Enqueue, queueName),
        ////        (cmd) =>
        ////        {
        ////            cmd.Parameters.Add(new SqlParameter("@Content", item));
        ////        });
        ////}

        ////public void DequeueItem(string queueName, TimeSpan waitTime, Action<byte[]> onItemReady)
        ////{
        ////    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
        ////    {
        ////        ExecuteReaderText(String.Format(query_Dequeue, queueName),
        ////            (reader) =>
        ////            {
        ////                if (reader.Read())
        ////                {
        ////                    long id = (long)reader["ID"];
        ////                    onItemReady((byte[])reader["Content"]);
        ////                    reader.Close();
        ////                    ExecuteNonQueryText(String.Format(query_DeleteFromQueue, queueName),
        ////                        (cmd) =>
        ////                        {
        ////                            cmd.Parameters.Add(new SqlParameter("@ID", id));
        ////                        });
        ////                }
        ////            },
        ////            null);
        ////        scope.Complete();
        ////    }
        ////}

        const string query_Enqueue = @" 
                                        INSERT INTO queue.Queue{0}
                                               ([ID], [Content])
                                         VALUES
                                               (@ID, @Content)";

        const string query_Dequeue = @" 
                                        BEGIN TRAN
                                        DECLARE @ID uniqueidentifier, @Content varbinary(max)

                                        SELECT TOP (1) @ID = [ID] , @Content = [Content] 
                                         FROM queue.Queue{0} WITH (updlock, readpast) 
                                            WHERE LockedByProcessID IS NULL OR @RunningProcessesIDs NOT LIKE '%,' + CONVERT(VARCHAR, LockedByProcessID) + ',%'
                                            ORDER BY timestamp

                                        IF @ID IS NOT NULL 
	                                        UPDATE queue.Queue{0} SET LockedByProcessID = @ProcessID WHERE ID = @ID
                                        COMMIT; 

                                        SELECT @ID ID, @Content Content WHERE @ID IS NOT NULL";

        const string query_DeleteFromQueue = "DELETE queue.Queue{0} WHERE ID = @ID";
        
        public int GetQueue(string queueName)
        {
            return 1;
        }

        public void EnqueueItem(int queueId, Guid itemId, byte[] item)
        {
            ExecuteNonQueryText(String.Format(query_Enqueue, queueId),
               (cmd) =>
               {
                   cmd.Parameters.Add(new SqlParameter("@ID", itemId));
                   cmd.Parameters.Add(new SqlParameter("@Content", item));
               });
        }

        public QueueItem DequeueItem(int queueId, int currentProcessId, IEnumerable<int> runningProcessesIds)
        {
            StringBuilder processIdsBuilder = new StringBuilder();
            processIdsBuilder.Append(',');
            foreach(var processId in runningProcessesIds)
            {
                processIdsBuilder.Append(processId);
                processIdsBuilder.Append(',');
            }
            return GetItemText(String.Format(query_Dequeue, queueId),
                    (reader) =>
                    {
                        return new QueueItem
                        {
                            ItemId = (Guid)reader["ID"],
                            Content = (byte[])reader["Content"]
                        };

                    },
                    (cmd) =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@RunningProcessesIDs", processIdsBuilder.ToString()));
                        cmd.Parameters.Add(new SqlParameter("@ProcessID", currentProcessId));
                    });
        }

        public void DeleteItem(int queueId, Guid itemId)
        {
            ExecuteNonQueryText(String.Format(query_DeleteFromQueue, queueId),
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@ID", itemId));
                });
        }
    }
}
