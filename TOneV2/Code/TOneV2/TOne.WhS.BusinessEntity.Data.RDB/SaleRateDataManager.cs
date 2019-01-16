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
            BEDataUtility.SetEffectiveAfterDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn);

            return queryContext.GetItems(SaleRateMapper);
        }

        public List<SaleRate> GetSaleRatesEffectiveAfter(int sellingNumberPlanId, DateTime minimumDate)
        {
            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();//TODO Join needs NoLock
            saleZoneDataManager.JoinSaleZone(join, "sz", TABLE_ALIAS, COL_ZoneID);

            var whereQuery = selectQuery.Where();
            BEDataUtility.SetFutureDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, minimumDate);

            whereQuery.EqualsCondition(SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);

            return queryContext.GetItems(SaleRateMapper);
        }

        public IEnumerable<SaleRate> GetSaleRatesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();//TODO Join needs NoLock
            salePriceListDataManager.JoinSalePriceList(join, "sp", TABLE_ALIAS, COL_PriceListID);

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
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var salePriceListJoin = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(salePriceListJoin, "p", TABLE_ALIAS, COL_PriceListID);

            var tempTableQuery = CreateTempTable(queryContext, customerInfos);

            var joinContext = selectQuery.Join();
            var joinStatement = joinContext.Join(tempTableQuery, "customerInfo");
            joinStatement.JoinType(RDBJoinType.Left);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(TABLE_ALIAS, SalePriceListDataManager.COL_OwnerID, "customerInfo", SalePriceListDataManager.COL_OwnerID);
            joinCondition.EqualsCondition(TABLE_ALIAS, SalePriceListDataManager.COL_OwnerType, "customerInfo", SalePriceListDataManager.COL_OwnerType);

            var whereQuery = selectQuery.Where();

            if (effectiveOn.HasValue)
            {
                if (isEffectiveInFuture)
                    BEDataUtility.SetEffectiveAfterDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
                else
                    BEDataUtility.SetFutureDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);

            }
            else
            {
                whereQuery.FalseCondition();
            }
            return queryContext.GetItems(SaleRateMapper);
        }

        public List<SaleRate> GetEffectiveAfterByMultipleOwners(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime effectiveAfter)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(join, "p", TABLE_ALIAS, COL_PriceListID);

            var tempTableQuery = CreateTempTable(queryContext, customerInfos);

            var joinContext = selectQuery.Join();
            var joinStatement = joinContext.Join(tempTableQuery, "customerInfo");
            joinStatement.JoinType(RDBJoinType.Left);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(TABLE_ALIAS, SalePriceListDataManager.COL_OwnerID, "customerInfo", SalePriceListDataManager.COL_OwnerID);
            joinCondition.EqualsCondition(TABLE_ALIAS, SalePriceListDataManager.COL_OwnerType, "customerInfo", SalePriceListDataManager.COL_OwnerType);

            var whereQuery = selectQuery.Where();
            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            var dateAndCondition = orCondition.ChildConditionGroup();
            dateAndCondition.GreaterThanCondition(COL_EED).Column(COL_BED);
            dateAndCondition.GreaterThanCondition(COL_EED).Value(effectiveAfter);

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

            salePriceListDataManager.JoinSalePriceList(join, priceListTableAlias, TABLE_ALIAS, COL_PriceListID);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);
            whereQuery.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID).Value(ownerId);

            whereQuery.ListCondition(RDBListConditionOperator.IN, zoneIds);

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
            salePriceListDataManager.JoinSalePriceList(join, priceListTableAlias, TABLE_ALIAS, COL_PriceListID);

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

            //TODO add withNolock on join
            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, priceListTableAlias, TABLE_ALIAS, COL_PriceListID);

            var whereQuery = selectQuery.Where();
            whereQuery.LessOrEqualCondition(COL_BED).DateNow();
            whereQuery.NotNullCondition(COL_EED);
            whereQuery.GreaterThanCondition(COL_EED).Column(COL_BED);

            whereQuery.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);
            whereQuery.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID).Value(ownerId);

            whereQuery.ListCondition(RDBListConditionOperator.IN, zoneIds);

            whereQuery.GreaterThanCondition(COL_EED).Value(effectiveOn);
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

            //TODO add withNolock on join
            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, priceListTableAlias, TABLE_ALIAS, COL_PriceListID);

            var whereQuery = selectQuery.Where();

            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Column(COL_BED);

            whereQuery.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)SalePriceListOwnerType.SellingProduct);

            whereQuery.ListCondition(RDBListConditionOperator.IN, saleZoneIds);

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

            //TODO add withNolock on join
            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, priceListTableAlias, TABLE_ALIAS, COL_PriceListID);

            var whereQuery = selectQuery.Where();

            whereQuery.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);
            whereQuery.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID).Value(ownerId);

            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Column(COL_BED);

            if (getNormalRates)
                whereQuery.NullCondition(COL_RateTypeID);

            if (getOtherRates)
                whereQuery.NotNullCondition(COL_RateTypeID);

            if (saleZoneIds != null && saleZoneIds.Any())
                whereQuery.ListCondition(RDBListConditionOperator.IN, saleZoneIds);

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

            //TODO add withNolock on join
            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, priceListTableAlias, TABLE_ALIAS, COL_PriceListID);

            var whereQuery = selectQuery.Where();

            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Column(COL_BED);

            if (getNormalRates)
                whereQuery.NullCondition(COL_RateTypeID);
            else
                whereQuery.NotNullCondition(COL_RateTypeID);

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

            //TODO add withNolock on join
            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, priceListTableAlias, TABLE_ALIAS, COL_PriceListID);

            var whereQuery = selectQuery.Where();

            whereQuery.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);

            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Value(minimumDate);

            var conditionContext = whereQuery.ChildConditionGroup();
            if (ownerIds != null && ownerIds.Any())
                conditionContext.ListCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID, RDBListConditionOperator.IN, ownerIds);

            if (zoneIds != null && zoneIds.Any())
                whereQuery.ListCondition(RDBListConditionOperator.IN, zoneIds);

            return queryContext.GetItems(SaleRateMapper);
        }

        public DateTime? GetNextOpenOrCloseTime(DateTime effectiveDate)
        {
            throw new NotImplementedException();
        }

        public object GetMaximumTimeStamp()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SaleRate> GetAllSaleRatesByOwners(IEnumerable<int> sellingProductIds, IEnumerable<int> customerIds, IEnumerable<long> zoneIds,
            bool getNormalRates, bool getOtherRates, DateTime? BED, DateTime? EED)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            string priceListTableAlias = "p";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            //TODO add withNolock on join
            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, "p", TABLE_ALIAS, COL_PriceListID);

            var whereQuery = selectQuery.Where();

            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Column(COL_BED);

            if (getNormalRates)
                whereQuery.NullCondition(COL_RateTypeID);

            if (getOtherRates)
                whereQuery.NotNullCondition(COL_RateTypeID);

            var ownerOrCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);

            var customerCondition = ownerOrCondition.ChildConditionGroup();
            customerCondition.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)SalePriceListOwnerType.Customer);
            customerCondition.ListCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID, RDBListConditionOperator.IN, customerIds);
            //TODO list condition with join

            var sellingProductCondition = ownerOrCondition.ChildConditionGroup();
            sellingProductCondition.EqualsCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)SalePriceListOwnerType.SellingProduct);
            sellingProductCondition.ListCondition(priceListTableAlias, SalePriceListDataManager.COL_OwnerID, RDBListConditionOperator.IN, sellingProductIds);
            //TODO list condition with join

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

        public IEnumerable<SaleRate> GetAllSaleRatesByOwnerType(SalePriceListOwnerType ownerType, IEnumerable<int> ownerIds, IEnumerable<long> zoneIds,
            bool getNormalRates, bool getOtherRates)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            //TODO add withNolock on join
            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, "p", TABLE_ALIAS, COL_PriceListID);

            var whereQuery = selectQuery.Where();

            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Column(COL_BED);

            if (getNormalRates)
                whereQuery.NullCondition(COL_RateTypeID);

            if (getOtherRates)
                whereQuery.NotNullCondition(COL_RateTypeID);

            whereQuery.EqualsCondition("p", SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);

            if (zoneIds != null && zoneIds.Any())
                whereQuery.ListCondition(RDBListConditionOperator.IN, zoneIds);

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
                ZoneId = reader.GetLong(COL_ZoneID),
                PriceListId = reader.GetInt(COL_PriceListID),
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

    }
}
