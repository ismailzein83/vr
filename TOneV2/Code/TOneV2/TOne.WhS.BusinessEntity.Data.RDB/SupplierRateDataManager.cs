using System;
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

        const string COL_StateBackupID = "StateBackupID";

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
            return GetSupplierRates(input.SupplierId, input.CountriesIds, input.SupplierZoneName, effectiveOn, false);
        }

        public IEnumerable<SupplierRate> GetFilteredSupplierPendingRates(SupplierRateQuery input, DateTime effectiveOn)
        {
            return GetSupplierRates(input.SupplierId, input.CountriesIds, input.SupplierZoneName, effectiveOn, true);
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

            var whereContext = selectQuery.Where();

            BEDataUtility.SetEffectiveAfterDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, minimumDate);
            whereContext.EqualsCondition(supplierPriceListTableAlias, SupplierPriceListDataManager.COL_SupplierID).Value(supplierId);

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

            BEDataUtility.SetDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, isEffectiveInFuture, effectiveOn);

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
            andConditionContext.LessOrEqualCondition(COL_BED).Value(tillDateTime);
            var orCondition = andConditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Value(fromDateTime);

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

            whereQuery.ListCondition(COL_ID, RDBListConditionOperator.IN, supplierRateIds);

            return queryContext.GetItems(SupplierRateMapper);
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
        #endregion

        #region Private Methods

        private IEnumerable<SupplierRate> GetSupplierRates(int supplierId, List<int> countriesIds, string supplierZoneName, DateTime effectiveOn, bool isPending)
        {
            string supplierZoneTableAlias = "spz";
            var supplierZoneDataManager = new SupplierZoneDataManager();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinSupplierZone = selectQuery.Join();
            supplierZoneDataManager.JoinSupplierZone(joinSupplierZone, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var whereContext = selectQuery.Where();

            whereContext.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_ID).Value(supplierId);

            if (countriesIds != null && countriesIds.Any())
                whereContext.ListCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_CountryID, RDBListConditionOperator.IN, countriesIds);

            if (!string.IsNullOrEmpty(supplierZoneName))
            {
                string zoneNameToLower = supplierZoneName.ToLower();
                whereContext.ContainsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_Name, zoneNameToLower);
            }

            whereContext.NullCondition(COL_RateTypeID);
            if (isPending)
                whereContext.GreaterThanCondition(COL_BED).Value(effectiveOn);
            else
                BEDataUtility.SetEffectiveDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn);

            return queryContext.GetItems(SupplierRateMapper);
        }


        #endregion

        #region Public Methods

        public IEnumerable<SupplierOtherRate> GetFilteredSupplierOtherRates(long zoneId, DateTime effectiveOn)
        {
            var supplierPriceListDataManager = new SupplierPriceListDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            supplierPriceListDataManager.JoinSupplierPriceList(join, "spl", TABLE_ALIAS, COL_PriceListID, RDBJoinType.Inner, true);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_ZoneID).Value(zoneId);
            whereQuery.NotNullCondition(COL_RateTypeID);

            BEDataUtility.SetEffectiveDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn);

            return queryContext.GetItems(SupplierOtherRateMapper);
        }

        #endregion

        #region StateBackup

        //public void BackupBySupplierId(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName, int supplierId)
        //{
        //    var supplierRateBackupDataManager = new SupplierRateBackupDataManager();
        //    var insertQuery = supplierRateBackupDataManager.GetInsertQuery(queryContext, backupDatabaseName);

        //    var selectQuery = insertQuery.FromSelect();
        //    selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

        //    var selectColumns = selectQuery.SelectColumns();
        //    selectColumns.Column(COL_ID, COL_ID);
        //    selectColumns.Column(COL_PriceListID, COL_PriceListID);
        //    selectColumns.Column(COL_ZoneID, COL_ZoneID);
        //    selectColumns.Column(COL_CurrencyID, COL_CurrencyID);
        //    selectColumns.Column(COL_Rate, COL_Rate);
        //    selectColumns.Column(COL_RateTypeID, COL_RateTypeID);
        //    selectColumns.Column(COL_Change, COL_Change);
        //    selectColumns.Column(COL_BED, COL_BED);
        //    selectColumns.Column(COL_EED, COL_EED);
        //    selectColumns.Column(COL_SourceID, COL_SourceID);
        //    selectColumns.Expression(SupplierRateBackupDataManager.COL_StateBackupID).Value(stateBackupId);
        //    selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

        //    var joinContext = selectQuery.Join();
        //    string supplierPriceListTableAlias = "spl";
        //    var supplierPriceListDataManager = new SupplierPriceListDataManager();
        //    supplierPriceListDataManager.JoinSupplierPriceList(joinContext, supplierPriceListTableAlias, TABLE_ALIAS, COL_PriceListID, RDBJoinType.Inner, true);

        //    var whereContext = selectQuery.Where();
        //    whereContext.EqualsCondition(supplierPriceListTableAlias, SupplierPriceListDataManager.COL_SupplierID).Value(supplierId);
        //}
        //public void SetDeleteQueryBySupplierId(RDBQueryContext queryContext, int supplierId)
        //{
        //    var deleteQuery = queryContext.AddDeleteQuery();
        //    deleteQuery.FromTable(TABLE_NAME);

        //    var joinContext = deleteQuery.Join(TABLE_ALIAS);
        //    string supplierZoneTableAlias = "spz";
        //    var supplierZoneDataManager = new SupplierZoneDataManager();
        //    supplierZoneDataManager.JoinSupplierZone(joinContext, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

        //    var wherContext = deleteQuery.Where();
        //    wherContext.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID).Value(supplierId);
        //}

        //public void GetRestoreQuery(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName)
        //{
        //    var insertQuery = queryContext.AddInsertQuery();
        //    insertQuery.IntoTable(TABLE_ALIAS);
        //    var supplierRateBackupDataManager = new SupplierRateBackupDataManager();
        //    supplierRateBackupDataManager.AddSelectQuery(insertQuery, backupDatabaseName, stateBackupId);
        //}


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
            return new SupplierOtherRate
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

        #endregion

    }
}
