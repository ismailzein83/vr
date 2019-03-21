using System.Collections.Generic;
using TOne.WhS.Deal.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Data.RDB
{
    public class DealProgressDataManager : IDealProgressDataManager
    {
        static string TABLE_NAME = "TOneWhS_Deal_DealProgress";
        static string TABLE_ALIAS = "dp";
        const string COL_ID = "ID";
        const string COL_DealID = "DealID";
        const string COL_ZoneGroupNb = "ZoneGroupNb";
        const string COL_IsSale = "IsSale";
        const string COL_CurrentTierNb = "CurrentTierNb";
        const string COL_ReachedDurationInSec = "ReachedDurationInSec";
        const string COL_TargetDurationInSec = "TargetDurationInSec";
        const string COL_IsComplete = "IsComplete";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        AffectedDealZoneGroupsToDeleteDataManager affectedDealZoneGroupsToDeleteDataManager = new AffectedDealZoneGroupsToDeleteDataManager();

        static DealProgressDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_DealID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ZoneGroupNb, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_IsSale, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CurrentTierNb, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ReachedDurationInSec, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 4 });
            columns.Add(COL_TargetDurationInSec, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 4 });
            columns.Add(COL_IsComplete, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_Deal",
                DBTableName = "DealProgress",
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

        public List<DealProgress> GetDealProgresses(HashSet<DealZoneGroup> dealZoneGroups, bool isSale)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var dealZoneGroupTempTable = CreateDealZoneGroupsTempTable(queryContext, dealZoneGroups);

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            var joinCondition = joinContext.Join(dealZoneGroupTempTable, "dzg").On();
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_DealID, "dzg", COL_DealID);
            joinCondition.EqualsCondition("dzg", COL_ZoneGroupNb, TABLE_ALIAS, COL_ZoneGroupNb);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_IsSale).Value(isSale);

            return queryContext.GetItems(DealProgressMapper);
        }

        public void InsertDealProgresses(List<DealProgress> dealProgresses)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertMultipleRowsQuery();
            insertQuery.IntoTable(TABLE_NAME);

            if (dealProgresses != null)
            {
                foreach (var dealProgress in dealProgresses)
                {
                    var rowContext = insertQuery.AddRow();
                    rowContext.Column(COL_DealID).Value(dealProgress.DealId);
                    rowContext.Column(COL_ZoneGroupNb).Value(dealProgress.ZoneGroupNb);
                    rowContext.Column(COL_IsSale).Value(dealProgress.IsSale);
                    rowContext.Column(COL_CurrentTierNb).Value(dealProgress.CurrentTierNb);
                    if (dealProgress.ReachedDurationInSeconds.HasValue)
                        rowContext.Column(COL_ReachedDurationInSec).Value(dealProgress.ReachedDurationInSeconds.Value);
                    if (dealProgress.TargetDurationInSeconds.HasValue)
                        rowContext.Column(COL_TargetDurationInSec).Value(dealProgress.TargetDurationInSeconds.Value);
                }
            }
            queryContext.ExecuteNonQuery();
        }

        public void UpdateDealProgresses(List<DealProgress> dealProgresses)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var dealProgressTempTable = CreateDealProgressTempTable(queryContext, dealProgresses);
            var tempTableAlias = "tempdp";

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_DealID).Column(tempTableAlias, COL_DealID);
            updateQuery.Column(COL_ZoneGroupNb).Column(tempTableAlias, COL_ZoneGroupNb);
            updateQuery.Column(COL_IsSale).Column(tempTableAlias, COL_IsSale);
            updateQuery.Column(COL_CurrentTierNb).Column(tempTableAlias, COL_CurrentTierNb);
            updateQuery.Column(COL_ReachedDurationInSec).Column(tempTableAlias, COL_ReachedDurationInSec);
            updateQuery.Column(COL_TargetDurationInSec).Column(tempTableAlias, COL_TargetDurationInSec);

            var joinContext = updateQuery.Join(TABLE_ALIAS);
            var joinCondition = joinContext.Join(dealProgressTempTable, tempTableAlias).On();
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_ID, tempTableAlias, COL_ID);

            queryContext.ExecuteNonQuery();
        }

        public void DeleteDealProgresses(HashSet<DealZoneGroup> dealZoneGroups, bool isSale)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var dealZoneGroupTempTable = CreateDealZoneGroupsTempTable(queryContext, dealZoneGroups);

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var joinContext = deleteQuery.Join(TABLE_ALIAS);
            var joinCondition = joinContext.Join(dealZoneGroupTempTable, "dzg").On();
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_DealID, "dzg", COL_DealID);
            joinCondition.EqualsCondition("dzg", COL_ZoneGroupNb, TABLE_ALIAS, COL_ZoneGroupNb);

            var where = deleteQuery.Where();
            where.EqualsCondition(COL_IsSale).Value(isSale);

            queryContext.ExecuteNonQuery();
        }

        public IEnumerable<DealZoneGroup> GetAffectedDealZoneGroups(bool isSale)
        {
            return affectedDealZoneGroupsToDeleteDataManager.GetAffectedDealZoneGroups(isSale);
        }

        public void InsertAffectedDealZoneGroups(HashSet<DealZoneGroup> dealZoneGroups, bool isSale)
        {
            affectedDealZoneGroupsToDeleteDataManager.InsertAffectedDealZoneGroups(dealZoneGroups, isSale);
        }

        public void DeleteAffectedDealZoneGroups()
        {
            affectedDealZoneGroupsToDeleteDataManager.DeleteAffectedDealZoneGroups();
        }

        private RDBTempTableQuery CreateDealZoneGroupsTempTable(RDBQueryContext queryContext, HashSet<DealZoneGroup> dealZoneGroups)
        {
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_DealID, RDBDataType.Int);
            tempTableQuery.AddColumn(COL_ZoneGroupNb, RDBDataType.Int);

            if (dealZoneGroups != null)
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

        private RDBTempTableQuery CreateDealProgressTempTable(RDBQueryContext queryContext, List<DealProgress> dealProgresses)
        {
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_ID, RDBDataType.BigInt);
            tempTableQuery.AddColumn(COL_DealID, RDBDataType.Int);
            tempTableQuery.AddColumn(COL_ZoneGroupNb, RDBDataType.Int);
            tempTableQuery.AddColumn(COL_IsSale, RDBDataType.Boolean);
            tempTableQuery.AddColumn(COL_CurrentTierNb, RDBDataType.Int);
            tempTableQuery.AddColumn(COL_ReachedDurationInSec, RDBDataType.Decimal, 20, 4);
            tempTableQuery.AddColumn(COL_TargetDurationInSec, RDBDataType.Decimal, 20, 4);

            if (dealProgresses != null)
            {
                var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
                insertMultipleRowsQuery.IntoTable(tempTableQuery);

                foreach (var dealProgress in dealProgresses)
                {
                    var rowContext = insertMultipleRowsQuery.AddRow();

                    rowContext.Column(COL_ID).Value(dealProgress.DealProgressId);
                    rowContext.Column(COL_DealID).Value(dealProgress.DealId);
                    rowContext.Column(COL_ZoneGroupNb).Value(dealProgress.ZoneGroupNb);
                    rowContext.Column(COL_IsSale).Value(dealProgress.IsSale);
                    rowContext.Column(COL_CurrentTierNb).Value(dealProgress.CurrentTierNb);
                    if (dealProgress.ReachedDurationInSeconds.HasValue)
                        rowContext.Column(COL_ReachedDurationInSec).Value(dealProgress.ReachedDurationInSeconds.Value);
                    if (dealProgress.TargetDurationInSeconds.HasValue)
                        rowContext.Column(COL_TargetDurationInSec).Value(dealProgress.TargetDurationInSeconds.Value);
                }
            }
            return tempTableQuery;
        }

        private DealProgress DealProgressMapper(IRDBDataReader reader)
        {
            return new DealProgress
            {
                DealProgressId = reader.GetLong(COL_ID),
                DealId = reader.GetInt(COL_DealID),
                ZoneGroupNb = reader.GetInt(COL_ZoneGroupNb),
                CurrentTierNb = reader.GetInt(COL_CurrentTierNb),
                IsSale = reader.GetBoolean(COL_IsSale),
                ReachedDurationInSeconds = reader.GetNullableDecimal(COL_ReachedDurationInSec),
                TargetDurationInSeconds = reader.GetNullableDecimal(COL_TargetDurationInSec),
                CreatedTime = reader.GetDateTime(COL_CreatedTime)
            };
        }
    }
}
