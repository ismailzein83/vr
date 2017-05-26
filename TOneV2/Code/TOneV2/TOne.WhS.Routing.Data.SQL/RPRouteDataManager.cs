using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class RPRouteDataManager : RoutingDataManager, IRPRouteDataManager
    {
        //static RPRouteDataManager()
        //{
        //    RPRouteOptionSupplier dummyRPRouteOptionSupplier = new RPRouteOptionSupplier();
        //    RPRouteOption dummyRPRouteOption = new RPRouteOption();
        //    RPRouteOptionSupplierZone dummyRPRouteOptionSupplierZone = new RPRouteOptionSupplierZone();
        //}

        readonly string[] columns = { "RoutingProductId", "SaleZoneId", "SaleZoneServices", "ExecutedRuleId", "OptionsDetailsBySupplier", "OptionsByPolicy", "IsBlocked" };
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

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(Entities.RPRoute record, object dbApplyStream)
        {
            string optionsDetailsBySupplier = Vanrise.Common.Serializer.Serialize(record.OptionsDetailsBySupplier, true);
            string rpOptionsByPolicy = Vanrise.Common.Serializer.Serialize(record.RPOptionsByPolicy, true);
            string saleZoneServices = (record.SaleZoneServiceIds != null && record.SaleZoneServiceIds.Count > 0) ? string.Join(",", record.SaleZoneServiceIds) : null;


            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}", record.RoutingProductId, record.SaleZoneId, saleZoneServices, record.ExecutedRuleId,
                optionsDetailsBySupplier, rpOptionsByPolicy, record.IsBlocked ? 1 : 0);
        }

        public IEnumerable<Entities.RPRoute> GetFilteredRPRoutes(Vanrise.Entities.DataRetrievalInput<Entities.RPRouteQuery> input)
        {
            query_GetFilteredRPRoutes.Replace("#LimitResult#", input.Query.LimitResult.ToString());

            string routingProductIdsFilter = " 1=1 ";
            string saleZoneIdsFilter = " 1=1 ";

            if (input.Query.RoutingProductIds != null && input.Query.RoutingProductIds.Count > 0)
                routingProductIdsFilter = string.Format("RoutingProductId In({0})", string.Join(",", input.Query.RoutingProductIds));

            if (input.Query.SaleZoneIds != null && input.Query.SaleZoneIds.Count > 0)
                saleZoneIdsFilter = string.Format("SaleZoneId In({0})", string.Join(",", input.Query.SaleZoneIds));

            query_GetFilteredRPRoutes.Replace("#ROUTING_PRODUCT_IDS#", routingProductIdsFilter);
            query_GetFilteredRPRoutes.Replace("#SALE_ZONE_IDS#", saleZoneIdsFilter);

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
            object routeOptionsSerialized = ExecuteScalarText(query_GetRouteOptions, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@RoutingProductId", routingProductId));
                cmd.Parameters.Add(new SqlParameter("@SaleZoneId", saleZoneId));
            }
            );

            if (routeOptionsSerialized == null)
                return null;

            return Vanrise.Common.Serializer.Deserialize<Dictionary<Guid, IEnumerable<RPRouteOption>>>(routeOptionsSerialized.ToString());
        }

        public Dictionary<int, RPRouteOptionSupplier> GetRouteOptionSuppliers(int routingProductId, long saleZoneId)
        {
            object routeOptionSuppliersSerialized = ExecuteScalarText(query_GetRouteOptionSuppliers, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@RoutingProductId", routingProductId));
                cmd.Parameters.Add(new SqlParameter("@SaleZoneId", saleZoneId));
            }
            );

            if (routeOptionSuppliersSerialized == null)
                return null;

            return Vanrise.Common.Serializer.Deserialize<Dictionary<int, RPRouteOptionSupplier>>(routeOptionSuppliersSerialized.ToString());
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

        const string query_CreateIX_ProductRoute_RoutingProductId = @"CREATE CLUSTERED INDEX [IX_ProductRoute_RoutingProductId] ON dbo.ProductRoute
                                                                    (
                                                                          [RoutingProductId] ASC,
                                                                          SaleZoneId ASC
                                                                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";

        const string query_CreateIX_ProductRoute_SaleZoneId = @"CREATE NONCLUSTERED INDEX [IX_ProductRoute_SaleZoneId] ON dbo.ProductRoute
                                                                (
                                                                      SaleZoneId ASC
                                                                )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";

        #region Private

        private RPRoute RPRouteMapper(IDataReader reader)
        {
            string saleZoneServices = (reader["SaleZoneServices"] as string);

            return new RPRoute()
            {
                RoutingProductId = (int)reader["RoutingProductId"],
                SaleZoneId = (long)reader["SaleZoneId"],
                SaleZoneServiceIds = !string.IsNullOrEmpty(saleZoneServices) ? new HashSet<int>(saleZoneServices.Split(',').Select(itm => int.Parse(itm))) : null,
                IsBlocked = (bool)reader["IsBlocked"],
                SaleZoneName = reader["Name"] as string,
                ExecutedRuleId = (int)reader["ExecutedRuleId"],
                OptionsDetailsBySupplier = reader["OptionsDetailsBySupplier"] != null ? Vanrise.Common.Serializer.Deserialize<Dictionary<int, RPRouteOptionSupplier>>(reader["OptionsDetailsBySupplier"].ToString()) : null,
                RPOptionsByPolicy = reader["OptionsByPolicy"] != null ? Vanrise.Common.Serializer.Deserialize<Dictionary<Guid, IEnumerable<RPRouteOption>>>(reader["OptionsByPolicy"].ToString()) : null
            };
        }

        DataTable BuildRPZoneTable(IEnumerable<RPZone> rpZones)
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

        #endregion

        #region Queries

        private StringBuilder query_GetFilteredRPRoutes = new StringBuilder(@"
                                                            SELECT TOP #LimitResult# pr.[RoutingProductId]
                                                                  ,pr.[SaleZoneId]
                                                                  ,sz.[Name] 
                                                                  ,pr.[SaleZoneServices]
                                                                  ,pr.[ExecutedRuleId]
                                                                  ,pr.[OptionsDetailsBySupplier]
                                                                  ,pr.[OptionsByPolicy]
                                                                  ,pr.[IsBlocked]
                                                            FROM [dbo].[ProductRoute] as pr with(nolock) JOIN [dbo].[SaleZone] as sz ON pr.SaleZoneId=sz.ID
                                                            Where (@IsBlocked is null or IsBlocked = @IsBlocked) AND #ROUTING_PRODUCT_IDS# AND #SALE_ZONE_IDS#");

        private const string query_GetRouteOptions = @"SELECT [OptionsByPolicy]
                                                        FROM [dbo].[ProductRoute] 
                                                        Where RoutingProductId = @RoutingProductId And SaleZoneId = @SaleZoneId";

        private const string query_GetRouteOptionSuppliers = @"SELECT [OptionsDetailsBySupplier]
                                                        FROM [dbo].[ProductRoute] 
                                                        Where RoutingProductId = @RoutingProductId And SaleZoneId = @SaleZoneId";

        private const string query_GetRPRoutesByRPZones = @"SELECT pr.[RoutingProductId],
                                                                pr.[SaleZoneId],
                                                                pr.[SaleZoneServices],
                                                                pr.[ExecutedRuleId],
                                                                pr.[OptionsDetailsBySupplier],
                                                                pr.[OptionsByPolicy],
                                                                pr.[IsBlocked]
                                                            FROM [dbo].[ProductRoute] pr with(nolock)
                                                            JOIN @RPZoneList z
                                                            ON z.RoutingProductId = pr.RoutingProductId AND z.SaleZoneId = pr.SaleZoneId";

        #endregion
    }
}
