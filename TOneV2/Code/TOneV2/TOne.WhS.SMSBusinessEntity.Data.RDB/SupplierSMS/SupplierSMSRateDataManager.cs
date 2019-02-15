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
    public class SupplierSMSRateDataManager : ISupplierSMSRateDataManager
    {
        #region RDB
        static string TABLE_NAME = "TOneWhS_SMSBE_SupplierRate";
        static string TABLE_ALIAS = "smsSupplierRate";

        const string COL_ID = "ID";
        const string COL_PriceListID = "PriceListID";
        const string COL_MobileNetworkID = "MobileNetworkID";
        const string COL_Rate = "Rate";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";

        static SupplierSMSRateDataManager()
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
                DBTableName = "SupplierRate",
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
        public List<SupplierSMSRate> GetSupplierSMSRatesEffectiveAfter(int supplierID, DateTime effectiveDate)
        {
            string otherTableAlias = "supplierPriceList";

            SupplierSMSPriceListDataManager supplierSMSPriceListDataManager = new SupplierSMSPriceListDataManager();
            string supplierIDCol = SupplierSMSPriceListDataManager.COL_SupplierID;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_PriceListID, COL_MobileNetworkID, COL_Rate, COL_BED, COL_EED);

            var where = selectQuery.Where();
            where.EqualsCondition(otherTableAlias, supplierIDCol).Value(supplierID);

            var childWhere = where.ChildConditionGroup().ConditionIfColumnNotNull(COL_EED);
            childWhere.NotEqualsCondition(COL_BED).Column(COL_EED);
            childWhere.GreaterThanCondition(COL_EED).Value(effectiveDate);

            var joinContext = selectQuery.Join();

            new SupplierSMSPriceListDataManager().JoinRateTableWithPriceListTable(joinContext, otherTableAlias, TABLE_ALIAS, COL_PriceListID);

            selectQuery.Sort().ByColumn(TABLE_ALIAS, COL_BED, RDBSortDirection.ASC);
            return queryContext.GetItems(SupplierSMSRateMapper);
        }

        public bool ApplySupplierRates(SupplierSMSPriceList supplierSMSPriceList, Dictionary<int, SupplierSMSRateChange> supplierRateChangesByMobileNetworkId)
        {
            List<int> mobileNetworkIDs = supplierRateChangesByMobileNetworkId.Keys.ToList();
            List<SupplierSMSRateChange> supplierRateChanges = supplierRateChangesByMobileNetworkId.Values.ToList();
            DateTime effectiveDate = supplierSMSPriceList.EffectiveOn;

            var queryContext = new RDBQueryContext(GetDataProvider());

            new SupplierSMSPriceListDataManager().AddInsertPriceListQueryContext(queryContext, supplierSMSPriceList);

            AddUpdateSupplierRatesQuery(queryContext, supplierSMSPriceList.SupplierID, effectiveDate, mobileNetworkIDs);

            AddInsertSupplierRatesQuery(queryContext, supplierSMSPriceList.ID, effectiveDate, supplierRateChanges);

            return queryContext.ExecuteNonQuery(true) > 0;
        }

        #endregion

        #region Private Methods

        private void AddUpdateSupplierRatesQuery(RDBQueryContext queryContext, int supplierID, DateTime effectiveDate, List<int> mobileNetworkIDs)
        {
            string otherTableAlias = "supplierPriceList";

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var caseExpression = updateQuery.Column(COL_EED).CaseExpression();
            var newCodesCondition = caseExpression.AddCase();
            newCodesCondition.When().GreaterThanCondition(COL_BED).Value(effectiveDate);
            newCodesCondition.Then().Column(COL_BED);
            caseExpression.Else().Value(effectiveDate);

            var joinContext = updateQuery.Join(TABLE_ALIAS);
            new SupplierSMSPriceListDataManager().JoinRateTableWithPriceListTable(joinContext, otherTableAlias, TABLE_ALIAS, COL_PriceListID);

            var where = updateQuery.Where();
            where.EqualsCondition(otherTableAlias, SupplierSMSPriceListDataManager.COL_SupplierID).Value(supplierID);
            where.ListCondition(TABLE_ALIAS, COL_MobileNetworkID, RDBListConditionOperator.IN, mobileNetworkIDs);

            var childConditionGroupWhere = where.ChildConditionGroup();

            var childWhere = childConditionGroupWhere.ChildConditionGroup(RDBConditionGroupOperator.OR);
            childWhere.NullCondition(TABLE_ALIAS, COL_EED);

            var childOfChildWhere = childWhere.ChildConditionGroup();

            childOfChildWhere.NotEqualsCondition(TABLE_ALIAS, COL_BED).Column(TABLE_ALIAS, COL_EED);
            childOfChildWhere.GreaterThanCondition(TABLE_ALIAS, COL_EED).Value(effectiveDate);
        }

        private void AddInsertSupplierRatesQuery(RDBQueryContext queryContext, long priceListID, DateTime effectiveDate, List<SupplierSMSRateChange> supplierRateChanges)
        {
            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(TABLE_NAME);

            foreach (var supplierRate in supplierRateChanges)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();

                rowContext.Column(COL_PriceListID).Value(priceListID);
                rowContext.Column(COL_MobileNetworkID).Value(supplierRate.MobileNetworkID);
                rowContext.Column(COL_Rate).Value(supplierRate.NewRate);
                rowContext.Column(COL_BED).Value(effectiveDate);
            }
        }

        private SupplierSMSRate SupplierSMSRateMapper(IRDBDataReader dataReader)
        {
            return new SupplierSMSRate()
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
