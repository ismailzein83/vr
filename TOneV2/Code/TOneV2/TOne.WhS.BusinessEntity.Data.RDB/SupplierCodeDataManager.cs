using System;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SupplierCodeDataManager : ISupplierCodeDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "sc";
        static string TABLE_NAME = "TOneWhS_BE_SupplierCode";
        const string COL_ID = "ID";
        const string COL_Code = "Code";
        const string COL_ZoneID = "ZoneID";
        const string COL_CodeGroupID = "CodeGroupID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";

        static SupplierCodeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 20 });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CodeGroupID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SupplierCode",
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

        #region Members

        public IEnumerable<SupplierCode> GetParentsBySupplier(int supplierId, string codeNumber)
        {
            SupplierZoneDataManager supplierZoneDataManager = new SupplierZoneDataManager();
            string supplierZoneTableAlias = "spz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinCondition = selectQuery.Join();
            supplierZoneDataManager.JoinSupplierZone(joinCondition, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, false);

            var whereContext = selectQuery.Where();

            var compareConditionContext = whereContext.CompareCondition(RDBCompareConditionOperator.StartWith);
            compareConditionContext.Expression1().Value(codeNumber);
            compareConditionContext.Expression2().Column(COL_Code);

            whereContext.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID).Value(supplierId);
            whereContext.LessOrEqualCondition(COL_BED).DateNow();

            var orDateCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);
            orDateCondition.GreaterThanCondition(COL_EED).DateNow();

            selectQuery.Sort().ByColumn(COL_Code, RDBSortDirection.ASC);
            return queryContext.GetItems(SupplierCodeMapper);
        }

        public IEnumerable<SupplierCode> GetFilteredSupplierCodes(SupplierCodeQuery query)
        {
            SupplierZoneDataManager supplierZoneDataManager = new SupplierZoneDataManager();
            string supplierZoneTableAlias = "spz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinCondition = selectQuery.Join();
            supplierZoneDataManager.JoinSupplierZone(joinCondition, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID).Value(query.SupplierId);

            if (string.IsNullOrEmpty(query.Code))
                whereContext.ContainsCondition(COL_Code, query.Code);

            if (query.ZoneIds != null && query.ZoneIds.Any())
                whereContext.ListCondition(RDBListConditionOperator.IN, query.ZoneIds);

            BEDataUtility.SetEffectiveDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, query.EffectiveOn);

            return queryContext.GetItems(SupplierCodeMapper);
        }

        public List<SupplierCode> GetSupplierCodesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            SupplierZoneDataManager supplierZoneDataManager = new SupplierZoneDataManager();
            string supplierZoneTableAlias = "spz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinCondition = selectQuery.Join();
            supplierZoneDataManager.JoinSupplierZone(joinCondition, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID).Value(supplierId);

            var ordDateCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            ordDateCondition.NullCondition(COL_EED);
            ordDateCondition.GreaterThanCondition(COL_EED).Value(minimumDate);
            ordDateCondition.NotEqualsCondition(COL_EED).Column(COL_BED);

            return queryContext.GetItems(SupplierCodeMapper);
        }

        public List<SupplierCode> GetSupplierCodes(DateTime from, DateTime to)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.LessOrEqualCondition(COL_BED).Value(from);

            var orDateCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);
            orDateCondition.GreaterThanCondition(COL_EED).Value(to);

            return queryContext.GetItems(SupplierCodeMapper);
        }

        public IEnumerable<CodePrefixInfo> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            BEDataUtility.SetDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, isFuture, effectiveOn);

            var groupByContext = selectQuery.GroupBy();
            var groupSelect = groupByContext.Select();
            groupSelect.Expression(TABLE_ALIAS).TextLeftPart(prefixLength).Column(COL_Code);
            groupByContext.SelectAggregates().Count("codeCount");

            selectQuery.Sort().ByAlias("codeCount", RDBSortDirection.DESC);

            return queryContext.GetItems(CodePrefixMapper);
        }

        public IEnumerable<CodePrefixInfo> GetSpecificCodeByPrefixes(int prefixLength, IEnumerable<string> codePrefixes, DateTime? effectiveOn, bool isFuture)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            string codePrefixAlias = "CodePrefix";
            string codeCountAlias = "CodeCount";

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumnsFromTable(codePrefixAlias);
            tempTableQuery.AddColumnsFromTable(codeCountAlias);

            var insertToTempTableQuery = queryContext.AddInsertQuery();
            insertToTempTableQuery.IntoTable(tempTableQuery);

            var fromSelectQuery = insertToTempTableQuery.FromSelect();

            fromSelectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            fromSelectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = fromSelectQuery.Where();
            BEDataUtility.SetDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, isFuture, effectiveOn);

            var groupByContext = fromSelectQuery.GroupBy();
            var groupSelect = groupByContext.Select();

            groupSelect.Expression(TABLE_ALIAS).TextLeftPart(prefixLength).Column(COL_Code);
            groupByContext.SelectAggregates().Count(codeCountAlias);


            var tempCodePrefixesTableQuery = queryContext.CreateTempTable();
            tempCodePrefixesTableQuery.AddColumn(codePrefixAlias, RDBDataType.NVarchar, true);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempCodePrefixesTableQuery);

            foreach (var queryItem in codePrefixes)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(codePrefixAlias).Value(queryItem);
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(tempCodePrefixesTableQuery, "allPrefixes", null);
            selectQuery.SelectColumns().AllTableColumns("allPrefixes");
            return queryContext.GetItems(CodePrefixMapper);
        }

        public bool AreSupplierCodesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public void LoadSupplierCodes(IEnumerable<RoutingSupplierInfo> activeSupplierInfo, string codePrefix, DateTime? effectiveOn, bool isFuture, Func<bool> shouldStop, Action<SupplierCode> onCodeLoaded)
        {
            //TODO test it in routing
            SupplierZoneDataManager supplierZoneDataManager = new SupplierZoneDataManager();

            string supplierZoneTableAlias = "spz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(SupplierZoneDataManager.COL_SupplierID, RDBDataType.Int, true);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableQuery);

            foreach (var queryItem in activeSupplierInfo)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(SupplierZoneDataManager.COL_SupplierID).Value(queryItem.SupplierId);
            }

            var joinCondition = selectQuery.Join();
            supplierZoneDataManager.JoinSupplierZone(joinCondition, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var supplierJoinCondition = selectQuery.Join();
            var supplierJoinStatement = supplierJoinCondition.Join(tempTableQuery, "supplierInfo");
            supplierJoinStatement.JoinType(RDBJoinType.Inner);
            var supplierJoinConditionOn = supplierJoinStatement.On();
            supplierJoinConditionOn.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID, "supplierInfo", SupplierZoneDataManager.COL_SupplierID);

            var whereContext = selectQuery.Where();

            if (effectiveOn.HasValue)
            {
                if (isFuture)
                    BEDataUtility.SetEffectiveDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
                else
                    BEDataUtility.SetFutureDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
            }
            else whereContext.FalseCondition();

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    if (shouldStop != null && shouldStop())
                        break;

                    onCodeLoaded(SupplierCodeMapper(reader));
                }
            });
        }
        public List<ZoneCodeGroup> GetCostZoneCodeGroups(DateTime? effectiveOn, bool isFuture)
        {
            CodeGroupDataManager codeGroupDataManager = new CodeGroupDataManager();
            Dictionary<long, List<string>> codeGroupsByZone = new Dictionary<long, List<string>>();

            string codeGroupTableAlias = "cg";

            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            var selectColoumns = selectQuery.SelectColumns();
            selectColoumns.Column(COL_ZoneID);
            selectColoumns.Column(TABLE_ALIAS, COL_Code, "CodeGroup");

            var joinCondition = selectQuery.Join();
            codeGroupDataManager.JoinCodeGroup(joinCondition, codeGroupTableAlias, TABLE_ALIAS, COL_CodeGroupID);

            var whereContext = selectQuery.Where();

            BEDataUtility.SetDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, isFuture, effectiveOn);

            whereContext.EqualsCondition(TABLE_ALIAS, COL_Code).Column(codeGroupTableAlias, CodeGroupDataManager.COL_Code);

            queryContext.ExecuteReader(
               (reader) =>
               {
                   while (reader.Read())
                   {
                       long zoneId = reader.GetLong(COL_ZoneID);
                       string codeGroup = reader.GetString("CodeGroup");
                       List<string> codeGroups = codeGroupsByZone.GetOrCreateItem(zoneId);
                       codeGroups.Add(codeGroup);
                   }
               });

            return codeGroupsByZone.Select(itm => new ZoneCodeGroup() { CodeGroups = itm.Value, ZoneId = itm.Key, IsSale = true }).ToList();
        }

        #endregion

        #region Mappers
        SupplierCode SupplierCodeMapper(IRDBDataReader reader)
        {
            return new SupplierCode
            {
                Code = reader.GetString(COL_Code),
                SupplierCodeId = reader.GetLong(COL_ID),
                ZoneId = reader.GetLong(COL_ZoneID),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                CodeGroupId = reader.GetIntWithNullHandling(COL_CodeGroupID),
                SourceId = reader.GetString(COL_SourceID)
            };
        }
        CodePrefixInfo CodePrefixMapper(IRDBDataReader reader)
        {
            return new CodePrefixInfo
            {
                CodePrefix = reader.GetString("CodePrefix"),
                Count = reader.GetInt("codeCount")
            };
        }

        #endregion

        #region Public Methods
        public bool HasSupplierCodesByCodeGroup(int codeGroupId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 2, true);
            selectQuery.SelectColumns().Column(COL_ID);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_CodeGroupID).Value(codeGroupId);

            return queryContext.ExecuteScalar().NullableLongValue.HasValue;
        }

        #endregion

    }
}
