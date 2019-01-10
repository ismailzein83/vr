using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
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
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_PricelistId, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Rate, new RDBTableColumnDefinition {DataType = RDBDataType.Decimal, Size = 20, Precision = 8}},
                {COL_RateTypeId, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_RecentRate,new RDBTableColumnDefinition {DataType = RDBDataType.Decimal, Size = 20, Precision = 8}},
                {COL_CountryID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_ZoneName, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar, Size = 150}},
                {COL_ZoneID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_Change, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_EED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_RoutingProductID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CurrencyID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
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
