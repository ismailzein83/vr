using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class RPRouteDataManager : RoutingDataManager, IRPRouteDataManager
    {
        readonly string[] columns = { "RoutingProductId", "SaleZoneId", "SaleZoneServices", "ExecutedRuleId", "OptionsDetailsBySupplier", "OptionsByPolicy", "IsBlocked" };

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

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(Entities.RPRoute record, object dbApplyStream)
        {
            string saleZoneServices = (record.SaleZoneServiceIds != null && record.SaleZoneServiceIds.Count > 0) ? string.Join(",", record.SaleZoneServiceIds) : null;
            string optionsDetailsBySupplier = this.SerializeOptionsDetailsBySupplier(record.OptionsDetailsBySupplier);
            string rpOptionsByPolicy = this.SerializeOptionsByPolicy(record.RPOptionsByPolicy);

            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}", record.RoutingProductId, record.SaleZoneId, saleZoneServices, record.ExecutedRuleId,
                                                                           optionsDetailsBySupplier, rpOptionsByPolicy, record.IsBlocked ? 1 : 0);
        }

        public void ApplyProductRouteForDB(object preparedProductRoute)
        {
            InsertBulkToTable(preparedProductRoute as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[ProductRoute]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        public IEnumerable<Entities.RPRoute> GetFilteredRPRoutes(Vanrise.Entities.DataRetrievalInput<Entities.RPRouteQueryByZone> input)
        {
            query_GetFilteredRPRoutes.Replace("#LimitResult#", input.Query.LimitResult.ToString());

            string routingProductIdsFilter = string.Empty;
            string saleZoneIdsFilter = string.Empty;
            string customerIdFilter = string.Empty;

            string joinCustomerZoneDetail = string.Empty;
            string effectiveRateValue = string.Empty;


            if (input.Query.RoutingProductIds != null && input.Query.RoutingProductIds.Count > 0)
                routingProductIdsFilter = string.Format(" AND pr.RoutingProductId In({0})", string.Join(",", input.Query.RoutingProductIds));

            if (input.Query.SaleZoneIds != null && input.Query.SaleZoneIds.Count > 0)
                saleZoneIdsFilter = string.Format(" AND pr.SaleZoneId In({0})", string.Join(",", input.Query.SaleZoneIds));

            if (input.Query.CustomerId.HasValue)
            {
                customerIdFilter = string.Format(" AND CustomerId = {0} ", input.Query.CustomerId);
                effectiveRateValue = " ,czd.[EffectiveRateValue] ";

                if (input.Query.SimulatedRoutingProductId.HasValue)
                {
                    joinCustomerZoneDetail = " JOIN [dbo].[CustomerZoneDetail] as czd ON pr.SaleZoneId = czd.SaleZoneId ";
                    customerIdFilter = string.Concat(customerIdFilter, string.Format(" AND pr.RoutingProductId = {0} ", input.Query.SimulatedRoutingProductId.Value));
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

            query_GetFilteredRPRoutes.Replace("#ROUTING_PRODUCT_IDS#", routingProductIdsFilter);
            query_GetFilteredRPRoutes.Replace("#SALE_ZONE_IDS#", saleZoneIdsFilter);
            query_GetFilteredRPRoutes.Replace("#CUSTOMER_IDS#", customerIdFilter);
            query_GetFilteredRPRoutes.Replace("#CUSTOMERZONEDETAILS#", joinCustomerZoneDetail);
            query_GetFilteredRPRoutes.Replace("#EFFECTIVERATE#", effectiveRateValue);

            bool? isBlocked = null;
            if (input.Query.RouteStatus.HasValue)
                isBlocked = input.Query.RouteStatus.Value == RouteStatus.Blocked ? true : false;

            return GetItemsText(query_GetFilteredRPRoutes.ToString(), RPRouteMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@IsBlocked", isBlocked.HasValue ? isBlocked.Value : (object)DBNull.Value));
            });
        }

        public IEnumerable<RPRoute> GetRPRoutes(IEnumerable<RPZone> rpZones)
        {
            DataTable dtRPZones = BuildRPZoneTable(rpZones);
            return GetItemsText(query_GetRPRoutesByRPZones, RPRouteMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@RPZoneList", SqlDbType.Structured);
                dtPrm.TypeName = "RPZonesType";
                dtPrm.Value = dtRPZones;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public Dictionary<Guid, IEnumerable<RPRouteOption>> GetRouteOptions(int routingProductId, long saleZoneId)
        {
            object serializedRouteOptions = ExecuteScalarText(query_GetRouteOptions, (cmd) =>
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

            trackStep("Starting create Clustered Index on ProductRoute table (RoutingProductId and SaleZoneId).");
            query = string.Format(query_CreateIX_ProductRoute_RoutingProductId, maxDOPSyntax);
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            trackStep("Finished create Clustered Index on ProductRoute table (RoutingProductId and SaleZoneId).");

            trackStep("Starting create Index on ProductRoute table (SaleZoneId).");
            query = string.Format(query_CreateIX_ProductRoute_SaleZoneId, maxDOPSyntax);
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            trackStep("Finished create Index on ProductRoute table (SaleZoneId).");
        }

        #endregion

        #region Private Methods

        private RPRoute RPRouteMapper(IDataReader reader)
        {
            string saleZoneServices = reader["SaleZoneServices"] as string;
            string optionsDetailsBySupplier = reader["OptionsDetailsBySupplier"] as string;
            string optionsByPolicy = reader["OptionsByPolicy"] as string;

            return new RPRoute()
            {
                RoutingProductId = (int)reader["RoutingProductId"],
                SaleZoneId = (long)reader["SaleZoneId"],
                SaleZoneName = reader["Name"] as string,
                SaleZoneServiceIds = !string.IsNullOrEmpty(saleZoneServices) ? new HashSet<int>(saleZoneServices.Split(',').Select(itm => int.Parse(itm))) : null,
                IsBlocked = (bool)reader["IsBlocked"],
                ExecutedRuleId = (int)reader["ExecutedRuleId"],
                EffectiveRateValue = GetReaderValue<decimal?>(reader, "EffectiveRateValue"),
                OptionsDetailsBySupplier = this.DeserializeOptionsDetailsBySupplier(optionsDetailsBySupplier),
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
                        routeOptionSupplierZone.SupplierRateId = long.Parse(supplierZoneParts[4]);

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
                    serializedRouteOptions.Add(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}", RouteOptionPropertiesSeparator, routeOption.SupplierId, routeOption.SupplierRate, routeOption.Percentage,
                                        routeOption.OptionWeight, routeOption.SaleZoneId, routeOption.SupplierServiceWeight, supplierZoneMatchHasClosedRate, Convert.ToInt32(routeOption.SupplierStatus), isForced));
                }

                string serializadedRouteOptionsAsString = string.Join(RouteOptionsSeparatorAsString, serializedRouteOptions);

                str.AppendFormat("{1}{0}{2}", PolicyRouteOptionPropertiesSeparator, policyId, serializadedRouteOptionsAsString);
            }
            return str.ToString();
        }

        public Dictionary<Guid, IEnumerable<RPRouteOption>> DeserializeOptionsByPolicy(string serializedOptionsDetailsBySupplier)
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

                        routeOptionsList.Add(rpRouteOption);
                    }

                    optionsByPolicy.Add(policyId, routeOptionsList);
                }
            }

            return optionsByPolicy;
        }

        #endregion

        #region Queries

        private StringBuilder query_GetFilteredRPRoutes = new StringBuilder(@"SELECT TOP #LimitResult# pr.[RoutingProductId]
                                                                                    ,pr.[SaleZoneId]
                                                                                    ,sz.[Name] 
                                                                                    ,pr.[SaleZoneServices]
                                                                                    #EFFECTIVERATE#
                                                                                    ,pr.[ExecutedRuleId]
                                                                                    ,pr.[OptionsDetailsBySupplier]
                                                                                    ,pr.[OptionsByPolicy]
                                                                                    ,pr.[IsBlocked]
                                                                              FROM [dbo].[ProductRoute] as pr with(nolock)
                                                                              JOIN [dbo].[SaleZone] as sz ON pr.SaleZoneId=sz.ID
                                                                              #CUSTOMERZONEDETAILS#
                                                                              Where (@IsBlocked is null or IsBlocked = @IsBlocked) #ROUTING_PRODUCT_IDS# #CUSTOMER_IDS# #SALE_ZONE_IDS#");

        private const string query_GetRouteOptions = @"SELECT [OptionsByPolicy]
                                                       FROM [dbo].[ProductRoute] 
                                                       Where RoutingProductId = @RoutingProductId And SaleZoneId = @SaleZoneId";

        private const string query_GetRouteOptionSuppliers = @"SELECT [OptionsDetailsBySupplier]
                                                               FROM [dbo].[ProductRoute] 
                                                               Where RoutingProductId = @RoutingProductId And SaleZoneId = @SaleZoneId";

        private const string query_GetRPRoutesByRPZones = @"SELECT pr.[RoutingProductId],
                                                                pr.[SaleZoneId],
                                                                sz.[Name],
                                                                pr.[SaleZoneServices],
                                                                pr.[ExecutedRuleId],
                                                                pr.[OptionsDetailsBySupplier],
                                                                pr.[OptionsByPolicy],
                                                                pr.[IsBlocked],
                                                                null as EffectiveRateValue
                                                            FROM [dbo].[ProductRoute] pr with(nolock) JOIN [dbo].[SaleZone] as sz ON pr.SaleZoneId=sz.ID
                                                            JOIN @RPZoneList z
                                                            ON z.RoutingProductId = pr.RoutingProductId AND z.SaleZoneId = pr.SaleZoneId";

        private const string query_CreateIX_ProductRoute_RoutingProductId = @"CREATE CLUSTERED INDEX [IX_ProductRoute_RoutingProductId] ON dbo.ProductRoute
                                                                              (
                                                                                 [RoutingProductId] ASC,
                                                                                 [SaleZoneId] ASC
                                                                              )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, 
                                                                                     ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";

        private const string query_CreateIX_ProductRoute_SaleZoneId = @"CREATE NONCLUSTERED INDEX [IX_ProductRoute_SaleZoneId] ON dbo.ProductRoute
                                                                        (
                                                                           [SaleZoneId] ASC
                                                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, 
                                                                               ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";

        #endregion
    }
}
