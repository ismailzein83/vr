using System.Linq;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities.SalePricelistChanges;

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
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_PricelistId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Rate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_RateTypeId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_RecentCurrencyId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_RecentRate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_RecentRateConverted, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_CountryID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ZoneName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 150 });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Change, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_RoutingProductID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CurrencyID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

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

        public void Bulk(IEnumerable<SalePricelistRateChange> rateChanges, long processInstanceId)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (var rateChange in rateChanges)
            {
                WriteRecordToStream(rateChange, processInstanceId, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
            ApplyForDB(prepareToApplyInfo);
        }

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
            salePriceListNewDataManager.JoinSalePriceListNew(joinContext, salePriceListTableAlias, TABLE_ALIAS, COL_PricelistId);

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
            string salePricelistNewTableAlias = "spn";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            salePriceLisNewDataManager.JoinSalePriceListNew(join, salePricelistNewTableAlias, TABLE_ALIAS, COL_PricelistId);

            var whereQueryContext = selectQuery.Where();
            whereQueryContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);
            whereQueryContext.EqualsCondition(COL_ZoneName).Value(zoneName);
            whereQueryContext.NotNullCondition(COL_RateTypeId);
            whereQueryContext.EqualsCondition(salePricelistNewTableAlias, SalePriceListNewDataManager.COL_OwnerID).Value(customerId);

            return queryContext.GetItems(SalePricelistRateChangeMapper);
        }
        public List<CustomerRatePreview> GetCustomerRatePreviews(long processInstanceId, List<int> customerIds)
        {
            SalePriceListNewDataManager salePriceListNewDataManager = new SalePriceListNewDataManager();
            string salePriceListNewTableAlias = "spn";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            salePriceListNewDataManager.JoinSalePriceListNew(join, salePriceListNewTableAlias, TABLE_ALIAS, COL_PricelistId);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            if (customerIds != null && customerIds.Any())
                whereContext.ListCondition(salePriceListNewTableAlias, SalePriceListNewDataManager.COL_OwnerID, RDBListConditionOperator.IN, customerIds);

            whereContext.NotNullCondition(COL_RateTypeId);

            var groupByContext = selectQuery.GroupBy();
            var groupByColumnsContext = groupByContext.Select();
            groupByColumnsContext.Column(salePriceListNewTableAlias, SalePriceListNewDataManager.COL_OwnerID);
            groupByColumnsContext.Column(TABLE_ALIAS, COL_ZoneID);

            return queryContext.GetItems(CustomerRatePreviewMapper);
        }
        public List<ZoneCustomerPair> GetCustomerRatePreviewZonePairs(long processInstanceId, List<int> customerIds)
        {
            SalePriceListNewDataManager salePriceListNewDataManager = new SalePriceListNewDataManager();
            string salePriceListNewTableAlias = "spn";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            salePriceListNewDataManager.JoinSalePriceListNew(join, salePriceListNewTableAlias, TABLE_ALIAS, COL_PricelistId);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            if (customerIds != null && customerIds.Any())
                whereContext.ListCondition(salePriceListNewTableAlias, SalePriceListNewDataManager.COL_OwnerID, RDBListConditionOperator.IN, customerIds);

            whereContext.NotNullCondition(COL_RateTypeId);

            var groupByContext = selectQuery.GroupBy();
            var groupByColumnsContext = groupByContext.Select();
            groupByColumnsContext.Column(salePriceListNewTableAlias, SalePriceListNewDataManager.COL_OwnerID);
            groupByColumnsContext.Column(TABLE_ALIAS, COL_ZoneID);

            return queryContext.GetItems(CustomerRatePreviewZonePairsMapper);
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
        private ZoneCustomerPair CustomerRatePreviewZonePairsMapper(IRDBDataReader reader)
        {
            return new ZoneCustomerPair
            {
                ZoneId = reader.GetNullableLong(COL_ZoneID),
                CustomerId = reader.GetInt(SalePriceListNewDataManager.COL_OwnerID)
            };
        }
        private SalePricelistRateChange SalePricelistRateChangeMapper(IRDBDataReader reader)
        {
            return new SalePricelistRateChange
            {
                ZoneName = reader.GetString(COL_ZoneName),
                RateTypeId = reader.GetNullableInt(COL_RateTypeId),
                RecentRate = reader.GetNullableDecimal(COL_RecentRate),
                Rate = reader.GetDecimal(COL_Rate),
                ChangeType = (RateChangeType)reader.GetIntWithNullHandling(COL_Change),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                CurrencyId = reader.GetNullableInt(COL_CurrencyID)
            };
        }
        private CustomerRatePreview CustomerRatePreviewMapper(IRDBDataReader reader)
        {
            return new CustomerRatePreview
            {
                ZoneName = reader.GetString(COL_ZoneName),
                ZoneId = reader.GetNullableLong(COL_ZoneID),
                RoutingProductId = reader.GetInt(COL_RoutingProductID),
                CountryId = reader.GetInt(COL_CountryID),
                RecentCurrencyId = reader.GetNullableInt(COL_RecentCurrencyId),
                RecentRate = reader.GetNullableDecimal(COL_RecentRate),
                RecentRateConverted = reader.GetNullableDecimal(COL_RecentRateConverted),
                Rate = reader.GetDecimal(COL_Rate),
                ChangeType = (RateChangeType)reader.GetInt(COL_Change),
                PricelistId = reader.GetInt(COL_PricelistId),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                CurrencyId = reader.GetNullableInt(COL_CurrencyID),
                CustomerId = reader.GetInt(SalePriceListNewDataManager.COL_OwnerID)
            };
        }
        #endregion

        #region Bulk Methods

        private object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', COL_PricelistId, COL_Rate, COL_RateTypeId, COL_RecentCurrencyId, COL_RecentRate, COL_RecentRateConverted, COL_CountryID
            , COL_ZoneName, COL_ZoneID, COL_Change, COL_ProcessInstanceID, COL_BED, COL_EED, COL_RoutingProductID, COL_CurrencyID);
            return streamForBulkInsert;
        }
        private void WriteRecordToStream(SalePricelistRateChange record, long processInstanceId, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();

            recordContext.Value(record.PricelistId);
            recordContext.Value(record.Rate);

            if (record.RateTypeId.HasValue)
                recordContext.Value(record.RateTypeId.Value);
            else
                recordContext.Value(string.Empty);

            if (record.RecentCurrencyId.HasValue)
                recordContext.Value(record.RecentCurrencyId.Value);
            else
                recordContext.Value(string.Empty);

            if (record.RecentRate.HasValue)
                recordContext.Value(record.RecentRate.Value);
            else
                recordContext.Value(string.Empty);

            if (record.RecentRateConverted.HasValue)
                recordContext.Value(record.RecentRateConverted.Value);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.CountryId);
            recordContext.Value(record.ZoneName);

            if (record.ZoneId.HasValue)
                recordContext.Value(record.ZoneId.Value);
            else
                recordContext.Value(string.Empty);

            recordContext.Value((int)record.ChangeType);
            recordContext.Value(processInstanceId);
            recordContext.Value(record.BED);

            if (record.EED.HasValue)
                recordContext.Value(record.EED.Value);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.RoutingProductId);

            if (record.CurrencyId.HasValue)
                recordContext.Value(record.CurrencyId.Value);
            else
                recordContext.Value(string.Empty);
        }
        private object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }
        private void ApplyForDB(object preparedObject)
        {
            preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        #endregion
    }
}
