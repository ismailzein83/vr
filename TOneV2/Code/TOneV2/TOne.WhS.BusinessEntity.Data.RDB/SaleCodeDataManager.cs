using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SaleCodeDataManager : ISaleCodeDataManager
    {
        #region RDB

        static string TABLE_NAME = "TOneWhS_BE_SaleCode";
        static string TABLE_ALIAS = "sc";

        public const string COL_ID = "ID";
        const string COL_Code = "Code";
        const string COL_ZoneID = "ZoneID";
        const string COL_CodeGroupID = "CodeGroupID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static SaleCodeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 20 });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CodeGroupID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SaleCode",
                Columns = columns,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #endregion

        #region ISaleCodeDataManager Members

        public IEnumerable<SaleCode> GetFilteredSaleCodes(SaleCodeQuery query)
        {
            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            string saleZoneTableAlias = "sz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID);

            var where = selectQuery.Where();

            if (query.GetEffectiveAfter)
                BEDataUtility.SetEffectiveAfterDateCondition(where, TABLE_ALIAS, COL_BED, COL_EED, query.EffectiveOn);
            else
                BEDataUtility.SetEffectiveDateCondition(where, TABLE_ALIAS, COL_BED, COL_EED, query.EffectiveOn);

            if (query.Code != null)
                where.StartsWithCondition(COL_Code, query.Code);

            if (query.SellingNumberPlanId.HasValue)
                where.EqualsCondition(saleZoneTableAlias, SaleZoneDataManager.COL_SellingNumberPlanID).Value(query.SellingNumberPlanId.Value);

            if (query.ZonesIds != null && query.ZonesIds.Any())
                where.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, query.ZonesIds);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesByZoneID(long zoneID, DateTime effectiveDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_ZoneID).Value(zoneID);

            where.LessOrEqualCondition(COL_BED).Value(effectiveDate);
            var orDateCondition = where.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);
            orDateCondition.GreaterThanCondition(COL_EED).Value(effectiveDate);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public IEnumerable<SaleCode> GetParentsByPlan(int sellingNumberPlan, string codeNumber)
        {
            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            string saleZoneTableAlias = "sz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID);

            var where = selectQuery.Where();
            BEDataUtility.SetParentCodeCondition(where, codeNumber, TABLE_ALIAS, COL_Code);

            where.EqualsCondition(saleZoneTableAlias, SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlan);

            BEDataUtility.SetEffectiveDateCondition(where, TABLE_ALIAS, COL_BED, COL_EED, DateTime.Now);

            selectQuery.Sort().ByColumn(COL_Code, RDBSortDirection.ASC);
            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesByCodeId(IEnumerable<long> codeIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.ListCondition(COL_ID, RDBListConditionOperator.IN, codeIds);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesEffectiveByZoneID(long zoneID, DateTime effectiveDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_ZoneID).Value(zoneID);
            BEDataUtility.SetEffectiveAfterDateCondition(where, TABLE_ALIAS, COL_BED, COL_EED, effectiveDate);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodes(DateTime effectiveOn)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            BEDataUtility.SetEffectiveDateCondition(where, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, DateTime effectiveOn, long? processInstanceId)
        {
            var saleZoneDataManager = new SaleZoneDataManager();
            string saleZoneTableAlias = "sz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID);

            var where = selectQuery.Where();
            where.EqualsCondition(saleZoneTableAlias, SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);

            var orDateCondition = where.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);

            var andDateCondition = orDateCondition.ChildConditionGroup();
            andDateCondition.NotEqualsCondition(COL_BED).Column(COL_EED);
            andDateCondition.GreaterThanCondition(COL_EED).Value(effectiveOn);

            if (processInstanceId != null)
            {
                var processCondition = where.ChildConditionGroup(RDBConditionGroupOperator.OR);
                processCondition.NullCondition(TABLE_ALIAS, COL_ProcessInstanceID);
                processCondition.LessThanCondition(TABLE_ALIAS, COL_ProcessInstanceID).Value(processInstanceId.Value);
            }
            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes) //to test // need revising: and or to if
        {
            var saleZoneDataManager = new SaleZoneDataManager();
            string saleZoneTableAlias = "sz";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID);

            var whereContext = selectQuery.Where();

            var codeCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);

            if (getChildCodes)
                codeCondition.StartsWithCondition(COL_Code, codePrefix);
            else
                codeCondition.FalseCondition();

            if (getParentCodes)
                BEDataUtility.SetParentCodeCondition(codeCondition, codePrefix, TABLE_ALIAS, COL_Code);
            else
                codeCondition.FalseCondition();

            BEDataUtility.SetDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, isFuture, effectiveOn);


            return queryContext.GetItems(SaleCodeMapper);
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

        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, int countryId, DateTime minimumDate)
        {
            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            string saleZoneTablAlias = "sz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, saleZoneTablAlias, TABLE_ALIAS, COL_ZoneID);

            var whereContext = selectQuery.Where();

            BEDataUtility.SetEffectiveAfterDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, minimumDate);

            whereContext.EqualsCondition(saleZoneTablAlias, SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);
            whereContext.EqualsCondition(saleZoneTablAlias, SaleZoneDataManager.COL_CountryID).Value(countryId);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesByZoneIDs(List<long> zoneIds, DateTime effectiveDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, zoneIds);

            var andCondition = whereContext.ChildConditionGroup();
            andCondition.NullCondition(COL_EED);
            andCondition.GreaterThanCondition(COL_EED).Value(effectiveDate);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public bool AreSaleCodesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<ZoneCodeGroup> GetSaleZoneCodeGroups(DateTime? effectiveOn, bool isFuture)
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

            if (!effectiveOn.HasValue)
            {
                if (isFuture)
                    BEDataUtility.SetEffectiveDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
                else
                    BEDataUtility.SetFutureDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
            }
            else
            {
                whereContext.FalseCondition();
            }

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

            return codeGroupsByZone.Select(itm => new ZoneCodeGroup() { CodeGroups = itm.Value, ZoneId = itm.Key, IsSale = false }).ToList();
        }
        #endregion

        #region Not Used Functions
        public bool AreZonesUpdated(ref object updateHandle)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SaleCode> GetSaleCodesByCode(string codeNumber)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.StartsWithCondition(COL_Code, codeNumber);
            BEDataUtility.SetEffectiveDateCondition(where, TABLE_ALIAS, COL_BED, COL_EED, DateTime.Now);
            selectQuery.Sort().ByColumn(COL_Code, RDBSortDirection.ASC);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public IEnumerable<SaleCode> GetAllSaleCodes()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SaleCodeMapper);
        }

        public IEnumerable<SaleCode> GetSaleCodesByZone(SaleCodeQueryByZone query)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_ZoneID).Value(query.ZoneId);

            if (query.EffectiveOn.HasValue)
                BEDataUtility.SetEffectiveAfterDateCondition(where, TABLE_ALIAS, COL_BED, COL_EED, query.EffectiveOn.Value);
            else
                where.FalseCondition();

            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesByCodeGroups(List<int> codeGroupsIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.ListCondition(COL_CodeGroupID, RDBListConditionOperator.IN, codeGroupsIds);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesByCountry(int countryId, DateTime effectiveDate)
        {
            var saleZoneDataManager = new SaleZoneDataManager();
            string saleZoneTableAlias = "sz";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID);

            var where = selectQuery.Where();
            where.EqualsCondition(saleZoneTableAlias, SaleZoneDataManager.COL_CountryID).Value(countryId);

            BEDataUtility.SetEffectiveAfterDateCondition(where, TABLE_ALIAS, COL_BED, COL_EED, effectiveDate);

            return queryContext.GetItems(SaleCodeMapper);
        }
        public List<SaleCode> GetSaleCodesByZoneName(int sellingNumberPlanId, string zoneName, DateTime effectiveDate)
        {
            var saleZoneDataManager = new SaleZoneDataManager();
            string saleZoneTableAlias = "sz";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID);

            var where = selectQuery.Where();
            where.EqualsCondition(saleZoneTableAlias, SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);
            where.EqualsCondition(saleZoneTableAlias, SaleZoneDataManager.COL_Name).Value(zoneName);

            BEDataUtility.SetEffectiveAfterDateCondition(where, TABLE_ALIAS, COL_BED, COL_EED, effectiveDate);

            return queryContext.GetItems(SaleCodeMapper);
        }

        #endregion

        #region Mappers
        SaleCode SaleCodeMapper(IRDBDataReader reader)
        {
            return new SaleCode
            {
                SaleCodeId = reader.GetLong(COL_ID),
                Code = reader.GetString(COL_Code),
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

        public SaleCode GetSaleCodeByCodeGroup(int codeGroupId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_CodeGroupID).Value(codeGroupId);
            return queryContext.GetItem(SaleCodeMapper);
        }
        public void BuildInsertQuery(RDBInsertQuery insertQuery, long processInstanceID)
        {
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ProcessInstanceID).Value(processInstanceID);
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
