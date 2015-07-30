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
    public class QueueItemDataManager : BaseSQLDataManager, IQueueItemDataManager
    {
        static QueueItemDataManager()
        {
            s_query_EnqueueItemAndHeaderTemplate = String.Format(@"{0} 
                                                                   {1} 
                                                                   {2}", 
                                                                  query_EnqueueItemTemplate,
                                                                  query_InsertQueueActivationIfNeededTemplate, 
                                                                  query_EnqueueItemHeaderTemplate);
        }
        public QueueItemDataManager()
            : base(GetConnectionStringName("QueueItemDBConnStringKey", "QueueItemDBConnString"))
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

        public void CreateQueue(int queueId)
        {
            ExecuteNonQueryText(String.Format(query_CreateQueueTable, queueId), null);
        }

        public long GenerateItemID()
        {
            return (long)ExecuteScalarSP("queue.sp_QueueItemIDGen_GenerateID");
        }

        public void EnqueueItem(int queueId, long itemId, long executionFlowTriggerItemId, byte[] item, string description, QueueItemStatus queueItemStatus)
        {
            ExecuteEnqueueItemQuery(String.Format(s_query_EnqueueItemAndHeaderTemplate, queueId, itemId, "null"), item, executionFlowTriggerItemId, description, queueItemStatus);
        }

        public void EnqueueItem(Dictionary<int, long> targetQueuesItemsIds, int sourceQueueId, long sourceItemId, long executionFlowTriggerItemId, byte[] item, string description, QueueItemStatus queueItemStatus)
        {
            
            StringBuilder queryItemBuilder = new StringBuilder();
            StringBuilder queryItemHeaderBuilder = new StringBuilder();
            StringBuilder queryInsertQueueActivationBuilder = new StringBuilder();
            foreach(var targetQueueItemId in targetQueuesItemsIds)
            {
                int queueId = targetQueueItemId.Key;
                long itemId = targetQueueItemId.Value;
                queryItemBuilder.AppendLine(String.Format(query_EnqueueItemTemplate, queueId, itemId));
                queryInsertQueueActivationBuilder.AppendLine(String.Format(query_InsertQueueActivationIfNeededTemplate, queueId));
                queryItemHeaderBuilder.AppendLine(String.Format(query_EnqueueItemHeaderTemplate, 
                    queueId, 
                    itemId, 
                    sourceQueueId != queueId ? sourceItemId.ToString() : "null"));
            }

            string query = String.Format(@" {0} 
                                            {1} 
                                            {2}", 
                                            queryItemBuilder, 
                                            queryInsertQueueActivationBuilder,
                                            queryItemHeaderBuilder);

            ExecuteEnqueueItemQuery(query, item, executionFlowTriggerItemId, description, queueItemStatus);
        }

        void ExecuteEnqueueItemQuery(string query, byte[] item, long executionFlowTriggerItemID, string description, QueueItemStatus queueItemStatus)
        {
            query = String.Format(@" BEGIN TRANSACTION 
                                     {0}
                                     COMMIT;",
                                            query);

            ExecuteNonQueryText(query,
               (cmd) =>
               {
                   cmd.Parameters.Add(new SqlParameter("@ExecutionFlowTriggerItemID", executionFlowTriggerItemID));
                   cmd.Parameters.Add(new SqlParameter("@Content", item));
                   cmd.Parameters.Add(new SqlParameter("@Description", description));
                   cmd.Parameters.Add(new SqlParameter("@Status", (int)queueItemStatus));
               });
        }

        public QueueItem DequeueItem(int queueId, int currentProcessId, IEnumerable<int> runningProcessesIds, bool singleQueueReader)
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
                            ItemId = (long)reader["ID"],
                            ExecutionFlowTriggerItemId = (long)reader["ExecutionFlowTriggerItemID"],
                            Content = (byte[])reader["Content"]
                        };

                    },
                    (cmd) =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@RunningProcessesIDs", processIdsBuilder.ToString()));
                        cmd.Parameters.Add(new SqlParameter("@ProcessID", currentProcessId));
                        cmd.Parameters.Add(new SqlParameter("@SingleQueueReader", singleQueueReader));
                    });
        }

        public void DeleteItem(int queueId, long itemId)
        {
            ExecuteNonQueryText(String.Format(query_DeleteFromQueue, queueId),
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@ID", itemId));
                });
        }

        public QueueItemHeader GetHeader(long itemId, int queueId)
        {
            return GetItemSP("queue.sp_QueueItemHeader_GetByID", QueueItemHeaderMapper, itemId, queueId);
        }

        public List<QueueItemHeader> GetHeaders(IEnumerable<int> queueIds, IEnumerable<QueueItemStatus> statuses, DateTime dateFrom, DateTime dateTo)
        {
            return GetItemsSP("queue.sp_QueueItemHeader_GetFiltered", QueueItemHeaderMapper, queueIds == null ? null : string.Join(",", queueIds.Select(n => ((int)n).ToString()).ToArray())
                , statuses == null ? null : string.Join(",", statuses.Select(n => ((int)n).ToString()).ToArray()),dateFrom,dateTo);
        }

        public void UpdateHeaderStatus(long itemId, QueueItemStatus queueItemStatus)
        {
            ExecuteNonQuerySP("queue.sp_QueueItemHeader_UpdateStatus", itemId, (int)queueItemStatus);
        }

        public void UpdateHeader(long itemId, QueueItemStatus queueItemStatus, int retryCount, string errorMessage)
        {
            ExecuteNonQuerySP("queue.sp_QueueItemHeader_Update", itemId, (int)queueItemStatus, retryCount, errorMessage);
        }
        
        public Dictionary<int, long> GetQueueIDsHavingNewItems(long afterQueueActivationId)
        {
            Dictionary<int, long> updatedQueues = new Dictionary<int, long>();
            ExecuteReaderSP("queue.sp_QueueActivation_GetUpdatedQueueIds",
                (reader) =>
                {
                    while (reader.Read())
                        updatedQueues.Add((int)reader["QueueID"], (long)reader["MaxID"]);
                },
                afterQueueActivationId);
            return updatedQueues;
        }

        #region Private Methods

        private QueueItemHeader QueueItemHeaderMapper(IDataReader reader)
        {
            return new QueueItemHeader
            {
                ItemId = (long)reader["ItemID"],
                QueueId = (int)reader["QueueID"],
                ExecutionFlowTriggerItemId = GetReaderValue<long>(reader, "ExecutionFlowTriggerItemID"),
                SourceItemId = GetReaderValue<long>(reader, "SourceItemID"),
                Description = reader["Description"] as string,
                Status = (QueueItemStatus)reader["Status"],
                RetryCount = GetReaderValue<int>(reader, "RetryCount"),
                ErrorMessage = reader["ErrorMessage"] as string,
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                LastUpdatedTime = GetReaderValue<DateTime>(reader, "LastUpdatedTime")
            };
        }

        #endregion

        #region Queries

        const string query_CreateQueueTable = @"CREATE TABLE [queue].[QueueItem_{0}](
									                                                [ID] [bigint] NOT NULL,
									                                                [Content] [varbinary](max) NOT NULL,
                                                                                    [ExecutionFlowTriggerItemID] [bigint] NOT NULL,
									                                                [LockedByProcessID] [int] NULL,
									                                                 CONSTRAINT [PK_QueueItem_{0}] PRIMARY KEY CLUSTERED 
									                                                (
										                                                [ID] ASC
									                                                ));";

        static string s_query_EnqueueItemAndHeaderTemplate;

        const string query_EnqueueItemTemplate = @" DECLARE @IsEmptyInitialy_{0} Bit
                                                    IF NOT EXISTS (SELECT TOP 1 null FROM queue.QueueItem_{0})
                                                       SET @IsEmptyInitialy_{0} = 1
                                                    ELSE
                                                       SET @IsEmptyInitialy_{0} = 0

                                                     INSERT INTO queue.QueueItem_{0}
                                                           ([ID], [Content], [ExecutionFlowTriggerItemID])
                                                     VALUES
                                                           ({1}, @Content, @ExecutionFlowTriggerItemID)
                                                         ";

        const string query_EnqueueItemHeaderTemplate = @" INSERT INTO [queue].[QueueItemHeader]
                                                                   ([QueueID]
                                                                   ,[ItemID]
                                                                   ,ExecutionFlowTriggerItemID
		                                                           ,[SourceItemID]
                                                                   ,[Description]
                                                                   ,[Status]
                                                                   ,CreatedTime
                                                                   ,LastUpdatedTime)
                                                             VALUES
                                                                   ({0}
                                                                   ,{1}
                                                                   ,@ExecutionFlowTriggerItemID
                                                                   ,{2}
                                                                   ,@Description
                                                                   ,@Status
                                                                   ,GETDATE()
                                                                   ,GETDATE())";

        const string query_InsertQueueActivationIfNeededTemplate = @"IF @IsEmptyInitialy_{0} = 1
                                                                        INSERT INTO [queue].[QueueActivation]
                                                                           ([QueueID])
		                                                                VALUES
                                                                           ({0})";


        const string query_Dequeue = @" 
                                        BEGIN TRAN
                                        DECLARE @ID bigint, @Content varbinary(max), @ExecutionFlowTriggerItemID bigint

                                        IF @SingleQueueReader = 1
                                        BEGIN
                                            DECLARE @LockedItemID bigint
            
                                            --Check if any item is currently locked by another reader
                                            UPDATE queue.QueueItem_{0} WITH (tablock)
                                            SET @LockedItemID = ID
                                            WHERE @RunningProcessesIDs LIKE '%,' + CONVERT(VARCHAR, LockedByProcessID) + ',%'

                                            --if NO item locked by another reader, select the first item in the queue table
                                            IF @LockedItemID IS NULL
                                                SELECT TOP (1) @ID = [ID] , @Content = [Content], @ExecutionFlowTriggerItemID = ExecutionFlowTriggerItemID 
                                                FROM queue.QueueItem_{0}
                                                ORDER BY ID
                                        END
                                        ELSE
                                        BEGIN
                                            SELECT TOP (1) @ID = [ID] , @Content = [Content], @ExecutionFlowTriggerItemID = [ExecutionFlowTriggerItemID]
                                            FROM queue.QueueItem_{0} WITH (updlock, readpast) 
                                            WHERE LockedByProcessID IS NULL OR @RunningProcessesIDs NOT LIKE '%,' + CONVERT(VARCHAR, LockedByProcessID) + ',%'
                                            ORDER BY ID
                                        END

                                        IF @ID IS NOT NULL 
	                                        UPDATE queue.QueueItem_{0} SET LockedByProcessID = @ProcessID WHERE ID = @ID
                                        COMMIT; 

                                        SELECT @ID ID, @Content Content, @ExecutionFlowTriggerItemID ExecutionFlowTriggerItemID WHERE @ID IS NOT NULL";

        const string query_DeleteFromQueue = "DELETE queue.QueueItem_{0} WHERE ID = @ID";     

        #endregion

    }
}
