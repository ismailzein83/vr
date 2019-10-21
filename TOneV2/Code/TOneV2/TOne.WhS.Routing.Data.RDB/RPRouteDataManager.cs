using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.RDB
{
    public class RPRouteDataManager : RoutingDataManager, IRPRouteDataManager
    {
        #region Fields/Ctor

        private static string RP_DBTABLE_SCHEMA = "dbo";
        internal static string RP_DBTABLE_NAME = "ProductRoute";
        private static string RP_TABLE_NAME = "dbo_ProductRoute";
        private static string RP_TABLE_ALIAS = "pr";

        private static string RPByCustomer_DBTABLE_SCHEMA = "dbo";
        internal static string RPByCustomer_DBTABLE_NAME = "ProductRouteByCustomer_#CustomerId#";
        private static string RPByCustomer_TABLE_NAME = "dbo_ProductRouteByCustomer_#CustomerId#";
        private static string RPByCustomer_TABLE_ALIAS = "prbycustomer";

        private const string COL_RoutingProductId = "RoutingProductId";
        private const string COL_SaleZoneId = "SaleZoneId";
        private const string COL_SellingNumberPlanId = "SellingNumberPlanId";
        private const string COL_SaleZoneServices = "SaleZoneServices";
        private const string COL_ExecutedRuleId = "ExecutedRuleId";
        private const string COL_OptionsDetailsBySupplier = "OptionsDetailsBySupplier";
        private const string COL_OptionsByPolicy = "OptionsByPolicy";
        private const string COL_IsBlocked = "IsBlocked";

        internal static Dictionary<string, RoutingTableColumnDefinition> s_ProductRouteRoutingTableColumnDefinitions;
        internal static Dictionary<string, RoutingTableColumnDefinition> s_ProductRouteByCustomerRoutingTableColumnDefinitions;
        private static Dictionary<string, RDBTableColumnDefinition> s_ProductRouteByCustomerTableColumnDefinitions;

        public IEnumerable<RoutingCustomerInfo> RoutingCustomerInfo { get; set; }

        static RPRouteDataManager()
        {
            s_ProductRouteRoutingTableColumnDefinitions = BuildProductRouteColumnDefinitions();
            s_ProductRouteByCustomerRoutingTableColumnDefinitions = BuildProductRouteBuCustomerColumnDefinitions();
            s_ProductRouteByCustomerTableColumnDefinitions = Helper.GetRDBTableColumnDefinitions(s_ProductRouteByCustomerRoutingTableColumnDefinitions);

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(RP_TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = RP_DBTABLE_SCHEMA,
                DBTableName = RP_DBTABLE_NAME,
                Columns = s_ProductRouteByCustomerTableColumnDefinitions
            });
        }

        #endregion

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var bulkInsertQueryContext = queryContext.StartBulkInsert();
            bulkInsertQueryContext.IntoTable(RP_TABLE_NAME, '^', COL_RoutingProductId, COL_SaleZoneId, COL_SellingNumberPlanId, COL_SaleZoneServices, COL_ExecutedRuleId,
                COL_OptionsDetailsBySupplier, COL_OptionsByPolicy, COL_IsBlocked);
            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(RPRouteByCustomer record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            if (!record.CustomerId.HasValue)
            {
                foreach (var rpRoute in record.RPRoutes)
                {
                    var recordContext = bulkInsertQueryContext.WriteRecord();
                    recordContext.Value(rpRoute.RoutingProductId);
                    recordContext.Value(rpRoute.SaleZoneId);
                    recordContext.Value(rpRoute.SellingNumberPlanID);

                    if (rpRoute.SaleZoneServiceIds != null && rpRoute.SaleZoneServiceIds.Count > 0)
                        recordContext.Value(string.Join(",", rpRoute.SaleZoneServiceIds));
                    else
                        recordContext.Value(string.Empty);

                    recordContext.Value(rpRoute.ExecutedRuleId);

                    string optionsDetailsBySupplier = TOne.WhS.Routing.Entities.Helper.SerializeOptionsDetailsBySupplier(rpRoute.OptionsDetailsBySupplier);
                    if (!string.IsNullOrEmpty(optionsDetailsBySupplier))
                        recordContext.Value(optionsDetailsBySupplier);
                    else
                        recordContext.Value(string.Empty);

                    string rpOptionsByPolicy = TOne.WhS.Routing.Entities.Helper.SerializeOptionsByPolicy(rpRoute.RPOptionsByPolicy);
                    if (!string.IsNullOrEmpty(rpOptionsByPolicy))
                        recordContext.Value(rpOptionsByPolicy);
                    else
                        recordContext.Value(string.Empty);
                }
            }
            else
            {
                foreach (var rpRoute in record.RPRoutes)
                {
                    var recordContext = bulkInsertQueryContext.WriteRecord();
                    recordContext.Value(rpRoute.RoutingProductId);
                    recordContext.Value(rpRoute.SaleZoneId);
                    recordContext.Value(rpRoute.SellingNumberPlanID);

                    if (rpRoute.SaleZoneServiceIds != null && rpRoute.SaleZoneServiceIds.Count > 0)
                        recordContext.Value(string.Join(",", rpRoute.SaleZoneServiceIds));
                    else
                        recordContext.Value(string.Empty);

                    recordContext.Value(rpRoute.ExecutedRuleId);

                    string rpOptionsByPolicy = TOne.WhS.Routing.Entities.Helper.SerializeOptionsByPolicy(rpRoute.RPOptionsByPolicy);
                    if (!string.IsNullOrEmpty(rpOptionsByPolicy))
                        recordContext.Value(rpOptionsByPolicy);
                    else
                        recordContext.Value(string.Empty);
                }
            }
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertQueryContext.CloseStream();
            return bulkInsertQueryContext;
        }

        public void ApplyProductRouteForDB(object preparedObject)
        {
            preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        public static void RegisterProductRouteByCustomerTableDefinition(int customerId)
        {
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(RPByCustomer_TABLE_NAME.Replace("#CustomerId#", customerId.ToString()), new RDBTableDefinition
            {
                DBSchemaName = RPByCustomer_DBTABLE_SCHEMA,
                DBTableName = RPByCustomer_DBTABLE_NAME.Replace("#CustomerId#", customerId.ToString()),
                Columns = s_ProductRouteByCustomerTableColumnDefinitions
            });
        }

        public IEnumerable<RPRouteByCode> GetFilteredRPRoutesByCode(Vanrise.Entities.DataRetrievalInput<RPRouteQueryByCode> input)
        {
            Dictionary<string, List<RPRouteByCode>> result = new Dictionary<string, List<RPRouteByCode>>();
            Dictionary<long, SupplierZoneDetail> supplierZoneDetailsById = new Dictionary<long, SupplierZoneDetail>();
            Dictionary<string, Dictionary<long, string>> supplierZoneIdsByCode = new Dictionary<string, Dictionary<long, string>>();

            string saleZoneTableAlias = "sz";
            string customerZoneDetailsTableAlias = "czd";
            string supplierZoneDetailsTableAlias = "szd";
            string codeSaleZoneMatchTableAlias = "cszm";
            string codeSupplierZoneMatchTableAlias = "cszm";

            string routesTempTableAlias = "routes";
            string distinctCodesTableAlias = "distinctCodes";
            string codeSupplierZoneMatchTempTableAlias = "codeSupplierZoneMatch";
            string distinctSupplierZoneIdsTableAlias = "distinctSupplierZoneIds";
            string effectiveRateValueColumnAlias = "EffectiveRateValue";

            string tableName, tableAlias;
            bool isExplicitCustomerTableExists = ExplicitCustomerTableExists(input.Query.CustomerId, out tableName, out tableAlias);

            //SELECT TOP #LimitResult# pr.[RoutingProductId], 
            //pr.[SaleZoneId], 
            //sz.Name as SaleZoneName, 
            //pr.SellingNumberPlanID,
            //pr.[SaleZoneServices],
            //pr.[ExecutedRuleId], 
            //cszm.Code, 
            //pr.IsBlocked,
            //#EFFECTIVERATE#
            //into #routes
            //FROM [dbo].#TABLENAME# pr with(nolock)
            //JOIN [dbo].[CodeSaleZoneMatch] cszm on pr.SaleZoneId = cszm.SaleZoneID
            //JOIN [dbo].[SaleZone] as sz ON cszm.SaleZoneID = sz.ID
            //#LEFT# #CUSTOMERZONEDETAILS# #CUSTOMER_IDS#
            //Where (@IsBlocked is null or IsBlocked = @IsBlocked) #ROUTING_PRODUCT_IDS#  
            //       #SIMULATE_ROUTING_PRODUCT_ID# #SALE_ZONE_IDS# #SELLING_NUMBER_PLAN_ID# #CodeFilter#

            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            var routesTempTableQuery = queryContext.CreateTempTable();
            routesTempTableQuery.AddColumn(COL_RoutingProductId, RDBDataType.Int);
            routesTempTableQuery.AddColumn(COL_SaleZoneId, RDBDataType.Int);
            routesTempTableQuery.AddColumn(COL_SellingNumberPlanId, RDBDataType.Int);
            routesTempTableQuery.AddColumn(COL_SaleZoneServices, RDBDataType.Varchar);
            routesTempTableQuery.AddColumn(COL_ExecutedRuleId, RDBDataType.Int);
            routesTempTableQuery.AddColumn(COL_IsBlocked, RDBDataType.Boolean);
            routesTempTableQuery.AddColumn(SaleZoneDataManager.COL_Name, RDBDataType.Varchar);
            routesTempTableQuery.AddColumn(CodeSaleZoneMatchDataManager.COL_Code, RDBDataType.Varchar, 20, null);
            routesTempTableQuery.AddColumn(CustomerZoneDetailsDataManager.COL_EffectiveRateValue, RDBDataType.Decimal, 20, 8);

            var insertIntoRoutesTempTableQuery = queryContext.AddInsertQuery();
            insertIntoRoutesTempTableQuery.IntoTable(routesTempTableQuery);

            RDBSelectQuery selectRPByCustomerQuery = insertIntoRoutesTempTableQuery.FromSelect();
            selectRPByCustomerQuery.From(tableName, tableAlias, input.Query.LimitResult, true);
            selectRPByCustomerQuery.SelectColumns().Column(COL_RoutingProductId);
            selectRPByCustomerQuery.SelectColumns().Column(COL_SaleZoneId);
            selectRPByCustomerQuery.SelectColumns().Column(COL_SellingNumberPlanId);
            selectRPByCustomerQuery.SelectColumns().Column(COL_SaleZoneServices);
            selectRPByCustomerQuery.SelectColumns().Column(COL_ExecutedRuleId);
            selectRPByCustomerQuery.SelectColumns().Column(COL_IsBlocked);
            selectRPByCustomerQuery.SelectColumns().Column(saleZoneTableAlias, SaleZoneDataManager.COL_Name, "saleZoneName");
            selectRPByCustomerQuery.SelectColumns().Column(codeSaleZoneMatchTableAlias, CodeSaleZoneMatchDataManager.COL_Code, "code");

            var codeSaleZoneMatchJoinContext = selectRPByCustomerQuery.Join();
            new CodeSaleZoneMatchDataManager().AddJoinCodeSaleZoneMatchBySaleZoneId(codeSaleZoneMatchJoinContext, RDBJoinType.Inner, codeSaleZoneMatchTableAlias, tableAlias, COL_SaleZoneId, false);

            var saleZoneJoinContext = selectRPByCustomerQuery.Join();
            new SaleZoneDataManager().AddJoinSaleZoneById(saleZoneJoinContext, RDBJoinType.Inner, saleZoneTableAlias, tableAlias, COL_SaleZoneId, false);

            var whereContext = selectRPByCustomerQuery.Where();

            if (input.Query.RouteStatus.HasValue)
            {
                if (input.Query.RouteStatus.Value == RouteStatus.Blocked)
                    whereContext.EqualsCondition(COL_IsBlocked).Value(true);
                else
                    whereContext.EqualsCondition(COL_IsBlocked).Value(false);
            }

            if (!string.IsNullOrEmpty(input.Query.CodePrefix))
                whereContext.StartsWithCondition(codeSaleZoneMatchTableAlias, CodeSaleZoneMatchDataManager.COL_Code, input.Query.CodePrefix);

            if (input.Query.RoutingProductIds != null && input.Query.RoutingProductIds.Count > 0)
                whereContext.ListCondition(COL_RoutingProductId, RDBListConditionOperator.IN, input.Query.RoutingProductIds);

            if (input.Query.SaleZoneIds != null && input.Query.SaleZoneIds.Count > 0)
                whereContext.ListCondition(COL_SaleZoneId, RDBListConditionOperator.IN, input.Query.SaleZoneIds);

            if (input.Query.SellingNumberPlanId.HasValue)
                whereContext.EqualsCondition(COL_SellingNumberPlanId).Value(input.Query.SellingNumberPlanId.Value);

            if (input.Query.CustomerId.HasValue)
            {
                selectRPByCustomerQuery.SelectColumns().Column(customerZoneDetailsTableAlias, CustomerZoneDetailsDataManager.COL_EffectiveRateValue, effectiveRateValueColumnAlias);

                var customerZoneDetailsJoinContext = selectRPByCustomerQuery.Join();
                RDBJoinType joinType = isExplicitCustomerTableExists ? RDBJoinType.Left : RDBJoinType.Inner;

                if (input.Query.SimulatedRoutingProductId.HasValue)
                {
                    new CustomerZoneDetailsDataManager().AddJoinCustomerZoneDetailsBySaleZoneId(customerZoneDetailsJoinContext, joinType, customerZoneDetailsTableAlias,
                        tableAlias, COL_SaleZoneId, input.Query.CustomerId.Value, false);

                    whereContext.EqualsCondition(COL_RoutingProductId).Value(input.Query.SimulatedRoutingProductId.Value);
                }
                else
                {
                    new CustomerZoneDetailsDataManager().AddJoinCustomerZoneDetailsBySaleZoneAndRoutingProduct(customerZoneDetailsJoinContext, joinType, customerZoneDetailsTableAlias,
                        tableAlias, COL_SaleZoneId, COL_RoutingProductId, input.Query.CustomerId.Value, false);
                }
            }
            else
            {
                selectRPByCustomerQuery.SelectColumns().Expression(effectiveRateValueColumnAlias).Null();
            }

            //select distinct code 
            //into #distinctCodes 
            //from #routes

            //select cszm.SupplierZoneID, cszm.Code, cszm.CodeMatch
            //into #CodeSupplierZoneMatch
            //FROM [dbo].[CodeSupplierZoneMatch] cszm 
            //JOIN #distinctCodes codes on cszm.Code = codes.Code

            var codeSupplierZoneMatchTempTableQuery = queryContext.CreateTempTable();
            codeSupplierZoneMatchTempTableQuery.AddColumn(CodeSupplierZoneMatchDataManager.COL_Code, RDBDataType.Varchar, 20, null);
            codeSupplierZoneMatchTempTableQuery.AddColumn(CodeSupplierZoneMatchDataManager.COL_SupplierZoneID, RDBDataType.BigInt);
            codeSupplierZoneMatchTempTableQuery.AddColumn(CodeSupplierZoneMatchDataManager.COL_CodeMatch, RDBDataType.Varchar, 20, null);

            var insertIntoCodeSupplierZoneMatchTempTableQuery = queryContext.AddInsertQuery();
            insertIntoCodeSupplierZoneMatchTempTableQuery.IntoTable(codeSupplierZoneMatchTempTableQuery);

            RDBSelectQuery selectCodeSupplierZoneMatchTableQuery = insertIntoCodeSupplierZoneMatchTempTableQuery.FromSelect();
            new CodeSupplierZoneMatchDataManager().AddSelectCodeSupplierZoneCodeMatch(selectCodeSupplierZoneMatchTableQuery, false);

            var joinDistinctCodesContext = selectCodeSupplierZoneMatchTableQuery.Join();
            var joinSelectDistinctCodesContext = joinDistinctCodesContext.JoinSelect(distinctCodesTableAlias);

            var selectDistinctCodesQuery = joinSelectDistinctCodesContext.SelectQuery();
            selectDistinctCodesQuery.From(routesTempTableQuery, routesTempTableAlias);
            selectDistinctCodesQuery.GroupBy().Select().Column(CodeSaleZoneMatchDataManager.COL_Code);

            joinDistinctCodesContext.JoinOnEqualOtherTableColumn(selectDistinctCodesQuery, distinctCodesTableAlias, CodeSaleZoneMatchDataManager.COL_Code,
                CodeSupplierZoneMatchDataManager.COL_Code, codeSupplierZoneMatchTableAlias);

            //SELECT * FROM #routes
            var selectRoutesTempTableQuery = queryContext.AddSelectQuery();
            selectRoutesTempTableQuery.From(routesTempTableQuery, routesTempTableAlias, null, false);
            selectRoutesTempTableQuery.SelectColumns().AllTableColumns(routesTempTableAlias);

            //SELECT * FROM #CodeSupplierZoneMatch
            var selectCodeSupplierZoneMatchTempTableQuery = queryContext.AddSelectQuery();
            selectCodeSupplierZoneMatchTempTableQuery.From(codeSupplierZoneMatchTempTableQuery, codeSupplierZoneMatchTempTableAlias, null, false);
            selectCodeSupplierZoneMatchTempTableQuery.SelectColumns().AllTableColumns(codeSupplierZoneMatchTempTableAlias);

            //select distinct supplierzoneid
            //into #distinctSupplierZoneIds
            //from #CodeSupplierZoneMatch

            //SELECT szd.[SupplierId],
            //szd.[SupplierZoneId],
            //szd.[EffectiveRateValue],
            //szd.[SupplierServiceIds],
            //szd.[ExactSupplierServiceIds],
            //szd.[SupplierServiceWeight],
            //szd.[SupplierRateId],
            //szd.[SupplierRateEED],
            //szd.[VersionNumber],
            //szd.[DealId]
            //FROM [dbo].[SupplierZoneDetail] szd with(nolock)
            //JOIN #distinctSupplierZoneIds sz on sz.SupplierZoneID = szd.SupplierZoneId");

            var selectSupplierZoneDetailsQuery = queryContext.AddSelectQuery();
            new SupplierZoneDetailsDataManager().AddSelectSupplierZoneDetails(selectSupplierZoneDetailsQuery, true);

            var joinDistinctSupplierZoneIdsContext = selectSupplierZoneDetailsQuery.Join();
            var joinSelectDistinctSupplierZoneIdsContext = joinDistinctSupplierZoneIdsContext.JoinSelect(distinctSupplierZoneIdsTableAlias);

            var selectDistinctSupplierZoneIdsQuery = joinSelectDistinctSupplierZoneIdsContext.SelectQuery();
            selectDistinctSupplierZoneIdsQuery.From(codeSupplierZoneMatchTempTableQuery, codeSupplierZoneMatchTableAlias);
            selectDistinctSupplierZoneIdsQuery.GroupBy().Select().Column(CodeSupplierZoneMatchDataManager.COL_SupplierZoneID);

            joinDistinctSupplierZoneIdsContext.JoinOnEqualOtherTableColumn(selectDistinctSupplierZoneIdsQuery, distinctSupplierZoneIdsTableAlias, CodeSupplierZoneMatchDataManager.COL_SupplierZoneID,
                SupplierZoneDetailsDataManager.COL_SupplierZoneId, supplierZoneDetailsTableAlias);

            queryContext.ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    string code = reader.GetString(CodeSaleZoneMatchDataManager.COL_Code);
                    RPRouteByCode rpRouteByCode = RPRouteByCodeMapper(reader);
                    List<RPRouteByCode> tempRPRouteByCodes = result.GetOrCreateItem(code);
                    tempRPRouteByCodes.Add(rpRouteByCode);
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        string code = reader.GetString(CodeSupplierZoneMatchDataManager.COL_Code);
                        long supplierZoneId = reader.GetLong(CodeSupplierZoneMatchDataManager.COL_SupplierZoneID);
                        string codeMatch = reader.GetString(CodeSupplierZoneMatchDataManager.COL_CodeMatch);

                        Dictionary<long, string> tempSupplierZoneIds = supplierZoneIdsByCode.GetOrCreateItem(code);
                        tempSupplierZoneIds.Add(supplierZoneId, codeMatch);
                    }
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        SupplierZoneDetail supplierZoneDetail = SupplierZoneDetailsDataManager.SupplierZoneDetailMapper(reader);
                        supplierZoneDetailsById.Add(supplierZoneDetail.SupplierZoneId, supplierZoneDetail);
                    }
                }
            });

            if (result.Count == 0)
                return null;

            List<RPRouteByCode> rpRouteByCodes = new List<RPRouteByCode>();
            foreach (var rpRouteBCodesKvp in result)
            {
                string code = rpRouteBCodesKvp.Key;
                List<SupplierZoneDetail> supplierZoneDetails = null;
                Dictionary<long, string> supplierCodeMatchByZoneId = null;

                Dictionary<long, string> supplierZoneIds = supplierZoneIdsByCode.GetRecord(code);
                if (supplierZoneIds != null)
                {
                    supplierZoneDetails = new List<SupplierZoneDetail>();
                    supplierCodeMatchByZoneId = new Dictionary<long, string>();

                    foreach (var supplierZoneIdKvp in supplierZoneIds)
                    {
                        supplierZoneDetails.Add(supplierZoneDetailsById.GetRecord(supplierZoneIdKvp.Key));
                        supplierCodeMatchByZoneId.Add(supplierZoneIdKvp.Key, supplierZoneIdKvp.Value);
                    }
                }

                foreach (var rpRouteByCode in rpRouteBCodesKvp.Value)
                {
                    rpRouteByCode.SupplierZoneDetails = supplierZoneDetails;
                    rpRouteByCode.SupplierCodeMatchByZoneId = supplierCodeMatchByZoneId;
                    rpRouteByCodes.Add(rpRouteByCode);
                }
            }

            return rpRouteByCodes;
        }

        public IEnumerable<Entities.BaseRPRoute> GetFilteredRPRoutesByZone(Vanrise.Entities.DataRetrievalInput<Entities.RPRouteQueryByZone> input)
        {
            string saleZoneTableAlias = "sz";
            string customerZoneDetailsTableAlias = "czd";
            string effectiveRateValueColumnAlias = "EffectiveRateValue";

            string tableName, tableAlias;
            bool isExplicitCustomerTableExists = ExplicitCustomerTableExists(input.Query.CustomerId, out tableName, out tableAlias);

            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(tableName, tableAlias, input.Query.LimitResult, true);
            selectQuery.SelectColumns().Column(COL_RoutingProductId);
            selectQuery.SelectColumns().Column(COL_SaleZoneId);
            selectQuery.SelectColumns().Column(COL_SellingNumberPlanId);
            selectQuery.SelectColumns().Column(COL_SaleZoneServices);
            selectQuery.SelectColumns().Column(COL_ExecutedRuleId);
            selectQuery.SelectColumns().Column(COL_OptionsByPolicy);
            selectQuery.SelectColumns().Column(COL_IsBlocked);
            selectQuery.SelectColumns().Column(saleZoneTableAlias, SaleZoneDataManager.COL_Name, "saleZoneName");

            var saleZoneJoinContext = selectQuery.Join();
            new SaleZoneDataManager().AddJoinSaleZoneById(saleZoneJoinContext, RDBJoinType.Inner, saleZoneTableAlias, tableAlias, COL_SaleZoneId, false);

            var whereContext = selectQuery.Where();

            if (input.Query.RouteStatus.HasValue)
            {
                if (input.Query.RouteStatus.Value == RouteStatus.Blocked)
                    whereContext.EqualsCondition(COL_IsBlocked).Value(true);
                else
                    whereContext.EqualsCondition(COL_IsBlocked).Value(false);
            }

            if (input.Query.RoutingProductIds != null && input.Query.RoutingProductIds.Count > 0)
                whereContext.ListCondition(COL_RoutingProductId, RDBListConditionOperator.IN, input.Query.RoutingProductIds);

            if (input.Query.SaleZoneIds != null && input.Query.SaleZoneIds.Count > 0)
                whereContext.ListCondition(COL_SaleZoneId, RDBListConditionOperator.IN, input.Query.SaleZoneIds);

            if (input.Query.SellingNumberPlanId.HasValue)
                whereContext.EqualsCondition(COL_SellingNumberPlanId).Value(input.Query.SellingNumberPlanId.Value);

            if (input.Query.CustomerId.HasValue)
            {
                selectQuery.SelectColumns().Column(customerZoneDetailsTableAlias, CustomerZoneDetailsDataManager.COL_EffectiveRateValue, effectiveRateValueColumnAlias);

                var customerZoneDetailsJoinContext = selectQuery.Join();
                RDBJoinType joinType = isExplicitCustomerTableExists ? RDBJoinType.Left : RDBJoinType.Inner;

                if (input.Query.SimulatedRoutingProductId.HasValue)
                {
                    new CustomerZoneDetailsDataManager().AddJoinCustomerZoneDetailsBySaleZoneId(customerZoneDetailsJoinContext, joinType, customerZoneDetailsTableAlias,
                        tableAlias, COL_SaleZoneId, input.Query.CustomerId.Value, false);

                    whereContext.EqualsCondition(COL_RoutingProductId).Value(input.Query.SimulatedRoutingProductId.Value);
                }
                else
                {
                    new CustomerZoneDetailsDataManager().AddJoinCustomerZoneDetailsBySaleZoneAndRoutingProduct(customerZoneDetailsJoinContext, joinType, customerZoneDetailsTableAlias,
                        tableAlias, COL_SaleZoneId, COL_RoutingProductId, input.Query.CustomerId.Value, false);
                }
            }
            else
            {
                selectQuery.SelectColumns().Expression(effectiveRateValueColumnAlias).Null();
            }

            return queryContext.GetItems<BaseRPRoute>(RPRouteMapper);

            //SELECT TOP #LimitResult# pr.[RoutingProductId]
            //      ,pr.[SaleZoneId]
            //      ,sz.[Name] 
            //      ,pr.[SellingNumberPlanID]
            //      ,pr.[SaleZoneServices]
            //      ,pr.[ExecutedRuleId]
            //      ,pr.[OptionsByPolicy]
            //      ,pr.[IsBlocked]
            //      #EFFECTIVERATE#
            //FROM [dbo].#TABLENAME# as pr with(nolock)
            //JOIN [dbo].[SaleZone] as sz ON pr.SaleZoneId=sz.ID
            //#LEFT# #CUSTOMERZONEDETAILS# #CUSTOMER_IDS#
            //Where (@IsBlocked is null or IsBlocked = @IsBlocked) #ROUTING_PRODUCT_IDS# #SIMULATE_ROUTING_PRODUCT_ID# 
            //       #SALE_ZONE_IDS# #SELLING_NUMBER_PLAN_ID#");
        }

        public IEnumerable<BaseRPRoute> GetRPRoutes(IEnumerable<RPZone> rpZones, int? customerId)
        {
            string tableName, tableAlias;
            bool explicitCustomerTableExists = ExplicitCustomerTableExists(customerId, out tableName, out tableAlias);

            string saleZoneTableAlias = "sz";
            string tempTableAlias = "rpZone";

            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_RoutingProductId, RDBDataType.Int, true);
            tempTableQuery.AddColumn(COL_SaleZoneId, RDBDataType.BigInt, true);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableQuery);

            foreach (var rpZone in rpZones)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_RoutingProductId).Value(rpZone.RoutingProductId);
                rowContext.Column(COL_SaleZoneId).Value(rpZone.SaleZoneId);
            }

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(tableName, tableAlias, null, true);
            selectQuery.SelectColumns().AllTableColumns(tableAlias);
            selectQuery.SelectColumns().Column(saleZoneTableAlias, SaleZoneDataManager.COL_Name, "saleZoneName");
            selectQuery.SelectColumns().Expression("EffectiveRateValue").Null();

            var saleZoneJoinContext = selectQuery.Join();
            new SaleZoneDataManager().AddJoinSaleZoneById(saleZoneJoinContext, RDBJoinType.Inner, saleZoneTableAlias, tableAlias, COL_SaleZoneId, false);

            var tempRPZoneJoinContext = selectQuery.Join();
            var joinStatementContext = tempRPZoneJoinContext.Join(tempTableQuery, tempTableAlias);
            var joinCondition = joinStatementContext.On();
            joinCondition.EqualsCondition(tableAlias, COL_RoutingProductId, tempTableAlias, COL_RoutingProductId);
            joinCondition.EqualsCondition(tableAlias, COL_SaleZoneId, tempTableAlias, COL_SaleZoneId);

            return queryContext.GetItems<BaseRPRoute>(RPRouteMapper);
        }

        public Dictionary<Guid, IEnumerable<RPRouteOption>> GetRouteOptions(int routingProductId, long saleZoneId, int? customerId)
        {
            string tableName, tableAlias;
            bool explicitCustomerTableExists = ExplicitCustomerTableExists(customerId, out tableName, out tableAlias);

            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(tableName, tableAlias, null, true);
            selectQuery.SelectColumns().Column(COL_OptionsByPolicy);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_RoutingProductId).Value(routingProductId);
            whereContext.EqualsCondition(COL_SaleZoneId).Value(saleZoneId);

            string serializedRouteOptions = queryContext.ExecuteScalar().StringValue;
            if (string.IsNullOrEmpty(serializedRouteOptions))
                return null;

            return TOne.WhS.Routing.Entities.Helper.DeserializeOptionsByPolicy(serializedRouteOptions);

            //SELECT [OptionsByPolicy]
            //FROM [dbo].#TABLENAME# with(nolock)
            //Where RoutingProductId = @RoutingProductId And SaleZoneId = @SaleZoneId";
        }

        public Dictionary<int, RPRouteOptionSupplier> GetRouteOptionSuppliers(int routingProductId, long saleZoneId)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(RP_TABLE_NAME, RP_TABLE_ALIAS, null, false); //withNoLock??
            selectQuery.SelectColumns().Column(COL_OptionsDetailsBySupplier);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_RoutingProductId).Value(routingProductId);
            whereContext.EqualsCondition(COL_SaleZoneId).Value(saleZoneId);

            string serializedOptionsDetailsBySuppliers = queryContext.ExecuteScalar().StringValue;
            if (string.IsNullOrEmpty(serializedOptionsDetailsBySuppliers))
                return null;

            return TOne.WhS.Routing.Entities.Helper.DeserializeOptionsDetailsBySupplier(serializedOptionsDetailsBySuppliers);

            //SELECT [OptionsDetailsBySupplier]
            //FROM [dbo].[ProductRoute] 
            //Where RoutingProductId = @RoutingProductId And SaleZoneId = @SaleZoneId";
        }

        public void FinalizeProductRoute(Action<string> trackStep, int commandTimeoutInSeconds, int? maxDOP)
        {
            int totalNumberOfIndexesDone = 0;
            int totalNumberOfIndexesToBuild = RoutingCustomerInfo != null ? RoutingCustomerInfo.Count() * 2 + 2 : 2;

            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            trackStep("Starting create Clustered Index on ProductRoute table (RoutingProductId and SaleZoneId).");
            CreateRoutingProductAndSaleZoneClusteredIndex(queryContext, RP_TABLE_NAME, maxDOP);
            trackStep("Finished create Clustered Index on ProductRoute table (RoutingProductId and SaleZoneId).");
            totalNumberOfIndexesDone++;
            trackStep($"Remaining Indexes to build: {totalNumberOfIndexesToBuild - totalNumberOfIndexesDone}");

            trackStep("Starting create Index on ProductRoute table (SaleZoneId).");
            CreateSaleZoneNonClusteredIndex(queryContext, RP_TABLE_NAME, maxDOP);
            trackStep("Finished create Index on ProductRoute table (SaleZoneId).");
            totalNumberOfIndexesDone++;
            trackStep($"Remaining Indexes to build: {totalNumberOfIndexesToBuild - totalNumberOfIndexesDone}");

            if (RoutingCustomerInfo != null)
            {
                foreach (var customer in RoutingCustomerInfo)
                {
                    string productRouteByCustomerTableName = RPByCustomer_TABLE_NAME.Replace("#CustomerId#", customer.CustomerId.ToString());

                    trackStep($"Starting create Clustered Index on {productRouteByCustomerTableName} table (RoutingProductId and SaleZoneId).");
                    CreateRoutingProductAndSaleZoneClusteredIndex(queryContext, productRouteByCustomerTableName, maxDOP);
                    trackStep($"Finished create Clustered Index on {productRouteByCustomerTableName} table (RoutingProductId and SaleZoneId).");
                    totalNumberOfIndexesDone++;
                    trackStep($"Remaining Indexes to build: {totalNumberOfIndexesToBuild - totalNumberOfIndexesDone}");

                    trackStep($"Starting create Index on {productRouteByCustomerTableName} table (SaleZoneId).");
                    CreateSaleZoneNonClusteredIndex(queryContext, productRouteByCustomerTableName, maxDOP);
                    trackStep($"Finished create Index on {productRouteByCustomerTableName} table (SaleZoneId).");
                    totalNumberOfIndexesDone++;
                    trackStep($"Remaining Indexes to build: {totalNumberOfIndexesToBuild - totalNumberOfIndexesDone}");
                }
            }

            queryContext.ExecuteNonQuery(commandTimeoutInSeconds);
        }

        #endregion

        #region Private Methods

        private bool ExplicitCustomerTableExists(int? customerId, out string tableName, out string tableAlias)
        {
            tableName = RP_TABLE_NAME;
            tableAlias = RP_TABLE_ALIAS;

            if (!customerId.HasValue)
                return false;

            string rpByCustomerTableName = RPByCustomer_TABLE_NAME.Replace("#CustomerId#", customerId.Value.ToString());

            var queryContext = new RDBQueryContext(GetDataProvider());

            RDBCheckIfTableExistsQuery rdbCheckIfTableExistsQuery = queryContext.AddCheckIfTableExistsQuery();
            rdbCheckIfTableExistsQuery.TableName(rpByCustomerTableName);

            bool isTableExists = queryContext.ExecuteScalar().BooleanValue;
            if (isTableExists)
            {
                tableName = rpByCustomerTableName;
                tableAlias = RPByCustomer_TABLE_ALIAS;
            }

            return isTableExists;
        }

        private void CreateRoutingProductAndSaleZoneClusteredIndex(RDBQueryContext queryContext, string tableName, int? maxDOP)
        {
            var createRoutingProductAndSaleZoneClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createRoutingProductAndSaleZoneClusteredIndexQuery.DBTableName(tableName);
            createRoutingProductAndSaleZoneClusteredIndexQuery.IndexName($"IX_{tableName}_RoutingProductId_SaleZoneId");
            createRoutingProductAndSaleZoneClusteredIndexQuery.IndexType(RDBCreateIndexType.Clustered);
            createRoutingProductAndSaleZoneClusteredIndexQuery.AddColumn(COL_RoutingProductId);
            createRoutingProductAndSaleZoneClusteredIndexQuery.AddColumn(COL_SaleZoneId);
            if (maxDOP.HasValue)
                createRoutingProductAndSaleZoneClusteredIndexQuery.MaxDOP(maxDOP.Value);

            //query = string.Format(query_CreateIX_ProductRoute_RoutingProductId, maxDOPSyntax).Replace("#TABLENAME#", "ProductRoute"); ;
            //ExecuteNonQueryText(query, null, commandTimeoutInSeconds);

            //private const string query_CreateIX_ProductRoute_RoutingProductId = @"CREATE CLUSTERED INDEX [IX_#TABLENAME#_RoutingProductId_SaleZoneId] ON dbo.#TABLENAME#
            //                                                                      (
            //                                                                         [RoutingProductId] ASC,
            //                                                                         [SaleZoneId] ASC
            //                                                                      )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, 
            //                                                                             ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";
        }

        private void CreateSaleZoneNonClusteredIndex(RDBQueryContext queryContext, string tableName, int? maxDOP)
        {
            var createSaleZoneNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createSaleZoneNonClusteredIndexQuery.DBTableName(tableName);
            createSaleZoneNonClusteredIndexQuery.IndexName($"IX_{tableName}_SaleZoneId");
            createSaleZoneNonClusteredIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createSaleZoneNonClusteredIndexQuery.AddColumn(COL_SaleZoneId);
            if (maxDOP.HasValue)
                createSaleZoneNonClusteredIndexQuery.MaxDOP(maxDOP.Value);

            //query = string.Format(query_CreateIX_ProductRoute_SaleZoneId, maxDOPSyntax).Replace("#TABLENAME#", "ProductRoute");
            //ExecuteNonQueryText(query, null, commandTimeoutInSeconds);

            //private const string query_CreateIX_ProductRoute_SaleZoneId = @"CREATE NONCLUSTERED INDEX [IX_#TABLENAME#_SaleZoneId] ON dbo.#TABLENAME#
            //                                                                (
            //                                                                   [SaleZoneId] ASC
            //                                                                )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, 
            //                                                                       ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";
        }

        private static Dictionary<string, RoutingTableColumnDefinition> BuildProductRouteColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_RoutingProductId, new RoutingTableColumnDefinition(COL_RoutingProductId, RDBDataType.Int, true));
            columnDefinitions.Add(COL_SaleZoneId, new RoutingTableColumnDefinition(COL_SaleZoneId, RDBDataType.BigInt, true));
            columnDefinitions.Add(COL_SellingNumberPlanId, new RoutingTableColumnDefinition(COL_SellingNumberPlanId, RDBDataType.Int, true));
            columnDefinitions.Add(COL_SaleZoneServices, new RoutingTableColumnDefinition(COL_SaleZoneServices, RDBDataType.Varchar));
            columnDefinitions.Add(COL_ExecutedRuleId, new RoutingTableColumnDefinition(COL_ExecutedRuleId, RDBDataType.Int));
            columnDefinitions.Add(COL_OptionsDetailsBySupplier, new RoutingTableColumnDefinition(COL_OptionsDetailsBySupplier, RDBDataType.NVarchar));
            columnDefinitions.Add(COL_OptionsByPolicy, new RoutingTableColumnDefinition(COL_OptionsByPolicy, RDBDataType.NVarchar));
            columnDefinitions.Add(COL_IsBlocked, new RoutingTableColumnDefinition(COL_IsBlocked, RDBDataType.Boolean));
            return columnDefinitions;
        }

        private static Dictionary<string, RoutingTableColumnDefinition> BuildProductRouteBuCustomerColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_RoutingProductId, new RoutingTableColumnDefinition(COL_RoutingProductId, RDBDataType.Int, true));
            columnDefinitions.Add(COL_SaleZoneId, new RoutingTableColumnDefinition(COL_SaleZoneId, RDBDataType.BigInt, true));
            columnDefinitions.Add(COL_SellingNumberPlanId, new RoutingTableColumnDefinition(COL_SellingNumberPlanId, RDBDataType.Int, true));
            columnDefinitions.Add(COL_SaleZoneServices, new RoutingTableColumnDefinition(COL_SaleZoneServices, RDBDataType.Varchar));
            columnDefinitions.Add(COL_ExecutedRuleId, new RoutingTableColumnDefinition(COL_ExecutedRuleId, RDBDataType.Int));
            columnDefinitions.Add(COL_OptionsDetailsBySupplier, new RoutingTableColumnDefinition(COL_OptionsDetailsBySupplier, RDBDataType.NVarchar));
            columnDefinitions.Add(COL_IsBlocked, new RoutingTableColumnDefinition(COL_IsBlocked, RDBDataType.Boolean));
            return columnDefinitions;
        }

        #endregion

        #region Mappers

        private BaseRPRoute RPRouteMapper(IRDBDataReader reader)
        {
            string saleZoneServices = reader.GetString("SaleZoneServices");
            string optionsByPolicy = reader.GetString("OptionsByPolicy");

            return new BaseRPRoute()
            {
                RoutingProductId = reader.GetInt("RoutingProductId"),
                SaleZoneId = reader.GetLong("SaleZoneId"),
                SellingNumberPlanID = reader.GetInt("SellingNumberPlanID"),
                SaleZoneName = reader.GetString("Name"),
                SaleZoneServiceIds = !string.IsNullOrEmpty(saleZoneServices) ? new HashSet<int>(saleZoneServices.Split(',').Select(itm => int.Parse(itm))) : null,
                IsBlocked = reader.GetBoolean("IsBlocked"),
                ExecutedRuleId = reader.GetInt("ExecutedRuleId"),
                EffectiveRateValue = reader.GetNullableDecimal("EffectiveRateValue"),
                RPOptionsByPolicy = TOne.WhS.Routing.Entities.Helper.DeserializeOptionsByPolicy(optionsByPolicy)
            };
        }

        private RPRouteByCode RPRouteByCodeMapper(IRDBDataReader reader)
        {
            string saleZoneServices = reader.GetString("SaleZoneServices");

            return new RPRouteByCode()
            {
                Code = reader.GetString("Code"),
                RoutingProductId = reader.GetInt("RoutingProductId"),
                SaleZoneId = reader.GetLong("SaleZoneId"),
                SaleZoneName = reader.GetString("SaleZoneName"),
                SellingNumberPlanID = reader.GetInt("SellingNumberPlanID"),
                SaleZoneServiceIds = !string.IsNullOrEmpty(saleZoneServices) ? new HashSet<int>(saleZoneServices.Split(',').Select(itm => int.Parse(itm))) : null,
                IsBlocked = reader.GetBoolean("IsBlocked"),
                ExecutedRuleId = reader.GetInt("ExecutedRuleId"),
                Rate = reader.GetNullableDecimal("EffectiveRateValue")
            };
        }

        #endregion
    }
}