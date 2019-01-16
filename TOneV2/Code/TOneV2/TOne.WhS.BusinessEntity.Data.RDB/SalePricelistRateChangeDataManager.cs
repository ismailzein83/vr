using Vanrise.Data.RDB;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePricelistRateChangeDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "sprc";
        static string TABLE_NAME = "TOneWhS_BE_SalePricelistRateChange";
        const string COL_PricelistId = "PricelistId";
        const string COL_Rate = "Rate";
        const string COL_RateTypeId = "RateTypeId";
        const string COL_RecentRate = "RecentRate";
        const string COL_CountryID = "CountryID";
        const string COL_ZoneName = "ZoneName";
        const string COL_ZoneID = "ZoneID";
        const string COL_Change = "Change";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_RoutingProductID = "RoutingProductID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";


        static SalePricelistRateChangeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_PricelistId, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_Rate, new RDBTableColumnDefinition {DataType = RDBDataType.Decimal, Size = 20, Precision = 8});
            columns.Add(COL_RateTypeId, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_RecentRate, new RDBTableColumnDefinition {DataType = RDBDataType.Decimal, Size = 20, Precision = 8});
            columns.Add(COL_CountryID, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_ZoneName, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar, Size = 150});
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt});
            columns.Add(COL_Change, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});
            columns.Add(COL_EED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});
            columns.Add(COL_RoutingProductID, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_CurrencyID, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SalePricelistRateChange",
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

        public IEnumerable<SalePricelistRateChange> GetFilteredRatesPreviewByPriceListId(int pricelistId, string zoneName)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_PricelistId).Value(pricelistId);
            whereQuery.EqualsCondition(COL_ZoneName).Value(zoneName);
            whereQuery.NotNullCondition(COL_RateTypeId);

            return queryContext.GetItems(SalePricelistRateChangeMapper);
        }

        public List<SalePricelistRateChange> GetSalePricelistRateChanges(int priceListId, List<int> countryIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_PricelistId).Value(priceListId);

            if (countryIds != null && countryIds.Any())
                whereQuery.ListCondition(COL_CountryID, RDBListConditionOperator.IN, countryIds);

            return queryContext.GetItems(SalePricelistRateChangeMapper);
        }
        public void BuildInsertQuery(RDBInsertQuery insertQuery, long processInstanceID)
        {
            insertQuery.IntoTable(TABLE_NAME);
            var salePriceListRateChangeNewDataManager = new SalePricelistRateChangeNewDataManager();
            salePriceListRateChangeNewDataManager.BuildSelectQuery(insertQuery.FromSelect(), processInstanceID);
        }

        #endregion

        #region Mappper

        private SalePricelistRateChange SalePricelistRateChangeMapper(IRDBDataReader reader)
        {
            return new SalePricelistRateChange
            {
                ZoneName = reader.GetString(COL_ZoneName),
                RateTypeId = reader.GetNullableInt(COL_RateTypeId),
                RecentRate = reader.GetNullableDecimal(COL_RecentRate),
                Rate = reader.GetDecimal(COL_Rate),
                ChangeType = (RateChangeType)reader.GetInt(COL_Change),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                CurrencyId = reader.GetInt(COL_CurrencyID),
            };
        }

        #endregion
    }
}
