using Vanrise.Entities;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePricelistRPChangeDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "sprpc";
        static string TABLE_NAME = "TOneWhS_BE_SalePricelistRPChange";
        const string COL_ZoneName = "ZoneName";
        const string COL_ZoneID = "ZoneID";
        const string COL_RoutingProductId = "RoutingProductId";
        const string COL_RecentRoutingProductId = "RecentRoutingProductId";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_PriceListId = "PriceListId";
        const string COL_CountryId = "CountryId";
        const string COL_CustomerId = "CustomerId";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static SalePricelistRPChangeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ZoneName, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar});
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt});
            columns.Add(COL_RoutingProductId, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_RecentRoutingProductId, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});
            columns.Add(COL_EED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});
            columns.Add(COL_PriceListId, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt});
            columns.Add(COL_CountryId, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_CustomerId, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SalePricelistRPChange",
                Columns = columns,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #endregion

        #region Public Methods

        public List<SalePricelistRPChange> GetSalePriceListRPChanges(int priceListId, List<int> countryIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_PriceListId).Value(priceListId);

            if (countryIds != null && countryIds.Any())
                whereQuery.ListCondition(COL_CountryId, RDBListConditionOperator.IN, countryIds);

            return queryContext.GetItems(SalePricelistRPChangeMapper);
        }

        public void BuildInsertQuery(RDBInsertQuery insertQuery)
        {
            insertQuery.IntoTable(TABLE_NAME);
        }
        #endregion

        #region Mapper

        SalePricelistRPChange SalePricelistRPChangeMapper(IRDBDataReader reader)
        {
            return new SalePricelistRPChange
            {
                ZoneName = reader.GetString(COL_ZoneName),
                ZoneId = reader.GetNullableLong(COL_ZoneID),
                RoutingProductId = reader.GetIntWithNullHandling(COL_RoutingProductId),
                RecentRoutingProductId = reader.GetIntWithNullHandling(COL_RecentRoutingProductId),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                CountryId = reader.GetIntWithNullHandling(COL_CountryId),
                CustomerId = reader.GetIntWithNullHandling(COL_CustomerId)
            };
        }

        #endregion
    }
}
