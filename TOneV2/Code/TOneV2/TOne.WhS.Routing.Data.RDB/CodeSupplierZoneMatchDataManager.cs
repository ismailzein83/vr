using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Common;
using System.Linq;
using System;

namespace TOne.WhS.Routing.Data.RDB
{
    public class CodeSupplierZoneMatchDataManager : RoutingDataManager
    {
        #region Fields/Ctor

        private static string DBTABLE_SCHEMA = "dbo";
        internal static string DBTABLE_NAME = "CodeSupplierZoneMatch";
        private static string TABLE_NAME = "dbo_CodeSupplierZoneMatch";
        private static string TABLE_ALIAS = "codeSupplierZoneMatch";

        internal const string COL_Code = "Code";
        internal const string COL_SupplierID = "SupplierID";
        internal const string COL_SupplierZoneID = "SupplierZoneID";
        internal const string COL_CodeMatch = "CodeMatch";

        internal static Dictionary<string, RoutingTableColumnDefinition> s_CodeSupplierZoneMatchColumnDefinitions;

        static CodeSupplierZoneMatchDataManager()
        {
            s_CodeSupplierZoneMatchColumnDefinitions = BuildCodeSupplierZoneMatchColumnDefinitions();
            Dictionary<string, RDBTableColumnDefinition> columns = Helper.GetRDBTableColumnDefinitions(s_CodeSupplierZoneMatchColumnDefinitions);

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
            bulkInsertQueryContext.IntoTable(TABLE_NAME, '^', COL_Code, COL_SupplierID, COL_SupplierZoneID, COL_CodeMatch);
            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(CodeSupplierZoneMatch record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            var recordContext = bulkInsertQueryContext.WriteRecord();
            recordContext.Value(record.Code);
            recordContext.Value(record.SupplierId);
            recordContext.Value(record.SupplierZoneId);
            recordContext.Value(record.CodeMatch);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertQueryContext.CloseStream();
            return bulkInsertQueryContext;
        }

        public void ApplySupplierZoneMatchesToTable(object preparedObject)
        {
            preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        public IEnumerable<CodeSupplierZoneMatch> GetSupplierZoneMatchBysupplierIds(IEnumerable<long> supplierIds, string codeStartWith)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.ListCondition(COL_SupplierID, RDBListConditionOperator.IN, supplierIds);

            if (!string.IsNullOrEmpty(codeStartWith))
                whereContext.StartsWithCondition(COL_Code, codeStartWith);

            return queryContext.GetItems<CodeSupplierZoneMatch>(CodeSupplierZoneMatchMapper);
        }

        public IEnumerable<CodeSupplierZoneMatch> GetSupplierZoneMatchBySupplierIdsAndSellingNumberPanId(int sellingNumberPlanId, IEnumerable<long> supplierIds, string codeStartWith)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.ListCondition(COL_SupplierID, RDBListConditionOperator.IN, supplierIds);

            if (!string.IsNullOrEmpty(codeStartWith))
                whereContext.StartsWithCondition(COL_Code, codeStartWith);

            return queryContext.GetItems<CodeSupplierZoneMatch>(CodeSupplierZoneMatchMapper);
        }

        public IEnumerable<CodeSupplierZoneMatch> GetSupplierZonesMatchedToSaleZones(IEnumerable<long> saleZoneIds, IEnumerable<int> supplierIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            new CodeSaleZoneMatchDataManager().AddJoinSelectCodeSaleZoneMatchByCode(joinContext, "codeSaleZoneMatch", TABLE_ALIAS, COL_Code, saleZoneIds);

            var whereContext = selectQuery.Where();

            if (supplierIds != null)
                whereContext.ListCondition(COL_SupplierID, RDBListConditionOperator.IN, supplierIds);

            return queryContext.GetItems<CodeSupplierZoneMatch>(CodeSupplierZoneMatchMapper);
        }

        public IEnumerable<CodeSupplierZoneMatch> GetOtherSupplierZonesMatchedToSupplierZones(int supplierId, IEnumerable<long> supplierZoneIds, IEnumerable<int> otherSupplierIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.NotEqualsCondition(COL_SupplierID).ObjectValue(supplierId);
            whereContext.ListCondition(COL_SupplierZoneID, RDBListConditionOperator.IN, supplierZoneIds);

            if (otherSupplierIds != null)
                whereContext.ListCondition(COL_SupplierID, RDBListConditionOperator.IN, otherSupplierIds);

            return queryContext.GetItems<CodeSupplierZoneMatch>(CodeSupplierZoneMatchMapper);
        }

        public List<PartialCodeMatches> GetPartialCodeMatchesByRouteCodes(HashSet<string> routeCodes, Func<IRDBDataReader, SupplierCodeMatchWithRate> supplierCodeMatchWithRateMapper)
        {
            Dictionary<string, PartialCodeMatches> result = new Dictionary<string, PartialCodeMatches>();

            string tempCodeTableAlias = "c";
            string supplierZoneDetailTableAlias = "szd";

            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            var tempCodeTableQuery = queryContext.CreateTempTable();
            tempCodeTableQuery.AddColumn(COL_Code, RDBDataType.Varchar, 20, null, true);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempCodeTableQuery);

            foreach (var routeCode in routeCodes)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_Code).Value(routeCode);
            }

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Column(COL_Code);
            selectQuery.SelectColumns().Column(COL_SupplierID);
            selectQuery.SelectColumns().Column(COL_SupplierZoneID);
            selectQuery.SelectColumns().Column(COL_CodeMatch);
            selectQuery.SelectColumns().Column(supplierZoneDetailTableAlias, SupplierZoneDetailsDataManager.COL_DealId, "DealId");
            selectQuery.SelectColumns().Column(supplierZoneDetailTableAlias, SupplierZoneDetailsDataManager.COL_SupplierServiceIds, "SupplierServiceIds");
            selectQuery.SelectColumns().Column(supplierZoneDetailTableAlias, SupplierZoneDetailsDataManager.COL_ExactSupplierServiceIds, "ExactSupplierServiceIds");
            selectQuery.SelectColumns().Column(supplierZoneDetailTableAlias, SupplierZoneDetailsDataManager.COL_EffectiveRateValue, "EffectiveRateValue");
            selectQuery.SelectColumns().Column(supplierZoneDetailTableAlias, SupplierZoneDetailsDataManager.COL_SupplierServiceWeight, "SupplierServiceWeight");
            selectQuery.SelectColumns().Column(supplierZoneDetailTableAlias, SupplierZoneDetailsDataManager.COL_SupplierRateId, "SupplierRateId");
            selectQuery.SelectColumns().Column(supplierZoneDetailTableAlias, SupplierZoneDetailsDataManager.COL_SupplierRateEED, "SupplierRateEED");

            var joinContext = selectQuery.Join();
            new SupplierZoneDetailsDataManager().AddJoinSupplierZoneDetailsBySupplierZoneId(joinContext, RDBJoinType.Inner, supplierZoneDetailTableAlias, TABLE_ALIAS, COL_SupplierZoneID, false);
            joinContext.JoinOnEqualOtherTableColumn(RDBJoinType.Inner, tempCodeTableQuery, tempCodeTableAlias, COL_Code, TABLE_ALIAS, COL_Code, false);

            queryContext.ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    string code = reader.GetString("Code");

                    PartialCodeMatches partialCodeMatches = result.GetOrCreateItem(code, () =>
                    {
                        return new PartialCodeMatches() { Code = code, SupplierCodeMatches = new List<SupplierCodeMatchWithRate>(), SupplierCodeMatchesBySupplier = new SupplierCodeMatchWithRateBySupplier() };
                    });

                    SupplierCodeMatchWithRate supplierCodeMatchWithRate = supplierCodeMatchWithRateMapper(reader);
                    partialCodeMatches.SupplierCodeMatches.Add(supplierCodeMatchWithRate);
                    partialCodeMatches.SupplierCodeMatchesBySupplier.Add(supplierCodeMatchWithRate.CodeMatch.SupplierId, supplierCodeMatchWithRate);
                }
            });

            return result.Values.ToList();
        }

        public void AddSelectCodeSupplierZoneCodeMatch(RDBSelectQuery selectQuery, bool withNoLock)
        {
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, withNoLock);
            selectQuery.SelectColumns().Column(COL_Code);
            selectQuery.SelectColumns().Column(COL_SupplierZoneID);
            selectQuery.SelectColumns().Column(COL_CodeMatch);
        }

        public void AddJoinCodeSupplierZoneMatchBySupplierZoneId(RDBJoinContext joinContext, RDBJoinType joinType, string codeSupplierZoneMatchTableAlias, string originalTableAlias,
            string originalTableSupplierZoneIdCol, bool withNoLock)
        {
            joinContext.JoinOnEqualOtherTableColumn(joinType, TABLE_NAME, codeSupplierZoneMatchTableAlias, COL_SupplierZoneID, originalTableAlias, originalTableSupplierZoneIdCol, withNoLock);
        }

        public void AddJoinCodeSupplierZoneMatchByCode(RDBJoinContext joinContext, RDBJoinType joinType, string codeSupplierZoneMatchTableAlias, string originalTableAlias,
            string originalTableCodeCol, bool withNoLock)
        {
            joinContext.JoinOnEqualOtherTableColumn(joinType, TABLE_NAME, TABLE_ALIAS, COL_Code, originalTableAlias, originalTableCodeCol, withNoLock);
        }

        public void AddJoinSelectCodeSupplierZoneMatchByCode(RDBJoinContext joinContext, string codeSupplierZoneMatchTableAlias, string originalTableAlias, string originalTableCodeCol,
            IEnumerable<long> supplierZoneIds)
        {
            var joinSelectContext = joinContext.JoinSelect(codeSupplierZoneMatchTableAlias);

            var joinSelectQuery = joinSelectContext.SelectQuery();
            joinSelectQuery.From(TABLE_NAME, "codeOfCodeSupplierZoneMatch");
            joinSelectQuery.SelectColumns().Columns(COL_Code);
            joinSelectQuery.Where().ListCondition(COL_SupplierZoneID, RDBListConditionOperator.IN, supplierZoneIds);

            joinContext.JoinOnEqualOtherTableColumn(TABLE_NAME, TABLE_ALIAS, COL_Code, originalTableAlias, originalTableCodeCol);
        }

        #endregion

        #region Private Methods

        private CodeSupplierZoneMatch CodeSupplierZoneMatchMapper(IRDBDataReader reader)
        {
            return new CodeSupplierZoneMatch()
            {
                Code = reader.GetString("Code"),
                SupplierId = reader.GetInt("SupplierId"),
                SupplierZoneId = reader.GetLong("SupplierZoneId"),
                CodeMatch = reader.GetString("CodeMatch")
            };
        }

        private static Dictionary<string, RoutingTableColumnDefinition> BuildCodeSupplierZoneMatchColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_Code, new RoutingTableColumnDefinition(COL_Code, RDBDataType.Varchar, 20, null, true));
            columnDefinitions.Add(COL_SupplierID, new RoutingTableColumnDefinition(COL_SupplierID, RDBDataType.Int, true));
            columnDefinitions.Add(COL_SupplierZoneID, new RoutingTableColumnDefinition(COL_SupplierZoneID, RDBDataType.BigInt, true));
            columnDefinitions.Add(COL_CodeMatch, new RoutingTableColumnDefinition(COL_CodeMatch, RDBDataType.Varchar, true));
            return columnDefinitions;

            //private const string query_CodeSupplierZoneMatchTable = @"CREATE TABLE [dbo].[CodeSupplierZoneMatch](
            //	                                                        [Code] [varchar](20) NOT NULL,
            //	                                                        [SupplierID] [int] NOT NULL,
            //	                                                        [SupplierZoneID] [bigint] NOT NULL,
            //                                                          [CodeMatch] [varchar](20) NOT NULL
            //                                                          ) ON [PRIMARY]
            //                                                          CREATE CLUSTERED INDEX [IX_CodeSupplierZoneMatch_Code] ON [dbo].[CodeSupplierZoneMatch]
            //                                                          (
            //                                                          	[Code] ASC
            //                                                          );
            //                                                          CREATE NONCLUSTERED INDEX [IX_CodeSupplierZoneMatch_SupplierID] ON dbo.CodeSupplierZoneMatch
            //                                                          (
            //                                                              [SupplierID] ASC
            //                                                          );";
        }

        #endregion
    }
}