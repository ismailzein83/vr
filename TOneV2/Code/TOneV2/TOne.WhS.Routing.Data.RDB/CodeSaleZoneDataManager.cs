using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.RDB
{
    public class CodeSaleZoneDataManager : RoutingDataManager, ICodeSaleZoneDataManager
    {
        #region Fields/Ctor

        private static string DBTABLE_SCHEMA = "dbo";
        internal static string DBTABLE_NAME = "CodeSaleZone";
        private static string TABLE_NAME = "dbo_CodeSaleZone";
        private static string TABLE_ALIAS = "csz";

        private const string COL_Code = "Code";
        private const string COL_SaleZoneId = "SaleZoneId";

        internal static Dictionary<string, RoutingTableColumnDefinition> s_CodeSaleZoneColumnDefinitions;

        static CodeSaleZoneDataManager()
        {
            s_CodeSaleZoneColumnDefinitions = BuildCodeSaleZoneColumnDefinitions();
            Dictionary<string, RDBTableColumnDefinition> columns = Helper.GetRDBTableColumnDefinitions(s_CodeSaleZoneColumnDefinitions);

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
            bulkInsertQueryContext.IntoTable(TABLE_NAME, '^', COL_Code, COL_SaleZoneId);
            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(CodeSaleZone record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            var recordContext = bulkInsertQueryContext.WriteRecord();

            if (!string.IsNullOrEmpty(record.Code))
                recordContext.Value(record.Code);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.SaleZoneId);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertQueryContext.CloseStream();
            return bulkInsertQueryContext;
        }

        public void ApplyCodeToCodeSaleZoneTable(object preparedObject)
        {
            preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        public Dictionary<long, RPCodeMatches> GetRPCodeMatchesBySaleZone(long fromZoneId, long toZoneId, Func<IRDBDataReader, SupplierCodeMatchWithRate> supplierCodeMatchWithRateMapper, 
            Func<bool> shouldStop)
        {
            Dictionary<long, RPCodeMatches> result = new Dictionary<long, RPCodeMatches>();

            string codeSupplierZoneMatchTableAlias = "cszm";
            string supplierZoneDetailTableAlias = "szd";

            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Column(COL_Code);
            selectQuery.SelectColumns().Column(COL_SaleZoneId);
            selectQuery.SelectColumns().Column(codeSupplierZoneMatchTableAlias, CodeSupplierZoneMatchDataManager.COL_SupplierID, "SupplierID");
            selectQuery.SelectColumns().Column(codeSupplierZoneMatchTableAlias, CodeSupplierZoneMatchDataManager.COL_SupplierZoneID, "SupplierZoneID");
            selectQuery.SelectColumns().Expression("SupplierCode").Null();
            selectQuery.SelectColumns().Column(supplierZoneDetailTableAlias, SupplierZoneDetailsDataManager.COL_DealId, "COL_DealId");
            selectQuery.SelectColumns().Column(supplierZoneDetailTableAlias, SupplierZoneDetailsDataManager.COL_SupplierServiceIds, "SupplierServiceIds");
            selectQuery.SelectColumns().Column(supplierZoneDetailTableAlias, SupplierZoneDetailsDataManager.COL_ExactSupplierServiceIds, "ExactSupplierServiceIds");
            selectQuery.SelectColumns().Column(supplierZoneDetailTableAlias, SupplierZoneDetailsDataManager.COL_EffectiveRateValue, "EffectiveRateValue");
            selectQuery.SelectColumns().Column(supplierZoneDetailTableAlias, SupplierZoneDetailsDataManager.COL_SupplierServiceWeight, "SupplierServiceWeight");
            selectQuery.SelectColumns().Column(supplierZoneDetailTableAlias, SupplierZoneDetailsDataManager.COL_SupplierRateId, "SupplierRateId");
            selectQuery.SelectColumns().Column(supplierZoneDetailTableAlias, SupplierZoneDetailsDataManager.COL_SupplierRateEED, "SupplierRateEED");

            var joinContext = selectQuery.Join();
            new CodeSupplierZoneMatchDataManager().AddJoinCodeSupplierZoneMatchByCode(joinContext, RDBJoinType.Left, codeSupplierZoneMatchTableAlias, TABLE_ALIAS, COL_Code, false);
            new SupplierZoneDetailsDataManager().AddJoinSupplierZoneDetailsBySupplierZoneId(joinContext, RDBJoinType.Left, supplierZoneDetailTableAlias, codeSupplierZoneMatchTableAlias,
                    SupplierZoneDetailsDataManager.COL_SupplierZoneId, false);

            var whereContext = selectQuery.Where();
            whereContext.GreaterOrEqualCondition(TABLE_ALIAS, COL_SaleZoneId).Value(fromZoneId);
            whereContext.LessOrEqualCondition(TABLE_ALIAS, COL_SaleZoneId).Value(toZoneId);

            queryContext.ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    if (shouldStop != null && shouldStop())
                        break;

                    string code = reader.GetString("Code");
                    long saleZoneId = reader.GetLong("SaleZoneID");
                    RPCodeMatches rpCodeMatches = result.GetOrCreateItem(saleZoneId, () =>
                    {
                        return new RPCodeMatches() { SaleZoneId = saleZoneId, Code = code, SupplierCodeMatches = new List<SupplierCodeMatchWithRate>() }; ;
                    });

                    SupplierCodeMatchWithRate supplierCodeMatchWithRate = supplierCodeMatchWithRateMapper(reader);
                    if (supplierCodeMatchWithRate != null)
                        rpCodeMatches.SupplierCodeMatches.Add(supplierCodeMatchWithRate);
                }
            });

            return result;
        }

        #endregion

        #region Private Methods

        private static Dictionary<string, RoutingTableColumnDefinition> BuildCodeSaleZoneColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_Code, new RoutingTableColumnDefinition(COL_Code, RDBDataType.Varchar, 20, 0, true));
            columnDefinitions.Add(COL_SaleZoneId, new RoutingTableColumnDefinition(COL_SaleZoneId, RDBDataType.BigInt, true));
            return columnDefinitions;
        }

        #endregion
    }
}