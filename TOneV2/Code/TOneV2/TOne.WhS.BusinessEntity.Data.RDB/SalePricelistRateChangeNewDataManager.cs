using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePricelistRateChangeNewDataManager
    {
        #region RDB 

        static string TABLE_ALIAS = "sprcn";
        static string TABLE_NAME = "TOneWhS_BE_SalePricelistRateChange_New";
        const string COL_PricelistId = "PricelistId";
        const string COL_Rate = "Rate";
        const string COL_RateTypeId = "RateTypeId";
        const string COL_RecentCurrencyId = "RecentCurrencyId";
        const string COL_RecentRate = "RecentRate";
        const string COL_RecentRateConverted = "RecentRateConverted";
        const string COL_CountryID = "CountryID";
        const string COL_ZoneName = "ZoneName";
        const string COL_ZoneID = "ZoneID";
        const string COL_Change = "Change";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_RoutingProductID = "RoutingProductID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";

        static SalePricelistRateChangeNewDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_PricelistId, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Rate, new RDBTableColumnDefinition {DataType = RDBDataType.Decimal, Size = 20, Precision = 8}},
                {COL_RateTypeId, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_RecentCurrencyId, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_RecentRate,new RDBTableColumnDefinition {DataType = RDBDataType.Decimal, Size = 20, Precision = 8}},
                {COL_RecentRateConverted,new RDBTableColumnDefinition {DataType = RDBDataType.Decimal, Size = 20, Precision = 8}},
                {COL_CountryID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_ZoneName, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar, Size = 150}},
                {COL_ZoneID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_Change, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_ProcessInstanceID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
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
                DBTableName = "SalePricelistRateChange_New",
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

        public IEnumerable<int> GetDistinctAffectedCustomerIds(long processInstanceId)
        {
            var lstAffectedCustomerIds = new List<int>();
            SalePriceListNewDataManager salePriceListNewDataManager = new SalePriceListNewDataManager();
            string salePriceListTableAlias = "sp";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            salePriceListNewDataManager.JoinSalePriceListNew(joinContext, salePriceListTableAlias, SalePriceListNewDataManager.COL_ID, TABLE_ALIAS, COL_PricelistId);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            var groupByContext = selectQuery.GroupBy();
            groupByContext.Select().Column(salePriceListTableAlias, SalePriceListNewDataManager.COL_OwnerID);

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    lstAffectedCustomerIds.Add(reader.GetInt("OwnerID"));
                }
            });
            return lstAffectedCustomerIds;
        }
        public IEnumerable<SalePricelistRateChange> GetFilteredRatesPreviewByProcessInstanceId(int processInstanceId, string zoneName, int customerId)
        {
            var salePriceLisNewDataManager = new SalePriceListNewDataManager();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            salePriceLisNewDataManager.JoinSalePriceListNew(join, "spn", "ID", TABLE_ALIAS, COL_PricelistId);

            var whereQueryContext = selectQuery.Where();
            whereQueryContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);
            whereQueryContext.EqualsCondition(COL_ZoneName).Value(zoneName);
            whereQueryContext.NotNullCondition(COL_RateTypeId);
            whereQueryContext.EqualsCondition("spn", "OwnerID").Value(customerId);

            return queryContext.GetItems(SalePricelistRateChangeMapper);
        }

        public void BuildSelectQuery(RDBSelectQuery selectQuery, long processInstanceID)
        {
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_PricelistId, COL_Rate, COL_RateTypeId, COL_RecentRate, COL_CountryID, COL_ZoneName,
                COL_Change, COL_BED, COL_EED, COL_RoutingProductID, COL_CurrencyID, COL_ZoneID);
            selectQuery.Where().EqualsCondition(COL_ProcessInstanceID).Value(processInstanceID);
        }
        public void DeleteRecords(RDBDeleteQuery deleteQuery, long processInstanceId)
        {
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);
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
