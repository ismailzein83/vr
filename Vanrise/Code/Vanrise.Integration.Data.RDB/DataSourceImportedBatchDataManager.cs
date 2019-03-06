using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Integration.Entities;
using Vanrise.Common;
using Vanrise.Queueing.Entities;

namespace Vanrise.Integration.Data.RDB
{
    public class DataSourceImportedBatchDataManager : IDataSourceImportedBatchDataManager
    {

        static string TABLE_NAME = "integration_DataSourceImportedBatch";
        static string TABLE_ALIAS = "vrDataSourceImportedBatch";

        const string COL_ID = "ID";
        const string COL_DataSourceId = "DataSourceId";
        const string COL_BatchDescription = "BatchDescription";
        const string COL_BatchSize = "BatchSize";
        const string COL_BatchState = "BatchState";
        const string COL_IsDuplicateSameSize = "IsDuplicateSameSize";
        const string COL_RecordsCount = "RecordsCount";
        const string COL_MappingResult = "MappingResult";
        const string COL_MapperMessage = "MapperMessage";
        const string COL_QueueItemIds = "QueueItemIds";
        const string COL_LogEntryTime = "LogEntryTime";
        const string COL_BatchStart = "BatchStart";
        const string COL_BatchEnd = "BatchEnd";
        const string COL_ExecutionStatus = "ExecutionStatus";


        const string LastImportedBatchTimeColumnName = "LastImportedBatchTime";
        const string NbImportedBatchColumnName = "NbImportedBatch";
        const string TotalRecordCountColumnName = "TotalRecordCount";
        const string MaxRecordCountColumnName = "MaxRecordCount";
        const string MinRecordCountColumnName = "MinRecordCount";
        const string MaxBatchSizeColumnName = "MaxBatchSize";
        const string MinBatchSizeColumnName = "MinBatchSize";
        const string NbInvalidBatchColumnName = "NbInvalidBatch";
        const string NbEmptyBatchColumnName = "NbEmptyBatch";
        const string MinBatchStartColumnName = "MinBatchStart";
        const string MaxBatchStartColumnName = "MaxBatchStart";

        static DataSourceImportedBatchDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_DataSourceId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_BatchDescription, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_BatchSize, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 18, Precision = 5 });
            columns.Add(COL_BatchState, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_IsDuplicateSameSize, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_RecordsCount, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_MappingResult, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_MapperMessage, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_QueueItemIds, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_LogEntryTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_BatchStart, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_BatchEnd, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ExecutionStatus, new RDBTableColumnDefinition { DataType = RDBDataType.Int });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "integration",
                DBTableName = "DataSourceImportedBatch",
                Columns = columns
            });
        }

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("IntegrationLogging", "BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString");
        }

        #endregion

        #region Mappers
        private DataSourceImportedBatch DataSourceImportedBatchMapper(IRDBDataReader reader)
        {
            DataSourceImportedBatch dataSourceImportedBatch = new DataSourceImportedBatch
            {
                ID = reader.GetLong(COL_ID),
                BatchDescription = reader.GetString(COL_BatchDescription),
                BatchSize = reader.GetDecimalWithNullHandling(COL_BatchSize),
                BatchState = (BatchState)reader.GetIntWithNullHandling(COL_BatchState),
                RecordsCount = reader.GetInt(COL_RecordsCount),
                MappingResult = (MappingResult)reader.GetIntWithNullHandling(COL_MappingResult),
                MapperMessage = reader.GetString(COL_MapperMessage),
                QueueItemIds = reader.GetString(COL_QueueItemIds),
                LogEntryTime = reader.GetDateTime(COL_LogEntryTime),
                BatchStart = reader.GetNullableDateTime(COL_BatchStart),
                BatchEnd = reader.GetNullableDateTime(COL_BatchEnd),
                ExecutionStatus = (ItemExecutionFlowStatus)reader.GetIntWithNullHandling(COL_ExecutionStatus)
            };

            return dataSourceImportedBatch;
        }

        private DataSourceSummary DataSourceSummaryMapper(IRDBDataReader reader)
        {
            return new DataSourceSummary()
            {
                DataSourceId = reader.GetGuid(COL_DataSourceId),
                LastImportedBatchTime = reader.GetDateTime("LastImportedBatchTime"),
                NbImportedBatch = reader.GetInt("NbImportedBatch"),
                TotalRecordCount = reader.GetInt("TotalRecordCount"),
                MaxRecordCount = reader.GetInt("MaxRecordCount"),
                MinRecordCount = reader.GetInt("MinRecordCount"),
                MaxBatchSize = reader.GetNullableDecimal("MaxBatchSize"),
                MinBatchSize = reader.GetNullableDecimal("MinBatchSize"),
                NbInvalidBatch = reader.GetInt("NbInvalidBatch"),
                NbEmptyBatch = reader.GetInt("NbEmptyBatch"),
                MinBatchStart = reader.GetNullableDateTime("MinBatchStart"),
                MaxBatchEnd = reader.GetNullableDateTime("MaxBatchEnd")
            };
        }

        #endregion

        #region IDataSourceImportedBatchManager

        public long InsertEntry(Guid dataSourceId, string batchDescription, decimal? batchSize, BatchState batchState, bool? isDuplicateSameSize, int recordCounts, MappingResult? result,
            string mapperMessage, string queueItemsIds, string logEntryTime, DateTime? batchStart, DateTime? batchEnd, ItemExecutionFlowStatus executionStatus)
        {

            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();
            insertQuery.Column(COL_DataSourceId).Value(dataSourceId);
            insertQuery.Column(COL_BatchDescription).Value(batchDescription);
            if (batchSize.HasValue)
                insertQuery.Column(COL_BatchSize).Value(batchSize.Value);
            insertQuery.Column(COL_BatchState).Value((int)batchState);
            if (isDuplicateSameSize.HasValue)
                insertQuery.Column(COL_IsDuplicateSameSize).Value(isDuplicateSameSize.Value);
            insertQuery.Column(COL_RecordsCount).Value(recordCounts);
            if (result.HasValue)
                insertQuery.Column(COL_MappingResult).Value((int)result.Value);
            insertQuery.Column(COL_MapperMessage).Value(mapperMessage);
            insertQuery.Column(COL_QueueItemIds).Value(queueItemsIds);
            insertQuery.Column(COL_LogEntryTime).Value(logEntryTime);
            if (batchStart.HasValue)
                insertQuery.Column(COL_BatchStart).Value(batchStart.Value);
            if (batchEnd.HasValue)
                insertQuery.Column(COL_BatchEnd).Value(batchEnd.Value);
            insertQuery.Column(COL_ExecutionStatus).Value((int)executionStatus);

            return queryContext.ExecuteScalar().LongValue;
        }

        public void UpdateExecutionStatus(Dictionary<long, ItemExecutionFlowStatus> executionStatusToUpdateById)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_ID, RDBDataType.BigInt, true);
            tempTableQuery.AddColumn(COL_ExecutionStatus, RDBDataType.Int);

            if (executionStatusToUpdateById != null)
            {
                var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
                insertMultipleRowsQuery.IntoTable(tempTableQuery);
                foreach (var updateEntity in executionStatusToUpdateById)
                {
                    var rowContext = insertMultipleRowsQuery.AddRow();
                    rowContext.Column(COL_ID).Value(updateEntity.Key);
                    rowContext.Column(COL_ExecutionStatus).Value((int)updateEntity.Value);
                }
            }
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var joinContext = updateQuery.Join(TABLE_ALIAS);
            var joinCondition = joinContext.Join(tempTableQuery, "updateEntities").On();
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_ID, "updateEntities", COL_ID);
            updateQuery.Column(COL_ExecutionStatus).Column("updateEntities", COL_ExecutionStatus);
            queryContext.ExecuteNonQuery();

        }
        public List<DataSourceImportedBatch> GetFilteredDataSourceImportedBatches(DataSourceImportedBatchQuery query)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, query.Top, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var where = selectQuery.Where();
            if (query.DataSourceId.HasValue)
                where.EqualsCondition(COL_DataSourceId).Value(query.DataSourceId.Value);
            if (!string.IsNullOrEmpty(query.BatchName))
                where.ContainsCondition(COL_BatchDescription, query.BatchName);
            if (query.MappingResults != null)
                where.ListCondition(COL_MappingResult, RDBListConditionOperator.IN, query.MappingResults.MapRecords(x => (int)x));
            if (query.From.HasValue)
                where.GreaterOrEqualCondition(COL_LogEntryTime).Value(query.From.Value);
            if (query.To.HasValue)
                where.LessOrEqualCondition(COL_LogEntryTime).Value(query.To.Value);
            if (query.ExecutionFlowsStatus != null)
                where.ListCondition(COL_ExecutionStatus, RDBListConditionOperator.IN, query.ExecutionFlowsStatus.MapRecords(x => (int)x));
            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);
            return queryContext.GetItems(DataSourceImportedBatchMapper);
        }

        public List<DataSourceSummary> GetDataSourcesSummary(DateTime fromTime, List<Guid> dataSourcesIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);


            var groupByContext = selectQuery.GroupBy();
            groupByContext.Select().Column(COL_DataSourceId);
            var selectAggregates = groupByContext.SelectAggregates();
            selectAggregates.Aggregate(RDBNonCountAggregateType.MAX, TABLE_ALIAS, COL_LogEntryTime, LastImportedBatchTimeColumnName);
            selectAggregates.Count(NbImportedBatchColumnName);
            selectAggregates.Aggregate(RDBNonCountAggregateType.SUM, TABLE_ALIAS, COL_RecordsCount, TotalRecordCountColumnName);
            selectAggregates.Aggregate(RDBNonCountAggregateType.MAX, TABLE_ALIAS, COL_RecordsCount, MaxRecordCountColumnName);
            selectAggregates.Aggregate(RDBNonCountAggregateType.MIN, TABLE_ALIAS, COL_RecordsCount, MinRecordCountColumnName);
            selectAggregates.Aggregate(RDBNonCountAggregateType.MAX, TABLE_ALIAS, COL_BatchSize, MaxBatchSizeColumnName);
            selectAggregates.Aggregate(RDBNonCountAggregateType.MIN, TABLE_ALIAS, COL_BatchSize, MinBatchSizeColumnName);

            var expToSet = selectAggregates.ExpressionAggregate(RDBNonCountAggregateType.SUM, NbInvalidBatchColumnName);
            var nbInvalidBatchExp = expToSet.CaseExpression();
            var case1 = nbInvalidBatchExp.AddCase();
            case1.When().EqualsCondition(COL_MappingResult).Value((int)MappingResult.Invalid);
            case1.Then().Value(1);
            nbInvalidBatchExp.Else().Value(0);

            var expToSet2 = selectAggregates.ExpressionAggregate(RDBNonCountAggregateType.SUM, NbEmptyBatchColumnName);
            var nbEmptyBatchExp = expToSet.CaseExpression();
            var case2 = nbEmptyBatchExp.AddCase();
            case2.When().EqualsCondition(COL_MappingResult).Value((int)MappingResult.Empty);
            case2.Then().Value(1);
            nbEmptyBatchExp.Else().Value(0);

            selectAggregates.Aggregate(RDBNonCountAggregateType.MIN, TABLE_ALIAS, COL_BatchStart, MinBatchStartColumnName);
            selectAggregates.Aggregate(RDBNonCountAggregateType.MAX, TABLE_ALIAS, COL_BatchStart, MaxBatchStartColumnName);

            var where = selectQuery.Where();
            where.GreaterOrEqualCondition(COL_LogEntryTime).Value(fromTime);
            where.ListCondition(COL_DataSourceId, RDBListConditionOperator.IN, dataSourcesIds);

            return queryContext.GetItems(DataSourceSummaryMapper);
        }

        public List<DataSourceImportedBatch> GetDataSourceImportedBatchesAfterID(int limitResult, long? afterId, List<ItemExecutionFlowStatus> executionFlowsStatus)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, limitResult, true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_BatchDescription, COL_BatchSize, COL_BatchState, COL_RecordsCount, COL_MappingResult, COL_MapperMessage, COL_QueueItemIds, COL_LogEntryTime, COL_BatchStart, COL_BatchEnd, COL_ExecutionStatus);
            var where = selectQuery.Where();
            if (afterId.HasValue)
                where.GreaterThanCondition(COL_ID).Value(afterId.Value);
            if (executionFlowsStatus != null)
            {
                where.ConditionIfColumnNotNull(COL_ExecutionStatus).ListCondition(COL_ExecutionStatus, RDBListConditionOperator.IN, executionFlowsStatus.MapRecords(x => (int)x));
            }
            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.ASC);

            return queryContext.GetItems(DataSourceImportedBatchMapper);
        }

        public List<DataSourceImportedBatch> GetDataSourceImportedBatches(Guid DataSourceId, DateTime from)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_BatchDescription, COL_BatchSize, COL_BatchState, COL_RecordsCount, COL_MappingResult, COL_MapperMessage, COL_QueueItemIds, COL_LogEntryTime, COL_BatchStart, COL_BatchEnd, COL_ExecutionStatus);
            var where = selectQuery.Where();
            where.EqualsCondition(COL_DataSourceId).Value(DataSourceId);
            where.GreaterOrEqualCondition(COL_LogEntryTime).Value(from);
            return queryContext.GetItems(DataSourceImportedBatchMapper);
        }

        #endregion
    }
}
