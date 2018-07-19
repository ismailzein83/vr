using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data.SQL
{
    public class QueueItemDataManager : BaseSQLDataManager, IQueueItemDataManager
    {

        #region ctor
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        static QueueItemDataManager()
        {
            _columnMapper.Add("Entity.ItemId", "ItemID");
            _columnMapper.Add("Entity.ExecutionFlowTriggerItemId", "ExecutionFlowTriggerItemID");
            _columnMapper.Add("Entity.SourceItemId", "SourceItemID");
            _columnMapper.Add("Entity.Description", "Description");
            _columnMapper.Add("Entity.Status", "Status");
            _columnMapper.Add("Entity.RetryCount", "RetryCount");
            _columnMapper.Add("Entity.ErrorMessage", "ErrorMessage");
            _columnMapper.Add("Entity.CreatedTime", "CreatedTime");
            _columnMapper.Add("Entity.LastUpdatedTime", "LastUpdatedTime");


            s_query_EnqueueItemAndHeaderTemplate = String.Format(@"{0} 
                                                                   {1}",
                                                                  query_EnqueueItemTemplate,
                                                                  query_EnqueueItemHeaderTemplate);
        }
        public QueueItemDataManager()
            : base(GetConnectionStringName("QueueItemDBConnStringKey", "QueueItemDBConnString"))
        {
        }

        #endregion

        #region Public Methods

        public void CreateQueue(int queueId)
        {
        }

        public long GenerateItemID()
        {
            return (long)ExecuteScalarSP("queue.sp_QueueItemIDGen_GenerateID");
        }

        public void EnqueueItem(QueueActivatorType queueActivatorType, int queueId, long itemId, Guid? dataSourceId, string batchDescription, DateTime batchStart, DateTime batchEnd, long executionFlowTriggerItemId,
            byte[] item, string description, QueueItemStatus queueItemStatus)
        {
            ExecuteEnqueueItemQuery(queueActivatorType, String.Format(s_query_EnqueueItemAndHeaderTemplate, queueId, itemId, "null"), dataSourceId, batchDescription, batchStart, batchEnd, item, executionFlowTriggerItemId, description, queueItemStatus);
        }

        public void EnqueueItem(QueueActivatorType queueActivatorType, Dictionary<int, long> targetQueuesItemsIds, int sourceQueueId, long sourceItemId, Guid? dataSourceId, string batchDescription, DateTime batchStart,
            DateTime batchEnd, long executionFlowTriggerItemId, byte[] item, string description, QueueItemStatus queueItemStatus)
        {
            StringBuilder queryItemBuilder = new StringBuilder();
            StringBuilder queryItemHeaderBuilder = new StringBuilder();
            foreach (var targetQueueItemId in targetQueuesItemsIds)
            {
                int queueId = targetQueueItemId.Key;
                long itemId = targetQueueItemId.Value;
                queryItemBuilder.AppendLine(String.Format(query_EnqueueItemTemplate, queueId, itemId));
                queryItemHeaderBuilder.AppendLine(String.Format(query_EnqueueItemHeaderTemplate,
                    queueId,
                    itemId,
                    sourceQueueId != queueId ? sourceItemId.ToString() : "null"));
            }

            string query = String.Format(@" {0} 
                                            {1}",
                                            queryItemBuilder,
                                            queryItemHeaderBuilder);

            ExecuteEnqueueItemQuery(queueActivatorType, query, dataSourceId, batchDescription, batchStart, batchEnd, item, executionFlowTriggerItemId, description, queueItemStatus);
        }

        public QueueItem DequeueItem(int queueId, int currentProcessId, IEnumerable<int> runningProcessesIds, int? maximumConcurrentReaders)
        {
            StringBuilder queryBuilder = new StringBuilder(query_Dequeue);
            if (maximumConcurrentReaders.HasValue)
            {
                queryBuilder.Replace("#VALIDATEMAXIMUMREADERS#", query_DequeueMaximumReadersValidation);
            }
            else
            {
                queryBuilder.Replace("#VALIDATEMAXIMUMREADERS#", "");
            }
            queryBuilder.Replace("#RUNNINGPROCESSIDS#", String.Join(",", runningProcessesIds));
            return GetItemText(String.Format(queryBuilder.ToString(), queueId),
                    QueueItemMapper,
                    (cmd) =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@ProcessID", currentProcessId));
                        if (maximumConcurrentReaders.HasValue)
                            cmd.Parameters.Add(new SqlParameter("@MaximumConcurrentReaders", maximumConcurrentReaders.Value));
                    });
        }

        public QueueItem DequeueItem(int queueId, Guid activatorInstanceId)
        {
            string query = @"SELECT TOP 1 ID, Content, ExecutionFlowTriggerItemID, DataSourceID, BatchDescription, BatchStart, BatchEnd
                            FROM queue.QueueItem WITH(NOLOCK)
                            WHERE QueueID = @QueueID AND ActivatorID = @ActivatorID AND ISNULL([IsSuspended], 0) = 0
                            ORDER BY ID";
            return GetItemText(query,
                QueueItemMapper,
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@QueueID", queueId));
                    cmd.Parameters.Add(new SqlParameter("@ActivatorID", activatorInstanceId));
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
                , statuses == null ? null : string.Join(",", statuses.Select(n => ((int)n).ToString()).ToArray()), dateFrom, dateTo);
        }

        public void UpdateHeaderStatus(long itemId, QueueItemStatus queueItemStatus)
        {
            ExecuteNonQuerySP("queue.sp_QueueItemHeader_UpdateStatus", itemId, (int)queueItemStatus);
        }

        public void UpdateHeader(long itemId, QueueItemStatus queueItemStatus, int retryCount, string errorMessage)
        {
            ExecuteNonQuerySP("queue.sp_QueueItemHeader_Update", itemId, (int)queueItemStatus, retryCount, errorMessage);
        }

        public void UnlockItem(int queueId, long itemId, bool isSuspended)
        {
            ExecuteNonQueryText(String.Format(query_UnlockItem, queueId),
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@ItemID", itemId));
                    cmd.Parameters.Add(new SqlParameter("@IsSuspended", isSuspended));
                });
        }

        public List<ItemExecutionFlowInfo> GetItemExecutionFlowInfo(List<long> itemIds)
        {
            List<ItemExecutionFlowInfo> result = new List<ItemExecutionFlowInfo>();

            ExecuteReaderSPCmd("queue.sp_QueueItemHeader_GetItemsExecutionStatus", (reader) =>
            {
                while (reader.Read())
                {
                    ItemExecutionFlowInfo item = new ItemExecutionFlowInfo()
                    {
                        ItemId = (long)reader["ItemID"],
                        ExecutionFlowTriggerItemId = GetReaderValue<long>(reader, "ExecutionFlowTriggerItemID"),
                        Status = (ItemExecutionFlowStatus)reader["Status"]
                    };
                    result.Add(item);
                }
            }, (cmd) =>
            {
                var dtParameter = new SqlParameter("@ItemIds", SqlDbType.Structured);
                dtParameter.Value = BuildItemIdsTable(itemIds);
                cmd.Parameters.Add(dtParameter);
            });

            return result;
        }

        public Vanrise.Entities.BigResult<QueueItemHeader> GetQueueItemsHeader(Vanrise.Entities.DataRetrievalInput<List<long>> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySPCmd("queue.sp_QueueItemHeader_CreateTempByFiltered", (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@TempTableName", tempTableName));
                    var dtParameter = new SqlParameter("@ItemIds", SqlDbType.Structured);
                    dtParameter.Value = BuildItemIdsTable(input.Query);
                    cmd.Parameters.Add(dtParameter);
                });
            };

            return RetrieveData(input, createTempTableAction, QueueItemHeaderMapper, _columnMapper);
        }

        #endregion

        #region Private Methods
        void ExecuteEnqueueItemQuery(QueueActivatorType queueActivatorType, string query, Guid? dataSourceId, string batchDescription, DateTime batchStart, DateTime batchEnd, byte[] item, long executionFlowTriggerItemID, string description, QueueItemStatus queueItemStatus)
        {
            query = String.Format(@" --BEGIN TRANSACTION 
                                     {0}
                                     --COMMIT;",
                                             queueActivatorType == QueueActivatorType.Normal ? query.Replace("#QUEUEITEMTABLE#", "QueueItem") : query.Replace("#QUEUEITEMTABLE#", "SummaryQueueItem"));

            ExecuteNonQueryText(query,
               (cmd) =>
               {
                   cmd.Parameters.Add(new SqlParameter("@BatchStart", ToDBNullIfDefaultOrInvalid(batchStart)));
                   cmd.Parameters.Add(new SqlParameter("@BatchEnd", ToDBNullIfDefaultOrInvalid(batchEnd)));
                   cmd.Parameters.Add(new SqlParameter("@DataSourceID", dataSourceId));
                   cmd.Parameters.Add(new SqlParameter("@BatchDescription", batchDescription));
                   cmd.Parameters.Add(new SqlParameter("@ExecutionFlowTriggerItemID", executionFlowTriggerItemID));
                   cmd.Parameters.Add(new SqlParameter("@Content", item));
                   cmd.Parameters.Add(new SqlParameter("@Description", description));
                   cmd.Parameters.Add(new SqlParameter("@Status", (int)queueItemStatus));
               });
        }

        private DataTable BuildItemIdsTable(List<long> itemIds)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemId", typeof(int));
            dt.BeginLoadData();

            foreach (var itemId in itemIds)
            {
                DataRow dr = dt.NewRow();
                dr["ItemId"] = itemId;
                dt.Rows.Add(dr);
            }

            dt.EndLoadData();
            return dt;
        }
        #endregion

        #region Mappers
        private QueueItemHeader QueueItemHeaderMapper(IDataReader reader)
        {
            return new QueueItemHeader
            {
                ItemId = (long)reader["ItemID"],
                QueueId = (int)reader["QueueID"],
                ExecutionFlowTriggerItemId = GetReaderValue<long>(reader, "ExecutionFlowTriggerItemID"),
                DataSourceID = GetReaderValue<Guid?>(reader, "DataSourceID"),
                BatchDescription = reader["BatchDescription"] as string,
                SourceItemId = GetReaderValue<long>(reader, "SourceItemID"),
                Description = reader["Description"] as string,
                Status = (QueueItemStatus)reader["Status"],
                RetryCount = GetReaderValue<int>(reader, "RetryCount"),
                ErrorMessage = reader["ErrorMessage"] as string,
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                LastUpdatedTime = GetReaderValue<DateTime>(reader, "LastUpdatedTime")
            };
        }

        private QueueItem QueueItemMapper(IDataReader reader)
        {
            return new QueueItem
            {
                ItemId = (long)reader["ID"],
                ExecutionFlowTriggerItemId = (long)reader["ExecutionFlowTriggerItemID"],
                DataSourceID = GetReaderValue<Guid?>(reader, "DataSourceID"),
                BatchDescription = reader["BatchDescription"] as string,
                BatchStart = GetReaderValue<DateTime>(reader, "BatchStart"),
                BatchEnd = GetReaderValue<DateTime>(reader, "BatchEnd"),
                Content = (byte[])reader["Content"]
            };
        }

        private PendingQueueItemInfo PendingQueueItemInfoMapper(IDataReader reader)
        {
            return new PendingQueueItemInfo
            {
                QueueItemId = (long)reader["ID"],
                ExecutionFlowTriggerItemID = (long)reader["ExecutionFlowTriggerItemID"],
                QueueId = (int)reader["QueueID"],
                ActivatorInstanceId = GetReaderValue<Guid?>(reader, "ActivatorID"),
                BatchStart = GetReaderValue<DateTime>(reader, "BatchStart"),
                BatchEnd = GetReaderValue<DateTime>(reader, "BatchEnd")
            };
        }

        #endregion

        #region Queries

        static string s_query_EnqueueItemAndHeaderTemplate;

        const string query_EnqueueItemTemplate = @" 
                                                     INSERT INTO queue.#QUEUEITEMTABLE#
                                                           ([ID], QueueID, DataSourceID, BatchDescription, [BatchStart], [BatchEnd], [Content], [ExecutionFlowTriggerItemID])
                                                     VALUES
                                                           ({1}, {0}, @DataSourceID, @BatchDescription, @BatchStart, @BatchEnd, @Content, @ExecutionFlowTriggerItemID)
                                                         ";

        const string query_EnqueueItemHeaderTemplate = @" INSERT INTO [queue].[QueueItemHeader]
                                                                   ([QueueID]
                                                                   ,[ItemID]
                                                                   ,ExecutionFlowTriggerItemID
                                                                   ,DataSourceID
                                                                   ,BatchDescription
		                                                           ,[SourceItemID]
                                                                   ,[Description]
                                                                   ,[Status]
                                                                   ,CreatedTime
                                                                   ,LastUpdatedTime)
                                                             VALUES
                                                                   ({0}
                                                                   ,{1}
                                                                   ,@ExecutionFlowTriggerItemID
                                                                   ,@DataSourceID
                                                                   ,@BatchDescription
		                                                           ,{2}
                                                                   ,@Description
                                                                   ,@Status
                                                                   ,GETDATE()
                                                                   ,GETDATE())";




        const string query_UnlockItem = @"UPDATE queue.QueueItem SET LockedByProcessID = NULL, [IsSuspended] = @IsSuspended WHERE [ID] = @ItemID";

        const string query_Dequeue = @"                                         
                                        DECLARE @ID bigint, @IsLocked bit
                                        
                                        SELECT TOP 1 @ID = ID FROM [queue].[QueueItem] WITH(NOLOCK)
			                                      WHERE QueueID = {0} 
                                                        AND 
                                                        (LockedByProcessID IS NULL OR LockedByProcessID NOT IN (#RUNNINGPROCESSIDS#)) 
                                                        AND 
                                                        ISNULL([IsSuspended], 0) = 0 
			                                      ORDER BY ID                                       
                                        IF @ID IS NOT NULL
                                        BEGIN
                                            UPDATE [queue].[QueueItem]
                                            SET LockedByProcessID = @ProcessID,
                                                @IsLocked = 1
                                            WHERE 
                                                ID = @ID
                                                AND 
                                                (LockedByProcessID IS NULL OR LockedByProcessID NOT IN (#RUNNINGPROCESSIDS#)) 
                                                AND 
                                                ISNULL([IsSuspended], 0) = 0        
                                              
                                            #VALIDATEMAXIMUMREADERS#
                                        END

                                        SELECT ID, Content, ExecutionFlowTriggerItemID, DataSourceID, BatchDescription, BatchStart, BatchEnd FROM [queue].[QueueItem_{0}] WITH(NOLOCK) WHERE ID = @ID AND ISNULL(@IsLocked, 0) = 1";

        const string query_DequeueMaximumReadersValidation = @"
                                                            IF ((SELECT COUNT(*) FROM [queue].[QueueItem_{0}] WITH (NOLOCK)
                                                            WHERE LockedByProcessID IN (#RUNNINGPROCESSIDS#) AND ISNULL([IsSuspended], 0) = 0
                                                            ) > @MaximumConcurrentReaders )
                                                            BEGIN
                                                                UPDATE [queue].[QueueItem_{0}]
                                                                SET LockedByProcessID = NULL,
                                                                    @IsLocked = 0
                                                                WHERE 
                                                                    ID = @ID                                                     
                                                            END
                                                            ";

        const string query_DeleteFromQueue = "DELETE queue.QueueItem WHERE ID = @ID";

        const string query_DeleteItemsFromQueue = "DELETE queue.SummaryQueueItem WHERE ID IN ({1})";

        const string query_SetItemsSuspended = @"UPDATE queue.SummaryQueueItem SET [IsSuspended] = 1 WHERE ID IN ({1})";

        const string query_GetSummaryBatchesByBatchStart = @"SELECT TOP(@NbOfRows) ID, Content, ExecutionFlowTriggerItemID, DataSourceID, BatchDescription, BatchStart, BatchEnd
                                                            FROM queue.SummaryQueueItem 
                                                            WHERE QueueID = {0} AND [BatchStart] = @BatchStart AND ISNULL([IsSuspended], 0) = 0
                                                            ORDER BY ID ";

        #endregion

        public IEnumerable<QueueItem> DequeueSummaryBatches(int queueId, DateTime batchStart, int nbOfBatches)
        {
            return GetItemsText(String.Format(query_GetSummaryBatchesByBatchStart, queueId), QueueItemMapper, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@BatchStart", batchStart));
                    cmd.Parameters.Add(new SqlParameter("@NbOfRows", nbOfBatches));
                });
        }

        public void DeleteItems(int queueId, IEnumerable<long> itemsIds)
        {
            ExecuteNonQueryText(string.Format(query_DeleteItemsFromQueue, queueId, String.Join(",", itemsIds)), null);
        }

        public void UpdateHeaderStatuses(IEnumerable<long> itemsIds, QueueItemStatus queueItemStatus)
        {
            ExecuteNonQuerySP("[queue].[sp_QueueItemHeader_UpdateStatuses]", String.Join(",", itemsIds), (int)queueItemStatus);
        }

        public void UpdateHeaders(IEnumerable<long> itemsIds, QueueItemStatus status, int retryCount, string errorMessage)
        {
            ExecuteNonQuerySP("[queue].[sp_QueueItemHeader_UpdateMultiple]", String.Join(",", itemsIds), (int)status, retryCount, errorMessage);
        }

        public void SetItemsSuspended(int queueId, IEnumerable<long> itemsIds)
        {
            ExecuteNonQueryText(string.Format(query_SetItemsSuspended, queueId, String.Join(",", itemsIds)), null);
        }

        public void GetPendingQueueItems(int maxNbOfPendingItems, Func<PendingQueueItemInfo, bool> onPendingQueueItemReady)
        {
            bool isFinalRow = false;
            ExecuteReaderSP("[queue].[sp_QueueItem_GetInfo]", (reader) =>
            {
                while (reader.Read() && !isFinalRow)
                    isFinalRow = onPendingQueueItemReady(PendingQueueItemInfoMapper(reader));
            }, maxNbOfPendingItems);
        }

        public void SetQueueItemsActivatorInstances(List<PendingQueueItemInfo> pendingQueueItemsToUpdate)
        {
            foreach (var pendingQueueItemInfo in pendingQueueItemsToUpdate)
            {
                if (!pendingQueueItemInfo.ActivatorInstanceId.HasValue)
                    throw new NullReferenceException(String.Format("pendingQueueItemInfo.ActivatorInstanceId. Item ID '{0}'", pendingQueueItemInfo.QueueItemId));
                ExecuteNonQuerySP("[queue].[sp_QueueItem_UpdateActivatorInstance]", pendingQueueItemInfo.QueueItemId, pendingQueueItemInfo.ActivatorInstanceId.Value);
            }
        }

        public List<SummaryBatch> GetSummaryBatches()
        {
            string query = @"SELECT DISTINCT QueueID, BatchStart, BatchEnd
	                        FROM [queue].[SummaryQueueItem] with(nolock)
	                        WHERE IsNULL(IsSuspended, 0) = 0";
            return GetItemsText(query,
                (reader) =>
                {
                    return new SummaryBatch
                    {
                        QueueId = (int)reader["QueueID"],
                        BatchStart = (DateTime)reader["BatchStart"],
                        BatchEnd = (DateTime)reader["BatchEnd"]
                    };
                },
                null);
        }

        public bool HasPendingQueueItems(List<int> queueIdsToCheck, DateTime from, DateTime to, bool onlyAssignedQueues)
        {
            string queueIdsToCheckAsString = null;
            if (queueIdsToCheck != null)
                queueIdsToCheckAsString = string.Join<int>(",", queueIdsToCheck);

            object hasPendingItems;
            ExecuteNonQuerySP("[queue].[sp_QueueItem_HasPendingItems]", out hasPendingItems, queueIdsToCheckAsString, from, to, onlyAssignedQueues);
            return (bool)hasPendingItems;
        }

        public bool HasPendingSummaryQueueItems(List<int> queueIdsToCheck, DateTime from, DateTime to)
        {
            string queueIdsToCheckAsString = null;
            if (queueIdsToCheck != null)
                queueIdsToCheckAsString = string.Join<int>(",", queueIdsToCheck);

            object hasPendingItems;
            ExecuteNonQuerySP("[queue].[sp_SummaryQueueItem_HasPendingItems]", out hasPendingItems, queueIdsToCheckAsString, from, to);
            return (bool)hasPendingItems;
        }
    }
}