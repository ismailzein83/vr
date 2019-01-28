﻿using System;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

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
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_PriceListID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CurrencyID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Rate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_RateTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Change, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

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

            var joinSupplierPricelist = selectQuery.Join();
            supplierPriceListDataManager.JoinSupplierPriceList(joinSupplierPricelist, "sp", TABLE_ALIAS, COL_PriceListID, RDBJoinType.Inner, true);

            var joinSupplierZone = selectQuery.Join();
            supplierZoneDataManager.JoinSupplierZone(joinSupplierZone, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var whereQuery = selectQuery.Where();
            if (zoneIds != null && zoneIds.Any())
                whereQuery.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, zoneIds);
            whereQuery.NullCondition(COL_RateTypeID);

            var ordDateCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            ordDateCondition.NullCondition(COL_EED);
            ordDateCondition.NotEqualsCondition(COL_BED).Column(COL_EED);

            selectQuery.Sort().ByColumn(COL_BED, RDBSortDirection.ASC);
            return queryContext.GetItems(SupplierRateMapper);
        }

        public IEnumerable<SupplierRate> GetFilteredSupplierRates(SupplierRateQuery input, DateTime effectiveOn)
        {
            string supplierZoneTableAlias = "spz";
            var supplierZoneDataManager = new SupplierZoneDataManager();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinSupplierZone = selectQuery.Join();//TODO Join needs NoLock
            supplierZoneDataManager.JoinSupplierZone(joinSupplierZone, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var whereQuery = selectQuery.Where();

            whereQuery.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID).Value(input.SupplierId);

            if (input.CountriesIds != null && input.CountriesIds.Any())
                whereQuery.ListCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_CountryID, RDBListConditionOperator.IN, input.CountriesIds);

            if (!string.IsNullOrEmpty(input.SupplierZoneName))
            {
                string zoneNameToLower = input.SupplierZoneName.ToLower();
                whereQuery.ContainsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_Name, zoneNameToLower);
            }

            whereQuery.NullCondition(COL_RateTypeID);
            BEDataUtility.SetEffectiveDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn);
            return queryContext.GetItems(SupplierRateMapper);
        }

        public IEnumerable<SupplierRate> GetFilteredSupplierPendingRates(SupplierRateQuery input, DateTime effectiveOn)
        {
            string supplierZoneTableAlias = "spz";
            var supplierZoneDataManager = new SupplierZoneDataManager();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinSupplierZone = selectQuery.Join();
            supplierZoneDataManager.JoinSupplierZone(joinSupplierZone, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var whereQuery = selectQuery.Where();

            whereQuery.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_ID).Value(input.SupplierId);

            if (input.CountriesIds != null && input.CountriesIds.Any())
                whereQuery.ListCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_CountryID, RDBListConditionOperator.IN, input.CountriesIds);

            if (!string.IsNullOrEmpty(input.SupplierZoneName))
            {
                string zoneNameToLower = input.SupplierZoneName.ToLower();
                whereQuery.ContainsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_Name, zoneNameToLower);
            }

            whereQuery.NullCondition(COL_RateTypeID);
            whereQuery.GreaterThanCondition(COL_BED).Value(effectiveOn);
            return queryContext.GetItems(SupplierRateMapper);
        }

        public IEnumerable<SupplierRate> GetSupplierRatesForZone(SupplierRateForZoneQuery input, DateTime effectiveOn)
        {
            return GetSupplierRatesByZoneIds(new List<long> { input.SupplierZoneId }, effectiveOn);
        }

        public IEnumerable<SupplierRate> GetSupplierRatesByZoneIds(List<long> supplierZoneIds, DateTime effectiveOn)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            whereQuery.NullCondition(COL_RateTypeID);
            BEDataUtility.SetEffectiveDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn);

            whereQuery.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, supplierZoneIds);

            return queryContext.GetItems(SupplierRateMapper);
        }

        public IEnumerable<SupplierRate> GetSupplierRates(List<long> supplierZoneIds, DateTime BED, DateTime? EED)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();

            whereQuery.NullCondition(COL_RateTypeID);
            whereQuery.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, supplierZoneIds);


            var dateAndCondition = whereQuery.ChildConditionGroup();

            if (EED.HasValue)
                dateAndCondition.LessThanCondition(COL_BED).Value(EED.Value);

            var orDateCondition = dateAndCondition.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);
            orDateCondition.GreaterThanCondition(COL_EED).Value(BED);

            return queryContext.GetItems(SupplierRateMapper);
        }

        public List<SupplierRate> GetSupplierRates(int supplierId, DateTime minimumDate)
        {
            SupplierPriceListDataManager supplierPriceListDataManager = new SupplierPriceListDataManager();
            string supplierPriceListTableAlias = "spl";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            supplierPriceListDataManager.JoinSupplierPriceList(join, supplierPriceListTableAlias, TABLE_ALIAS, COL_PriceListID, RDBJoinType.Left, true);

            var whereQuery = selectQuery.Where();

            var orDateCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);
            var andDateCondition = orDateCondition.ChildConditionGroup();
            andDateCondition.GreaterThanCondition(COL_EED).Value(minimumDate);
            andDateCondition.NotEqualsCondition(COL_EED).Column(COL_BED);
            whereQuery.EqualsCondition(supplierPriceListTableAlias, SupplierPriceListDataManager.COL_SupplierID).Value(supplierId);

            return queryContext.GetItems(SupplierRateMapper);
        }

        public List<SupplierRate> GetEffectiveSupplierRatesBySuppliers(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingSupplierInfo> supplierInfos)
        {
            SupplierPriceListDataManager supplierPriceListDataManager = new SupplierPriceListDataManager();
            string supplierPriceListTableAlias = "spl";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            supplierPriceListDataManager.JoinSupplierPriceList(join, supplierPriceListTableAlias, TABLE_ALIAS, COL_PriceListID, RDBJoinType.Inner, true);

            var whereQuery = selectQuery.Where();

            whereQuery.ListCondition(supplierPriceListTableAlias, SupplierPriceListDataManager.COL_SupplierID, RDBListConditionOperator.IN, supplierInfos.Select(item => item.SupplierId));

            if (effectiveOn.HasValue)
            {
                if (isEffectiveInFuture)
                    BEDataUtility.SetEffectiveDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
                else
                    BEDataUtility.SetFutureDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
            }
            else
                whereQuery.FalseCondition();

            return queryContext.GetItems(SupplierRateMapper);
        }

        public List<SupplierRate> GetSupplierRatesInBetweenPeriod(DateTime fromDateTime, DateTime tillDateTime)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();

            var andConditionContext = whereQuery.ChildConditionGroup();
            andConditionContext.LessOrEqualCondition(COL_BED).Value(fromDateTime);
            var orCondition = andConditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Value(tillDateTime);

            return queryContext.GetItems(SupplierRateMapper);
        }

        public List<SupplierRate> GetEffectiveSupplierRates(DateTime fromDate, DateTime toDate)
        {
            return GetSupplierRatesInBetweenPeriod(fromDate, toDate);
        }

        public bool AreSupplierRatesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public SupplierRate GetSupplierRateById(long rateId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_ID).Value(rateId);

            return queryContext.GetItem(SupplierRateMapper);
        }

        public List<SupplierRate> GetSupplierRates(HashSet<long> supplierRateIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();

            whereQuery.ListCondition(RDBListConditionOperator.IN, supplierRateIds);

            return queryContext.GetItems(SupplierRateMapper);
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
            supplierPriceListDataManager.JoinSupplierPriceList(join, "spr", TABLE_ALIAS, COL_PriceListID, RDBJoinType.Inner, true);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_ZoneID).Value(zoneId);
            whereQuery.NotNullCondition(COL_RateTypeID);

            BEDataUtility.SetEffectiveDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn);

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
                RateChange = (RateChangeType)reader.GetIntWithNullHandling(COL_Change),
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
                RateChange = (RateChangeType)reader.GetIntWithNullHandling(COL_Change),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED)
            };
            return supplierOtherRate;
        }

        #endregion

    }
}
