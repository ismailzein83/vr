using System.Collections.Generic;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Data.RDB
{
    public class DealDataManager : IDealDataManager
    {
        static string TABLE_NAME = "TOneWhS_Deal_Deal";
        static string TABLE_ALIAS = "d";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_IsDeleted = "IsDeleted";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static DealDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_IsDeleted, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_Deal",
                DBTableName = "Deal",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Deal", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        public IEnumerable<DealDefinition> GetDeals()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_Name, COL_Settings, COL_IsDeleted);

            return queryContext.GetItems(DealMapper);
        }

        public IEnumerable<DealDefinition> GetDealsModifiedAfterLastUpdateHandle(object lastDealDefinitionUpdateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_Name, COL_Settings, COL_IsDeleted);

            var where = selectQuery.Where();
            queryContext.AddGreaterThanReceivedDataInfoCondition(TABLE_NAME, where, lastDealDefinitionUpdateHandle);

            return queryContext.GetItems(DealMapper);
        }

        public bool Insert(DealDefinition deal, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Name).Value(deal.Name);
            notExistsCondition.EqualsCondition(COL_IsDeleted).Value(0);

            insertQuery.Column(COL_Name).Value(deal.Name);
            insertQuery.Column(COL_Settings).Value(Serializer.Serialize(deal.Settings));

            insertedId = queryContext.ExecuteScalar().IntValue;
            if (insertedId > 0)
                return true;
            return false;
        }

        public bool Update(DealDefinition deal)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Name).Value(deal.Name);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(deal.DealId);
            notExistsCondition.EqualsCondition(COL_IsDeleted).Value(0);

            updateQuery.Column(COL_Settings).Value(Serializer.Serialize(deal.Settings));
            updateQuery.Column(COL_Name).Value(deal.Name);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_ID).Value(deal.DealId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Delete(int dealId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_IsDeleted).Value(1);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_ID).Value(dealId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public object GetMaxUpdateHandle()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.GetMaxReceivedDataInfo(TABLE_NAME);
        }

        public bool AreDealsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        private DealDefinition DealMapper(IRDBDataReader reader)
        {
            return new DealDefinition
            {
                DealId = reader.GetInt(COL_ID),
                Name = reader.GetString(COL_Name),
                IsDeleted = reader.GetBoolean(COL_IsDeleted),
                Settings = Serializer.Deserialize<DealSettings>(reader.GetString(COL_Settings))
            };
        }
    }
}
