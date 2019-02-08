using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common.Business;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.SMSBusinessEntity.Data.RDB
{
    public class CustomerSMSRateDataManager : ICustomerSMSRateDataManager
    {
        #region RDB
        static string TABLE_NAME = "TOneWhS_SMSBE_SaleRate";
        static string TABLE_ALIAS = "smsSaleRate";

        const string COL_ID = "ID";
        const string COL_PriceListID = "PriceListID";
        const string COL_MobileNetworkID = "MobileNetworkID";
        const string COL_Rate = "Rate";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";

        static CustomerSMSRateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_PriceListID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_MobileNetworkID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Rate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 18, Precision = 0 });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_SMSBE",
                DBTableName = "SaleRate",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_SMSBuisenessEntity", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        #endregion

        #region Public Methods
        public List<CustomerSMSRate> GetCustomerSMSRatesEffectiveAfter(int customerID, DateTime effectiveDate)
        {
            string otherTableAlias = "salePriceList";

            CustomerSMSPriceListDataManager customerSMSPriceListDataManager = new CustomerSMSPriceListDataManager();
            string customerIDCol = CustomerSMSPriceListDataManager.COL_CustomerID;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_PriceListID, COL_MobileNetworkID, COL_Rate, COL_BED, COL_EED);

            var where = selectQuery.Where().ChildConditionGroup();
            where.EqualsCondition(otherTableAlias, customerIDCol).Value(customerID);
            where.NotEqualsCondition(COL_BED).Column(COL_EED);
            var conditionColNotNull = where.ConditionIfColumnNotNull(COL_EED);
            conditionColNotNull.GreaterThanCondition(COL_EED).Value(effectiveDate);

            var joinContext = selectQuery.Join();

            new CustomerSMSPriceListDataManager().JoinRateTableWithPriceListTable(joinContext, otherTableAlias, TABLE_ALIAS, COL_PriceListID);

            selectQuery.Sort().ByColumn(TABLE_ALIAS, COL_BED, RDBSortDirection.ASC);
            return queryContext.GetItems(CustomerSMSRateMapper);
        }

        public List<CustomerSMSRate> GetOverlappedCustomerSMSRates(int customerID, DateTime effectiveDate, List<int> mobileNetworkIDs)
        {
            string otherTableAlias = "salePriceList";

            CustomerSMSPriceListDataManager customerSMSPriceListDataManager = new CustomerSMSPriceListDataManager();
            string customerIDCol = CustomerSMSPriceListDataManager.COL_CustomerID;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_PriceListID, COL_MobileNetworkID, COL_Rate, COL_BED, COL_EED);

            var where = selectQuery.Where();
            where.EqualsCondition(otherTableAlias, customerIDCol).Value(customerID);
            where.NotEqualsCondition(TABLE_ALIAS, COL_BED).Column(TABLE_ALIAS, COL_EED);
            where.ListCondition(TABLE_ALIAS, COL_MobileNetworkID, RDBListConditionOperator.IN, mobileNetworkIDs);

            var childWhere = where.ChildConditionGroup(RDBConditionGroupOperator.OR);
            childWhere.GreaterThanCondition(TABLE_ALIAS, COL_EED).Value(effectiveDate);
            childWhere.NullCondition(TABLE_ALIAS, COL_EED);

            var joinContext = selectQuery.Join();

            new CustomerSMSPriceListDataManager().JoinRateTableWithPriceListTable(joinContext, otherTableAlias, TABLE_ALIAS, COL_PriceListID);

            selectQuery.Sort().ByColumn(TABLE_ALIAS, COL_BED, RDBSortDirection.ASC);
            return queryContext.GetItems(CustomerSMSRateMapper);
        }



        public bool ApplySaleRates(CustomerSMSPriceList customerSMSPriceList, Dictionary<int, CustomerSMSRateChange> saleRateChangesByMobileNetworkId)
        {
            List<int> mobileNetworkIDs = saleRateChangesByMobileNetworkId.Keys.ToList();
            List<CustomerSMSRateChange> saleRateChanges = saleRateChangesByMobileNetworkId.Values.ToList();
            DateTime effectiveDate = customerSMSPriceList.EffectiveOn;

            var queryContext = new RDBQueryContext(GetDataProvider());

            new CustomerSMSPriceListDataManager().AddInsertPriceListQueryContext(queryContext, customerSMSPriceList);

            AddInsertSaleRatesQuery(queryContext, customerSMSPriceList.ID, effectiveDate, saleRateChanges);

            AddUpdateSaleRatesQuery(queryContext, customerSMSPriceList.CustomerID, effectiveDate, mobileNetworkIDs);

            return queryContext.ExecuteNonQuery(true) > 0;
        }

        #endregion

        #region Private Methods

        private void AddInsertSaleRatesQuery(RDBQueryContext queryContext, long priceListID, DateTime effectiveDate, List<CustomerSMSRateChange> saleRateChanges)
        {
            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(TABLE_NAME);

            foreach (var saleRate in saleRateChanges)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();

                rowContext.Column(COL_PriceListID).Value(priceListID);
                rowContext.Column(COL_MobileNetworkID).Value(saleRate.MobileNetworkID);
                rowContext.Column(COL_Rate).Value(saleRate.NewRate);
                rowContext.Column(COL_BED).Value(effectiveDate);
                rowContext.Column(COL_EED).Null();
            }
        }

        private void AddUpdateSaleRatesQuery(RDBQueryContext queryContext, int customerID, DateTime effectiveDate, List<int> mobileNetworkIDs)
        {
            string customerIDCol = CustomerSMSPriceListDataManager.COL_CustomerID;
            string otherTableAlias = "salePriceList";

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var caseExpression = updateQuery.Column(COL_EED).CaseExpression();
            var newCodesCondition = caseExpression.AddCase();
            newCodesCondition.When().GreaterThanCondition(COL_BED).Value(effectiveDate);
            newCodesCondition.Then().Column(COL_BED);
            caseExpression.Else().Value(effectiveDate);
            var where = updateQuery.Where();
            where.EqualsCondition(otherTableAlias, customerIDCol).Value(customerID);
            where.NotEqualsCondition(TABLE_ALIAS, COL_BED).Column(TABLE_ALIAS, COL_EED);
            where.ListCondition(TABLE_ALIAS, COL_MobileNetworkID, RDBListConditionOperator.IN, mobileNetworkIDs);

            var childWhere = where.ChildConditionGroup(RDBConditionGroupOperator.OR);
            childWhere.GreaterThanCondition(TABLE_ALIAS, COL_EED).Value(effectiveDate);
            childWhere.NullCondition(TABLE_ALIAS, COL_EED);

            var joinContext = updateQuery.Join(TABLE_ALIAS);

            new CustomerSMSPriceListDataManager().JoinRateTableWithPriceListTable(joinContext, otherTableAlias, TABLE_ALIAS, COL_PriceListID);
        }

        private CustomerSMSRate CustomerSMSRateMapper(IRDBDataReader dataReader)
        {
            return new CustomerSMSRate()
            {
                ID = dataReader.GetLong(COL_ID),
                PriceListID = dataReader.GetInt(COL_PriceListID),
                MobileNetworkID = dataReader.GetInt(COL_MobileNetworkID),
                Rate = dataReader.GetDecimal(COL_Rate),
                BED = dataReader.GetDateTime(COL_BED),
                EED = dataReader.GetNullableDateTime(COL_EED)
            };
        }

        #endregion
    }
}
