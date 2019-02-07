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
            saleZoneDataManager.JoinSaleZone(join, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var whereContext = selectQuery.Where();

            if (query.GetEffectiveAfter)
            {
                var orDateCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                orDateCondition.NullCondition(COL_EED);
                var andDateCondition = orDateCondition.ChildConditionGroup();
                andDateCondition.GreaterThanCondition(COL_EED).Column(COL_BED);
                andDateCondition.GreaterThanCondition(COL_EED).Value(query.EffectiveOn);
            }
            else
                BEDataUtility.SetEffectiveDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, query.EffectiveOn);

            if (query.Code != null)
                whereContext.StartsWithCondition(COL_Code, query.Code);

            if (query.SellingNumberPlanId.HasValue)
                whereContext.EqualsCondition(saleZoneTableAlias, SaleZoneDataManager.COL_SellingNumberPlanID).Value(query.SellingNumberPlanId.Value);

            if (query.ZonesIds != null && query.ZonesIds.Any())
                whereContext.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, query.ZonesIds);

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
            saleZoneDataManager.JoinSaleZone(join, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID, false);

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
            saleZoneDataManager.JoinSaleZone(join, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(saleZoneTableAlias, SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);

            BEDataUtility.SetEffectiveAfterDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn);

            if (processInstanceId != null)
            {
                var processCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                processCondition.NullCondition(TABLE_ALIAS, COL_ProcessInstanceID);
                processCondition.LessThanCondition(TABLE_ALIAS, COL_ProcessInstanceID).Value(processInstanceId.Value);
            }
            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes)
        {
            var saleZoneDataManager = new SaleZoneDataManager();
            string saleZoneTableAlias = "sz";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

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
            BEDataUtility.SetDistinctCodePrefixesQuery(queryContext, TABLE_NAME, TABLE_ALIAS, COL_BED, COL_EED, isFuture, effectiveOn, prefixLength, COL_Code);
            return queryContext.GetItems(CodePrefixMapper);
        }

        public IEnumerable<CodePrefixInfo> GetSpecificCodeByPrefixes(int prefixLength, IEnumerable<string> codePrefixes, DateTime? effectiveOn, bool isFuture)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            BEDataUtility.SetCodePrefixQuery(queryContext, TABLE_NAME, TABLE_ALIAS, COL_BED, COL_EED, isFuture, effectiveOn, prefixLength, COL_Code, codePrefixes);
            return queryContext.GetItems(CodePrefixMapper);
        }

        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, int countryId, DateTime minimumDate)
        {
            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            string saleZoneTableAlias = "sz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var whereContext = selectQuery.Where();

            BEDataUtility.SetEffectiveAfterDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, minimumDate);

            whereContext.EqualsCondition(saleZoneTableAlias, SaleZoneDataManager.COL_CountryID).Value(countryId);
            whereContext.EqualsCondition(saleZoneTableAlias, SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);

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

            var andCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            andCondition.NullCondition(COL_EED);
            andCondition.GreaterThanCondition(COL_EED).Value(effectiveDate);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public bool AreSaleCodesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        #endregion

        #region Not Used Functions
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
            saleZoneDataManager.JoinSaleZone(join, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

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
            saleZoneDataManager.JoinSaleZone(join, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

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
        public List<ZoneCodeGroup> GetSaleZoneCodeGroups(DateTime? effectiveOn, bool isFuture)
        {
            CodeGroupDataManager codeGroupDataManager = new CodeGroupDataManager();
            Dictionary<long, List<string>> codeGroupsByZone = new Dictionary<long, List<string>>();

            string codeGroupTableAlias = "cg";

            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

            var selectColumns = selectQuery.SelectColumns();
            selectColumns.Column(COL_ZoneID);
            selectColumns.Column(TABLE_ALIAS, COL_Code, "CodeGroup");

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

            return codeGroupsByZone.Select(itm => new ZoneCodeGroup() { CodeGroups = itm.Value, ZoneId = itm.Key, IsSale = false }).ToList();
        }
        public bool HasSaleCodesByCodeGroup(int codeGroupId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
            selectQuery.SelectColumns().Column(COL_ID);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_CodeGroupID).Value(codeGroupId);

            return queryContext.ExecuteScalar().NullableLongValue.HasValue;
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
