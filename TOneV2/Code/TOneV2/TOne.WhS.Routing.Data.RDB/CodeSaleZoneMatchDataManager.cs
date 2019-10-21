using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Data.RDB
{
    public class CodeSaleZoneMatchDataManager : RoutingDataManager
    {
        #region Fields/Ctor

        private static string DBTABLE_SCHEMA = "dbo";
        internal static string DBTABLE_NAME = "CodeSaleZoneMatch";
        private static string TABLE_NAME = "dbo_CodeSaleZoneMatch";
        private static string TABLE_ALIAS = "codeSaleZoneMatch";

        internal const string COL_Code = "Code";
        private const string COL_SellingNumberPlanID = "SellingNumberPlanID";
        private const string COL_SaleZoneID = "SaleZoneID";
        internal const string COL_CodeMatch = "CodeMatch";

        internal static Dictionary<string, RoutingTableColumnDefinition> s_CodeSaleZoneMatchColumnDefinitions;

        static CodeSaleZoneMatchDataManager()
        {
            s_CodeSaleZoneMatchColumnDefinitions = BuildCodeSaleZoneMatchColumnDefinitions();
            Dictionary<string, RDBTableColumnDefinition> columns = Helper.GetRDBTableColumnDefinitions(s_CodeSaleZoneMatchColumnDefinitions);

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
            bulkInsertQueryContext.IntoTable(TABLE_NAME, '^', COL_Code, COL_SellingNumberPlanID, COL_SaleZoneID, COL_CodeMatch);
            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(CodeSaleZoneMatch record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            var recordContext = bulkInsertQueryContext.WriteRecord();
            recordContext.Value(record.Code);
            recordContext.Value(record.SellingNumberPlanId);
            recordContext.Value(record.SaleZoneId);
            recordContext.Value(record.CodeMatch);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertQueryContext.CloseStream();
            return bulkInsertQueryContext;
        }

        public void ApplySaleZoneMatchesToTable(object preparedObject)
        {
            preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        public IEnumerable<SaleZoneDefintion> GetSaleZonesMatchedToSupplierZones(IEnumerable<long> supplierZoneIds, int? sellingNumberPlanId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

            var joinContext = selectQuery.Join();
            new CodeSupplierZoneMatchDataManager().AddJoinSelectCodeSupplierZoneMatchByCode(joinContext, "codeSupplierZoneMatch", TABLE_ALIAS, COL_Code, supplierZoneIds);

            if (sellingNumberPlanId.HasValue)
            {
                var whereContext = selectQuery.Where();
                whereContext.EqualsCondition(COL_SellingNumberPlanID, sellingNumberPlanId.Value.ToString());
            }

            var groupByContext = selectQuery.GroupBy();
            groupByContext.Select().Column(COL_SellingNumberPlanID, COL_SaleZoneID);

            return queryContext.GetItems<SaleZoneDefintion>(SaleZoneDefintionMapper);
        }

        public IEnumerable<CodeSaleZoneMatch> GetSaleZoneMatchBySellingNumberPlanId(int sellingNumberPlanId, string codeStartWith)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_SellingNumberPlanID, sellingNumberPlanId.ToString());

            if (!string.IsNullOrEmpty(codeStartWith))
                whereContext.StartsWithCondition(COL_Code, codeStartWith);

            return queryContext.GetItems<CodeSaleZoneMatch>(CodeSaleZoneMatchMapper);
        }

        public void AddJoinCodeSaleZoneMatchBySaleZoneId(RDBJoinContext joinContext, RDBJoinType joinType, string codeSaleZoneMatchTableAlias, string originalTableAlias, string originalTableSaleZoneIdCol, bool withNoLock)
        {
            joinContext.JoinOnEqualOtherTableColumn(joinType, TABLE_NAME, codeSaleZoneMatchTableAlias, COL_SaleZoneID, originalTableAlias, originalTableSaleZoneIdCol, withNoLock);
        }

        public void AddJoinSelectCodeSaleZoneMatchByCode(RDBJoinContext joinContext, string codeSaleZoneMatchTableAlias, string originalTableAlias, string originalTableCodeCol, IEnumerable<long> saleZoneIds)
        {
            var joinSelectContext = joinContext.JoinSelect(codeSaleZoneMatchTableAlias);
            this.AddSelectCodeToJoinSelect(joinSelectContext, saleZoneIds);

            joinContext.JoinOnEqualOtherTableColumn(TABLE_NAME, TABLE_ALIAS, COL_Code, originalTableAlias, originalTableCodeCol);
        }

        #endregion

        #region Private Methods

        private void AddSelectCodeToJoinSelect(RDBJoinSelectContext joinSelectContext, IEnumerable<long> saleZoneIds)
        {
            var joinSelectQuery = joinSelectContext.SelectQuery();
            joinSelectQuery.From(TABLE_NAME, "codeOfCodeSaleZoneMatch");
            joinSelectQuery.SelectColumns().Columns(COL_Code);
            joinSelectQuery.Where().ListCondition(COL_SaleZoneID, RDBListConditionOperator.IN, saleZoneIds);
        }

        private SaleZoneDefintion SaleZoneDefintionMapper(IRDBDataReader reader)
        {
            return new SaleZoneDefintion()
            {
                SellingNumberPlanId = reader.GetInt("SellingNumberPlanId"),
                SaleZoneId = reader.GetLong("SaleZoneId")
            };
        }

        private CodeSaleZoneMatch CodeSaleZoneMatchMapper(IRDBDataReader reader)
        {
            return new CodeSaleZoneMatch()
            {
                Code = reader.GetString("Code"),
                SellingNumberPlanId = reader.GetInt("SellingNumberPlanId"),
                SaleZoneId = reader.GetLong("SaleZoneId"),
                CodeMatch = reader.GetString("CodeMatch")
            };
        }

        private static Dictionary<string, RoutingTableColumnDefinition> BuildCodeSaleZoneMatchColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_Code, new RoutingTableColumnDefinition(COL_Code, RDBDataType.Varchar, 20, null, true));
            columnDefinitions.Add(COL_SellingNumberPlanID, new RoutingTableColumnDefinition(COL_SellingNumberPlanID, RDBDataType.Int, true));
            columnDefinitions.Add(COL_SaleZoneID, new RoutingTableColumnDefinition(COL_SaleZoneID, RDBDataType.BigInt, true));
            columnDefinitions.Add(COL_CodeMatch, new RoutingTableColumnDefinition(COL_CodeMatch, RDBDataType.Varchar, 20, null, true));
            return columnDefinitions;
        }

        #endregion
    }
}