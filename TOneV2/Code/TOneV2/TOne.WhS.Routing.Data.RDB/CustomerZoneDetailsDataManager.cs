using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.RDB
{
    public class CustomerZoneDetailsDataManager : RoutingDataManager, ICustomerZoneDetailsDataManager
    {
        #region Fields/Ctor

        private static string DBTABLE_SCHEMA = "dbo";
        internal static string DBTABLE_NAME = "CustomerZoneDetail";
        private static string TABLE_NAME = "dbo_CustomerZoneDetail";
        private static string TABLE_ALIAS = "czd";

        internal const string COL_CustomerId = "CustomerId";
        internal const string COL_SaleZoneId = "SaleZoneId";
        private const string COL_SellingNumberPlanId = "SellingNumberPlanId";
        private const string COL_RoutingProductId = "RoutingProductId";
        private const string COL_RoutingProductSource = "RoutingProductSource";
        internal const string COL_SellingProductId = "SellingProductId";
        internal const string COL_EffectiveRateValue = "EffectiveRateValue";
        private const string COL_RateSource = "RateSource";
        internal const string COL_SaleZoneServiceIds = "SaleZoneServiceIds";
        private const string COL_DealId = "DealId";
        internal const string COL_VersionNumber = "VersionNumber";

        internal static Dictionary<string, RoutingTableColumnDefinition> s_CustomerZoneDetailsColumnDefinitions;

        public DateTime? EffectiveDate { get; set; }

        public bool? IsFuture { get; set; }

        static CustomerZoneDetailsDataManager()
        {
            s_CustomerZoneDetailsColumnDefinitions = BuildCustomerZoneDetailsColumnDefinitions();
            Dictionary<string, RDBTableColumnDefinition> columns = Helper.GetRDBTableColumnDefinitions(s_CustomerZoneDetailsColumnDefinitions);

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = DBTABLE_SCHEMA,
                DBTableName = DBTABLE_NAME,
                Columns = columns
            });
        }

        #endregion

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var bulkInsertQueryContext = queryContext.StartBulkInsert();
            bulkInsertQueryContext.IntoTable(TABLE_NAME, '^', COL_CustomerId, COL_SaleZoneId, COL_SellingNumberPlanId, COL_RoutingProductId, COL_RoutingProductSource,
                COL_SellingProductId, COL_EffectiveRateValue, COL_RateSource, COL_SaleZoneServiceIds, COL_DealId, COL_VersionNumber);

            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(CustomerZoneDetail record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            string saleZoneServiceIds = record.SaleZoneServiceIds != null ? string.Join(",", record.SaleZoneServiceIds) : null;

            var recordContext = bulkInsertQueryContext.WriteRecord();
            recordContext.Value(record.CustomerId);
            recordContext.Value(record.SaleZoneId);
            recordContext.Value(record.SellingNumberPlanId);
            recordContext.Value(record.RoutingProductId);
            recordContext.Value((int)record.RoutingProductSource);
            recordContext.Value(record.SellingProductId);
            recordContext.Value(record.EffectiveRateValue);
            recordContext.Value((int)record.RateSource);
            recordContext.Value(saleZoneServiceIds);

            if (record.DealId.HasValue)
                recordContext.Value(record.DealId.Value);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.VersionNumber);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertQueryContext.CloseStream();
            return bulkInsertQueryContext;
        }

        public void ApplyCustomerZoneDetailsToDB(object preparedObject)
        {
            preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        public void SaveCustomerZoneDetailsToDB(List<CustomerZoneDetail> customerZoneDetails)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (CustomerZoneDetail customerZoneDetail in customerZoneDetails)
                WriteRecordToStream(customerZoneDetail, dbApplyStream);
            Object preparedCustomerZoneDetails = FinishDBApplyStream(dbApplyStream);
            ApplyCustomerZoneDetailsToDB(preparedCustomerZoneDetails);
        }

        public IEnumerable<CustomerZoneDetail> GetCustomerZoneDetails()
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems<CustomerZoneDetail>(CustomerZoneDetailMapper);
        }

        public List<CustomerZoneDetail> GetCustomerZoneDetailsAfterVersionNumber(int versionNumber)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.GreaterThanCondition(COL_VersionNumber).ObjectValue(versionNumber);

            return queryContext.GetItems<CustomerZoneDetail>(CustomerZoneDetailMapper);
        }

        public List<CustomerZoneDetail> GetCustomerZoneDetailsByZoneIdsAndCustomerIds(List<long> saleZoneIds, List<int> customerIds)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            if (saleZoneIds != null && saleZoneIds.Count() > 0)
                whereContext.ListCondition(COL_SaleZoneId, RDBListConditionOperator.IN, saleZoneIds);

            if (customerIds != null && customerIds.Count() > 0)
                whereContext.ListCondition(COL_CustomerId, RDBListConditionOperator.IN, customerIds);

            return queryContext.GetItems<CustomerZoneDetail>(CustomerZoneDetailMapper);
        }

        public IEnumerable<CustomerZoneDetail> GetFilteredCustomerZoneDetailsByZone(IEnumerable<long> saleZoneIds)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.ListCondition(COL_SaleZoneId, RDBListConditionOperator.IN, saleZoneIds);

            return queryContext.GetItems<CustomerZoneDetail>(CustomerZoneDetailMapper);
        }

        public List<CustomerZoneDetail> GetCustomerZoneDetails(HashSet<CustomerSaleZone> customerSaleZones)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            string tempTableAlias = "customerSaleZone";
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_CustomerId, RDBDataType.Int, true);
            tempTableQuery.AddColumn(COL_SaleZoneId, RDBDataType.BigInt, true);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableQuery);

            foreach (var customerSaleZone in customerSaleZones)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_CustomerId).Value(customerSaleZone.CustomerId);
                rowContext.Column(COL_SaleZoneId).Value(customerSaleZone.SaleZoneId);
            }

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            var joinStatementContext = joinContext.Join(tempTableQuery, tempTableAlias);

            var joinCondition = joinStatementContext.On();
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_CustomerId, tempTableAlias, COL_CustomerId);
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_SaleZoneId, tempTableAlias, COL_SaleZoneId);

            return queryContext.GetItems<CustomerZoneDetail>(CustomerZoneDetailMapper);
        }

        public void UpdateCustomerZoneDetails(List<CustomerZoneDetail> customerZoneDetails)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            string tempTableAlias = "temp_czd";
            var tempTableQuery = queryContext.CreateTempTable();
            Helper.AddRoutingTempTableColumns(tempTableQuery, s_CustomerZoneDetailsColumnDefinitions, new List<string>() { COL_CustomerId, COL_SaleZoneId });

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableQuery);

            foreach (var customerZoneDetail in customerZoneDetails)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_RoutingProductId).Value(customerZoneDetail.RoutingProductId);
                rowContext.Column(COL_RoutingProductSource).Value((int)customerZoneDetail.RoutingProductSource);
                rowContext.Column(COL_SellingProductId).Value(customerZoneDetail.SellingProductId);
                rowContext.Column(COL_EffectiveRateValue).Value(customerZoneDetail.EffectiveRateValue);
                rowContext.Column(COL_RateSource).Value((int)customerZoneDetail.RateSource);
                rowContext.Column(COL_VersionNumber).Value(customerZoneDetail.VersionNumber);

                string saleZoneServiceIds = customerZoneDetail.SaleZoneServiceIds != null ? string.Join(",", customerZoneDetail.SaleZoneServiceIds) : null;
                if (!string.IsNullOrEmpty(saleZoneServiceIds))
                    rowContext.Column(COL_SaleZoneServiceIds).Value(saleZoneServiceIds);
                else
                    rowContext.Column(COL_SaleZoneServiceIds).Null();

                if (customerZoneDetail.DealId.HasValue)
                    rowContext.Column(COL_DealId).Value(customerZoneDetail.DealId.Value);
                else
                    rowContext.Column(COL_DealId).Null();
            }

            RDBUpdateQuery updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var joinContext = updateQuery.Join(TABLE_ALIAS);
            var joinStatementContext = joinContext.Join(tempTableQuery, tempTableAlias);

            var joinCondition = joinStatementContext.On();
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_CustomerId, tempTableAlias, COL_CustomerId);
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_SaleZoneId, tempTableAlias, COL_SaleZoneId);

            updateQuery.Column(COL_RoutingProductSource).Column(tempTableAlias, COL_RoutingProductSource);
            updateQuery.Column(COL_SellingProductId).Column(tempTableAlias, COL_SellingProductId);
            updateQuery.Column(COL_EffectiveRateValue).Column(tempTableAlias, COL_EffectiveRateValue);
            updateQuery.Column(COL_RateSource).Column(tempTableAlias, COL_RateSource);
            updateQuery.Column(COL_SaleZoneServiceIds).Column(tempTableAlias, COL_SaleZoneServiceIds);
            updateQuery.Column(COL_DealId).Column(tempTableAlias, COL_DealId);
            updateQuery.Column(COL_VersionNumber).Column(tempTableAlias, COL_VersionNumber);

            queryContext.ExecuteNonQuery();
        }

        public void AddJoinCustomerZoneDetailsByCustomerAndSaleZone(RDBJoinContext joinContext, RDBJoinType joinType, string customerZoneDetailsTableAlias, string originalTableAlias,
           string originalTableCustomerCol, string originalTableSaleZoneIdCol, bool withNoLock)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, customerZoneDetailsTableAlias);
            joinStatement.JoinType(joinType);

            if (withNoLock)
                joinStatement.WithNoLock();

            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(customerZoneDetailsTableAlias, COL_CustomerId, originalTableAlias, originalTableCustomerCol);
            joinCondition.EqualsCondition(customerZoneDetailsTableAlias, COL_SaleZoneId, originalTableAlias, originalTableSaleZoneIdCol);
        }

        public void AddJoinCustomerZoneDetailsBySaleZoneId(RDBJoinContext joinContext, RDBJoinType joinType, string customerZoneDetailsTableAlias, string originalTableAlias,
            string originalTableSaleZoneIdCol, int customerId, bool withNoLock)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, customerZoneDetailsTableAlias);
            joinStatement.JoinType(joinType);

            if (withNoLock)
                joinStatement.WithNoLock();

            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(customerZoneDetailsTableAlias, COL_SaleZoneId, originalTableAlias, originalTableSaleZoneIdCol);
            joinCondition.EqualsCondition(COL_CustomerId).Value(customerId);
        }

        public void AddJoinCustomerZoneDetailsBySaleZoneAndRoutingProduct(RDBJoinContext joinContext, RDBJoinType joinType, string customerZoneDetailsTableAlias, string originalTableAlias,
            string originalTableSaleZoneIdCol, string originalTableRoutingProductIdCol, int customerId, bool withNoLock)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, customerZoneDetailsTableAlias);
            joinStatement.JoinType(joinType);

            if (withNoLock)
                joinStatement.WithNoLock();

            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(customerZoneDetailsTableAlias, COL_SaleZoneId, originalTableAlias, originalTableSaleZoneIdCol);
            joinCondition.EqualsCondition(customerZoneDetailsTableAlias, COL_RoutingProductId, originalTableAlias, originalTableRoutingProductIdCol);
            joinCondition.EqualsCondition(COL_CustomerId).Value(customerId);
        }

        public List<CustomerZoneDetail> GetCustomerZoneDetailsByZoneRange(long fromZoneId, long toZoneId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private static Dictionary<string, RoutingTableColumnDefinition> BuildCustomerZoneDetailsColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_CustomerId, new RoutingTableColumnDefinition(COL_CustomerId, RDBDataType.Int, true));
            columnDefinitions.Add(COL_SaleZoneId, new RoutingTableColumnDefinition(COL_SaleZoneId, RDBDataType.BigInt, true));
            columnDefinitions.Add(COL_SellingNumberPlanId, new RoutingTableColumnDefinition(COL_SellingNumberPlanId, RDBDataType.Int, true));
            columnDefinitions.Add(COL_RoutingProductId, new RoutingTableColumnDefinition(COL_RoutingProductId, RDBDataType.Int));
            columnDefinitions.Add(COL_RoutingProductSource, new RoutingTableColumnDefinition(COL_RoutingProductSource, RDBDataType.Int));
            columnDefinitions.Add(COL_SellingProductId, new RoutingTableColumnDefinition(COL_SellingProductId, RDBDataType.Int));
            columnDefinitions.Add(COL_EffectiveRateValue, new RoutingTableColumnDefinition(COL_EffectiveRateValue, RDBDataType.Decimal, 20, 8, false));
            columnDefinitions.Add(COL_RateSource, new RoutingTableColumnDefinition(COL_RateSource, RDBDataType.Int));
            columnDefinitions.Add(COL_SaleZoneServiceIds, new RoutingTableColumnDefinition(COL_SaleZoneServiceIds, RDBDataType.NVarchar));
            columnDefinitions.Add(COL_DealId, new RoutingTableColumnDefinition(COL_DealId, RDBDataType.Int));
            columnDefinitions.Add(COL_VersionNumber, new RoutingTableColumnDefinition(COL_VersionNumber, RDBDataType.Int, true));
            return columnDefinitions;
        }

        private CustomerZoneDetail CustomerZoneDetailMapper(IRDBDataReader reader)
        {
            CustomerZoneDetail customerZoneDetail = new CustomerZoneDetail
            {
                CustomerId = reader.GetInt(COL_CustomerId),
                SaleZoneId = reader.GetLong(COL_SaleZoneId),
                SellingNumberPlanId = reader.GetInt(COL_SellingNumberPlanId),
                RoutingProductId = reader.GetInt(COL_RoutingProductId),
                RoutingProductSource = (SaleEntityZoneRoutingProductSource)reader.GetInt(COL_RoutingProductSource),
                SellingProductId = reader.GetInt(COL_SellingNumberPlanId),
                EffectiveRateValue = reader.GetDecimal(COL_EffectiveRateValue),
                RateSource = (SalePriceListOwnerType)reader.GetInt(COL_RateSource),
                DealId = reader.GetNullableInt(COL_DealId),
                VersionNumber = reader.GetInt(COL_VersionNumber)
            };

            string saleZoneServiceIdsAsString = reader.GetString(COL_SaleZoneServiceIds);
            if (!string.IsNullOrEmpty(saleZoneServiceIdsAsString))
                customerZoneDetail.SaleZoneServiceIds = new HashSet<int>(saleZoneServiceIdsAsString.Split(',').Select(itm => int.Parse(itm)));

            return customerZoneDetail;
        }

        #endregion
    }
}