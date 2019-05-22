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
    public class CustomerRouteDataManager : RoutingDataManager, ICustomerRouteDataManager
    {
        readonly string[] columns = { "CustomerId", "Code", "SaleZoneId", "IsBlocked", "ExecutedRuleId", "SupplierIds", "RouteOptions" };

        public int ParentWFRuntimeProcessId { get; set; }

        public long ParentBPInstanceId { get; set; }

        public Vanrise.BusinessProcess.IBPContext BPContext { set; get; }

        static CustomerRouteDataManager()
        {
            RouteOption dummy = new RouteOption();
        }

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(Entities.CustomerRoute record, object dbApplyStream)
        {
            string supplierIds = Helper.GetConcatenatedSupplierIds(record);
            string serializedOptions = record.Options != null ? Helper.SerializeOptions(record.Options) : null;

            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;

            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}", record.CustomerId, record.Code, record.SaleZoneId, record.IsBlocked ? 1 : 0, record.ExecutedRuleId, supplierIds, serializedOptions);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[CustomerRoute]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns,
            };
        }

        public void ApplyCustomerRouteForDB(object preparedCustomerRoute)
        {
            var streamInfo = preparedCustomerRoute as StreamBulkInsertInfo;
            DateTime start = DateTime.Now;
            InsertBulkToTable(streamInfo);
            if (this.BPContext != null)
            {
                this.BPContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Routes saved to database in {1}", streamInfo.Stream.RecordCount, (DateTime.Now - start));
            }
        }

        public IEnumerable<Entities.CustomerRoute> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<Entities.CustomerRouteQuery> input)
        {
            StringBuilder queryBuilder = new StringBuilder(query_GetCustomerRoutes);
            queryBuilder.Replace("#LimitResult#", string.Format("Top {0}", input.Query.LimitResult.ToString()));

            bool? isBlocked = null;
            if (input.Query.RouteStatus.HasValue)
                isBlocked = input.Query.RouteStatus.Value == RouteStatus.Blocked ? true : false;

            string customerIdsFilter = string.Empty;
            if (input.Query.CustomerIds != null && input.Query.CustomerIds.Count > 0)
                customerIdsFilter = string.Format("AND cr.CustomerId In ({0})", string.Join(",", input.Query.CustomerIds));

            string saleZoneIdsFilter = string.Empty;
            if (input.Query.SaleZoneIds != null && input.Query.SaleZoneIds.Count > 0)
                saleZoneIdsFilter = string.Format("AND cr.SaleZoneId In ({0})", string.Join(",", input.Query.SaleZoneIds));

            string sellingNumberPlanId = string.Empty;
            if (input.Query.SellingNumberPlanId.HasValue)
                sellingNumberPlanId = string.Format("AND czd.SellingNumberPlanId = {0}", input.Query.SellingNumberPlanId.Value);

            string supplierIdsFilter = string.Empty;
            var supplierIds = input.Query.SupplierIds;
            if (supplierIds != null && supplierIds.Count > 0)
            {
                supplierIdsFilter = " AND (cr.SupplierIds like '%$";
                supplierIdsFilter = supplierIdsFilter + string.Join("$%' OR cr.SupplierIds like '%$", supplierIds);
                supplierIdsFilter = supplierIdsFilter + "$%')";
            }

            queryBuilder.Replace("#FILTER#", string.Format("Where (@Code is null or cr.Code like @Code) and (@IsBlocked is null or IsBlocked = @IsBlocked) {0} {1} {2} {3}", customerIdsFilter, saleZoneIdsFilter, sellingNumberPlanId, supplierIdsFilter));

            IEnumerable<Entities.CustomerRoute> customerRoutes = GetItemsText(queryBuilder.ToString(), CustomerRouteMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@Code", !string.IsNullOrEmpty(input.Query.Code) ? string.Format("{0}%", input.Query.Code) : (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@IsBlocked", isBlocked.HasValue ? isBlocked.Value : (object)DBNull.Value));
            });

            CompleteSupplierData(customerRoutes);
            return customerRoutes;
        }

        public void LoadRoutes(int? customerId, string codePrefix, Func<bool> shouldStop, Action<CustomerRoute> onRouteLoaded)
        {
            StringBuilder queryBuilder = new StringBuilder(query_GetCustomerRoutes);
            queryBuilder.Replace("#LimitResult#", string.Empty);

            List<string> filters = new List<string>();
            if (customerId.HasValue)
                filters.Add("cr.[CustomerID] = @CustomerID");
            if (!String.IsNullOrEmpty(codePrefix))
                filters.Add(String.Format("cr.[Code] like '{0}%'", codePrefix));

            if (filters.Count > 0)
                queryBuilder.Replace("#FILTER#", String.Format("WHERE {0}", String.Join(" AND ", filters)));
            else
                queryBuilder.Replace("#FILTER#", "");

            SupplierZoneDetailsDataManager supplierZoneDetailsDataManager = new SupplierZoneDetailsDataManager();
            supplierZoneDetailsDataManager.RoutingDatabase = RoutingDatabase;

            ExecuteReaderText(queryBuilder.ToString(), (reader) =>
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
                },
                (cmd) =>
                {
                    if (customerId.HasValue)
                        cmd.Parameters.Add(new SqlParameter("@CustomerID", customerId.Value));
                });
        }

        public List<CustomerRoute> GetCustomerRoutesAfterVersionNb(int versionNb)
        {
            StringBuilder queryBuilder = new StringBuilder(query_GetCustomerRoutes);
            queryBuilder.Replace("#LimitResult#", string.Empty);
            queryBuilder.Replace("#FILTER#", string.Format("Where mcr.VersionNumber > {0}", versionNb));

            List<CustomerRoute> customerRoutes = GetItemsText(queryBuilder.ToString(), CustomerRouteMapper, (cmd) => { });
            CompleteSupplierData(customerRoutes);
            return customerRoutes;
        }

        public List<CustomerRouteData> GetAffectedCustomerRoutes(List<AffectedRoutes> affectedRoutesList, List<AffectedRouteOptions> affectedRouteOptionsList, long partialRoutesNumberLimit, out bool maximumExceeded)
        {
            HashSet<string> addedCustomerRouteDefinitions = new HashSet<string>();
            List<CustomerRouteData> customerRouteDataList = new List<CustomerRouteData>();
            maximumExceeded = false;
            long addedItems = 0;

            List<string> routesConditions = BuildAffectedRoutes(affectedRoutesList);
            if (routesConditions != null)
            {
                string query1 = query_GetAffectedCustomerRoutes.Replace("#AFFECTEDROUTES#", string.Join(" or ", routesConditions)).Replace("#CODESUPPLIERZONEMATCH#", string.Empty);
                addedItems = ExecuteGetAffectedCustomerRoutesQuery(query1, addedCustomerRouteDefinitions, customerRouteDataList, partialRoutesNumberLimit, addedItems, out maximumExceeded);
            }

            if (maximumExceeded)
                return null;

            bool hasSupplierZoneCriteria;
            List<string> routeOptionsConditions = BuildAffectedRouteOptions(affectedRouteOptionsList, out hasSupplierZoneCriteria);
            if (routeOptionsConditions != null)
            {
                string query2;
                if (!hasSupplierZoneCriteria)
                    query2 = query_GetAffectedCustomerRoutes.Replace("#CODESUPPLIERZONEMATCH#", string.Empty);
                else
                    query2 = query_GetAffectedCustomerRoutes.Replace("#CODESUPPLIERZONEMATCH#", "JOIN [dbo].[CodeSupplierZoneMatch] as cszm ON cr.Code = cszm.Code");

                query2 = query2.Replace("#AFFECTEDROUTES#", string.Join(" or ", routeOptionsConditions));

                addedItems = ExecuteGetAffectedCustomerRoutesQuery(query2, addedCustomerRouteDefinitions, customerRouteDataList, partialRoutesNumberLimit, addedItems, out maximumExceeded);
            }

            if (maximumExceeded)
                return null;

            return customerRouteDataList.Count > 0 ? customerRouteDataList : null;
        }

        public long GetTotalCount()
        {
            return (long)ExecuteScalarText(@"IF OBJECT_ID('[dbo].[CustomerRoute]', N'U') IS NOT NULL 
                                Begin
                                    SELECT CAST(p.rows AS bigint)
                                    FROM sys.tables AS tbl
                                    INNER JOIN sys.indexes AS idx ON idx.object_id = tbl.object_id and idx.index_id < 2
                                    INNER JOIN sys.partitions AS p ON p.object_id = CAST(tbl.object_id AS int) and p.index_id = idx.index_id
                                    WHERE ((tbl.name=N'CustomerRoute' AND SCHEMA_NAME(tbl.schema_id)=N'dbo'))
                                End", (cmd) => { });
        }

        public void UpdateCustomerRoutes(List<CustomerRouteData> customerRouteDataList)
        {
            DataTable dtCustomerRoutes = BuildCustomerRouteTable(customerRouteDataList);
            ExecuteNonQueryText(query_UpdateCustomerRoutes, (cmd) =>
            {
                var dtPrm = new SqlParameter("@Routes", SqlDbType.Structured);
                dtPrm.TypeName = "CustomerRouteType";
                dtPrm.Value = dtCustomerRoutes;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public void FinalizeCurstomerRoute(Action<string> trackStep, int commandTimeoutInSeconds, int? maxDOP)
        {
            string maxDOPSyntax = maxDOP.HasValue ? string.Format(",MAXDOP={0}", maxDOP.Value) : "";
            string query;

            //trackStep("Starting create Index on CustomerRoute table (CustomerId).");
            //query = string.Format(query_CreateIX_CustomerRoute_CustomerId, maxDOPSyntax);
            //ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            //trackStep("Finished create Index on CustomerRoute table (CustomerId).");

            trackStep("Starting create Index on CustomerRoute table (Code).");
            query = string.Format(query_CreateIX_CustomerRoute_Code, maxDOPSyntax);
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            trackStep("Finished create Index on CustomerRoute table (Code).");

            trackStep("Starting create Index on CustomerRoute table (SaleZoneId).");
            query = string.Format(query_CreateIX_CustomerRoute_SaleZone, maxDOPSyntax);
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            trackStep("Finished create Index on CustomerRoute table (SaleZoneId).");

            //trackStep("Starting create Index on CustomerRoute table (VersionNumber).");
            //query = string.Format(query_CreateIX_CustomerRoute_VersionNumber, maxDOPSyntax);
            //ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            //trackStep("Finished create Index on CustomerRoute table (VersionNumber).");

            trackStep("Starting create CLUSTERED Index on CustomerRoute table (CustomerId and Code).");
            query = string.Format(query_CreateIX_CustomerRoute_CustomerId_Code, maxDOPSyntax);
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            trackStep("Finished create CLUSTERED on CustomerRoute table (CustomerId and Code).");
        }

        #endregion

        #region Private Methods

        internal void CompleteSupplierData(IEnumerable<ICompleteSupplierDataRoute> routes)
        {
            if (routes == null || routes.Count() == 0)
                return;

            HashSet<long> supplierZoneIds = new HashSet<long>();
            foreach (var route in routes)
            {
                var routeOptionsList = route.GetAvailableRouteOptionsList();

                if (routeOptionsList == null || routeOptionsList.Count == 0)
                    continue;

                foreach (var routeOptions in routeOptionsList)
                {
                    if (routeOptions == null || routeOptions.Count == 0)
                        continue;

                    foreach (RouteOption routeOption in routeOptions)
                    {
                        supplierZoneIds.Add(routeOption.SupplierZoneId);
                        if (routeOption.Backups == null || routeOption.Backups.Count == 0)
                            continue;

                        foreach (RouteBackupOption backup in routeOption.Backups)
                            supplierZoneIds.Add(backup.SupplierZoneId);
                    }
                }
            }

            if (supplierZoneIds.Count > 0)
            {
                SupplierZoneDetailsDataManager supplierZoneDetailsDataManager = new SupplierZoneDetailsDataManager();
                supplierZoneDetailsDataManager.RoutingDatabase = this.RoutingDatabase;

                Dictionary<long, SupplierZoneDetail> supplierZoneDetails = supplierZoneDetailsDataManager.GetFilteredSupplierZoneDetailsBySupplierZone(supplierZoneIds).ToDictionary(itm => itm.SupplierZoneId, itm => itm);

                foreach (var route in routes)
                {
                    var routeOptionsList = route.GetAvailableRouteOptionsList();

                    if (routeOptionsList == null || routeOptionsList.Count == 0)
                        continue;

                    foreach (var routeOptions in routeOptionsList)
                    {
                        if (routeOptions == null || routeOptions.Count == 0)
                            continue;

                        foreach (RouteOption routeOption in routeOptions)
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
        }

        private long ExecuteGetAffectedCustomerRoutesQuery(string query, HashSet<string> addedCustomerRouteDefinitions, List<CustomerRouteData> customerRouteDataList, long partialRoutesNumberLimit, long addedItems, out bool maximumExceeded)
        {
            maximumExceeded = false;
            ExecuteReaderText(query, (reader) =>
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
            }, (cmd) => { });

            if (addedItems > partialRoutesNumberLimit)
                maximumExceeded = true;

            return addedItems;
        }

        private List<string> BuildAffectedRoutes(List<AffectedRoutes> affectedRoutesList)
        {
            if (affectedRoutesList == null || affectedRoutesList.Count == 0)
                return null;

            List<string> conditions = new List<string>();
            foreach (AffectedRoutes affectedRoutes in affectedRoutesList)
            {
                List<string> subConditions = new List<string>();

                if (affectedRoutes.CustomerIds != null && affectedRoutes.CustomerIds.Count() > 0)
                {
                    subConditions.Add(string.Format("cr.CustomerID in ({0})", string.Join<int>(",", affectedRoutes.CustomerIds)));
                }

                if (affectedRoutes.ZoneIds != null && affectedRoutes.ZoneIds.Count() > 0)
                {
                    subConditions.Add(string.Format("cr.SaleZoneID in ({0})", string.Join<long>(",", affectedRoutes.ZoneIds)));
                }

                if (affectedRoutes.Codes != null && affectedRoutes.Codes.Count() > 0)
                {
                    List<string> codesConditions = new List<string>();

                    List<string> codesWithoutSubCodes = new List<string>();
                    List<string> codesWithSubCodes = new List<string>();
                    foreach (CodeCriteria codeCriteria in affectedRoutes.Codes)
                    {
                        if (codeCriteria.WithSubCodes)
                            codesWithSubCodes.Add(codeCriteria.Code);
                        else
                            codesWithoutSubCodes.Add(codeCriteria.Code);
                    }

                    if (codesWithoutSubCodes.Count > 0)
                        codesConditions.Add(string.Format("cr.Code in ('{0}')", string.Join<string>("','", codesWithoutSubCodes)));

                    if (codesWithSubCodes.Count > 0)
                    {
                        foreach (string code in codesWithSubCodes)
                        {
                            codesConditions.Add(string.Format("cr.Code like ('{0}%')", code));
                        }
                    }

                    subConditions.Add(string.Format("({0})", string.Join<string>(" or ", codesConditions)));
                }

                RoutingExcludedDestinationData routingExcludedDestinationData = affectedRoutes.RoutingExcludedDestinationData;
                if (routingExcludedDestinationData != null)
                {
                    if (routingExcludedDestinationData.ExcludedCodes != null && routingExcludedDestinationData.ExcludedCodes.Count > 0)
                        subConditions.Add(string.Format("cr.Code not in ('{0}')", string.Join<string>("','", routingExcludedDestinationData.ExcludedCodes)));

                    if (routingExcludedDestinationData.CodeRanges != null && routingExcludedDestinationData.CodeRanges.Count > 0)
                    {
                        foreach (var excludedDestinationData in routingExcludedDestinationData.CodeRanges)
                            subConditions.Add(string.Format("(Len(cr.Code) <> Len('{0}') or cr.Code < '{0}' or cr.Code > '{1}')", excludedDestinationData.FromCode, excludedDestinationData.ToCode));
                    }
                }

                conditions.Add(string.Format("({0})", string.Join<string>(" and ", subConditions)));
            }
            return conditions;
        }

        private List<string> BuildAffectedRouteOptions(List<AffectedRouteOptions> affectedRouteOptionsList, out bool hasSupplierZoneCriteria)
        {
            hasSupplierZoneCriteria = false;

            if (affectedRouteOptionsList == null || affectedRouteOptionsList.Count == 0)
                return null;

            List<string> conditions = new List<string>();
            foreach (AffectedRouteOptions affectedRouteOptions in affectedRouteOptionsList)
            {
                List<string> subConditions = new List<string>();

                if (affectedRouteOptions.SupplierWithZones != null && affectedRouteOptions.SupplierWithZones.Count() > 0)
                {
                    hasSupplierZoneCriteria = true;
                    List<string> supplierWithZonesConditions = new List<string>();
                    HashSet<int> supplierIds = new HashSet<int>();

                    foreach (SupplierWithZones supplierWithZones in affectedRouteOptions.SupplierWithZones)
                    {
                        if (supplierWithZones.SupplierZoneIds != null && supplierWithZones.SupplierZoneIds.Count > 0)
                            supplierWithZonesConditions.Add(string.Format("(cszm.SupplierId = {0} and cszm.SupplierZoneID in ({1}))", supplierWithZones.SupplierId, string.Join<long>(",", supplierWithZones.SupplierZoneIds)));
                        else
                            supplierIds.Add(supplierWithZones.SupplierId);
                    }

                    if (supplierIds.Count > 0)
                        supplierWithZonesConditions.Add(string.Format("cszm.SupplierId in ({0})", string.Join<int>(",", supplierIds)));

                    subConditions.Add(string.Format("({0})", string.Join<string>(" or ", supplierWithZonesConditions)));
                }

                if (affectedRouteOptions.CustomerIds != null && affectedRouteOptions.CustomerIds.Count() > 0)
                {
                    subConditions.Add(string.Format("cr.CustomerID in ({0})", string.Join<int>(",", affectedRouteOptions.CustomerIds)));
                }

                if (affectedRouteOptions.ZoneIds != null && affectedRouteOptions.ZoneIds.Count() > 0)
                {
                    subConditions.Add(string.Format("cr.SaleZoneID in ({0})", string.Join<long>(",", affectedRouteOptions.ZoneIds)));
                }

                if (affectedRouteOptions.Codes != null && affectedRouteOptions.Codes.Count() > 0)
                {
                    List<string> codesConditions = new List<string>();

                    List<string> codesWithoutSubCodes = new List<string>();
                    List<string> codesWithSubCodes = new List<string>();
                    foreach (CodeCriteria codeCriteria in affectedRouteOptions.Codes)
                    {
                        if (codeCriteria.WithSubCodes)
                            codesConditions.Add(string.Format("cr.Code like ('{0}%')", codeCriteria.Code));
                        else
                            codesWithoutSubCodes.Add(codeCriteria.Code);
                    }

                    if (codesWithoutSubCodes.Count > 0)
                        codesConditions.Add(string.Format("cr.Code in ('{0}')", string.Join<string>("','", codesWithoutSubCodes)));

                    subConditions.Add(string.Format("({0})", string.Join<string>(" or ", codesConditions)));
                }

                RoutingExcludedDestinationData routingExcludedDestinationData = affectedRouteOptions.RoutingExcludedDestinationData;
                if (routingExcludedDestinationData != null)
                {
                    if (routingExcludedDestinationData.ExcludedCodes != null && routingExcludedDestinationData.ExcludedCodes.Count > 0)
                        subConditions.Add(string.Format("cr.Code not in ('{0}')", string.Join<string>("','", routingExcludedDestinationData.ExcludedCodes)));

                    if (routingExcludedDestinationData.CodeRanges != null && routingExcludedDestinationData.CodeRanges.Count > 0)
                    {
                        foreach (var excludedDestinationData in routingExcludedDestinationData.CodeRanges)
                            subConditions.Add(string.Format("(Len(cr.Code) <> Len('{0}') or cr.Code < '{0}' or cr.Code > '{1}')", excludedDestinationData.FromCode, excludedDestinationData.ToCode));
                    }
                }

                conditions.Add(string.Format("({0})", string.Join<string>(" and ", subConditions)));
            }
            return conditions;
        }

        private DataTable BuildCustomerRouteTable(List<CustomerRouteData> customerRouteDataList)
        {
            DataTable dtCustomerRoutes = new DataTable();
            dtCustomerRoutes.Columns.Add("CustomerId", typeof(int));
            dtCustomerRoutes.Columns.Add("Code", typeof(string));
            dtCustomerRoutes.Columns.Add("SaleZoneId", typeof(long));
            dtCustomerRoutes.Columns.Add("IsBlocked", typeof(int));
            dtCustomerRoutes.Columns.Add("ExecutedRuleId", typeof(int));
            dtCustomerRoutes.Columns.Add("SupplierIds", typeof(string));
            dtCustomerRoutes.Columns.Add("RouteOptions", typeof(string));
            dtCustomerRoutes.Columns.Add("VersionNumber", typeof(int));
            dtCustomerRoutes.BeginLoadData();
            foreach (var customerRoute in customerRouteDataList)
            {
                DataRow dr = dtCustomerRoutes.NewRow();
                dr["CustomerId"] = customerRoute.CustomerId;
                dr["Code"] = customerRoute.Code;
                dr["SaleZoneId"] = customerRoute.SaleZoneId;
                dr["IsBlocked"] = customerRoute.IsBlocked;

                if (customerRoute.ExecutedRuleId.HasValue)
                    dr["ExecutedRuleId"] = customerRoute.ExecutedRuleId;
                else
                    dr["ExecutedRuleId"] = DBNull.Value;

                if (!string.IsNullOrEmpty(customerRoute.SupplierIds))
                    dr["SupplierIds"] = customerRoute.SupplierIds;
                else
                    dr["SupplierIds"] = DBNull.Value;

                if (!string.IsNullOrEmpty(customerRoute.Options))
                    dr["RouteOptions"] = customerRoute.Options;
                else
                    dr["RouteOptions"] = DBNull.Value;

                dr["VersionNumber"] = customerRoute.VersionNumber;
                dtCustomerRoutes.Rows.Add(dr);
            }
            dtCustomerRoutes.EndLoadData();
            return dtCustomerRoutes;
        }

        #endregion

        #region Mappers

        private CustomerRoute CustomerRouteMapper(IDataReader reader)
        {
            string saleZoneServiceIds = (reader["SaleZoneServiceIds"] as string);

            return new CustomerRoute()
            {
                CustomerId = (int)reader["CustomerID"],
                CustomerName = reader["CustomerName"] as string,
                Code = reader["Code"] as string,
                SaleZoneId = (long)reader["SaleZoneID"],
                SaleZoneName = reader["SaleZoneName"] as string,
                Rate = GetReaderValue<decimal?>(reader, "Rate"),
                SaleZoneServiceIds = !string.IsNullOrEmpty(saleZoneServiceIds) ? new HashSet<int>(saleZoneServiceIds.Split(',').Select(itm => int.Parse(itm))) : null,
                IsBlocked = (bool)reader["IsBlocked"],
                ExecutedRuleId = GetReaderValue<int?>(reader, "ExecutedRuleId"),
                Options = reader["RouteOptions"] != DBNull.Value ? Helper.DeserializeOptions(reader["RouteOptions"] as string) : null,
                VersionNumber = GetReaderValue<int>(reader, "VersionNumber")
            };
        }

        private CustomerRouteData CustomerRouteDataMapper(IDataReader reader)
        {
            return new CustomerRouteData()
            {
                CustomerId = (int)reader["CustomerID"],
                Code = reader["Code"] as string,
                SaleZoneId = (long)reader["SaleZoneID"],
                IsBlocked = (bool)reader["IsBlocked"],
                ExecutedRuleId = GetReaderValue<int?>(reader, "ExecutedRuleId"),
                Options = reader["RouteOptions"] as string,
                VersionNumber = GetReaderValue<int>(reader, "VersionNumber")
            };
        }

        #endregion

        #region Queries

        const string query_GetAffectedCustomerRoutes = @" SELECT cr.[CustomerId]
                                                                  ,cr.[Code]
                                                                  ,cr.[SaleZoneId]
                                                                  ,cr.[IsBlocked]
                                                                  ,cr.[ExecutedRuleId]
                                                                  ,cr.[RouteOptions]
                                                                  ,mcr.[VersionNumber]
                                                            FROM [dbo].[CustomerRoute] cr with(nolock) 
                                                            LEFT JOIN [dbo].[ModifiedCustomerRoute] as mcr ON cr.CustomerId = mcr.CustomerId and cr.Code = mcr.Code
                                                            #CODESUPPLIERZONEMATCH#
                                                            Where #AFFECTEDROUTES#";

        const string query_GetCustomerRoutes = @"SELECT #LimitResult# cr.CustomerID
                                                        ,ca.Name as CustomerName
                                                        ,cr.Code
                                                        ,cr.SaleZoneID
                                                        ,sz.Name as SaleZoneName
                                                        ,czd.EffectiveRateValue as Rate
                                                        ,czd.SaleZoneServiceIds
                                                        ,cr.IsBlocked
                                                        ,cr.ExecutedRuleId
                                                        ,cr.RouteOptions
                                                        ,mcr.VersionNumber
                                                  FROM [dbo].[CustomerRoute] cr with(nolock) 
                                                  LEFT JOIN [dbo].[ModifiedCustomerRoute] as mcr ON cr.CustomerId = mcr.CustomerId and cr.Code = mcr.Code
                                                  JOIN [dbo].[SaleZone] as sz ON cr.SaleZoneId = sz.ID 
                                                  JOIN [dbo].[CarrierAccount] as ca ON cr.CustomerID = ca.ID
                                                  JOIN [dbo].[CustomerZoneDetail] as czd ON czd.SaleZoneId = cr.SaleZoneID and czd.CustomerId = cr.CustomerID
                                                  #FILTER#
                                                  Order by cr.CustomerID, cr.Code";

        const string query_UpdateCustomerRoutes = @"UPDATE customerRoute set customerRoute.IsBlocked = routes.IsBlocked, customerRoute.ExecutedRuleId = routes.ExecutedRuleId, 
                                                           customerRoute.SupplierIds = routes.SupplierIds, customerRoute.RouteOptions = routes.RouteOptions
                                                    FROM [dbo].[CustomerRoute] customerRoute WITH(INDEX(IX_CustomerRoute_CustomerId_Code))
                                                    JOIN @Routes routes on routes.CustomerId = customerRoute.CustomerId and routes.Code = customerRoute.Code                                                    UPDATE modifiedCustomerRoute set modifiedCustomerRoute.VersionNumber = routes.VersionNumber
                                                    FROM [dbo].[ModifiedCustomerRoute] modifiedCustomerRoute WITH(INDEX(IX_ModifiedCustomerRoute_CustomerId_Code))
                                                    JOIN @Routes routes on routes.CustomerId = modifiedCustomerRoute.CustomerId and routes.Code = modifiedCustomerRoute.Code                                                    INSERT INTO [dbo].[ModifiedCustomerRoute](CustomerId, Code, VersionNumber)
                                                    SELECT routes.CustomerId, routes.Code, routes.VersionNumber FROM @Routes routes 
                                                    LEFT JOIN [dbo].[ModifiedCustomerRoute] mcr ON routes.CustomerId = mcr.CustomerId and routes.Code = mcr.Code                                                    WHERE mcr.CustomerId IS NULL";

        const string query_CreateIX_CustomerRoute_CustomerId = @"CREATE NONCLUSTERED INDEX [IX_CustomerRoute_CustomerId] ON dbo.CustomerRoute
                                                                (
                                                                      CustomerID ASC
                                                                )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";

        const string query_CreateIX_CustomerRoute_Code = @"CREATE NONCLUSTERED INDEX [IX_CustomerRoute_Code] ON dbo.CustomerRoute
                                                           (
                                                                   Code ASC
                                                           )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";

        const string query_CreateIX_CustomerRoute_SaleZone = @"CREATE NONCLUSTERED INDEX [IX_CustomerRoute_SaleZone] ON dbo.CustomerRoute
                                                               (
                                                                     SaleZoneId ASC
                                                               )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";

        const string query_CreateIX_CustomerRoute_VersionNumber = @"CREATE NONCLUSTERED INDEX [IX_CustomerRoute_VersionNumber] ON dbo.CustomerRoute
                                                                    (
                                                                        VersionNumber ASC
                                                                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";

        const string query_CreateIX_CustomerRoute_CustomerId_Code = @"CREATE UNIQUE CLUSTERED INDEX [IX_CustomerRoute_CustomerId_Code] ON [dbo].[CustomerRoute]
                                                                    (
	                                                                    [CustomerId] ASC,
	                                                                    [Code] ASC
                                                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";

        #endregion
    }
}