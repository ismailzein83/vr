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
    public class CustomerRouteDataManager : RoutingDataManager , ICustomerRouteDataManager
    {
        #region Fields/Ctor

        private static string DBTABLE_SCHEMA = "dbo";
        internal static string DBTABLE_NAME = "CustomerRoute";
        private static string TABLE_NAME = "dbo_CustomerRoute";
        private static string TABLE_ALIAS = "cr";

        private const string COL_CustomerId = "CustomerId";
        private const string COL_Code = "Code";
        private const string COL_SaleZoneId = "SaleZoneId";
        private const string COL_IsBlocked = "IsBlocked";
        private const string COL_ExecutedRuleId = "ExecutedRuleId";
        private const string COL_SupplierIds = "SupplierIds";
        private const string COL_RouteOptions = "RouteOptions";

        private const string saleZoneTableAlias = "sz";
        private const string carrierAccountTableAlias = "ca";
        private const string customerZoneDetailsTableAlias = "czd";
        private const string codeSupplierZoneMatchTableAlias = "cszm";
        private const string modifiedCustomerRouteTableAlias = "mcr";

        internal static Dictionary<string, RoutingTableColumnDefinition> s_CustomerRouteColumnDefinitions;

        public int ParentWFRuntimeProcessId { get; set; }

        public long ParentBPInstanceId { get; set; }

        public Vanrise.BusinessProcess.IBPContext BPContext { set; get; }

        static CustomerRouteDataManager()
        {
            s_CustomerRouteColumnDefinitions = BuildCustomerRouteColumnDefinitions();
            Dictionary<string, RDBTableColumnDefinition> columns = Helper.GetRDBTableColumnDefinitions(s_CustomerRouteColumnDefinitions);

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
            bulkInsertQueryContext.IntoTable(TABLE_NAME, '^', COL_CustomerId, COL_Code, COL_SaleZoneId, COL_IsBlocked, COL_ExecutedRuleId, COL_SupplierIds, COL_RouteOptions);
            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(CustomerRoute record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            var recordContext = bulkInsertQueryContext.WriteRecord();
            recordContext.Value(record.CustomerId);
            recordContext.Value(record.Code);
            recordContext.Value(record.SaleZoneId);
            recordContext.Value(record.IsBlocked);

            if (record.ExecutedRuleId.HasValue)
                recordContext.Value(record.ExecutedRuleId.Value);
            else
                recordContext.Value(string.Empty);

            string supplierIds = TOne.WhS.Routing.Entities.Helper.GetConcatenatedSupplierIds(record);
            if (!string.IsNullOrEmpty(supplierIds))
                recordContext.Value(supplierIds);
            else
                recordContext.Value(string.Empty);

            string serializedOptions = record.Options != null ? TOne.WhS.Routing.Entities.Helper.SerializeOptions(record.Options) : null;
            if (!string.IsNullOrEmpty(serializedOptions))
                recordContext.Value(serializedOptions);
            else
                recordContext.Value(string.Empty);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertQueryContext.CloseStream();
            return bulkInsertQueryContext;
        }

        public void ApplyCustomerRouteForDB(object preparedObject)
        {
            preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        public IEnumerable<Entities.CustomerRoute> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<Entities.CustomerRouteQuery> input)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());
            RDBSelectQuery selectQuery = this.GetCustomerRoutesSelectQuery(queryContext, input.Query.LimitResult);

            var whereContext = selectQuery.Where();

            bool? isBlocked = null;
            if (input.Query.RouteStatus.HasValue)
                isBlocked = input.Query.RouteStatus.Value == RouteStatus.Blocked ? true : false;

            if (isBlocked.HasValue)
                whereContext.EqualsCondition(COL_IsBlocked).Value(isBlocked.Value);

            if (!string.IsNullOrEmpty(input.Query.Code))
                whereContext.StartsWithCondition(COL_Code, input.Query.Code);

            if (input.Query.CustomerIds != null && input.Query.CustomerIds.Count > 0)
                whereContext.ListCondition(COL_CustomerId, RDBListConditionOperator.IN, input.Query.CustomerIds);

            if (input.Query.SaleZoneIds != null && input.Query.SaleZoneIds.Count > 0)
                whereContext.ListCondition(COL_SaleZoneId, RDBListConditionOperator.IN, input.Query.SaleZoneIds);

            if (input.Query.SellingNumberPlanId.HasValue)
                whereContext.EqualsCondition(customerZoneDetailsTableAlias, CustomerZoneDetailsDataManager.COL_SellingProductId).Value(input.Query.SellingNumberPlanId.Value);

            if (input.Query.SupplierIds != null && input.Query.SupplierIds.Count > 0)
            {
                var supplierIdsCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);

                foreach (var supplierId in input.Query.SupplierIds)
                    supplierIdsCondition.ContainsCondition(COL_SupplierIds, $"${supplierId}$");
            }

            IEnumerable<Entities.CustomerRoute> customerRoutes = queryContext.GetItems(CustomerRouteMapper);

            CompleteSupplierData(customerRoutes);
            return customerRoutes;
        }

        public void LoadRoutes(int? customerId, string codePrefix, Func<bool> shouldStop, Action<CustomerRoute> onRouteLoaded)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());
            RDBSelectQuery selectQuery = this.GetCustomerRoutesSelectQuery(queryContext, null);

            var whereContext = selectQuery.Where();

            if (customerId.HasValue)
                whereContext.EqualsCondition(COL_CustomerId).Value(customerId.Value);

            if (!String.IsNullOrEmpty(codePrefix))
                whereContext.StartsWithCondition(COL_Code, codePrefix);

            SupplierZoneDetailsDataManager supplierZoneDetailsDataManager = new SupplierZoneDetailsDataManager();
            supplierZoneDetailsDataManager.RoutingDatabase = RoutingDatabase;

            queryContext.ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    if (shouldStop != null && shouldStop())
                        break;

                    CustomerRoute customerRoute = CustomerRouteMapper(reader);

                    if (customerRoute.Options != null && customerRoute.Options.Count > 0)
                    {
                        var cachedSupplierZoneDetails = supplierZoneDetailsDataManager.GetCachedSupplierZoneDetails();

                        foreach (RouteOption routeOption in customerRoute.Options)
                        {
                            SupplierZoneDetail supplierZoneDetail = cachedSupplierZoneDetails.GetRecord(routeOption.SupplierZoneId);
                            routeOption.SupplierId = supplierZoneDetail.SupplierId;
                            routeOption.SupplierRate = supplierZoneDetail.EffectiveRateValue;
                            routeOption.ExactSupplierServiceIds = supplierZoneDetail.ExactSupplierServiceIds;

                            if (routeOption.Backups == null || routeOption.Backups.Count == 0)
                                continue;

                            foreach (RouteBackupOption backup in routeOption.Backups)
                            {
                                SupplierZoneDetail backupSupplierZoneDetail = cachedSupplierZoneDetails.GetRecord(backup.SupplierZoneId);
                                backup.SupplierId = backupSupplierZoneDetail.SupplierId;
                                backup.SupplierRate = backupSupplierZoneDetail.EffectiveRateValue;
                                backup.ExactSupplierServiceIds = backupSupplierZoneDetail.ExactSupplierServiceIds;
                            }
                        }
                    }

                    onRouteLoaded(customerRoute);
                }
            });
        }

        public List<CustomerRoute> GetCustomerRoutesAfterVersionNb(int versionNb)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());
            RDBSelectQuery selectQuery = this.GetCustomerRoutesSelectQuery(queryContext, null);

            var whereContext = selectQuery.Where();
            whereContext.GreaterThanCondition(modifiedCustomerRouteTableAlias, ModifiedCustomerRouteDataManager.COL_VersionNumber).Value(versionNb);

            List<CustomerRoute> customerRoutes = queryContext.GetItems(CustomerRouteMapper);

            CompleteSupplierData(customerRoutes);
            return customerRoutes;
        }

        public List<CustomerRouteData> GetAffectedCustomerRoutes(List<AffectedRoutes> affectedRoutesList, List<AffectedRouteOptions> affectedRouteOptionsList, long partialRoutesNumberLimit, out bool maximumExceeded)
        {
            HashSet<string> addedCustomerRouteDefinitions = new HashSet<string>();
            List<CustomerRouteData> customerRouteDataList = new List<CustomerRouteData>();
            maximumExceeded = false;
            long addedItems = 0;

            if (affectedRoutesList != null && affectedRoutesList.Count > 0)
            {
                RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());
                RDBSelectQuery selectQuery = this.GetAffectedCustomerRoutesSelectQuery(queryContext);
                BuildAffectedRoutesConditionContext(selectQuery, affectedRoutesList);

                addedItems = ExecuteGetAffectedCustomerRoutesQuery(queryContext, addedCustomerRouteDefinitions, customerRouteDataList, partialRoutesNumberLimit, addedItems, out maximumExceeded);
            }

            if (maximumExceeded)
                return null;

            if (affectedRouteOptionsList != null && affectedRouteOptionsList.Count > 0)
            {
                RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());
                RDBSelectQuery selectQuery = this.GetAffectedCustomerRoutesSelectQuery(queryContext);
                BuildAffectedRouteOptionsConditionContext(selectQuery, affectedRouteOptionsList);

                addedItems = ExecuteGetAffectedCustomerRoutesQuery(queryContext, addedCustomerRouteDefinitions, customerRouteDataList, partialRoutesNumberLimit, addedItems, out maximumExceeded);
            }

            if (maximumExceeded)
                return null;

            return customerRouteDataList.Count > 0 ? customerRouteDataList : null;
        }

        public long GetTotalCount()
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());
            var selectRowsCountQuery = queryContext.AddSelectTableRowsCountQuery();
            selectRowsCountQuery.TableName(TABLE_NAME);
            return queryContext.ExecuteScalar().LongValue;
        }

        public void UpdateCustomerRoutes(List<CustomerRouteData> customerRouteDataList)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            string tempCustomerRouteTableAlias = "temp_cr";
            var tempCustomerRouteTableQuery = queryContext.CreateTempTable();
            tempCustomerRouteTableQuery.AddColumn(COL_CustomerId, RDBDataType.Int, null, null, true);
            tempCustomerRouteTableQuery.AddColumn(COL_Code, RDBDataType.Varchar, 20, 0, true);
            tempCustomerRouteTableQuery.AddColumn(COL_SaleZoneId, RDBDataType.BigInt, null, null, false);
            tempCustomerRouteTableQuery.AddColumn(COL_IsBlocked, RDBDataType.Boolean, null, null, false);
            tempCustomerRouteTableQuery.AddColumn(COL_ExecutedRuleId, RDBDataType.Int, null, null, false);
            tempCustomerRouteTableQuery.AddColumn(COL_SupplierIds, RDBDataType.Varchar, null, null, false);
            tempCustomerRouteTableQuery.AddColumn(COL_RouteOptions, RDBDataType.Varchar, null, null, false);
            tempCustomerRouteTableQuery.AddColumn(ModifiedCustomerRouteDataManager.COL_VersionNumber, RDBDataType.Int, null, null, false);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempCustomerRouteTableQuery);

            foreach (var itm in customerRouteDataList)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_CustomerId).Value(itm.CustomerId);
                rowContext.Column(COL_Code).Value(itm.Code);
                rowContext.Column(COL_SaleZoneId).Value(itm.SaleZoneId);
                rowContext.Column(COL_IsBlocked).Value(itm.IsBlocked);
                rowContext.Column(ModifiedCustomerRouteDataManager.COL_VersionNumber).Value(itm.VersionNumber);

                if (itm.ExecutedRuleId.HasValue)
                    rowContext.Column(COL_ExecutedRuleId).Value(itm.ExecutedRuleId.Value);
                else
                    rowContext.Column(COL_ExecutedRuleId).Null();

                if (!string.IsNullOrEmpty(itm.SupplierIds))
                    rowContext.Column(COL_SupplierIds).Value(itm.SupplierIds);
                else
                    rowContext.Column(COL_SupplierIds).Null();

                if (!string.IsNullOrEmpty(itm.Options))
                    rowContext.Column(COL_RouteOptions).Value(itm.Options);
                else
                    rowContext.Column(COL_RouteOptions).Null();
            }

            this.UpdateCustomerRoutes(queryContext, tempCustomerRouteTableQuery, tempCustomerRouteTableAlias);
            this.UpdateModifiedCustomerRoutes(queryContext, tempCustomerRouteTableQuery, tempCustomerRouteTableAlias);
            this.InsertIntoModifiedCustomerRoutes(queryContext, tempCustomerRouteTableQuery, tempCustomerRouteTableAlias);

            queryContext.ExecuteNonQuery();
        }

        public void FinalizeCurstomerRoute(Action<string> trackStep, int commandTimeoutInSeconds, int? maxDOP)
        {
            var queryContext = new RDBQueryContext(this.GetDataProvider());

            trackStep("Starting create Index on CustomerRoute table (Code).");
            var createCodeNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createCodeNonClusteredIndexQuery.DBTableName(DBTABLE_NAME);
            createCodeNonClusteredIndexQuery.IndexName("IX_CustomerRoute_Code");
            createCodeNonClusteredIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createCodeNonClusteredIndexQuery.AddColumn(COL_Code);
            if (maxDOP.HasValue)
                createCodeNonClusteredIndexQuery.MaxDOP(maxDOP.Value);
            trackStep("Finished create Index on CustomerRoute table (Code).");

            trackStep("Starting create Index on CustomerRoute table (SaleZoneId).");
            var createSaleZoneNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createSaleZoneNonClusteredIndexQuery.DBTableName(DBTABLE_NAME);
            createSaleZoneNonClusteredIndexQuery.IndexName("IX_CustomerRoute_SaleZone");
            createSaleZoneNonClusteredIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createSaleZoneNonClusteredIndexQuery.AddColumn(COL_SaleZoneId);
            if (maxDOP.HasValue)
                createSaleZoneNonClusteredIndexQuery.MaxDOP(maxDOP.Value);
            trackStep("Finished create Index on CustomerRoute table (SaleZoneId).");

            trackStep("Starting create CLUSTERED Index on CustomerRoute table (CustomerId and Code).");
            var createCustomerSaleZoneClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createCustomerSaleZoneClusteredIndexQuery.DBTableName(DBTABLE_NAME);
            createCustomerSaleZoneClusteredIndexQuery.IndexName("IX_CustomerRoute_CustomerId_Code");
            createCustomerSaleZoneClusteredIndexQuery.IndexType(RDBCreateIndexType.UniqueClustered);
            createCustomerSaleZoneClusteredIndexQuery.AddColumn(COL_CustomerId);
            createCustomerSaleZoneClusteredIndexQuery.AddColumn(COL_Code);
            if (maxDOP.HasValue)
                createCustomerSaleZoneClusteredIndexQuery.MaxDOP(maxDOP.Value);
            trackStep("Finished create CLUSTERED on CustomerRoute table (CustomerId and Code).");

            queryContext.ExecuteNonQuery(commandTimeoutInSeconds);
        }

        #endregion

        #region Private Methods

        private RDBSelectQuery GetCustomerRoutesSelectQuery(RDBQueryContext queryContext, int? limitResult)
        {
            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, limitResult, true);
            selectQuery.SelectColumns().Column(COL_CustomerId);
            selectQuery.SelectColumns().Column(carrierAccountTableAlias, CarrierAccountDataManager.COL_Name, "CustomerName");
            selectQuery.SelectColumns().Column(COL_Code);
            selectQuery.SelectColumns().Column(COL_SaleZoneId);
            selectQuery.SelectColumns().Column(saleZoneTableAlias, SaleZoneDataManager.COL_Name, "SaleZoneName");
            selectQuery.SelectColumns().Column(customerZoneDetailsTableAlias, CustomerZoneDetailsDataManager.COL_EffectiveRateValue, "Rate");
            selectQuery.SelectColumns().Column(customerZoneDetailsTableAlias, CustomerZoneDetailsDataManager.COL_SaleZoneServiceIds, "SaleZoneServiceIds");
            selectQuery.SelectColumns().Column(COL_IsBlocked);
            selectQuery.SelectColumns().Column(COL_ExecutedRuleId);
            selectQuery.SelectColumns().Column(COL_RouteOptions);
            selectQuery.SelectColumns().Column(modifiedCustomerRouteTableAlias, ModifiedCustomerRouteDataManager.COL_VersionNumber, "VersionNumber");

            var sortContext = selectQuery.Sort();
            sortContext.ByColumn(COL_CustomerId, RDBSortDirection.ASC);
            sortContext.ByColumn(COL_Code, RDBSortDirection.ASC);

            var joinContext = selectQuery.Join();
            new ModifiedCustomerRouteDataManager().AddJoinModifiedCustomerRouteByCustomerAndCode(joinContext, RDBJoinType.Left, modifiedCustomerRouteTableAlias, TABLE_ALIAS, COL_CustomerId, COL_Code, false);
            new SaleZoneDataManager().AddJoinSaleZoneById(joinContext, RDBJoinType.Inner, saleZoneTableAlias, TABLE_ALIAS, COL_SaleZoneId, false);
            new CarrierAccountDataManager().AddJoinCarrierAccountById(joinContext, RDBJoinType.Inner, carrierAccountTableAlias, TABLE_ALIAS, COL_CustomerId, false);
            new CustomerZoneDetailsDataManager().AddJoinCustomerZoneDetailsByCustomerAndSaleZone(joinContext, RDBJoinType.Inner, customerZoneDetailsTableAlias, TABLE_ALIAS, COL_CustomerId, COL_SaleZoneId, false);

            return selectQuery;
        }

        private RDBSelectQuery GetAffectedCustomerRoutesSelectQuery(RDBQueryContext queryContext)
        {
            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Column(COL_CustomerId);
            selectQuery.SelectColumns().Column(COL_Code);
            selectQuery.SelectColumns().Column(COL_SaleZoneId);
            selectQuery.SelectColumns().Column(COL_IsBlocked);
            selectQuery.SelectColumns().Column(COL_ExecutedRuleId);
            selectQuery.SelectColumns().Column(COL_RouteOptions);
            selectQuery.SelectColumns().Column(modifiedCustomerRouteTableAlias, ModifiedCustomerRouteDataManager.COL_VersionNumber, "VersionNumber");

            var joinContext = selectQuery.Join();
            new ModifiedCustomerRouteDataManager().AddJoinModifiedCustomerRouteByCustomerAndCode(joinContext, RDBJoinType.Left, modifiedCustomerRouteTableAlias, TABLE_ALIAS, COL_CustomerId, COL_Code, false);

            return selectQuery;

            //const string query_GetAffectedCustomerRoutes = @" SELECT cr.[CustomerId]
            //                                                      ,cr.[Code]
            //                                                      ,cr.[SaleZoneId]
            //                                                      ,cr.[IsBlocked]
            //                                                      ,cr.[ExecutedRuleId]
            //                                                      ,cr.[RouteOptions]
            //                                                      ,mcr.[VersionNumber]
            //                                                FROM [dbo].[CustomerRoute] cr with(nolock) 
            //                                                LEFT JOIN [dbo].[ModifiedCustomerRoute] as mcr ON cr.CustomerId = mcr.CustomerId and cr.Code = mcr.Code
            //                                                #CODESUPPLIERZONEMATCH#
            //                                                Where #AFFECTEDROUTES#";
        }

        private void BuildAffectedRoutesConditionContext(RDBSelectQuery selectQuery, List<AffectedRoutes> affectedRoutesList)
        {
            if (affectedRoutesList == null || affectedRoutesList.Count == 0)
                return;

            var whereContext = selectQuery.Where(RDBConditionGroupOperator.OR);

            foreach (AffectedRoutes affectedRoutes in affectedRoutesList)
            {
                var childConditionContext = whereContext.ChildConditionGroup(RDBConditionGroupOperator.AND);

                if (affectedRoutes.CustomerIds != null && affectedRoutes.CustomerIds.Count() > 0)
                    childConditionContext.ListCondition(COL_CustomerId, RDBListConditionOperator.IN, affectedRoutes.CustomerIds);

                if (affectedRoutes.ZoneIds != null && affectedRoutes.ZoneIds.Count() > 0)
                    childConditionContext.ListCondition(COL_SaleZoneId, RDBListConditionOperator.IN, affectedRoutes.ZoneIds);

                if (affectedRoutes.Codes != null && affectedRoutes.Codes.Count() > 0)
                {
                    var codesConditionContext = childConditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);

                    List<string> codesWithSubCodes = new List<string>(); //Can Be Deleted
                    List<string> codesWithoutSubCodes = new List<string>();

                    foreach (CodeCriteria codeCriteria in affectedRoutes.Codes)
                    {
                        if (codeCriteria.WithSubCodes)
                            codesWithSubCodes.Add(codeCriteria.Code);
                        else
                            codesWithoutSubCodes.Add(codeCriteria.Code);
                    }

                    if (codesWithoutSubCodes.Count > 0)
                        codesConditionContext.ListCondition(COL_Code, RDBListConditionOperator.IN, codesWithoutSubCodes);

                    if (codesWithSubCodes.Count > 0)
                    {
                        foreach (string code in codesWithSubCodes)
                            codesConditionContext.StartsWithCondition(COL_Code, code);
                    }
                }

                RoutingExcludedDestinationData routingExcludedDestinationData = affectedRoutes.RoutingExcludedDestinationData;
                if (routingExcludedDestinationData != null)
                {
                    if (routingExcludedDestinationData.ExcludedCodes != null && routingExcludedDestinationData.ExcludedCodes.Count > 0)
                        childConditionContext.ListCondition(COL_Code, RDBListConditionOperator.NotIN, routingExcludedDestinationData.ExcludedCodes);

                    if (routingExcludedDestinationData.CodeRanges != null && routingExcludedDestinationData.CodeRanges.Count > 0)
                    {
                        foreach (var excludedDestinationData in routingExcludedDestinationData.CodeRanges)
                        {
                            var excludedCodeRangesConditionContext = childConditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);

                            var compareConditionContext = excludedCodeRangesConditionContext.CompareCondition(RDBCompareConditionOperator.NEq);
                            compareConditionContext.Expression1().TextLength().Column(COL_Code);
                            compareConditionContext.Expression2().Value(excludedDestinationData.FromCode.Length);

                            excludedCodeRangesConditionContext.LessThanCondition(COL_Code).Value(excludedDestinationData.FromCode);
                            excludedCodeRangesConditionContext.GreaterThanCondition(COL_Code).Value(excludedDestinationData.ToCode);
                        }
                    }
                }
            }
        }

        private void BuildAffectedRouteOptionsConditionContext(RDBSelectQuery selectQuery, List<AffectedRouteOptions> affectedRouteOptionsList)
        {
            if (affectedRouteOptionsList == null || affectedRouteOptionsList.Count == 0)
                return;

            var whereContext = selectQuery.Where(RDBConditionGroupOperator.OR);

            foreach (AffectedRouteOptions affectedRouteOptions in affectedRouteOptionsList)
            {
                var childConditionContext = whereContext.ChildConditionGroup();

                if (affectedRouteOptions.SupplierWithZones != null && affectedRouteOptions.SupplierWithZones.Count() > 0)
                {
                    var joinContext = selectQuery.Join();
                    new CodeSupplierZoneMatchDataManager().AddJoinCodeSupplierZoneMatchByCode(joinContext, RDBJoinType.Inner, codeSupplierZoneMatchTableAlias, TABLE_ALIAS, COL_Code, false);

                    var supplierZonesConditionContext = childConditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);

                    HashSet<int> supplierIds = new HashSet<int>();
                    foreach (SupplierWithZones supplierWithZones in affectedRouteOptions.SupplierWithZones)
                    {
                        if (supplierWithZones.SupplierZoneIds != null && supplierWithZones.SupplierZoneIds.Count > 0)
                        {
                            var conditionContext = supplierZonesConditionContext.ChildConditionGroup();
                            conditionContext.EqualsCondition(codeSupplierZoneMatchTableAlias, CodeSupplierZoneMatchDataManager.COL_SupplierID).Value(supplierWithZones.SupplierId);
                            conditionContext.ListCondition(codeSupplierZoneMatchTableAlias, CodeSupplierZoneMatchDataManager.COL_SupplierZoneID, RDBListConditionOperator.IN, supplierWithZones.SupplierZoneIds);
                        }
                        else
                        {
                            supplierIds.Add(supplierWithZones.SupplierId);
                        }
                    }

                    if (supplierIds.Count > 0)
                        supplierZonesConditionContext.ListCondition(codeSupplierZoneMatchTableAlias, CodeSupplierZoneMatchDataManager.COL_SupplierID, RDBListConditionOperator.IN, supplierIds);
                }

                if (affectedRouteOptions.CustomerIds != null && affectedRouteOptions.CustomerIds.Count() > 0)
                    childConditionContext.ListCondition(COL_CustomerId, RDBListConditionOperator.IN, affectedRouteOptions.CustomerIds);

                if (affectedRouteOptions.ZoneIds != null && affectedRouteOptions.ZoneIds.Count() > 0)
                    childConditionContext.ListCondition(COL_SaleZoneId, RDBListConditionOperator.IN, affectedRouteOptions.ZoneIds);

                if (affectedRouteOptions.Codes != null && affectedRouteOptions.Codes.Count() > 0)
                {
                    var codesConditionContext = childConditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);

                    List<string> codesWithoutSubCodes = new List<string>();

                    foreach (CodeCriteria codeCriteria in affectedRouteOptions.Codes)
                    {
                        if (codeCriteria.WithSubCodes)
                            codesConditionContext.StartsWithCondition(COL_Code, codeCriteria.Code);
                        else
                            codesWithoutSubCodes.Add(codeCriteria.Code);
                    }

                    if (codesWithoutSubCodes.Count > 0)
                        codesConditionContext.ListCondition(COL_Code, RDBListConditionOperator.IN, codesWithoutSubCodes);
                }

                RoutingExcludedDestinationData routingExcludedDestinationData = affectedRouteOptions.RoutingExcludedDestinationData;
                if (routingExcludedDestinationData != null)
                {
                    if (routingExcludedDestinationData.ExcludedCodes != null && routingExcludedDestinationData.ExcludedCodes.Count > 0)
                        childConditionContext.ListCondition(COL_Code, RDBListConditionOperator.NotIN, routingExcludedDestinationData.ExcludedCodes);

                    if (routingExcludedDestinationData.CodeRanges != null && routingExcludedDestinationData.CodeRanges.Count > 0)
                    {
                        foreach (var excludedDestinationData in routingExcludedDestinationData.CodeRanges)
                        {
                            var excludedCodeRangesConditionContext = childConditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);

                            var compareConditionContext = excludedCodeRangesConditionContext.CompareCondition(RDBCompareConditionOperator.NEq);
                            compareConditionContext.Expression1().TextLength().Column(COL_Code);
                            compareConditionContext.Expression2().Value(excludedDestinationData.FromCode.Length);

                            excludedCodeRangesConditionContext.LessThanCondition(COL_Code).Value(excludedDestinationData.FromCode);
                            excludedCodeRangesConditionContext.GreaterThanCondition(COL_Code).Value(excludedDestinationData.ToCode);
                        }
                    }
                }
            }
        }

        private long ExecuteGetAffectedCustomerRoutesQuery(RDBQueryContext queryContext, HashSet<string> addedCustomerRouteDefinitions, List<CustomerRouteData> customerRouteDataList,
            long partialRoutesNumberLimit, long addedItems, out bool maximumExceeded)
        {
            maximumExceeded = false;

            queryContext.ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    CustomerRouteData customerRouteData = CustomerRouteDataMapper(reader);
                    string customerRouteDefinitionAsString = string.Concat(customerRouteData.CustomerId, "~", customerRouteData.Code);

                    if (!addedCustomerRouteDefinitions.Contains(customerRouteDefinitionAsString))
                    {
                        addedCustomerRouteDefinitions.Add(customerRouteDefinitionAsString);
                        customerRouteDataList.Add(customerRouteData);
                        addedItems++;
                        if (addedItems > partialRoutesNumberLimit)
                            break;
                    }
                }
            });

            if (addedItems > partialRoutesNumberLimit)
                maximumExceeded = true;

            return addedItems;
        }

        private void UpdateCustomerRoutes(RDBQueryContext queryContext, RDBTempTableQuery tempCustomerRouteTableQuery, string tempCustomerRouteTableAlias)
        {
            RDBUpdateQuery updateCustomerRouteQuery = queryContext.AddUpdateQuery();
            updateCustomerRouteQuery.FromTable(TABLE_NAME);

            var joinTempCustomerRouteContext = updateCustomerRouteQuery.Join(TABLE_ALIAS);
            var joinTempCustomerRouteStatementContext = joinTempCustomerRouteContext.Join(tempCustomerRouteTableQuery, tempCustomerRouteTableAlias);

            var joinTempCustomerRouteCondition = joinTempCustomerRouteStatementContext.On();
            joinTempCustomerRouteCondition.EqualsCondition(TABLE_ALIAS, COL_CustomerId, tempCustomerRouteTableAlias, COL_CustomerId);
            joinTempCustomerRouteCondition.EqualsCondition(TABLE_ALIAS, COL_Code, tempCustomerRouteTableAlias, COL_Code);

            updateCustomerRouteQuery.Column(COL_IsBlocked).Column(tempCustomerRouteTableAlias, COL_IsBlocked);
            updateCustomerRouteQuery.Column(COL_ExecutedRuleId).Column(tempCustomerRouteTableAlias, COL_ExecutedRuleId);
            updateCustomerRouteQuery.Column(COL_SupplierIds).Column(tempCustomerRouteTableAlias, COL_SupplierIds);
            updateCustomerRouteQuery.Column(COL_RouteOptions).Column(tempCustomerRouteTableAlias, COL_RouteOptions);

            //UPDATE CustomerRoute 
            //SET customerRoute.IsBlocked = routes.IsBlocked, customerRoute.ExecutedRuleId = routes.ExecutedRuleId, 
            //    customerRoute.SupplierIds = routes.SupplierIds, customerRoute.RouteOptions = routes.RouteOptions
            //FROM [dbo].[CustomerRoute] customerRoute WITH(INDEX(IX_CustomerRoute_CustomerId_Code)) ????
            //JOIN @Routes routes on routes.CustomerId = customerRoute.CustomerId and routes.Code = customerRoute.Code
        }

        private void UpdateModifiedCustomerRoutes(RDBQueryContext queryContext, RDBTempTableQuery tempCustomerRouteTableQuery, string tempCustomerRouteTableAlias)
        {
            RDBUpdateQuery updateModifiedCustomerRouteQuery = queryContext.AddUpdateQuery();
            updateModifiedCustomerRouteQuery.FromTable(ModifiedCustomerRouteDataManager.TABLE_NAME);

            var joinTempCustomerRouteContext = updateModifiedCustomerRouteQuery.Join(ModifiedCustomerRouteDataManager.TABLE_NAME);
            var joinTempCustomerRouteStatementContext = joinTempCustomerRouteContext.Join(tempCustomerRouteTableQuery, tempCustomerRouteTableAlias);

            var joinTempCustomerRouteCondition = joinTempCustomerRouteStatementContext.On();
            joinTempCustomerRouteCondition.EqualsCondition(ModifiedCustomerRouteDataManager.TABLE_ALIAS, ModifiedCustomerRouteDataManager.COL_CustomerId, tempCustomerRouteTableAlias, COL_CustomerId);
            joinTempCustomerRouteCondition.EqualsCondition(ModifiedCustomerRouteDataManager.TABLE_ALIAS, ModifiedCustomerRouteDataManager.COL_Code, tempCustomerRouteTableAlias, COL_Code);

            updateModifiedCustomerRouteQuery.Column(ModifiedCustomerRouteDataManager.COL_VersionNumber).Column(tempCustomerRouteTableAlias, ModifiedCustomerRouteDataManager.COL_VersionNumber);

            //UPDATE modifiedCustomerRoute 
            //Set modifiedCustomerRoute.VersionNumber = routes.VersionNumber
            //FROM [dbo].[ModifiedCustomerRoute] modifiedCustomerRoute WITH(INDEX(IX_ModifiedCustomerRoute_CustomerId_Code)) ????
            //JOIN @Routes routes on routes.CustomerId = modifiedCustomerRoute.CustomerId and routes.Code = modifiedCustomerRoute.Code
        }

        private void InsertIntoModifiedCustomerRoutes(RDBQueryContext queryContext, RDBTempTableQuery tempCustomerRouteTableQuery, string tempCustomerRouteTableAlias)
        {
            var insertIntoModifiedCustomerRoutesTableQuery = queryContext.AddInsertQuery();
            insertIntoModifiedCustomerRoutesTableQuery.IntoTable(ModifiedCustomerRouteDataManager.TABLE_NAME);

            RDBSelectQuery selectTempCustomerRoutesQuery = insertIntoModifiedCustomerRoutesTableQuery.FromSelect();
            selectTempCustomerRoutesQuery.From(tempCustomerRouteTableQuery, tempCustomerRouteTableAlias, null, true);
            selectTempCustomerRoutesQuery.SelectColumns().Column(COL_CustomerId);
            selectTempCustomerRoutesQuery.SelectColumns().Column(COL_Code);
            selectTempCustomerRoutesQuery.SelectColumns().Column(ModifiedCustomerRouteDataManager.COL_VersionNumber);

            var joinModifiedCustomerRouteContext = selectTempCustomerRoutesQuery.Join();
            var joinModifiedCustomerRouteStatementContext = joinModifiedCustomerRouteContext.Join(ModifiedCustomerRouteDataManager.TABLE_NAME, ModifiedCustomerRouteDataManager.TABLE_ALIAS);
            joinModifiedCustomerRouteStatementContext.JoinType(RDBJoinType.Left);

            var joinModifiedCustomerRouteCondition = joinModifiedCustomerRouteStatementContext.On();
            joinModifiedCustomerRouteCondition.EqualsCondition(ModifiedCustomerRouteDataManager.TABLE_ALIAS, ModifiedCustomerRouteDataManager.COL_CustomerId, tempCustomerRouteTableAlias, COL_CustomerId);
            joinModifiedCustomerRouteCondition.EqualsCondition(ModifiedCustomerRouteDataManager.TABLE_ALIAS, ModifiedCustomerRouteDataManager.COL_Code, tempCustomerRouteTableAlias, COL_Code);

            var whereContext = selectTempCustomerRoutesQuery.Where();
            whereContext.EqualsCondition(ModifiedCustomerRouteDataManager.TABLE_ALIAS, ModifiedCustomerRouteDataManager.COL_CustomerId).Null();

            //INSERT INTO [dbo].[ModifiedCustomerRoute](CustomerId, Code, VersionNumber)
            //SELECT routes.CustomerId, routes.Code, routes.VersionNumber 
            //FROM @Routes routes 
            //LEFT JOIN [dbo].[ModifiedCustomerRoute] mcr ON routes.CustomerId = mcr.CustomerId and routes.Code = mcr.Code
            //WHERE mcr.CustomerId IS NULL";
        }

        private void CompleteSupplierData(IEnumerable<CustomerRoute> customerRoutes)
        {
            if (customerRoutes == null || customerRoutes.Count() == 0)
                return;

            HashSet<long> supplierZoneIds = new HashSet<long>();
            foreach (CustomerRoute customerRoute in customerRoutes)
            {
                if (customerRoute.Options == null || customerRoute.Options.Count == 0)
                    continue;

                foreach (RouteOption routeOption in customerRoute.Options)
                {
                    supplierZoneIds.Add(routeOption.SupplierZoneId);
                    if (routeOption.Backups == null || routeOption.Backups.Count == 0)
                        continue;

                    foreach (RouteBackupOption backup in routeOption.Backups)
                        supplierZoneIds.Add(backup.SupplierZoneId);
                }
            }

            if (supplierZoneIds.Count > 0)
            {
                SupplierZoneDetailsDataManager supplierZoneDetailsDataManager = new SupplierZoneDetailsDataManager();
                supplierZoneDetailsDataManager.RoutingDatabase = this.RoutingDatabase;

                Dictionary<long, SupplierZoneDetail> supplierZoneDetails = supplierZoneDetailsDataManager.GetFilteredSupplierZoneDetailsBySupplierZone(supplierZoneIds).ToDictionary(itm => itm.SupplierZoneId, itm => itm);

                foreach (CustomerRoute customerRoute in customerRoutes)
                {
                    if (customerRoute.Options == null || customerRoute.Options.Count == 0)
                        continue;

                    foreach (RouteOption routeOption in customerRoute.Options)
                    {
                        SupplierZoneDetail supplierZoneDetail = supplierZoneDetails.GetRecord(routeOption.SupplierZoneId);
                        routeOption.SupplierId = supplierZoneDetail.SupplierId;
                        routeOption.SupplierRate = supplierZoneDetail.EffectiveRateValue;
                        routeOption.ExactSupplierServiceIds = supplierZoneDetail.ExactSupplierServiceIds;

                        if (routeOption.Backups == null || routeOption.Backups.Count == 0)
                            continue;

                        foreach (RouteBackupOption backup in routeOption.Backups)
                        {
                            SupplierZoneDetail backupSupplierZoneDetail = supplierZoneDetails.GetRecord(backup.SupplierZoneId);
                            backup.SupplierId = backupSupplierZoneDetail.SupplierId;
                            backup.SupplierRate = backupSupplierZoneDetail.EffectiveRateValue;
                            backup.ExactSupplierServiceIds = backupSupplierZoneDetail.ExactSupplierServiceIds;
                        }
                    }
                }
            }
        }

        private static Dictionary<string, RoutingTableColumnDefinition> BuildCustomerRouteColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_CustomerId, new RoutingTableColumnDefinition(COL_CustomerId, RDBDataType.Int, true));
            columnDefinitions.Add(COL_Code, new RoutingTableColumnDefinition(COL_Code, RDBDataType.Varchar, 20, 0, true));
            columnDefinitions.Add(COL_SaleZoneId, new RoutingTableColumnDefinition(COL_SaleZoneId, RDBDataType.BigInt, true));
            columnDefinitions.Add(COL_IsBlocked, new RoutingTableColumnDefinition(COL_IsBlocked, RDBDataType.Boolean, true));
            columnDefinitions.Add(COL_ExecutedRuleId, new RoutingTableColumnDefinition(COL_ExecutedRuleId, RDBDataType.Int));
            columnDefinitions.Add(COL_SupplierIds, new RoutingTableColumnDefinition(COL_SupplierIds, RDBDataType.Varchar));
            columnDefinitions.Add(COL_RouteOptions, new RoutingTableColumnDefinition(COL_RouteOptions, RDBDataType.Varchar));
            return columnDefinitions;
        }

        #endregion

        #region Mappers

        private CustomerRoute CustomerRouteMapper(IRDBDataReader reader)
        {
            string saleZoneServiceIds = reader.GetString("SaleZoneServiceIds");
            string serializedRouteOptions = reader.GetString("RouteOptions");

            return new CustomerRoute()
            {
                CustomerId = reader.GetInt("CustomerID"),
                CustomerName = reader.GetString("CustomerName"),
                Code = reader.GetString("Code"),
                SaleZoneId = reader.GetLong("SaleZoneID"),
                SaleZoneName = reader.GetString("SaleZoneName"),
                Rate = reader.GetNullableDecimal("Rate"),
                SaleZoneServiceIds = !string.IsNullOrEmpty(saleZoneServiceIds) ? new HashSet<int>(saleZoneServiceIds.Split(',').Select(itm => int.Parse(itm))) : null,
                IsBlocked = reader.GetBoolean("IsBlocked"),
                ExecutedRuleId = reader.GetNullableInt("ExecutedRuleId"),
                Options = !string.IsNullOrEmpty(serializedRouteOptions) ? TOne.WhS.Routing.Entities.Helper.DeserializeOptions(serializedRouteOptions) : null,
                VersionNumber = reader.GetInt("VersionNumber")
            };
        }

        private CustomerRouteData CustomerRouteDataMapper(IRDBDataReader reader)
        {
            return new CustomerRouteData()
            {
                CustomerId = reader.GetInt("CustomerID"),
                Code = reader.GetString("Code") as string,
                SaleZoneId = reader.GetLong("SaleZoneID"),
                IsBlocked = reader.GetBoolean("IsBlocked"),
                ExecutedRuleId = reader.GetNullableInt("ExecutedRuleId"),
                Options = reader.GetString("RouteOptions"),
                VersionNumber = reader.GetInt("VersionNumber")
            };
        }

        #endregion
    }
}