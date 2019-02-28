using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class RPRouteDataManager : RoutingDataManager, IRPRouteDataManager
    {
        readonly string[] productRouteColumns = { "RoutingProductId", "SaleZoneId", "SellingNumberPlanId", "SaleZoneServices", "ExecutedRuleId", "OptionsDetailsBySupplier", "OptionsByPolicy", "IsBlocked" };
        readonly string[] productRouteByCustomerColumns = { "RoutingProductId", "SaleZoneId", "SellingNumberPlanId", "SaleZoneServices", "ExecutedRuleId", "OptionsByPolicy", "IsBlocked" };

        public IEnumerable<RoutingCustomerInfo> RoutingCustomerInfo { get; set; }

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            ProductRouteBulkInsert productRouteBulkInsert = new ProductRouteBulkInsert();
            productRouteBulkInsert.RPRoutesStream = base.InitializeStreamForBulkInsert();
            if (RoutingCustomerInfo != null && RoutingCustomerInfo.Count() > 0)
            {
                productRouteBulkInsert.RPRoutesByCustomerStream = new Dictionary<int, StreamForBulkInsert>();
                foreach (var routingCustomerInfo in RoutingCustomerInfo)
                {
                    productRouteBulkInsert.RPRoutesByCustomerStream.Add(routingCustomerInfo.CustomerId, base.InitializeStreamForBulkInsert());
                }
            }
            return productRouteBulkInsert;
        }

        public void WriteRecordToStream(Entities.RPRouteByCustomer record, object dbApplyStream)
        {
            ProductRouteBulkInsert streamForBulkInsert = dbApplyStream as ProductRouteBulkInsert;

            if (!record.CustomerId.HasValue)
            {
                foreach (var rproute in record.RPRoutes)
                {
                    string saleZoneServices = (rproute.SaleZoneServiceIds != null && rproute.SaleZoneServiceIds.Count > 0) ? string.Join(",", rproute.SaleZoneServiceIds) : null;
                    string optionsDetailsBySupplier = Helper.SerializeOptionsDetailsBySupplier(rproute.OptionsDetailsBySupplier);
                    string rpOptionsByPolicy = Helper.SerializeOptionsByPolicy(rproute.RPOptionsByPolicy);

                    streamForBulkInsert.RPRoutesStream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}", rproute.RoutingProductId, rproute.SaleZoneId, rproute.SellingNumberPlanID, saleZoneServices, rproute.ExecutedRuleId,
                                                                                   optionsDetailsBySupplier, rpOptionsByPolicy, rproute.IsBlocked ? 1 : 0);
                }
            }
            else
            {
                var customerStream = streamForBulkInsert.RPRoutesByCustomerStream.GetRecord(record.CustomerId.Value);

                foreach (var rproute in record.RPRoutes)
                {
                    string saleZoneServices = (rproute.SaleZoneServiceIds != null && rproute.SaleZoneServiceIds.Count > 0) ? string.Join(",", rproute.SaleZoneServiceIds) : null;
                    string rpOptionsByPolicy = Helper.SerializeOptionsByPolicy(rproute.RPOptionsByPolicy);

                    customerStream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}", rproute.RoutingProductId, rproute.SaleZoneId, rproute.SellingNumberPlanID, saleZoneServices, rproute.ExecutedRuleId,
                                                                                    rpOptionsByPolicy, rproute.IsBlocked ? 1 : 0);
                }
            }
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            List<StreamBulkInsertInfo> streamBulkInsertInfoList = new List<StreamBulkInsertInfo>();

            ProductRouteBulkInsert streamForBulkInsert = dbApplyStream as ProductRouteBulkInsert;
            streamForBulkInsert.RPRoutesStream.Close();
            streamBulkInsertInfoList.Add(new StreamBulkInsertInfo()
            {
                TableName = "[dbo].[ProductRoute]",
                Stream = streamForBulkInsert.RPRoutesStream,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = productRouteColumns
            });

            if (streamForBulkInsert.RPRoutesByCustomerStream != null && streamForBulkInsert.RPRoutesByCustomerStream.Count > 0)
            {
                foreach (var rPRoutesByCusotmerStream in streamForBulkInsert.RPRoutesByCustomerStream)
                {
                    rPRoutesByCusotmerStream.Value.Close();
                    streamBulkInsertInfoList.Add(new StreamBulkInsertInfo()
                    {
                        TableName = string.Format("[dbo].[ProductRouteByCustomer_{0}]", rPRoutesByCusotmerStream.Key),
                        Stream = rPRoutesByCusotmerStream.Value,
                        TabLock = true,
                        KeepIdentity = false,
                        FieldSeparator = '^',
                        ColumnNames = productRouteByCustomerColumns
                    });
                }
            }
            return streamBulkInsertInfoList;

        }

        public void ApplyProductRouteForDB(object preparedProductRoute)
        {
            var streamBulkInsertInfoList = preparedProductRoute as List<StreamBulkInsertInfo>;
            foreach (var streamBulkInsertInfo in streamBulkInsertInfoList)
            {
                InsertBulkToTable(streamBulkInsertInfo);
            }
        }

        public IEnumerable<RPRouteByCode> GetFilteredRPRoutesByCode(Vanrise.Entities.DataRetrievalInput<RPRouteQueryByCode> input)
        {
            Dictionary<string, List<RPRouteByCode>> result = new Dictionary<string, List<RPRouteByCode>>();
            Dictionary<long, SupplierZoneDetail> supplierZoneDetailsById = new Dictionary<long, SupplierZoneDetail>();
            Dictionary<string, Dictionary<long, string>> supplierZoneIdsByCode = new Dictionary<string, Dictionary<long, string>>();

            string tableName;
            bool explicitCustomerTableExists = ExplicitCustomerTableExists(input.Query.CustomerId, out tableName);

            string codeFilter = string.Empty;
            string routingProductIdsFilter = string.Empty;
            string saleZoneIdsFilter = string.Empty;
            string sellingNumberPlanIdFilter = string.Empty;
            string customerIdFilter = string.Empty;

            string joinCustomerZoneDetail = string.Empty;
            string effectiveRateValue = string.Empty;
            string simulatedRoutingProductIdFilter = string.Empty;
            string leftJoin = explicitCustomerTableExists ? " LEFT " : string.Empty;

            bool? isBlocked = null;
            if (input.Query.RouteStatus.HasValue)
                isBlocked = input.Query.RouteStatus.Value == RouteStatus.Blocked ? true : false;

            if (!string.IsNullOrEmpty(input.Query.CodePrefix))
                codeFilter = string.Format(" AND cszm.Code like '{0}%' ", input.Query.CodePrefix);

            if (input.Query.RoutingProductIds != null && input.Query.RoutingProductIds.Count > 0)
                routingProductIdsFilter = string.Format(" AND pr.RoutingProductId In({0})", string.Join(",", input.Query.RoutingProductIds));

            if (input.Query.SaleZoneIds != null && input.Query.SaleZoneIds.Count > 0)
                saleZoneIdsFilter = string.Format(" AND pr.SaleZoneId In({0})", string.Join(",", input.Query.SaleZoneIds));

            if (input.Query.SellingNumberPlanId.HasValue)
                sellingNumberPlanIdFilter = string.Format("AND pr.SellingNumberPlanId = {0}", input.Query.SellingNumberPlanId.Value);

            if (input.Query.CustomerId.HasValue)
            {
                customerIdFilter = string.Format(" AND CustomerId = {0} ", input.Query.CustomerId);
                effectiveRateValue = "czd.[EffectiveRateValue] ";

                if (input.Query.SimulatedRoutingProductId.HasValue)
                {
                    joinCustomerZoneDetail = " JOIN [dbo].[CustomerZoneDetail] as czd ON pr.SaleZoneId = czd.SaleZoneId ";
                    simulatedRoutingProductIdFilter = string.Format(" AND pr.RoutingProductId = {0} ", input.Query.SimulatedRoutingProductId.Value);
                }
                else
                {
                    joinCustomerZoneDetail = " JOIN [dbo].[CustomerZoneDetail] as czd ON pr.SaleZoneId = czd.SaleZoneId and pr.RoutingProductId = czd.RoutingProductId ";
                }
            }
            else
            {
                effectiveRateValue = "null as EffectiveRateValue ";
            }

            string query = query_GetFilteredRPRoutesByCode.ToString();
            query = query.Replace("#TABLENAME#", tableName);
            query = query.Replace("#LimitResult#", input.Query.LimitResult.ToString());
            query = query.Replace("#ROUTING_PRODUCT_IDS#", routingProductIdsFilter);
            query = query.Replace("#SIMULATE_ROUTING_PRODUCT_ID#", simulatedRoutingProductIdFilter);
            query = query.Replace("#SALE_ZONE_IDS#", saleZoneIdsFilter);
            query = query.Replace("#SELLING_NUMBER_PLAN_ID#", sellingNumberPlanIdFilter);
            query = query.Replace("#CodeFilter#", codeFilter);

            query = query.Replace("#CUSTOMER_IDS#", customerIdFilter);
            query = query.Replace("#CUSTOMERZONEDETAILS#", joinCustomerZoneDetail);
            query = query.Replace("#EFFECTIVERATE#", effectiveRateValue);
            query = query.Replace("#LEFT#", leftJoin);

            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    string code = reader["Code"] as string;
                    RPRouteByCode rpRouteByCode = RPRouteByCodeMapper(reader);
                    List<RPRouteByCode> tempRPRouteByCodes = result.GetOrCreateItem(code);
                    tempRPRouteByCodes.Add(rpRouteByCode);
                }

                if (reader.NextResult())
                    while (reader.Read())
                    {
                        string code = reader["Code"] as string;
                        long supplierZoneId = (long)reader["supplierZoneId"];
                        string codeMatch = reader["CodeMatch"] as string;

                        Dictionary<long, string> tempSupplierZoneIds = supplierZoneIdsByCode.GetOrCreateItem(code);
                        tempSupplierZoneIds.Add(supplierZoneId, codeMatch);
                    }

                if (reader.NextResult())
                    while (reader.Read())
                    {
                        SupplierZoneDetail supplierZoneDetail = SupplierZoneDetailsDataManager.SupplierZoneDetailMapper(reader);
                        supplierZoneDetailsById.Add(supplierZoneDetail.SupplierZoneId, supplierZoneDetail);
                    }
            }, (cmd) => { cmd.Parameters.Add(new SqlParameter("@IsBlocked", isBlocked.HasValue ? isBlocked.Value : (object)DBNull.Value)); });

            if (result.Count == 0)
                return null;

            List<RPRouteByCode> rpRouteByCodes = new List<RPRouteByCode>();
            foreach (var rpRouteBCodesKvp in result)
            {
                string code = rpRouteBCodesKvp.Key;
                Dictionary<long, string> supplierZoneIds = supplierZoneIdsByCode.GetRecord(code);
                List<SupplierZoneDetail> supplierZoneDetails = null;
                Dictionary<long, string> supplierCodeMatchByZoneId = null;

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
            string tableName;
            bool explicitCustomerTableExists = ExplicitCustomerTableExists(input.Query.CustomerId, out tableName);

            string routingProductIdsFilter = string.Empty;
            string saleZoneIdsFilter = string.Empty;
            string sellingNumberPlanIdFilter = string.Empty;
            string customerIdFilter = string.Empty;

            string joinCustomerZoneDetail = string.Empty;
            string effectiveRateValue = string.Empty;
            string simulatedRoutingProductIdFilter = string.Empty;
            string leftJoin = explicitCustomerTableExists ? " LEFT " : string.Empty;

            bool? isBlocked = null;
            if (input.Query.RouteStatus.HasValue)
                isBlocked = input.Query.RouteStatus.Value == RouteStatus.Blocked ? true : false;

            if (input.Query.RoutingProductIds != null && input.Query.RoutingProductIds.Count > 0)
                routingProductIdsFilter = string.Format(" AND pr.RoutingProductId In({0})", string.Join(",", input.Query.RoutingProductIds));

            if (input.Query.SaleZoneIds != null && input.Query.SaleZoneIds.Count > 0)
                saleZoneIdsFilter = string.Format(" AND pr.SaleZoneId In({0})", string.Join(",", input.Query.SaleZoneIds));

            if (input.Query.SellingNumberPlanId.HasValue)
                sellingNumberPlanIdFilter = string.Format("AND pr.SellingNumberPlanId = {0}", input.Query.SellingNumberPlanId.Value);

            if (input.Query.CustomerId.HasValue)
            {
                customerIdFilter = string.Format(" AND CustomerId = {0} ", input.Query.CustomerId);
                effectiveRateValue = " ,czd.[EffectiveRateValue] ";

                if (input.Query.SimulatedRoutingProductId.HasValue)
                {
                    joinCustomerZoneDetail = " JOIN [dbo].[CustomerZoneDetail] as czd ON pr.SaleZoneId = czd.SaleZoneId ";
                    simulatedRoutingProductIdFilter = string.Format(" AND pr.RoutingProductId = {0} ", input.Query.SimulatedRoutingProductId.Value);
                }
                else
                {
                    joinCustomerZoneDetail = " JOIN [dbo].[CustomerZoneDetail] as czd ON pr.SaleZoneId = czd.SaleZoneId and pr.RoutingProductId = czd.RoutingProductId ";
                }
            }
            else
            {
                effectiveRateValue = " ,null as EffectiveRateValue ";
            }

            string query = query_GetFilteredRPRoutesByZone.ToString();
            query = query.Replace("#TABLENAME#", tableName);
            query = query.Replace("#LimitResult#", input.Query.LimitResult.ToString());
            query = query.Replace("#ROUTING_PRODUCT_IDS#", routingProductIdsFilter);
            query = query.Replace("#SIMULATE_ROUTING_PRODUCT_ID#", simulatedRoutingProductIdFilter);
            query = query.Replace("#SALE_ZONE_IDS#", saleZoneIdsFilter);
            query = query.Replace("#SELLING_NUMBER_PLAN_ID#", sellingNumberPlanIdFilter);

            query = query.Replace("#CUSTOMER_IDS#", customerIdFilter);
            query = query.Replace("#CUSTOMERZONEDETAILS#", joinCustomerZoneDetail);
            query = query.Replace("#EFFECTIVERATE#", effectiveRateValue);
            query = query.Replace("#LEFT#", leftJoin);

            return GetItemsText(query, RPRouteMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@IsBlocked", isBlocked.HasValue ? isBlocked.Value : (object)DBNull.Value));
            });
        }

        public IEnumerable<BaseRPRoute> GetRPRoutes(IEnumerable<RPZone> rpZones, int? customerId)
        {
            string tableName;
            bool explicitCustomerTableExists = ExplicitCustomerTableExists(customerId, out tableName);

            string query = query_GetRPRoutesByRPZones.Replace("#TABLENAME#", tableName);

            DataTable dtRPZones = BuildRPZoneTable(rpZones);
            return GetItemsText(query, RPRouteMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@RPZoneList", SqlDbType.Structured);
                dtPrm.TypeName = "RPZonesType";
                dtPrm.Value = dtRPZones;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public Dictionary<Guid, IEnumerable<RPRouteOption>> GetRouteOptions(int routingProductId, long saleZoneId, int? customerId)
        {
            string tableName;
            bool explicitCustomerTableExists = ExplicitCustomerTableExists(customerId, out tableName);

            string query = query_GetRouteOptions.Replace("#TABLENAME#", tableName);

            object serializedRouteOptions = ExecuteScalarText(query, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@RoutingProductId", routingProductId));
                cmd.Parameters.Add(new SqlParameter("@SaleZoneId", saleZoneId));
            });

            string serializedRouteOptionsAsString = serializedRouteOptions as String;
            if (string.IsNullOrEmpty(serializedRouteOptionsAsString))
                return null;

            return Helper.DeserializeOptionsByPolicy(serializedRouteOptionsAsString);
        }

        public Dictionary<int, RPRouteOptionSupplier> GetRouteOptionSuppliers(int routingProductId, long saleZoneId)
        {
            object serializedRouteOptionSuppliers = ExecuteScalarText(query_GetRouteOptionSuppliers, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@RoutingProductId", routingProductId));
                cmd.Parameters.Add(new SqlParameter("@SaleZoneId", saleZoneId));
            });

            string serializedRouteOptionSuppliersAsString = serializedRouteOptionSuppliers as String;
            if (string.IsNullOrEmpty(serializedRouteOptionSuppliersAsString))
                return null;

            return Helper.DeserializeOptionsDetailsBySupplier(serializedRouteOptionSuppliersAsString);
        }

        public void FinalizeProductRoute(Action<string> trackStep, int commandTimeoutInSeconds, int? maxDOP)
        {
            string maxDOPSyntax = maxDOP.HasValue ? string.Format(",MAXDOP={0}", maxDOP.Value) : "";
            string query;

            int totalNumberOfIndexesToBuild = RoutingCustomerInfo != null ? RoutingCustomerInfo.Count() * 2 + 2 : 2;
            int totalNumberOfIndexesDone = 0;

            trackStep("Starting create Clustered Index on ProductRoute table (RoutingProductId and SaleZoneId).");
            query = string.Format(query_CreateIX_ProductRoute_RoutingProductId, maxDOPSyntax).Replace("#TABLENAME#", "ProductRoute"); ;
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            trackStep("Finished create Clustered Index on ProductRoute table (RoutingProductId and SaleZoneId).");
            totalNumberOfIndexesDone++;
            trackStep(string.Format("Remaining Indexes to build: {0}", totalNumberOfIndexesToBuild - totalNumberOfIndexesDone));

            trackStep("Starting create Index on ProductRoute table (SaleZoneId).");
            query = string.Format(query_CreateIX_ProductRoute_SaleZoneId, maxDOPSyntax).Replace("#TABLENAME#", "ProductRoute");
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            trackStep("Finished create Index on ProductRoute table (SaleZoneId).");
            totalNumberOfIndexesDone++;
            trackStep(string.Format("Remaining Indexes to build: {0}", totalNumberOfIndexesToBuild - totalNumberOfIndexesDone));

            if (RoutingCustomerInfo != null)
            {
                foreach (var customer in RoutingCustomerInfo)
                {
                    string productRouteByCustomerTableName = string.Format("ProductRouteByCustomer_{0}", customer.CustomerId);

                    trackStep(string.Format("Starting create Clustered Index on ProductRouteByCustomer_{0} table (RoutingProductId and SaleZoneId).", customer.CustomerId));
                    query = string.Format(query_CreateIX_ProductRoute_RoutingProductId.Replace("#TABLENAME#", productRouteByCustomerTableName), maxDOPSyntax);
                    ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
                    trackStep(string.Format("Finished create Clustered Index on ProductRouteByCustomer_{0} table (RoutingProductId and SaleZoneId).", customer.CustomerId));
                    totalNumberOfIndexesDone++;
                    trackStep(string.Format("Remaining Indexes to build: {0}", totalNumberOfIndexesToBuild - totalNumberOfIndexesDone));

                    trackStep(string.Format("Starting create Index on ProductRouteByCustomer_{0} table (SaleZoneId).", customer.CustomerId));
                    query = string.Format(query_CreateIX_ProductRoute_SaleZoneId.Replace("#TABLENAME#", productRouteByCustomerTableName), maxDOPSyntax);
                    ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
                    trackStep(string.Format("Finished create Index on ProductRouteByCustomer_{0} table (SaleZoneId).", customer.CustomerId));
                    totalNumberOfIndexesDone++;
                    trackStep(string.Format("Remaining Indexes to build: {0}", totalNumberOfIndexesToBuild - totalNumberOfIndexesDone));
                }
            }
        }

        #endregion

        #region Private Methods

        private RPRouteByCode RPRouteByCodeMapper(IDataReader reader)
        {
            string saleZoneServices = reader["SaleZoneServices"] as string;

            return new RPRouteByCode()
            {
                Code = reader["Code"] as string,
                RoutingProductId = (int)reader["RoutingProductId"],
                SaleZoneId = (long)reader["SaleZoneId"],
                SaleZoneName = reader["SaleZoneName"] as string,
                SellingNumberPlanID = (int)reader["SellingNumberPlanID"],
                SaleZoneServiceIds = !string.IsNullOrEmpty(saleZoneServices) ? new HashSet<int>(saleZoneServices.Split(',').Select(itm => int.Parse(itm))) : null,
                IsBlocked = (bool)reader["IsBlocked"],
                ExecutedRuleId = (int)reader["ExecutedRuleId"],
                Rate = GetReaderValue<decimal?>(reader, "EffectiveRateValue")
            };
        }

        private BaseRPRoute RPRouteMapper(IDataReader reader)
        {
            string saleZoneServices = reader["SaleZoneServices"] as string;
            string optionsByPolicy = reader["OptionsByPolicy"] as string;

            return new BaseRPRoute()
            {
                RoutingProductId = (int)reader["RoutingProductId"],
                SaleZoneId = (long)reader["SaleZoneId"],
                SellingNumberPlanID = (int)reader["SellingNumberPlanID"],
                SaleZoneName = reader["Name"] as string,
                SaleZoneServiceIds = !string.IsNullOrEmpty(saleZoneServices) ? new HashSet<int>(saleZoneServices.Split(',').Select(itm => int.Parse(itm))) : null,
                IsBlocked = (bool)reader["IsBlocked"],
                ExecutedRuleId = (int)reader["ExecutedRuleId"],
                EffectiveRateValue = GetReaderValue<decimal?>(reader, "EffectiveRateValue"),
                RPOptionsByPolicy = Helper.DeserializeOptionsByPolicy(optionsByPolicy)
            };
        }

        private DataTable BuildRPZoneTable(IEnumerable<RPZone> rpZones)
        {
            DataTable dtRPZonesInfo = new DataTable();
            dtRPZonesInfo.Columns.Add("RoutingProductID", typeof(Int32));
            dtRPZonesInfo.Columns.Add("SaleZoneID", typeof(Int64));
            dtRPZonesInfo.BeginLoadData();
            foreach (var item in rpZones)
            {
                DataRow dr = dtRPZonesInfo.NewRow();
                dr["RoutingProductID"] = item.RoutingProductId;
                dr["SaleZoneID"] = item.SaleZoneId;
                dtRPZonesInfo.Rows.Add(dr);
            }
            dtRPZonesInfo.EndLoadData();
            return dtRPZonesInfo;
        }

        private bool ExplicitCustomerTableExists(int? customerId, out string tableName)
        {
            tableName = "ProductRoute";
            if (!customerId.HasValue)
                return false;

            string customerRouteTableName = string.Format("ProductRouteByCustomer_{0}", customerId.Value.ToString());

            string query = query_CheckProductRouteByCustomerTable.Replace("#CUSOMTERTABLENAME#", customerRouteTableName);
            bool result = ExecuteScalarText(query, null) != null;
            if (result)
                tableName = customerRouteTableName;

            return result;
        }

        #endregion

        #region private classes

        private class ProductRouteBulkInsert
        {
            public StreamForBulkInsert RPRoutesStream { get; set; }

            public Dictionary<int, StreamForBulkInsert> RPRoutesByCustomerStream { get; set; }
        }

        #endregion

        #region Queries

        private string query_CheckProductRouteByCustomerTable = @"SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
                                                                  WHERE TABLE_NAME = '#CUSOMTERTABLENAME#'";

        private StringBuilder query_GetFilteredRPRoutesByZone = new StringBuilder(@"SELECT TOP #LimitResult# pr.[RoutingProductId]
                                                                                    ,pr.[SaleZoneId]
                                                                                    ,sz.[Name] 
                                                                                    ,pr.[SellingNumberPlanID]
                                                                                    ,pr.[SaleZoneServices]
                                                                                    ,pr.[ExecutedRuleId]
                                                                                    ,pr.[OptionsByPolicy]
                                                                                    ,pr.[IsBlocked]
                                                                                    #EFFECTIVERATE#
                                                                                    FROM [dbo].#TABLENAME# as pr with(nolock)
                                                                                    JOIN [dbo].[SaleZone] as sz ON pr.SaleZoneId=sz.ID
                                                                                    #LEFT# #CUSTOMERZONEDETAILS# #CUSTOMER_IDS#
                                                                                    Where (@IsBlocked is null or IsBlocked = @IsBlocked) #ROUTING_PRODUCT_IDS# #SIMULATE_ROUTING_PRODUCT_ID# 
                                                                                           #SALE_ZONE_IDS# #SELLING_NUMBER_PLAN_ID#");

        private StringBuilder query_GetFilteredRPRoutesByCode = new StringBuilder(@"SELECT TOP #LimitResult# pr.[RoutingProductId], 
                                                                                    pr.[SaleZoneId], 
                                                                                    sz.Name as SaleZoneName, 
                                                                                    pr.[SellingNumberPlanID],
                                                                                    pr.[SaleZoneServices],
                                                                                    pr.[ExecutedRuleId], 
                                                                                    pr.[IsBlocked],
                                                                                    cszm.[Code], 
                                                                                    #EFFECTIVERATE#
                                                                                    into #routes
                                                                                    FROM [dbo].#TABLENAME# pr with(nolock)
                                                                                    JOIN [dbo].[CodeSaleZoneMatch] cszm on pr.SaleZoneId = cszm.SaleZoneID
                                                                                    JOIN [dbo].[SaleZone] as sz ON cszm.SaleZoneID = sz.ID
                                                                                    #LEFT# #CUSTOMERZONEDETAILS# #CUSTOMER_IDS#
                                                                                    Where (@IsBlocked is null or IsBlocked = @IsBlocked) #ROUTING_PRODUCT_IDS# #SIMULATE_ROUTING_PRODUCT_ID# 
                                                                                           #SALE_ZONE_IDS# #SELLING_NUMBER_PLAN_ID# #CodeFilter#

                                                                                    select distinct code 
                                                                                    into #distinctCodes 
                                                                                    from #routes

                                                                                    select cszm.SupplierZoneID, cszm.Code, cszm.CodeMatch
                                                                                    into #CodeSupplierZoneMatch
                                                                                    FROM [dbo].[CodeSupplierZoneMatch] cszm 
                                                                                    JOIN #distinctCodes codes on cszm.Code = codes.Code

                                                                                    select distinct supplierzoneid
                                                                                    into #distinctSupplierZoneIds
                                                                                    from #CodeSupplierZoneMatch

                                                                                    SELECT * FROM #routes

                                                                                    SELECT * FROM #CodeSupplierZoneMatch

                                                                                    SELECT szd.[SupplierId],
                                                                                    szd.[SupplierZoneId],
                                                                                    szd.[EffectiveRateValue],
                                                                                    szd.[SupplierServiceIds],
                                                                                    szd.[ExactSupplierServiceIds],
                                                                                    szd.[SupplierServiceWeight],
                                                                                    szd.[SupplierRateId],
                                                                                    szd.[SupplierRateEED],
                                                                                    szd.[VersionNumber],
                                                                                    szd.[DealId]
                                                                                    FROM [dbo].[SupplierZoneDetail] szd with(nolock)
                                                                                    JOIN #distinctSupplierZoneIds sz on sz.SupplierZoneID = szd.SupplierZoneId");

        private const string query_GetRouteOptions = @"SELECT [OptionsByPolicy]
                                                       FROM [dbo].#TABLENAME# with(nolock)
                                                       Where RoutingProductId = @RoutingProductId And SaleZoneId = @SaleZoneId";

        private const string query_GetRouteOptionSuppliers = @"SELECT [OptionsDetailsBySupplier]
                                                               FROM [dbo].[ProductRoute] 
                                                               Where RoutingProductId = @RoutingProductId And SaleZoneId = @SaleZoneId";

        private const string query_GetRPRoutesByRPZones = @"SELECT pr.[RoutingProductId],
                                                                pr.[SaleZoneId],
                                                                pr.[SellingNumberPlanID],
                                                                sz.[Name],
                                                                pr.[SaleZoneServices],
                                                                pr.[ExecutedRuleId],
                                                                pr.[OptionsByPolicy],
                                                                pr.[IsBlocked],
                                                                null as EffectiveRateValue
                                                            FROM [dbo].#TABLENAME# pr with(nolock) 
                                                            JOIN [dbo].[SaleZone] as sz ON pr.SaleZoneId = sz.ID
                                                            JOIN @RPZoneList as z ON z.RoutingProductId = pr.RoutingProductId AND z.SaleZoneId = pr.SaleZoneId";

        private const string query_CreateIX_ProductRoute_RoutingProductId = @"CREATE CLUSTERED INDEX [IX_#TABLENAME#_RoutingProductId_SaleZoneId] ON dbo.#TABLENAME#
                                                                              (
                                                                                 [RoutingProductId] ASC,
                                                                                 [SaleZoneId] ASC
                                                                              )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, 
                                                                                     ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";

        private const string query_CreateIX_ProductRoute_SaleZoneId = @"CREATE NONCLUSTERED INDEX [IX_#TABLENAME#_SaleZoneId] ON dbo.#TABLENAME#
                                                                        (
                                                                           [SaleZoneId] ASC
                                                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, 
                                                                               ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";

        #endregion
    }
}