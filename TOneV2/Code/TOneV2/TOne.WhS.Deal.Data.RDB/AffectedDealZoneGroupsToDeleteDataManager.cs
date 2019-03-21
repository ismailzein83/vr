using System.Collections.Generic;
using TOne.WhS.Deal.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Data.RDB
{
    public class AffectedDealZoneGroupsToDeleteDataManager
    {
        static string TABLE_NAME = "TOneWhS_Deal_AffectedDealZoneGroupsToDelete";
        static string TABLE_ALIAS = "adzgtd";
        const string COL_DealID = "DealID";
        const string COL_ZoneGroupNb = "ZoneGroupNb";
        const string COL_IsSale = "IsSale";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static AffectedDealZoneGroupsToDeleteDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_DealID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ZoneGroupNb, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_IsSale, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_Deal",
                DBTableName = "AffectedDealZoneGroupsToDelete",
                Columns = columns,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Deal", "TOneWhS_Analytics_DBConnStringKey", "TOneAnalyticsDBConnString");
        }

        public IEnumerable<DealZoneGroup> GetAffectedDealZoneGroups(bool isSale)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().Columns(COL_DealID, COL_ZoneGroupNb);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_IsSale).Value(isSale);

            return queryContext.GetItems(DealZoneGroupMapper);
        }

        public void InsertAffectedDealZoneGroups(HashSet<DealZoneGroup> dealZoneGroups, bool isSale)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertMultipleRowsQuery();
            insertQuery.IntoTable(TABLE_NAME);

            if (dealZoneGroups != null)
            {
                foreach (var dealZoneGroup in dealZoneGroups)
                {
                    var rowContext = insertQuery.AddRow();
                    rowContext.Column(COL_DealID).Value(dealZoneGroup.DealId);
                    rowContext.Column(COL_ZoneGroupNb).Value(dealZoneGroup.ZoneGroupNb);
                    rowContext.Column(COL_IsSale).Value(isSale);
                }
            }
            queryContext.ExecuteNonQuery();
        }

        public void DeleteAffectedDealZoneGroups()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            queryContext.ExecuteNonQuery();
        }

        private DealZoneGroup DealZoneGroupMapper(IRDBDataReader reader)
        {
            return new DealZoneGroup
            {
                DealId = reader.GetInt(COL_DealID),
                ZoneGroupNb = reader.GetInt(COL_ZoneGroupNb)
            };
        }
    }
}
