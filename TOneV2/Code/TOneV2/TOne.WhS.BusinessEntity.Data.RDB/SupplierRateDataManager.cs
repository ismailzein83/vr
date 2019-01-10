using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SupplierRateDataManager : ISupplierRateDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "spr";
        static string TABLE_NAME = "TOneWhS_BE_SupplierRate";
        const string COL_ID = "ID";
        const string COL_PriceListID = "PriceListID";
        const string COL_ZoneID = "ZoneID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_Rate = "Rate";
        const string COL_RateTypeID = "RateTypeID";
        const string COL_Change = "Change";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";

        static SupplierRateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_PriceListID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_ZoneID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_CurrencyID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Rate, new RDBTableColumnDefinition {DataType = RDBDataType.Decimal, Size = 20, Precision = 8}},
                {COL_RateTypeID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Change, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_EED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_SourceID, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 50}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SupplierRate",
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

        #region ISupplierRateDataManager Members
        public IEnumerable<SupplierRate> GetZoneRateHistory(List<long> zoneIds)
        {
            string supplierZoneTableAlias = "spz";
            var supplierPriceListDataManager = new SupplierPriceListDataManager();
            var supplierZoneDataManager = new SupplierZoneDataManager();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinSupplierPricelist = selectQuery.Join();//TODO Join needs NoLock
            supplierPriceListDataManager.JoinSupplierPriceList(joinSupplierPricelist, "sp", TABLE_ALIAS, COL_PriceListID);

            var joinSupplierZone = selectQuery.Join();//TODO Join needs NoLock
            supplierZoneDataManager.JoinSupplierZone(joinSupplierZone, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID);

            var whereQuery = selectQuery.Where();
            if (zoneIds != null && zoneIds.Any())
                whereQuery.ListCondition(RDBListConditionOperator.IN, zoneIds);
            whereQuery.NullCondition(COL_RateTypeID);

            var ordDateCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            ordDateCondition.NullCondition(COL_EED);
            ordDateCondition.GreaterThanCondition(COL_BED).Column(COL_EED);

            return queryContext.GetItems(SupplierRateMapper);
        }

        public IEnumerable<SupplierRate> GetFilteredSupplierRates(SupplierRateQuery input, DateTime effectiveOn)
        {
            return null;
        }

        public IEnumerable<SupplierRate> GetFilteredSupplierPendingRates(SupplierRateQuery input, DateTime effectiveOn)
        {
            string supplierZoneTableAlias = "spz";
            var supplierZoneDataManager = new SupplierZoneDataManager();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinSupplierZone = selectQuery.Join();//TODO Join needs NoLock
            supplierZoneDataManager.JoinSupplierZone(joinSupplierZone, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID);

            var whereQuery = selectQuery.Where();

            whereQuery.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_ID).Value(input.SupplierId);

            if (input.CountriesIds != null && input.CountriesIds.Any())
                whereQuery.ListCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_CountryID, RDBListConditionOperator.IN, input.CountriesIds);

            if (!string.IsNullOrEmpty(input.SupplierZoneName))
            {
                string zoneNameToLower = input.SupplierZoneName.ToLower();
                whereQuery.ContainsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_Name, zoneNameToLower);
            }

            BEDataUtility.SetEffectiveAfterDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn);
            return queryContext.GetItems(SupplierRateMapper);
        }

        public IEnumerable<SupplierRate> GetSupplierRatesForZone(SupplierRateForZoneQuery input, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SupplierRate> GetSupplierRatesByZoneIds(List<long> supplierZoneIds, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SupplierRate> GetSupplierRates(List<long> supplierZoneIds, DateTime BED, DateTime? EED)
        {
            throw new NotImplementedException();
        }

        public List<SupplierRate> GetSupplierRates(int supplierId, DateTime minimumDate)
        {
            throw new NotImplementedException();
        }

        public List<SupplierRate> GetEffectiveSupplierRatesBySuppliers(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingSupplierInfo> supplierInfos)
        {
            throw new NotImplementedException();
        }

        public List<SupplierRate> GetSupplierRatesInBetweenPeriod(DateTime fromDateTime, DateTime tillDateTime)
        {
            throw new NotImplementedException();
        }

        public List<SupplierRate> GetEffectiveSupplierRates(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public bool AreSupplierRatesUpdated(ref object updateHandle)
        {
            throw new NotImplementedException();
        }

        public SupplierRate GetSupplierRateById(long rateId)
        {
            throw new NotImplementedException();
        }

        public List<SupplierRate> GetSupplierRates(HashSet<long> supplierRateIds)
        {
            throw new NotImplementedException();
        }

        public DateTime? GetNextOpenOrCloseTime(DateTime effectiveDate)
        {
            throw new NotImplementedException();
        }

        public object GetMaximumTimeStamp()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Public Methods

        public IEnumerable<SupplierOtherRate> GetFilteredSupplierOtherRates(long zoneId, DateTime effectiveOn)
        {
            SupplierPriceListDataManager supplierPriceListDataManager = new SupplierPriceListDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            supplierPriceListDataManager.JoinSupplierPriceList(join, "spr", TABLE_ALIAS, COL_PriceListID);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_ZoneID).Value(zoneId);
            whereQuery.NotNullCondition(COL_RateTypeID);

            BEDataUtility.SetEffectiveAfterDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn);

            return queryContext.GetItems(SupplierOtherRateMapper);
        }

        #endregion


        #region Mappers
        SupplierRate SupplierRateMapper(IRDBDataReader reader)
        {
            return new SupplierRate
            {
                SupplierRateId = reader.GetLong(COL_ID),
                PriceListId = reader.GetInt(COL_PriceListID),
                ZoneId = reader.GetLong(COL_ZoneID),
                CurrencyId = reader.GetNullableInt(COL_CurrencyID),
                Rate = reader.GetDecimal(COL_Rate),
                RateTypeId = reader.GetNullableInt(COL_RateTypeID),
                RateChange = (RateChangeType)reader.GetInt(COL_Change),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED)
            };
        }
        SupplierOtherRate SupplierOtherRateMapper(IRDBDataReader reader)
        {
            SupplierOtherRate supplierOtherRate = new SupplierOtherRate
            {
                SupplierRateId = reader.GetLong(COL_ID),
                PriceListId = reader.GetInt(COL_PriceListID),
                ZoneId = reader.GetLong(COL_ZoneID),
                CurrencyId = reader.GetNullableInt(COL_CurrencyID),
                Rate = reader.GetDecimal(COL_Rate),
                RateTypeId = reader.GetNullableInt(COL_RateTypeID),
                RateChange = (RateChangeType)reader.GetInt(COL_Change),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED)
            };
            return supplierOtherRate;
        }
        #endregion

    }
}
