﻿using System;
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
        readonly string[] productRouteColumns = { "RoutingProductId", "SaleZoneId", "SaleZoneServices", "ExecutedRuleId", "OptionsDetailsBySupplier", "OptionsByPolicy", "IsBlocked" };
        readonly string[] productRouteByCustomerColumns = { "RoutingProductId", "SaleZoneId", "SaleZoneServices", "ExecutedRuleId", "OptionsByPolicy", "IsBlocked" };


        const char RouteOptionSuppliersSeparator = '|';
        const char RouteOptionSupplierPropertiesSeparator = '~';
        const char SupplierZonesSeparator = '#';
        const string SupplierZonesSeparatorAsString = "#";
        const char SupplierZonePropertiesSeparator = '$';
        const char SupplierServicesSeparator = '@';
        const string SupplierServicesSeparatorAsString = "@";

        const char PolicyRouteOptionsSeparator = '|';
        const char PolicyRouteOptionPropertiesSeparator = '~';
        const char RouteOptionsSeparator = '#';
        const string RouteOptionsSeparatorAsString = "#";
        const char RouteOptionPropertiesSeparator = '$';

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
                    string optionsDetailsBySupplier = this.SerializeOptionsDetailsBySupplier(rproute.OptionsDetailsBySupplier);
                    string rpOptionsByPolicy = this.SerializeOptionsByPolicy(rproute.RPOptionsByPolicy);

                    streamForBulkInsert.RPRoutesStream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}", rproute.RoutingProductId, rproute.SaleZoneId, saleZoneServices, rproute.ExecutedRuleId,
                                                                                   optionsDetailsBySupplier, rpOptionsByPolicy, rproute.IsBlocked ? 1 : 0);
                }
            }
            else
            {
                var customerStream = streamForBulkInsert.RPRoutesByCustomerStream.GetRecord(record.CustomerId.Value);

                foreach (var rproute in record.RPRoutes)
                {
                    string saleZoneServices = (rproute.SaleZoneServiceIds != null && rproute.SaleZoneServiceIds.Count > 0) ? string.Join(",", rproute.SaleZoneServiceIds) : null;
                    string rpOptionsByPolicy = this.SerializeOptionsByPolicy(rproute.RPOptionsByPolicy);

                    customerStream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}", rproute.RoutingProductId, rproute.SaleZoneId, saleZoneServices, rproute.ExecutedRuleId,
                                                                                    rpOptionsByPolicy, rproute.IsBlocked ? 1 : 0);
                }
            }
        }

        public void ApplyProductRouteForDB(object preparedProductRoute)
        {
            var streamBulkInsertInfoList = preparedProductRoute as List<StreamBulkInsertInfo>;
            foreach (var streamBulkInsertInfo in streamBulkInsertInfoList)
            {
                InsertBulkToTable(streamBulkInsertInfo);
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

        public IEnumerable<RPRouteByCode> GetFilteredRPRoutesByCode(Vanrise.Entities.DataRetrievalInput<RPRouteQueryByCode> input)
        {
            Dictionary<string, List<RPRouteByCode>> result = new Dictionary<string, List<RPRouteByCode>>();
            Dictionary<long, SupplierZoneDetail> supplierZoneDetailsById = new Dictionary<long, SupplierZoneDetail>();
            Dictionary<string, Dictionary<long, string>> supplierZoneIdsByCode = new Dictionary<string, Dictionary<long, string>>();

            string tableName;
            bool explicitCustomerTableExists = ExplicitCustomerTableExists(input.Query.CustomerId, out tableName);

            string query = query_GetFilteredRPRoutesByCode.ToString();

            string codeFilter = string.Empty;
            if (!string.IsNullOrEmpty(input.Query.CodePrefix))
                codeFilter = string.Format(" AND cszm.Code like '{0}%' ", input.Query.CodePrefix);

            string routingProductIdsFilter = string.Empty;
            string saleZoneIdsFilter = string.Empty;
            string sellingNumberPlanIdFilter = string.Empty;
            string customerIdFilter = string.Empty;

            string joinCustomerZoneDetail = string.Empty;
            string effectiveRateValue = string.Empty;

            string leftJoin = explicitCustomerTableExists ? " LEFT " : string.Empty;
            string simulatedRoutingProductIdFilter = string.Empty;

            if (input.Query.RoutingProductIds != null && input.Query.RoutingProductIds.Count > 0)
                routingProductIdsFilter = string.Format(" AND pr.RoutingProductId In({0})", string.Join(",", input.Query.RoutingProductIds));

            if (input.Query.SaleZoneIds != null && input.Query.SaleZoneIds.Count > 0)
                saleZoneIdsFilter = string.Format(" AND pr.SaleZoneId In({0})", string.Join(",", input.Query.SaleZoneIds));

            if (input.Query.SellingNumberPlanId.HasValue)
                sellingNumberPlanIdFilter = string.Format("AND czd.SellingNumberPlanId = {0}", input.Query.SellingNumberPlanId.Value);

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
                if (input.Query.SellingNumberPlanId.HasValue)
                    joinCustomerZoneDetail = " JOIN [dbo].[CustomerZoneDetail] as czd ON pr.SaleZoneId = czd.SaleZoneId and pr.RoutingProductId = czd.RoutingProductId ";
                effectiveRateValue = "null as EffectiveRateValue ";
            }

            query = query.Replace("#TABLENAME#", tableName);
            query = query.Replace("#LimitResult#", input.Query.LimitResult.ToString());
            query = query.Replace("#ROUTING_PRODUCT_IDS#", routingProductIdsFilter);
            query = query.Replace("#SIMULATE_ROUTING_PRODUCT_ID#", simulatedRoutingProductIdFilter);
            query = query.Replace("#SALE_ZONE_IDS#", saleZoneIdsFilter);
            query = query.Replace("#SELLING_NUMBER_PLAN_ID#", sellingNumberPlanIdFilter);
            query = query.Replace("#CUSTOMER_IDS#", customerIdFilter);
            query = query.Replace("#CUSTOMERZONEDETAILS#", joinCustomerZoneDetail);
            query = query.Replace("#EFFECTIVERATE#", effectiveRateValue);
            query = query.Replace("#CodeFilter#", codeFilter);
            query = query.Replace("#LEFT#", leftJoin);

            bool? isBlocked = null;
            if (input.Query.RouteStatus.HasValue)
                isBlocked = input.Query.RouteStatus.Value == RouteStatus.Blocked ? true : false;


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

            string query = query_GetFilteredRPRoutesByZone.ToString();

            string routingProductIdsFilter = string.Empty;
            string saleZoneIdsFilter = string.Empty;
            string sellingNumberPlanIdFilter = string.Empty;
            string customerIdFilter = string.Empty;

            string joinCustomerZoneDetail = string.Empty;
            string effectiveRateValue = string.Empty;
            string leftJoin = explicitCustomerTableExists ? " LEFT " : string.Empty;
            string simulatedRoutingProductIdFilter = string.Empty;

            if (input.Query.RoutingProductIds != null && input.Query.RoutingProductIds.Count > 0)
                routingProductIdsFilter = string.Format(" AND pr.RoutingProductId In({0})", string.Join(",", input.Query.RoutingProductIds));

            if (input.Query.SaleZoneIds != null && input.Query.SaleZoneIds.Count > 0)
                saleZoneIdsFilter = string.Format(" AND pr.SaleZoneId In({0})", string.Join(",", input.Query.SaleZoneIds));

            if (input.Query.SellingNumberPlanId.HasValue)
                sellingNumberPlanIdFilter = string.Format("AND czd.SellingNumberPlanId = {0}", input.Query.SellingNumberPlanId.Value);

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
                if (input.Query.SellingNumberPlanId.HasValue)
                    joinCustomerZoneDetail = " JOIN [dbo].[CustomerZoneDetail] as czd ON pr.SaleZoneId = czd.SaleZoneId and pr.RoutingProductId = czd.RoutingProductId ";
                effectiveRateValue = " ,null as EffectiveRateValue ";
            }

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

            bool? isBlocked = null;
            if (input.Query.RouteStatus.HasValue)
                isBlocked = input.Query.RouteStatus.Value == RouteStatus.Blocked ? true : false;

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

            return this.DeserializeOptionsByPolicy(serializedRouteOptionsAsString);
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

            return this.DeserializeOptionsDetailsBySupplier(serializedRouteOptionSuppliersAsString);
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
                    string customerRouteTableName = string.Format("ProductRouteByCustomer_{0}", customer.CustomerId);

                    trackStep(string.Format("Starting create Clustered Index on ProductRouteByCustomer_{0} table (RoutingProductId and SaleZoneId).", customer.CustomerId));
                    query = string.Format(query_CreateIX_ProductRoute_RoutingProductId.Replace("#TABLENAME#", customerRouteTableName), maxDOPSyntax);
                    ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
                    trackStep(string.Format("Finished create Clustered Index on ProductRouteByCustomer_{0} table (RoutingProductId and SaleZoneId).", customer.CustomerId));
                    totalNumberOfIndexesDone++;
                    trackStep(string.Format("Remaining Indexes to build: {0}", totalNumberOfIndexesToBuild - totalNumberOfIndexesDone));

                    trackStep(string.Format("Starting create Index on ProductRouteByCustomer_{0} table (SaleZoneId).", customer.CustomerId));
                    query = string.Format(query_CreateIX_ProductRoute_SaleZoneId.Replace("#TABLENAME#", customerRouteTableName), maxDOPSyntax);
                    ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
                    trackStep(string.Format("Finished create Index on ProductRouteByCustomer_{0} table (SaleZoneId).", customer.CustomerId));
                    totalNumberOfIndexesDone++;
                    trackStep(string.Format("Remaining Indexes to build: {0}", totalNumberOfIndexesToBuild - totalNumberOfIndexesDone));
                }
            }
        }

        private bool ExplicitCustomerTableExists(int? customerId, out string tableName)
        {
            tableName = "ProductRoute";
            if (!customerId.HasValue)
                return false;

            string customerRouteTableName = string.Format("ProductRouteByCustomer_{0}", customerId.HasValue ? customerId.Value.ToString() : string.Empty);

            string query = query_CheckProductRouteByCustomerTable.Replace("#CUSOMTERTABLENAME#", customerRouteTableName);
            bool result = ExecuteScalarText(query, null) != null;
            if (result)
                tableName = customerRouteTableName;

            return result;
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
                SaleZoneName = reader["Name"] as string,
                SaleZoneServiceIds = !string.IsNullOrEmpty(saleZoneServices) ? new HashSet<int>(saleZoneServices.Split(',').Select(itm => int.Parse(itm))) : null,
                IsBlocked = (bool)reader["IsBlocked"],
                ExecutedRuleId = (int)reader["ExecutedRuleId"],
                EffectiveRateValue = GetReaderValue<decimal?>(reader, "EffectiveRateValue"),
                RPOptionsByPolicy = this.DeserializeOptionsByPolicy(optionsByPolicy)
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

        /// <summary>
        /// SupplierID~SZ1#SZ2#...#SZn~SupplierStatus~...~SupplierServiceWeight|SupplierID~SZ1#SZ2#...#SZn~SupplierStatus~...~SupplierServiceWeight
        /// SZ1 --> SupplierZoneId$SupplierRate$SupplierServiceID1@SupplierServiceID2@...@SupplierServiceID1SupplierServiceID1n$IsBlocked$SupplierRateId
        /// </summary>
        /// <param name="optionsDetailsBySupplier"></param>
        /// <returns></returns>
        private string SerializeOptionsDetailsBySupplier(Dictionary<int, RPRouteOptionSupplier> optionsDetailsBySupplier)
        {
            if (optionsDetailsBySupplier == null)
                return string.Empty;

            StringBuilder str = new StringBuilder();

            foreach (var routeOptionSupplier in optionsDetailsBySupplier.Values)
            {
                if (str.Length > 0)
                    str.Append(RouteOptionSuppliersSeparator);

                List<string> serializedSupplierZones = new List<string>();

                foreach (var supplierZone in routeOptionSupplier.SupplierZones)
                {
                    string exactSupplierServiceIdsAsString = supplierZone.ExactSupplierServiceIds != null ? string.Join(SupplierServicesSeparatorAsString, supplierZone.ExactSupplierServiceIds) : string.Empty;
                    string isBlocked = !supplierZone.IsBlocked ? string.Empty : "1";
                    string isForced = !supplierZone.IsForced ? string.Empty : "1";
                    serializedSupplierZones.Add(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}", SupplierZonePropertiesSeparator, supplierZone.SupplierZoneId, supplierZone.SupplierRate,
                                                    exactSupplierServiceIdsAsString, supplierZone.ExecutedRuleId, supplierZone.SupplierRateId, isBlocked, isForced));
                }

                string serializedSupplierZonesAsString = string.Join(SupplierZonesSeparatorAsString, serializedSupplierZones);

                str.AppendFormat("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}", RouteOptionSupplierPropertiesSeparator, routeOptionSupplier.SupplierId, serializedSupplierZonesAsString,
                                    routeOptionSupplier.NumberOfBlockedZones, routeOptionSupplier.NumberOfUnblockedZones, routeOptionSupplier.Percentage, routeOptionSupplier.SupplierServiceWeight);
            }
            return str.ToString();
        }

        public Dictionary<int, RPRouteOptionSupplier> DeserializeOptionsDetailsBySupplier(string serializedOptionsDetailsBySupplier)
        {
            if (string.IsNullOrEmpty(serializedOptionsDetailsBySupplier))
                return null;

            Dictionary<int, RPRouteOptionSupplier> optionsDetailsBySupplier = new Dictionary<int, RPRouteOptionSupplier>();

            string[] lines = serializedOptionsDetailsBySupplier.Split(RouteOptionSuppliersSeparator);

            foreach (var line in lines)
            {
                string[] parts = line.Split(RouteOptionSupplierPropertiesSeparator);

                var routeOptionSupplier = new RPRouteOptionSupplier();
                routeOptionSupplier.SupplierId = int.Parse(parts[0]);
                routeOptionSupplier.NumberOfBlockedZones = int.Parse(parts[2]);
                routeOptionSupplier.NumberOfUnblockedZones = int.Parse(parts[3]);
                routeOptionSupplier.Percentage = !string.IsNullOrEmpty(parts[4]) ? int.Parse(parts[4]) : default(int?);
                routeOptionSupplier.SupplierServiceWeight = int.Parse(parts[5]);

                string supplierZonesAsString = parts[1];
                if (!string.IsNullOrEmpty(supplierZonesAsString))
                {
                    string[] supplierZones = supplierZonesAsString.Split(SupplierZonesSeparator);
                    routeOptionSupplier.SupplierZones = new List<RPRouteOptionSupplierZone>();

                    foreach (var supplierZone in supplierZones)
                    {
                        string[] supplierZoneParts = supplierZone.Split(SupplierZonePropertiesSeparator);

                        var routeOptionSupplierZone = new RPRouteOptionSupplierZone();
                        //routeOptionSupplierZone.SupplierCode = supplierZoneParts[0];
                        routeOptionSupplierZone.SupplierZoneId = long.Parse(supplierZoneParts[0]);
                        routeOptionSupplierZone.SupplierRate = decimal.Parse(supplierZoneParts[1]);
                        routeOptionSupplierZone.ExecutedRuleId = !string.IsNullOrEmpty(supplierZoneParts[3]) ? int.Parse(supplierZoneParts[3]) : default(int?);
                        routeOptionSupplierZone.SupplierRateId = !string.IsNullOrEmpty(supplierZoneParts[4]) ? long.Parse(supplierZoneParts[4]) : default(long?);

                        if (!string.IsNullOrEmpty(supplierZoneParts[2]))
                            routeOptionSupplierZone.ExactSupplierServiceIds = new HashSet<int>(supplierZoneParts[2].Split(SupplierServicesSeparator).Select(itm => int.Parse(itm)));

                        string isBlockedAsString = supplierZoneParts[5];
                        if (!string.IsNullOrEmpty(isBlockedAsString))
                        {
                            int isBlocked;
                            if (int.TryParse(isBlockedAsString, out isBlocked))
                                routeOptionSupplierZone.IsBlocked = isBlocked > 0;
                        }

                        string isFrocedAsString = supplierZoneParts[6];
                        if (!string.IsNullOrEmpty(isFrocedAsString))
                        {
                            int isForced;
                            if (int.TryParse(isFrocedAsString, out isForced))
                                routeOptionSupplierZone.IsForced = isForced > 0;
                        }

                        routeOptionSupplier.SupplierZones.Add(routeOptionSupplierZone);
                    }
                }

                optionsDetailsBySupplier.Add(routeOptionSupplier.SupplierId, routeOptionSupplier);
            }

            return optionsDetailsBySupplier;
        }

        /// <summary>
        /// PolicyID~S1#S2#...#Sn|PolicyID~S1#S2#...#Sn
        /// S1 --> SupplierId$SupplierRate$...$SupplierZoneMatchHasClosedRate
        /// </summary>
        /// <param name="optionsByPolicy"></param>
        /// <returns></returns>
        private string SerializeOptionsByPolicy(Dictionary<Guid, IEnumerable<RPRouteOption>> optionsByPolicy)
        {
            if (optionsByPolicy == null)
                return string.Empty;

            StringBuilder str = new StringBuilder();
            foreach (var kvp in optionsByPolicy)
            {
                Guid policyId = kvp.Key;
                IEnumerable<RPRouteOption> routeOptions = kvp.Value;

                if (str.Length > 0)
                    str.Append(PolicyRouteOptionsSeparator);

                List<string> serializedRouteOptions = new List<string>();

                foreach (var routeOption in routeOptions)
                {
                    string supplierZoneMatchHasClosedRate = !routeOption.SupplierZoneMatchHasClosedRate ? string.Empty : "1";
                    string isForced = !routeOption.IsForced ? string.Empty : "1";
                    string supplierZoneId = routeOption.SupplierZoneId.HasValue ? routeOption.SupplierZoneId.ToString() : string.Empty;
                    string supplierServicesIds = routeOption.SupplierServicesIds != null ? string.Join(",", routeOption.SupplierServicesIds) : string.Empty;

                    serializedRouteOptions.Add(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}", RouteOptionPropertiesSeparator, routeOption.SupplierId, routeOption.SupplierRate, routeOption.Percentage,
                                        routeOption.OptionWeight, routeOption.SaleZoneId, routeOption.SupplierServiceWeight, supplierZoneMatchHasClosedRate, Convert.ToInt32(routeOption.SupplierStatus), isForced, supplierZoneId, supplierServicesIds));
                }

                string serializadedRouteOptionsAsString = string.Join(RouteOptionsSeparatorAsString, serializedRouteOptions);

                str.AppendFormat("{1}{0}{2}", PolicyRouteOptionPropertiesSeparator, policyId, serializadedRouteOptionsAsString);
            }
            return str.ToString();
        }

        private Dictionary<Guid, IEnumerable<RPRouteOption>> DeserializeOptionsByPolicy(string serializedOptionsDetailsBySupplier)
        {
            if (string.IsNullOrEmpty(serializedOptionsDetailsBySupplier))
                return null;

            Dictionary<Guid, IEnumerable<RPRouteOption>> optionsByPolicy = new Dictionary<Guid, IEnumerable<RPRouteOption>>();

            string[] lines = serializedOptionsDetailsBySupplier.Split(PolicyRouteOptionsSeparator);

            foreach (var line in lines)
            {
                string[] parts = line.Split(PolicyRouteOptionPropertiesSeparator);

                Guid policyId = Guid.Parse(parts[0]);
                string routeOptionsAsString = parts[1];

                if (!string.IsNullOrEmpty(routeOptionsAsString))
                {
                    List<RPRouteOption> routeOptionsList = new List<RPRouteOption>();
                    string[] routeOptions = routeOptionsAsString.Split(RouteOptionsSeparator);

                    foreach (var routeOption in routeOptions)
                    {
                        string[] routeOptionParts = routeOption.Split(RouteOptionPropertiesSeparator);

                        var rpRouteOption = new RPRouteOption();
                        rpRouteOption.SupplierId = int.Parse(routeOptionParts[0]);
                        rpRouteOption.SupplierRate = decimal.Parse(routeOptionParts[1]);
                        rpRouteOption.Percentage = !string.IsNullOrEmpty(routeOptionParts[2]) ? int.Parse(routeOptionParts[2]) : default(int?);
                        rpRouteOption.OptionWeight = decimal.Parse(routeOptionParts[3]);
                        rpRouteOption.SaleZoneId = long.Parse(routeOptionParts[4]);
                        rpRouteOption.SupplierServiceWeight = int.Parse(routeOptionParts[5]);
                        rpRouteOption.SupplierStatus = (SupplierStatus)int.Parse(routeOptionParts[7]);

                        string supplierZoneMatchHasClosedRateAsString = routeOptionParts[6];
                        if (!string.IsNullOrEmpty(supplierZoneMatchHasClosedRateAsString))
                        {
                            int supplierZoneMatchHasClosedRate;
                            if (int.TryParse(supplierZoneMatchHasClosedRateAsString, out supplierZoneMatchHasClosedRate))
                                rpRouteOption.SupplierZoneMatchHasClosedRate = supplierZoneMatchHasClosedRate > 0;
                        }

                        string isFrocedAsString = routeOptionParts[8];
                        if (!string.IsNullOrEmpty(isFrocedAsString))
                        {
                            int isForced;
                            if (int.TryParse(isFrocedAsString, out isForced))
                                rpRouteOption.IsForced = isForced > 0;
                        }

                        string supplierZoneIdAsString = routeOptionParts[9];
                        if (!string.IsNullOrEmpty(supplierZoneIdAsString))
                            rpRouteOption.SupplierZoneId = long.Parse(supplierZoneIdAsString);

                        string supplierServicesAsString = routeOptionParts[10];
                        if (!string.IsNullOrEmpty(supplierServicesAsString))
                        {
                            var supplierServicesIds = supplierServicesAsString.Split(',');
                            rpRouteOption.SupplierServicesIds = new HashSet<int>();
                            foreach (var supplierServiceId in supplierServicesIds)
                            {
                                rpRouteOption.SupplierServicesIds.Add(int.Parse(supplierServiceId));
                            }
                        }

                        routeOptionsList.Add(rpRouteOption);
                    }

                    optionsByPolicy.Add(policyId, routeOptionsList);
                }
            }

            return optionsByPolicy;
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
                                                                                    ,pr.[SaleZoneServices]
                                                                                    #EFFECTIVERATE#
                                                                                    ,pr.[ExecutedRuleId]
                                                                                    ,pr.[OptionsByPolicy]
                                                                                    ,pr.[IsBlocked]
                                                                                    FROM [dbo].#TABLENAME# as pr with(nolock)
                                                                                    JOIN [dbo].[SaleZone] as sz ON pr.SaleZoneId=sz.ID
                                                                                    #LEFT# #CUSTOMERZONEDETAILS# #CUSTOMER_IDS#
                                                                                    Where (@IsBlocked is null or IsBlocked = @IsBlocked) #ROUTING_PRODUCT_IDS# #SIMULATE_ROUTING_PRODUCT_ID# #SALE_ZONE_IDS# #SELLING_NUMBER_PLAN_ID#");

        private StringBuilder query_GetFilteredRPRoutesByCode = new StringBuilder(@"SELECT TOP #LimitResult# pr.[RoutingProductId], 
                                                                                    pr.[SaleZoneId], 
                                                                                    sz.Name as SaleZoneName, 
                                                                                    cszm.SellingNumberPlanID,
                                                                                    [SaleZoneServices],
                                                                                    [ExecutedRuleId], 
                                                                                    cszm.Code, 
                                                                                    pr.IsBlocked,
                                                                                    #EFFECTIVERATE#
                                                                                    into #routes
                                                                                    FROM [dbo].#TABLENAME# pr with(nolock)
                                                                                    JOIN [dbo].[CodeSaleZoneMatch] cszm on pr.SaleZoneId = cszm.SaleZoneID
                                                                                    JOIN [dbo].[SaleZone] as sz ON cszm.SaleZoneID = sz.ID
                                                                                    #LEFT# #CUSTOMERZONEDETAILS# #CUSTOMER_IDS#
                                                                                    Where (@IsBlocked is null or IsBlocked = @IsBlocked) #ROUTING_PRODUCT_IDS# #SIMULATE_ROUTING_PRODUCT_ID# #SALE_ZONE_IDS# #SELLING_NUMBER_PLAN_ID# #CodeFilter#

                                                                                    select distinct code into #distinctCodes from #routes

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
                                                                sz.[Name],
                                                                pr.[SaleZoneServices],
                                                                pr.[ExecutedRuleId],
                                                                pr.[OptionsByPolicy],
                                                                pr.[IsBlocked],
                                                                null as EffectiveRateValue
                                                            FROM [dbo].#TABLENAME# pr with(nolock) JOIN [dbo].[SaleZone] as sz ON pr.SaleZoneId=sz.ID
                                                            JOIN @RPZoneList z
                                                            ON z.RoutingProductId = pr.RoutingProductId AND z.SaleZoneId = pr.SaleZoneId";

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
