using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Deal.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Data.RDB
{
    public class DealDetailedProgressDataManager : IDealDetailedProgressDataManager
    {
        static string TABLE_NAME = "TOneWhS_Deal_DealDetailedProgress";
        static string TABLE_ALIAS = "ddp";
        const string COL_ID = "ID";
        const string COL_DealID = "DealID";
        const string COL_ZoneGroupNb = "ZoneGroupNb";
        const string COL_IsSale = "IsSale";
        const string COL_TierNb = "TierNb";
        const string COL_RateTierNb = "RateTierNb";
        const string COL_FromTime = "FromTime";
        const string COL_ToTime = "ToTime";
        const string COL_ReachedDurationInSec = "ReachedDurationInSec";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static DealDetailedProgressDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_DealID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ZoneGroupNb, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_IsSale, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_TierNb, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_RateTierNb, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_FromTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ToTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ReachedDurationInSec, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 4 });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_Deal",
                DBTableName = "DealDetailedProgress",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Deal", "TOneWhS_Analytics_DBConnStringKey", "TOneAnalyticsDBConnString");
        }

        #region Public Methods
        public List<DealDetailedProgress> GetDealsDetailedProgress(List<int> dealIds, DateTime fromDate, DateTime toDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();

            if (dealIds != null && dealIds.Any())
                where.ListCondition(COL_DealID, RDBListConditionOperator.IN, dealIds);

            if (fromDate != null)
                where.GreaterOrEqualCondition(COL_ToTime).Value(fromDate);

            if (toDate != null)
                where.LessThanCondition(COL_FromTime).Value(toDate);

            where.NotNullCondition(COL_TierNb);

            return queryContext.GetItems(DealDetailedProgressMapper);
        }

        public List<DealDetailedProgress> GetDealDetailedProgresses(HashSet<DealZoneGroup> dealZoneGroups, bool isSale, DateTime? beginDate, DateTime? endDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var dealZoneGroupsTempTable = CreateDealZoneGroupsTempTable(queryContext, dealZoneGroups);

            var selectQuery = queryContext.AddSelectQuery();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            var joinCondition = joinContext.Join(dealZoneGroupsTempTable, "dzg").On();
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_DealID, "dzg", COL_DealID);
            joinCondition.EqualsCondition("dzg", COL_ZoneGroupNb, TABLE_ALIAS, COL_ZoneGroupNb);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_IsSale).Value(isSale);

            if (endDate.HasValue)
                where.LessThanCondition(COL_FromTime).Value(endDate.Value);

            if (beginDate.HasValue)
                where.GreaterThanCondition(COL_ToTime).Value(beginDate.Value);

            return queryContext.GetItems(DealDetailedProgressMapper);
        }

        public List<DealDetailedProgress> GetDealDetailedProgressesByDate(bool isSale, DateTime? beginDate, DateTime? endDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_IsSale).Value(isSale);
            if (endDate.HasValue)
                where.LessThanCondition(COL_FromTime).Value(endDate.Value);
            if (beginDate.HasValue)
                where.GreaterThanCondition(COL_ToTime).Value(beginDate.Value);

            return queryContext.GetItems(DealDetailedProgressMapper);
        }

        public void InsertDealDetailedProgresses(List<DealDetailedProgress> dealDetailedProgresses)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertMultipleRowsQuery();
            insertQuery.IntoTable(TABLE_NAME);

            foreach (var dealDetailedProgress in dealDetailedProgresses)
            {
                var rowContext = insertQuery.AddRow();
                rowContext.Column(COL_DealID).Value(dealDetailedProgress.DealId);
                rowContext.Column(COL_ZoneGroupNb).Value(dealDetailedProgress.ZoneGroupNb);
                rowContext.Column(COL_IsSale).Value(dealDetailedProgress.IsSale);
                if (dealDetailedProgress.TierNb.HasValue)
                    rowContext.Column(COL_TierNb).Value(dealDetailedProgress.TierNb.Value);
                if (dealDetailedProgress.RateTierNb.HasValue)
                    rowContext.Column(COL_RateTierNb).Value(dealDetailedProgress.RateTierNb.Value);
                rowContext.Column(COL_FromTime).Value(dealDetailedProgress.FromTime);
                rowContext.Column(COL_ToTime).Value(dealDetailedProgress.ToTime);
                rowContext.Column(COL_ReachedDurationInSec).Value(dealDetailedProgress.ReachedDurationInSeconds);
            }
            queryContext.ExecuteNonQuery();
        }

        public void UpdateDealDetailedProgresses(List<DealDetailedProgress> dealDetailedProgresses)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var dealDetailedProgressTempTable = CreateDealDetailedProgressesTempTable(queryContext, dealDetailedProgresses);

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            string tempTableAlias = "tempddp";
            updateQuery.Column(COL_DealID).Column(tempTableAlias, COL_DealID);
            updateQuery.Column(COL_ZoneGroupNb).Column(tempTableAlias, COL_ZoneGroupNb);
            updateQuery.Column(COL_IsSale).Column(tempTableAlias, COL_IsSale);
            updateQuery.Column(COL_TierNb).Column(tempTableAlias, COL_TierNb);
            updateQuery.Column(COL_RateTierNb).Column(tempTableAlias, COL_RateTierNb);
            updateQuery.Column(COL_ReachedDurationInSec).Column(tempTableAlias, COL_ReachedDurationInSec);
            updateQuery.Column(COL_FromTime).Column(tempTableAlias, COL_FromTime);
            updateQuery.Column(COL_ToTime).Column(tempTableAlias, COL_ToTime);

            var joinContext = updateQuery.Join(TABLE_ALIAS);
            var joinCondition = joinContext.Join(dealDetailedProgressTempTable, tempTableAlias).On();
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_ID, tempTableAlias, COL_ID);

            queryContext.ExecuteNonQuery();
        }

        public void DeleteDealDetailedProgresses(List<long> dealDetailedProgressIds)
        {
            if (dealDetailedProgressIds == null || dealDetailedProgressIds.Count == 0)
                return;

            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var where = deleteQuery.Where();
            where.ListCondition(COL_ID, RDBListConditionOperator.IN, dealDetailedProgressIds);

            queryContext.ExecuteNonQuery();
        }

        public void DeleteDealDetailedProgresses(bool isSale, DateTime? beginDate, DateTime? endDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var where = deleteQuery.Where();
            where.EqualsCondition(COL_IsSale).Value(isSale);
            if (endDate.HasValue)
                where.LessThanCondition(COL_FromTime).Value(endDate.Value);
            if (beginDate.HasValue)
                where.GreaterThanCondition(COL_ToTime).Value(beginDate.Value);

            queryContext.ExecuteNonQuery();
        }

        public DateTime? GetDealEvaluatorBeginDate(object lastDealDetailedProgressUpdateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectAggregates().Aggregate(RDBNonCountAggregateType.MIN, COL_FromTime, "BegintDate");

            var where = selectQuery.Where();
            queryContext.AddGreaterThanReceivedDataInfoCondition(TABLE_NAME, where, lastDealDetailedProgressUpdateHandle);

            return queryContext.ExecuteScalar().NullableDateTimeValue;
        }

        public object GetMaxUpdateHandle()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.GetMaxReceivedDataInfo(TABLE_NAME);
        }
        #endregion

        #region Private Methods
        private RDBTempTableQuery CreateDealZoneGroupsTempTable(RDBQueryContext queryContext, HashSet<DealZoneGroup> dealZoneGroups)
        {
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_DealID, RDBDataType.Int);
            tempTableQuery.AddColumn(COL_ZoneGroupNb, RDBDataType.Int);

            if (dealZoneGroups != null && dealZoneGroups.Count > 0)
            {
                var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
                insertMultipleRowsQuery.IntoTable(tempTableQuery);
                foreach (var dealZoneGroup in dealZoneGroups)
                {
                    var rowContext = insertMultipleRowsQuery.AddRow();
                    rowContext.Column(COL_DealID).Value(dealZoneGroup.DealId);
                    rowContext.Column(COL_ZoneGroupNb).Value(dealZoneGroup.ZoneGroupNb);
                }
            }
            return tempTableQuery;
        }

        private RDBTempTableQuery CreateDealDetailedProgressesTempTable(RDBQueryContext queryContext, List<DealDetailedProgress> dealDetailedProgresses)
        {
            var tempTableQuery = queryContext.CreateTempTable();
            // tempTableQuery.AddColumnsFromTable(TABLE_NAME);
            tempTableQuery.AddColumn(COL_ID, RDBDataType.BigInt, true);
            tempTableQuery.AddColumn(COL_DealID, RDBDataType.Int);
            tempTableQuery.AddColumn(COL_ZoneGroupNb, RDBDataType.Int);
            tempTableQuery.AddColumn(COL_IsSale, RDBDataType.Boolean);
            tempTableQuery.AddColumn(COL_TierNb, RDBDataType.Int);
            tempTableQuery.AddColumn(COL_RateTierNb, RDBDataType.Int);
            tempTableQuery.AddColumn(COL_FromTime, RDBDataType.DateTime);
            tempTableQuery.AddColumn(COL_ToTime, RDBDataType.DateTime);
            tempTableQuery.AddColumn(COL_ReachedDurationInSec, RDBDataType.Decimal, 20, 4);

            if (dealDetailedProgresses != null && dealDetailedProgresses.Count > 0)
            {
                var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
                insertMultipleRowsQuery.IntoTable(tempTableQuery);

                foreach (var dealDetailedProgress in dealDetailedProgresses)
                {
                    var rowContext = insertMultipleRowsQuery.AddRow();

                    rowContext.Column(COL_ID).Value(dealDetailedProgress.DealDetailedProgressId);
                    rowContext.Column(COL_DealID).Value(dealDetailedProgress.DealId);
                    rowContext.Column(COL_ZoneGroupNb).Value(dealDetailedProgress.ZoneGroupNb);
                    rowContext.Column(COL_IsSale).Value(dealDetailedProgress.IsSale);
                    if (dealDetailedProgress.TierNb.HasValue)
                        rowContext.Column(COL_TierNb).Value(dealDetailedProgress.TierNb.Value);
                    if (dealDetailedProgress.RateTierNb.HasValue)
                        rowContext.Column(COL_RateTierNb).Value(dealDetailedProgress.RateTierNb.Value);
                    rowContext.Column(COL_FromTime).Value(dealDetailedProgress.FromTime);
                    rowContext.Column(COL_ToTime).Value(dealDetailedProgress.ToTime);
                    rowContext.Column(COL_ReachedDurationInSec).Value(dealDetailedProgress.ReachedDurationInSeconds);
                }
            }

            return tempTableQuery;
        }
        #endregion

        #region  Mappers
        private DealDetailedProgress DealDetailedProgressMapper(IRDBDataReader reader)
        {
            return new DealDetailedProgress
            {
                DealDetailedProgressId = reader.GetLong(COL_ID),
                DealId = reader.GetInt(COL_DealID),
                ZoneGroupNb = reader.GetInt(COL_ZoneGroupNb),
                IsSale = reader.GetBoolean(COL_IsSale),
                TierNb = reader.GetNullableInt(COL_TierNb),
                RateTierNb = reader.GetNullableInt(COL_RateTierNb),
                FromTime = reader.GetDateTime(COL_FromTime),
                ToTime = reader.GetDateTime(COL_ToTime),
                ReachedDurationInSeconds = reader.GetDecimal(COL_ReachedDurationInSec),
                CreatedTime = reader.GetDateTime(COL_CreatedTime)
            };
        }
        #endregion
    }
}
