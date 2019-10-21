using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Routing.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.RDB
{
    public class SupplierZoneDetailsDataManager : RoutingDataManager, ISupplierZoneDetailsDataManager
    {
        #region Fields/Ctor

        private static string DBTABLE_SCHEMA = "dbo";
        internal static string DBTABLE_NAME = "SupplierZoneDetail";
        private static string TABLE_NAME = "dbo_SupplierZoneDetail";
        private static string TABLE_ALIAS = "szd";

        private const string COL_SupplierId = "SupplierId";
        internal const string COL_SupplierZoneId = "SupplierZoneId";
        internal const string COL_EffectiveRateValue = "EffectiveRateValue";
        internal const string COL_SupplierServiceIds = "SupplierServiceIds";
        internal const string COL_ExactSupplierServiceIds = "ExactSupplierServiceIds";
        internal const string COL_SupplierServiceWeight = "SupplierServiceWeight";
        internal const string COL_SupplierRateId = "SupplierRateId";
        internal const string COL_SupplierRateEED = "SupplierRateEED";
        internal const string COL_DealId = "DealId";
        internal const string COL_VersionNumber = "VersionNumber";

        internal static Dictionary<string, RoutingTableColumnDefinition> s_SupplierZoneDetailColumnDefinitions;

        public DateTime? EffectiveDate { get; set; }

        public bool? IsFuture { get; set; }

        static SupplierZoneDetailsDataManager()
        {
            s_SupplierZoneDetailColumnDefinitions = BuildCustomerZoneDetailColumnDefinitions();
            Dictionary<string, RDBTableColumnDefinition> columns = Helper.GetRDBTableColumnDefinitions(s_SupplierZoneDetailColumnDefinitions);

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
            bulkInsertQueryContext.IntoTable(TABLE_NAME, '^', COL_SupplierId, COL_SupplierZoneId, COL_EffectiveRateValue, COL_SupplierServiceIds, COL_ExactSupplierServiceIds,
                COL_SupplierServiceWeight, COL_SupplierRateId, COL_SupplierRateEED, COL_DealId, COL_VersionNumber);

            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(SupplierZoneDetail record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            string supplierServiceIds = record.SupplierServiceIds != null ? string.Join(",", record.SupplierServiceIds) : null;
            string exactSupplierServiceIds = record.ExactSupplierServiceIds != null ? string.Join(",", record.ExactSupplierServiceIds) : null;

            var recordContext = bulkInsertQueryContext.WriteRecord();
            recordContext.Value(record.SupplierId);
            recordContext.Value(record.SupplierZoneId);
            recordContext.Value(record.EffectiveRateValue);
            recordContext.Value(supplierServiceIds);
            recordContext.Value(exactSupplierServiceIds);
            recordContext.Value(record.SupplierServiceWeight);

            if (record.SupplierRateId.HasValue)
                recordContext.Value(record.SupplierRateId.Value);
            else
                recordContext.Value(string.Empty);

            if (record.SupplierRateEED.HasValue)
                recordContext.Value(record.SupplierRateEED.Value);
            else
                recordContext.Value(string.Empty);

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

        public void ApplySupplierZoneDetailsForDB(object preparedSupplierZoneDetails)
        {
            preparedSupplierZoneDetails.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        public Dictionary<long, SupplierZoneDetail> GetCachedSupplierZoneDetails()
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierZoneDetailsCacheManager>();
            var cacheName = new GetCachedSupplierZoneDetailsCacheName() { RoutingDatabaseId = this.RoutingDatabase.ID };

            return cacheManager.GetOrCreateObject(cacheName, SupplierZoneDetailsCacheExpirationChecker.Instance, () =>
            {
                RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

                RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
                selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
                selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

                List<SupplierZoneDetail> supplierZoneDetails = queryContext.GetItems<SupplierZoneDetail>(SupplierZoneDetailMapper);
                return supplierZoneDetails.ToDictionary(itm => itm.SupplierZoneId, itm => itm);
            });
        }

        public IEnumerable<SupplierZoneDetail> GetSupplierZoneDetailsByCode(string code)
        {
            string codeSupplierZoneMatch = "cszm";

            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            new CodeSupplierZoneMatchDataManager().AddJoinCodeSupplierZoneMatchBySupplierZoneId(joinContext, RDBJoinType.Inner, codeSupplierZoneMatch, TABLE_ALIAS, COL_SupplierZoneId, false);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(codeSupplierZoneMatch, CodeSupplierZoneMatchDataManager.COL_Code).Value(code);

            return queryContext.GetItems(SupplierZoneDetailMapper);
        }

        public IEnumerable<SupplierZoneDetail> GetSupplierZoneDetails()
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems<SupplierZoneDetail>(SupplierZoneDetailMapper);
        }

        public IEnumerable<SupplierZoneDetail> GetFilteredSupplierZoneDetailsBySupplierZone(IEnumerable<long> supplierZoneIds)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.ListCondition(COL_SupplierZoneId, RDBListConditionOperator.IN, supplierZoneIds);

            return queryContext.GetItems<SupplierZoneDetail>(SupplierZoneDetailMapper);
        }

        public List<SupplierZoneDetail> GetSupplierZoneDetailsAfterVersionNumber(int versionNumber)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.GreaterThanCondition(COL_VersionNumber).ObjectValue(versionNumber);

            return queryContext.GetItems<SupplierZoneDetail>(SupplierZoneDetailMapper);
        }

        public void UpdateSupplierZoneDetails(List<SupplierZoneDetail> supplierZoneDetails)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            string tempTableAlias = "supplierZoneDetail";
            var tempTableQuery = queryContext.CreateTempTable();
            Helper.AddRoutingTempTableColumns(tempTableQuery, s_SupplierZoneDetailColumnDefinitions, new List<string>() { COL_SupplierZoneId });

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableQuery);

            foreach (var supplierZoneDetail in supplierZoneDetails)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_EffectiveRateValue).Value(supplierZoneDetail.EffectiveRateValue);
                rowContext.Column(COL_SupplierServiceWeight).Value(supplierZoneDetail.SupplierServiceWeight);
                rowContext.Column(COL_VersionNumber).Value(supplierZoneDetail.VersionNumber);

                string supplierServiceIds = supplierZoneDetail.SupplierServiceIds != null ? string.Join(",", supplierZoneDetail.SupplierServiceIds) : null;
                if (!string.IsNullOrEmpty(supplierServiceIds))
                    rowContext.Column(COL_SupplierServiceIds).Value(supplierServiceIds);
                else
                    rowContext.Column(COL_SupplierServiceIds).Null();

                string exactSupplierServiceIds = supplierZoneDetail.ExactSupplierServiceIds != null ? string.Join(",", supplierZoneDetail.ExactSupplierServiceIds) : null;
                if (!string.IsNullOrEmpty(exactSupplierServiceIds))
                    rowContext.Column(COL_ExactSupplierServiceIds).Value(exactSupplierServiceIds);
                else
                    rowContext.Column(COL_ExactSupplierServiceIds).Null();

                if (supplierZoneDetail.SupplierRateId.HasValue)
                    rowContext.Column(COL_SupplierRateId).Value(supplierZoneDetail.SupplierRateId.Value);
                else
                    rowContext.Column(COL_SupplierRateId).Null();

                if (supplierZoneDetail.SupplierRateEED.HasValue)
                    rowContext.Column(COL_SupplierRateEED).Value(supplierZoneDetail.SupplierRateEED.Value);
                else
                    rowContext.Column(COL_SupplierRateEED).Null();

                if (supplierZoneDetail.DealId.HasValue)
                    rowContext.Column(COL_DealId).Value(supplierZoneDetail.DealId.Value);
                else
                    rowContext.Column(COL_DealId).Null();
            }
             
            RDBUpdateQuery updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var joinContext = updateQuery.Join(TABLE_ALIAS);
            joinContext.JoinOnEqualOtherTableColumn(TABLE_NAME, TABLE_ALIAS, COL_SupplierZoneId, tempTableAlias, COL_SupplierZoneId);

            updateQuery.Column(COL_EffectiveRateValue).Column(tempTableAlias, COL_EffectiveRateValue);
            updateQuery.Column(COL_SupplierServiceIds).Column(tempTableAlias, COL_SupplierServiceIds);
            updateQuery.Column(COL_ExactSupplierServiceIds).Column(tempTableAlias, COL_ExactSupplierServiceIds);
            updateQuery.Column(COL_SupplierServiceWeight).Column(tempTableAlias, COL_SupplierServiceWeight);
            updateQuery.Column(COL_SupplierRateId).Column(tempTableAlias, COL_SupplierRateId);
            updateQuery.Column(COL_SupplierRateEED).Column(tempTableAlias, COL_SupplierRateEED);
            updateQuery.Column(COL_DealId).Column(tempTableAlias, COL_DealId);
            updateQuery.Column(COL_VersionNumber).Column(tempTableAlias, COL_VersionNumber);

            //Update supplierZoneDetails set
            //supplierZoneDetails.EffectiveRateValue = szd.EffectiveRateValue,
            //supplierZoneDetails.SupplierServiceIds = szd.SupplierServiceIds,
            //supplierZoneDetails.ExactSupplierServiceIds = szd.ExactSupplierServiceIds,
            //supplierZoneDetails.SupplierServiceWeight = szd.SupplierServiceWeight,
            //supplierZoneDetails.SupplierRateId = szd.SupplierRateId,
            //supplierZoneDetails.SupplierRateEED = szd.SupplierRateEED,
            //supplierZoneDetails.DealId = szd.DealId,
            //supplierZoneDetails.VersionNumber = szd.VersionNumber

            queryContext.ExecuteNonQuery();
        }

        public void AddSelectSupplierZoneDetails(RDBSelectQuery selectQuery, bool withNoLock)
        {
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, withNoLock);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
        }

        public void AddJoinSupplierZoneDetailsBySupplierZoneId(RDBJoinContext joinContext, RDBJoinType joinType, string supplierZoneDetailsTableAlias, string originalTableAlias,
            string originalTableCodeCol, bool withNoLock)
        {
            joinContext.JoinOnEqualOtherTableColumn(joinType, TABLE_NAME, TABLE_ALIAS, COL_SupplierZoneId, originalTableAlias, originalTableCodeCol, withNoLock);
        }

        #endregion

        #region Private Methods

        private static Dictionary<string, RoutingTableColumnDefinition> BuildCustomerZoneDetailColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_SupplierId, new RoutingTableColumnDefinition(COL_SupplierId, RDBDataType.Int, true));
            columnDefinitions.Add(COL_SupplierZoneId, new RoutingTableColumnDefinition(COL_SupplierZoneId, RDBDataType.BigInt, true));
            columnDefinitions.Add(COL_EffectiveRateValue, new RoutingTableColumnDefinition(COL_EffectiveRateValue, RDBDataType.Decimal, 20, 8, true));
            columnDefinitions.Add(COL_SupplierServiceIds, new RoutingTableColumnDefinition(COL_SupplierServiceIds, RDBDataType.NVarchar));
            columnDefinitions.Add(COL_ExactSupplierServiceIds, new RoutingTableColumnDefinition(COL_ExactSupplierServiceIds, RDBDataType.NVarchar));
            columnDefinitions.Add(COL_SupplierServiceWeight, new RoutingTableColumnDefinition(COL_SupplierServiceWeight, RDBDataType.Int, true));
            columnDefinitions.Add(COL_SupplierRateId, new RoutingTableColumnDefinition(COL_SupplierRateId, RDBDataType.BigInt));
            columnDefinitions.Add(COL_SupplierRateEED, new RoutingTableColumnDefinition(COL_SupplierRateEED, RDBDataType.DateTime));
            columnDefinitions.Add(COL_DealId, new RoutingTableColumnDefinition(COL_DealId, RDBDataType.Int));
            columnDefinitions.Add(COL_VersionNumber, new RoutingTableColumnDefinition(COL_VersionNumber, RDBDataType.Int, true));
            return columnDefinitions;
        }

        public static SupplierZoneDetail SupplierZoneDetailMapper(IRDBDataReader reader)
        {
            SupplierZoneDetail supplierZoneDetail = new SupplierZoneDetail
            {
                SupplierId = reader.GetInt(COL_SupplierId),
                SupplierZoneId = reader.GetLong(COL_SupplierZoneId),
                EffectiveRateValue = reader.GetDecimal(COL_EffectiveRateValue),
                SupplierServiceWeight = reader.GetInt(COL_SupplierServiceWeight),
                SupplierRateId = reader.GetNullableLong(COL_SupplierRateId),
                SupplierRateEED = reader.GetNullableDateTime(COL_SupplierRateEED),
                DealId = reader.GetNullableInt(COL_DealId),
                VersionNumber = reader.GetInt(COL_VersionNumber)
            };

            string supplierServiceIdsAsString = reader.GetString(COL_SupplierServiceIds);
            if (!string.IsNullOrEmpty(supplierServiceIdsAsString))
                supplierZoneDetail.SupplierServiceIds = new HashSet<int>(supplierServiceIdsAsString.Split(',').Select(itm => int.Parse(itm)));

            string exactSupplierServiceIdsAsString = reader.GetString(COL_ExactSupplierServiceIds);
            if (!string.IsNullOrEmpty(exactSupplierServiceIdsAsString))
                supplierZoneDetail.ExactSupplierServiceIds = new HashSet<int>(exactSupplierServiceIdsAsString.Split(',').Select(itm => int.Parse(itm)));

            return supplierZoneDetail;
        }

        #endregion

        #region Private Classes

        private class SupplierZoneDetailsCacheManager : BaseCacheManager
        {

        }

        private class SupplierZoneDetailsCacheExpirationChecker : CacheExpirationChecker
        {
            static SupplierZoneDetailsCacheExpirationChecker s_instance = new SupplierZoneDetailsCacheExpirationChecker();
            public static SupplierZoneDetailsCacheExpirationChecker Instance { get { return s_instance; } }

            public override bool IsCacheExpired(Vanrise.Caching.ICacheExpirationCheckerContext context)
            {
                TimeSpan entitiesTimeSpan = TimeSpan.FromMinutes(5);
                SlidingWindowCacheExpirationChecker slidingWindowCacheExpirationChecker = new SlidingWindowCacheExpirationChecker(entitiesTimeSpan);
                return slidingWindowCacheExpirationChecker.IsCacheExpired(context);
            }
        }

        private struct GetCachedSupplierZoneDetailsCacheName
        {
            public int RoutingDatabaseId { get; set; }

            public override int GetHashCode()
            {
                return this.RoutingDatabaseId.GetHashCode();
            }
        }

        #endregion
    }
}