using System;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SaleRateDataManager : ISaleRateDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "sr";
        static string TABLE_NAME = "TOneWhS_BE_SaleRate";
        public const string COL_ID = "ID";
        const string COL_PriceListID = "PriceListID";
        const string COL_ZoneID = "ZoneID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_RateTypeID = "RateTypeID";
        public const string COL_Rate = "Rate";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_Change = "Change";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";

        const string COL_StateBackupID = "StateBackupID";
        static SaleRateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_PriceListID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_ZoneID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_CurrencyID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_RateTypeID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Rate, new RDBTableColumnDefinition {DataType = RDBDataType.Decimal, Size = 20, Precision = 8}},
                {COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_EED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_SourceID, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 50}},
                {COL_Change, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SaleRate",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #endregion

        #region ISaleRateDataManager Members

        public List<SaleRate> GetEffectiveSaleRates(DateTime effectiveOn)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            BEDataUtility.SetEffectiveDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn);

            return queryContext.GetItems(SaleRateMapper);
        }

        public List<SaleRate> GetSaleRatesEffectiveAfter(int sellingNumberPlanId, DateTime minimumDate)
        {
            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            string saleZoneTableAlias = "sz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var whereQuery = selectQuery.Where();

            var orDateCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);
            orDateCondition.GreaterThanCondition(COL_EED).Value(minimumDate);

            whereQuery.EqualsCondition(saleZoneTableAlias, SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);

            return queryContext.GetItems(SaleRateMapper);
        }

        public IEnumerable<SaleRate> GetSaleRatesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(join, "sp", TABLE_ALIAS, COL_PriceListID, true);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition("sp", SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);
            whereQuery.EqualsCondition("sp", SalePriceListDataManager.COL_OwnerID).Value(ownerId);

            var andCondition = whereQuery.ChildConditionGroup();
            var orCondition = andCondition.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            var dateAndCondition = orCondition.ChildConditionGroup();
            dateAndCondition.GreaterThanCondition(COL_EED).Column(COL_BED);
            dateAndCondition.GreaterThanCondition(COL_EED).Value(minimumDate);

            return queryContext.GetItems(SaleRateMapper);
        }

        public List<SaleRate> GetSaleRatesInBetweenPeriod(DateTime fromTime, DateTime tillTime)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            whereQuery.LessOrEqualCondition(COL_BED).Value(tillTime);
            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Value(fromTime);

            return queryContext.GetItems(SaleRateMapper);
        }

        public List<SaleRate> GetEffectiveSaleRateByOwner(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            string salePriceListTableAlias = "sp";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var tempTableQuery = CreateTempTable(queryContext, customerInfos);
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var salePriceListJoin = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(salePriceListJoin, salePriceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var joinContext = selectQuery.Join();
            var joinStatement = joinContext.Join(tempTableQuery, "customerInfo");
            joinStatement.JoinType(RDBJoinType.Inner);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(salePriceListTableAlias, SalePriceListDataManager.COL_OwnerID, "customerInfo", SalePriceListDataManager.COL_OwnerID);
            joinCondition.EqualsCondition(salePriceListTableAlias, SalePriceListDataManager.COL_OwnerType, "customerInfo", SalePriceListDataManager.COL_OwnerType);

            var whereContext = selectQuery.Where();

            BEDataUtility.SetDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, isEffectiveInFuture, effectiveOn);
            return queryContext.GetItems(SaleRateMapper);
        }

        public List<SaleRate> GetEffectiveAfterByMultipleOwners(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime effectiveAfter)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            string salePriceListTableAlias = "sp";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var tempTableQuery = CreateTempTable(queryContext, customerInfos);

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();

            salePriceListDataManager.JoinSalePriceList(joinContext, salePriceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var joinStatement = joinContext.Join(tempTableQuery, "customerInfo");
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(salePriceListTableAlias, SalePriceListDataManager.COL_OwnerID, "customerInfo", SalePriceListDataManager.COL_OwnerID);
            joinCondition.EqualsCondition(salePriceListTableAlias, SalePriceListDataManager.COL_OwnerType, "customerInfo", SalePriceListDataManager.COL_OwnerType);

            var whereQuery = selectQuery.Where();

            SetDateCondition(whereQuery, effectiveAfter);

            return queryContext.GetItems(SaleRateMapper);
        }

        public bool AreSaleRatesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public IEnumerable<SaleRate> GetExistingRatesByZoneIds(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime minEED)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            string priceListTableAlias = "p";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();

            salePriceListDataManager.JoinSalePriceList(join, priceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);
            whereQuery.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID).Value(ownerId);

            whereQuery.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, zoneIds);

            SetDateCondition(whereQuery, minEED);

            return queryContext.GetItems(SaleRateMapper);
        }

        public IEnumerable<SaleRate> GetFutureSaleRates(SalePriceListOwnerType ownerType, int ownerId)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            string priceListTableAlias = "p";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(join, priceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);
            whereQuery.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID).Value(ownerId);

            SetDateCondition(whereQuery, DateTime.Now);

            return queryContext.GetItems(SaleRateMapper);
        }

        public SaleRate GetSaleRateById(long rateId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_ID).Value(rateId);
            return queryContext.GetItem(SaleRateMapper);
        }

        public IEnumerable<SaleRate> GetSaleRatesEffectiveAfterByOwnerAndZones(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime effectiveOn)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            string priceListTableAlias = "p";

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, priceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var whereContext = selectQuery.Where();
            whereContext.LessOrEqualCondition(COL_BED).DateNow();
            whereContext.NotNullCondition(COL_EED);
            whereContext.GreaterThanCondition(COL_EED).Column(COL_BED);

            whereContext.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);
            whereContext.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID).Value(ownerId);

            whereContext.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, zoneIds);

            whereContext.GreaterThanCondition(COL_EED).Value(effectiveOn);
            return queryContext.GetItems(SaleRateMapper);
        }

        public IEnumerable<SaleRate> GetZoneRatesBySellingProducts(IEnumerable<int> sellingProductIds, IEnumerable<long> saleZoneIds)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            string priceListTableAlias = "p";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, priceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var whereQuery = selectQuery.Where();

            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Column(COL_BED);

            whereQuery.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)SalePriceListOwnerType.SellingProduct);

            whereQuery.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, saleZoneIds);
            whereQuery.ListCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID, RDBListConditionOperator.IN, sellingProductIds);

            return queryContext.GetItems(SaleRateMapper);
        }

        public IEnumerable<SaleRate> GetAllSaleRatesByOwner(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> saleZoneIds, bool getNormalRates, bool getOtherRates)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            string priceListTableAlias = "p";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, priceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var whereContext = selectQuery.Where();

            whereContext.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);
            whereContext.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID).Value(ownerId);

            var orCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Column(COL_BED);

            var rateTypeCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);

            if (getNormalRates)
                rateTypeCondition.NullCondition(COL_RateTypeID);

            if (getOtherRates)
                rateTypeCondition.NotNullCondition(COL_RateTypeID);

            if (saleZoneIds != null && saleZoneIds.Any())
                whereContext.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, saleZoneIds);

            return queryContext.GetItems(SaleRateMapper);

        }

        public IEnumerable<SaleRate> GetAllSaleRatesBySellingProductAndCustomer(IEnumerable<long> saleZoneIds, int sellingProductId, int customerId, bool getNormalRates, bool getOtherRates)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            string priceListTableAlias = "p";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, priceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var whereQuery = selectQuery.Where();

            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Column(COL_BED);

            var rateTypeCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            if (getNormalRates)
                rateTypeCondition.NullCondition(COL_RateTypeID);
            else
                rateTypeCondition.NotNullCondition(COL_RateTypeID);

            whereQuery.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, saleZoneIds);

            var ownerOrCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);

            var customerCondition = ownerOrCondition.ChildConditionGroup();
            customerCondition.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)SalePriceListOwnerType.Customer);
            customerCondition.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID).Value(customerId);

            var sellingProductCondition = ownerOrCondition.ChildConditionGroup();
            sellingProductCondition.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)SalePriceListOwnerType.SellingProduct);
            sellingProductCondition.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID).Value(sellingProductId);

            return queryContext.GetItems(SaleRateMapper);
        }

        public IEnumerable<SaleRate> GetSaleRatesEffectiveAfterByOwnersAndZones(SalePriceListOwnerType ownerType, IEnumerable<int> ownerIds, IEnumerable<long> zoneIds, DateTime minimumDate)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            string priceListTableAlias = "p";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, priceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var whereContext = selectQuery.Where();

            var orCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Value(minimumDate);

            whereContext.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);
            whereContext.ListCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID, RDBListConditionOperator.IN, ownerIds);
            whereContext.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, zoneIds);

            return queryContext.GetItems(SaleRateMapper);
        }

        public DateTime? GetNextOpenOrCloseTime(DateTime effectiveDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.SelectAggregates().Aggregate(RDBNonCountAggregateType.MIN, "NextOpenOrCloseTime");

            var unionSelect = selectQuery.FromSelectUnion("v");

            var firstSelect = unionSelect.AddSelect();
            firstSelect.From(TABLE_NAME, TABLE_ALIAS, null, true);
            firstSelect.SelectColumns().Expression("NextOpenOrCloseTime").Aggregate(RDBNonCountAggregateType.MIN, COL_BED);

            var firstSelectWhereContext = firstSelect.Where();
            firstSelectWhereContext.GreaterThanCondition(COL_BED).Value(effectiveDate);
            firstSelectWhereContext.NotEqualsCondition(COL_BED).Column(COL_EED);


            var secondSelect = unionSelect.AddSelect();
            secondSelect.From(TABLE_NAME, TABLE_ALIAS, null, true);
            secondSelect.SelectColumns().Expression("NextOpenOrCloseTime").Aggregate(RDBNonCountAggregateType.MIN, COL_EED);

            var secondSelectWhereContext = secondSelect.Where();
            secondSelectWhereContext.GreaterThanCondition(COL_EED).Value(effectiveDate);
            secondSelectWhereContext.NotEqualsCondition(COL_BED).Column(COL_EED);

            object nextOpenOrCloseTimeAsObj = queryContext.ExecuteScalar().DateTimeValue;

            DateTime? nextOpenOrCloseTime = null;
            if (nextOpenOrCloseTimeAsObj != DBNull.Value)
                nextOpenOrCloseTime = (DateTime)nextOpenOrCloseTimeAsObj;

            return nextOpenOrCloseTime;
        }

        public object GetMaximumTimeStamp()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.GetMaxReceivedDataInfo(TABLE_NAME);
        }

        public IEnumerable<SaleRate> GetAllSaleRatesByOwners(IEnumerable<int> sellingProductIds, IEnumerable<int> customerIds, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates, DateTime? BED, DateTime? EED)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            string priceListTableAlias = "sp";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, priceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var whereQuery = selectQuery.Where();

            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Column(COL_BED);

            var orRateTypeCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            if (getNormalRates)
                orRateTypeCondition.NullCondition(COL_RateTypeID);
            if (getOtherRates)
                orRateTypeCondition.NotNullCondition(COL_RateTypeID);


            var ownerOrCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);

            var customerCondition = ownerOrCondition.ChildConditionGroup();
            customerCondition.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)SalePriceListOwnerType.Customer);
            customerCondition.ListCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID, RDBListConditionOperator.IN, customerIds);

            var sellingProductCondition = ownerOrCondition.ChildConditionGroup();
            sellingProductCondition.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)SalePriceListOwnerType.SellingProduct);
            sellingProductCondition.ListCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID, RDBListConditionOperator.IN, sellingProductIds);

            if (zoneIds != null && zoneIds.Any())
                whereQuery.ListCondition(RDBListConditionOperator.IN, zoneIds);

            if (BED.HasValue)
            {
                if (EED.HasValue)
                {
                    var andCondition = whereQuery.ChildConditionGroup();
                    andCondition.LessOrEqualCondition(COL_BED).Value(EED.Value);

                    var orDateCondition = andCondition.ChildConditionGroup(RDBConditionGroupOperator.OR);
                    orDateCondition.NullCondition(COL_EED);
                    orDateCondition.GreaterThanCondition(COL_EED).Value(BED.Value);
                }
                else whereQuery.FalseCondition();
            }
            return queryContext.GetItems(SaleRateMapper);
        }

        public IEnumerable<SaleRate> GetAllSaleRatesByOwnerType(SalePriceListOwnerType ownerType, IEnumerable<int> ownerIds, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            string priceListTableAlias = "priceList";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, priceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var whereContext = selectQuery.Where();

            var orCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Column(COL_BED);

            var rateTypeCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            if (getNormalRates)
                rateTypeCondition.NullCondition(COL_RateTypeID);

            if (getOtherRates)
                rateTypeCondition.NotNullCondition(COL_RateTypeID);

            whereContext.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);
            whereContext.ListCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID, RDBListConditionOperator.IN, ownerIds);

            if (zoneIds != null && zoneIds.Any())
                whereContext.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, zoneIds);

            return queryContext.GetItems(SaleRateMapper);
        }

        #endregion

        #region Not Used Functions

        public IEnumerable<SaleRate> GetZoneRatesBySellingProduct(int sellingProductId, IEnumerable<long> saleZoneIds)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private void SetDateCondition(RDBConditionContext conditionContext, DateTime minDate)
        {
            var andCondition = conditionContext.ChildConditionGroup();
            var orCondition = andCondition.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            var dateAndCondition = orCondition.ChildConditionGroup();
            dateAndCondition.NotEqualsCondition(COL_EED).Column(COL_BED);
            dateAndCondition.GreaterThanCondition(COL_EED).Value(minDate);
        }
        private RDBTempTableQuery CreateTempTable(RDBQueryContext queryContext, IEnumerable<RoutingCustomerInfoDetails> customerInfos)
        {
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(SalePriceListDataManager.COL_OwnerID, RDBDataType.Int, true);
            tempTableQuery.AddColumn(SalePriceListDataManager.COL_OwnerType, RDBDataType.Int, true);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableQuery);

            HashSet<int> addedSellingProductIds = new HashSet<int>();
            foreach (var queryItem in customerInfos)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(SalePriceListDataManager.COL_OwnerID).Value(queryItem.CustomerId);
                rowContext.Column(SalePriceListDataManager.COL_OwnerType).Value((int)SalePriceListOwnerType.Customer);

                if (addedSellingProductIds.Contains(queryItem.SellingProductId))
                    continue;

                var rowSellingProductContext = insertMultipleRowsQuery.AddRow();
                rowSellingProductContext.Column(SalePriceListDataManager.COL_OwnerID).Value(queryItem.SellingProductId);
                rowSellingProductContext.Column(SalePriceListDataManager.COL_OwnerType).Value((int)SalePriceListOwnerType.SellingProduct);

                addedSellingProductIds.Add(queryItem.SellingProductId);
            }
            return tempTableQuery;
        }
        #endregion

        #region Mappers

        private SaleRate SaleRateMapper(IRDBDataReader reader)
        {
            return new SaleRate
            {
                SaleRateId = reader.GetLong(COL_ID),
                PriceListId = reader.GetInt(COL_PriceListID),
                ZoneId = reader.GetLong(COL_ZoneID),
                RateTypeId = reader.GetNullableInt(COL_RateTypeID),
                Rate = reader.GetDecimal(COL_Rate),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                RateChange = (RateChangeType)reader.GetIntWithNullHandling(COL_Change),
                CurrencyId = reader.GetNullableInt(COL_CurrencyID)
            };
        }

        #endregion

        #region Public Methods
        public void BuildInsertQuery(RDBInsertQuery insertQuery)
        {
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_Change).Value((int)RateChangeType.New);
        }

        public void BuildUpdateQuery(RDBUpdateQuery updateQuery, long processInstanceID, string joinTableAlias, string columnName)
        {
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_EED).Column(joinTableAlias, COL_EED);
            updateQuery.Where().EqualsCondition(joinTableAlias, columnName).Value(processInstanceID);
        }
        #endregion

        #region StateBackup

        //public void BackupBySNPId(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName, int sellingNumberPlanId)
        //{
        //    var saleRateBackupDataManager = new SaleRateBackupDataManager();
        //    var insertQuery = saleRateBackupDataManager.GetInsertQuery(queryContext, backupDatabaseName);

        //    var selectQuery = insertQuery.FromSelect();

        //    selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

        //    var selectColumns = selectQuery.SelectColumns();
        //    selectColumns.Column(COL_ID, COL_ID);
        //    selectColumns.Column(COL_PriceListID, COL_PriceListID);
        //    selectColumns.Column(COL_ZoneID, COL_ZoneID);
        //    selectColumns.Column(COL_CurrencyID, COL_CurrencyID);
        //    selectColumns.Column(COL_RateTypeID, COL_RateTypeID);
        //    selectColumns.Column(COL_Rate, COL_Rate);
        //    selectColumns.Column(COL_BED, COL_BED);
        //    selectColumns.Column(COL_EED, COL_EED);
        //    selectColumns.Column(COL_SourceID, COL_SourceID);
        //    selectColumns.Column(COL_Change, COL_Change);
        //    selectColumns.Expression(SaleRateBackupDataManager.COL_StateBackupID).Value(stateBackupId);
        //    selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

        //    var joinContext = selectQuery.Join();
        //    var saleZoneDataManager = new SaleZoneDataManager();
        //    string saleZoneTableAlias = "sz";
        //    saleZoneDataManager.JoinSaleZone(joinContext, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

        //    var whereContext = selectQuery.Where();
        //    whereContext.EqualsCondition(saleZoneTableAlias, SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);

        //}
        //public void BackupByOwner(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName, IEnumerable<int> ownerIds, int ownerType)
        //{
        //    var saleRateBackupDataManager = new SaleRateBackupDataManager();
        //    var insertQuery = saleRateBackupDataManager.GetInsertQuery(queryContext, backupDatabaseName);

        //    var selectQuery = insertQuery.FromSelect();
        //    selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

        //    var selectColumns = selectQuery.SelectColumns();
        //    selectColumns.Column(COL_ID, COL_ID);
        //    selectColumns.Column(COL_PriceListID, COL_PriceListID);
        //    selectColumns.Column(COL_ZoneID, COL_ZoneID);
        //    selectColumns.Column(COL_CurrencyID, COL_CurrencyID);
        //    selectColumns.Column(COL_RateTypeID, COL_RateTypeID);
        //    selectColumns.Column(COL_Rate, COL_Rate);
        //    selectColumns.Column(COL_BED, COL_BED);
        //    selectColumns.Column(COL_EED, COL_EED);
        //    selectColumns.Column(COL_SourceID, COL_SourceID);
        //    selectColumns.Column(COL_Change, COL_Change);
        //    selectColumns.Expression(SaleRateBackupDataManager.COL_StateBackupID).Value(stateBackupId);
        //    selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

        //    var joinContext = selectQuery.Join();
        //    string priceListTableAlias = "spl";
        //    var salePriceListDataManager = new SalePriceListDataManager();
        //    salePriceListDataManager.JoinSalePriceList(joinContext, priceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

        //    var whereContext = selectQuery.Where();
        //    whereContext.ListCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID, RDBListConditionOperator.IN, ownerIds);
        //    whereContext.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value(ownerType);
        //}
        //public void SetDeleteQueryBySNPId(RDBQueryContext queryContext, long sellingNumberPlanId)
        //{
        //    var deleteQuery = queryContext.AddDeleteQuery();
        //    deleteQuery.FromTable(TABLE_NAME);

        //    string saleZoneTableAlias = "sz";
        //    var joinContext = deleteQuery.Join(TABLE_ALIAS);
        //    var saleZoneDataManager = new SaleZoneDataManager();
        //    saleZoneDataManager.JoinSaleZone(joinContext, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID, false);

        //    deleteQuery.Where().EqualsCondition(saleZoneTableAlias, SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);
        //}

        //public void SetDeleteQueryByOwner(RDBQueryContext queryContext, IEnumerable<int> ownerIds, int ownerType)
        //{
        //    var deleteQuery = queryContext.AddDeleteQuery();
        //    deleteQuery.FromTable(TABLE_NAME);

        //    var joinContext = deleteQuery.Join(TABLE_ALIAS);
        //    string salePriceListTableAlias = "spl";
        //    var salePriceListDataManager = new SalePriceListDataManager();
        //    salePriceListDataManager.JoinSalePriceList(joinContext, salePriceListTableAlias, TABLE_ALIAS, COL_PriceListID, false);

        //    var whereContext = deleteQuery.Where();
        //    whereContext.ListCondition(salePriceListTableAlias, SalePriceListDataManager.COL_OwnerID, RDBListConditionOperator.IN, ownerIds);
        //    whereContext.EqualsCondition(salePriceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value(ownerType);
        //}

        //public void SetRestoreQuery(RDBQueryContext queryContext, long stateBackupId, string backupDataBaseName)
        //{
        //    var insertQuery = queryContext.AddInsertQuery();
        //    insertQuery.IntoTable(TABLE_NAME);
        //    var saleRateBackupDataManager = new SaleRateBackupDataManager();
        //    saleRateBackupDataManager.AddSelectQuery(insertQuery, backupDataBaseName, stateBackupId);
        //}
        #endregion
    }
}
