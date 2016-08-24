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

        readonly string[] columns = { "RoutingProductId", "SaleZoneId", "ExecutedRuleId", "OptionsDetailsBySupplier", "OptionsByPolicy", "IsBlocked" };
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


            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}", record.RoutingProductId, record.SaleZoneId, record.ExecutedRuleId,
                optionsDetailsBySupplier, rpOptionsByPolicy, record.IsBlocked ? 1 : 0);
        }

        public Vanrise.Entities.BigResult<Entities.RPRoute> GetFilteredRPRoutes(Vanrise.Entities.DataRetrievalInput<Entities.RPRouteQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                query_GetFilteredRPRoutes.Replace("#TEMPTABLE#", tempTableName);
                query_GetFilteredRPRoutes.Replace("#LimitResult#", input.Query.LimitResult.ToString());

                string routingProductIdsFilter = " 1=1 ";
                string saleZoneIdsFilter = " 1=1 ";


                if (input.Query.RoutingProductIds != null && input.Query.RoutingProductIds.Count > 0)
                    routingProductIdsFilter = string.Format("RoutingProductId In({0})", string.Join(",", input.Query.RoutingProductIds));

                if (input.Query.SaleZoneIds != null && input.Query.SaleZoneIds.Count > 0)
                    saleZoneIdsFilter = string.Format("SaleZoneId In({0})", string.Join(",", input.Query.SaleZoneIds));

                query_GetFilteredRPRoutes.Replace("#ROUTING_PRODUCT_IDS#", routingProductIdsFilter);
                query_GetFilteredRPRoutes.Replace("#SALE_ZONE_IDS#", saleZoneIdsFilter);

                ExecuteNonQueryText(query_GetFilteredRPRoutes.ToString(), null);
            };

            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");
            return RetrieveData(input, createTempTableAction, RPRouteMapper);
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

        public Dictionary<int, IEnumerable<RPRouteOption>> GetRouteOptions(int routingProductId, long saleZoneId)
        {
            object routeOptionsSerialized = ExecuteScalarText(query_GetRouteOptions, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@RoutingProductId", routingProductId));
                cmd.Parameters.Add(new SqlParameter("@SaleZoneId", saleZoneId));
            }
            );

            if (routeOptionsSerialized == null)
                return null;

            return Vanrise.Common.Serializer.Deserialize<Dictionary<int, IEnumerable<RPRouteOption>>>(routeOptionsSerialized.ToString());
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

        public void FinalizeProductRoute(Action<string> trackStep)
        {
            string query;

            trackStep("Starting create Clustered Index on ProductRoute table (RoutingProductId and SaleZoneId).");
            query = @"CREATE CLUSTERED INDEX [IX_ProductRoute_RoutingProductId] ON dbo.ProductRoute
                    (
                          [RoutingProductId] ASC,
                          SaleZoneId ASC
                    )";
            ExecuteNonQueryText(query, null);
            trackStep("Finished create Clustered Index on ProductRoute table (RoutingProductId and SaleZoneId).");

            trackStep("Starting create Index on ProductRoute table (SaleZoneId).");
            query = @"CREATE NONCLUSTERED INDEX [IX_ProductRoute_SaleZoneId] ON dbo.ProductRoute
                    (
                          SaleZoneId ASC
                    )";
            ExecuteNonQueryText(query, null);
            trackStep("Finished create Index on ProductRoute table (SaleZoneId).");
        }
        #region Private

        private RPRoute RPRouteMapper(IDataReader reader)
        {
            return new RPRoute()
            {
                RoutingProductId = (int)reader["RoutingProductId"],
                SaleZoneId = (long)reader["SaleZoneId"],
                IsBlocked = (bool)reader["IsBlocked"],
                ExecutedRuleId = (int)reader["ExecutedRuleId"],
                OptionsDetailsBySupplier = reader["OptionsDetailsBySupplier"] != null ? Vanrise.Common.Serializer.Deserialize<Dictionary<int, RPRouteOptionSupplier>>(reader["OptionsDetailsBySupplier"].ToString()) : null,
                RPOptionsByPolicy = reader["OptionsByPolicy"] != null ? Vanrise.Common.Serializer.Deserialize<Dictionary<int, IEnumerable<RPRouteOption>>>(reader["OptionsByPolicy"].ToString()) : null
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

        private StringBuilder query_GetFilteredRPRoutes = new StringBuilder(@"IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                                                        BEGIN
                                                            SELECT TOP #LimitResult# [RoutingProductId]
                                                                  ,[SaleZoneId]
                                                                  ,[ExecutedRuleId]
                                                                  ,[OptionsDetailsBySupplier]
                                                                  ,[OptionsByPolicy]
                                                                  ,[IsBlocked]
                                                            INTO #TEMPTABLE# FROM [dbo].[ProductRoute] with(nolock)
                                                            Where #ROUTING_PRODUCT_IDS# AND #SALE_ZONE_IDS# 
                                                            END");

        private const string query_GetRouteOptions = @"SELECT [OptionsByPolicy]
                                                        FROM [dbo].[ProductRoute] 
                                                        Where RoutingProductId = @RoutingProductId And SaleZoneId = @SaleZoneId";

        private const string query_GetRouteOptionSuppliers = @"SELECT [OptionsDetailsBySupplier]
                                                        FROM [dbo].[ProductRoute] 
                                                        Where RoutingProductId = @RoutingProductId And SaleZoneId = @SaleZoneId";

        private const string query_GetRPRoutesByRPZones = @"SELECT pr.[RoutingProductId],
                                                                pr.[SaleZoneId],
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
