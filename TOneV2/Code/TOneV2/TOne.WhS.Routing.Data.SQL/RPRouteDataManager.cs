using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class RPRouteDataManager : RoutingDataManager, IRPRouteDataManager
    {
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
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(Entities.RPRoute record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}", record.RoutingProductId, record.SaleZoneId, record.ExecutedRuleId, Vanrise.Common.Serializer.Serialize(record.OptionsDetailsBySupplier, true), Vanrise.Common.Serializer.Serialize(record.RPOptionsByPolicy, true), record.IsBlocked ? 1 : 0);
        }

        public Vanrise.Entities.BigResult<Entities.RPRoute> GetFilteredRPRoutes(Vanrise.Entities.DataRetrievalInput<Entities.RPRouteQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                query_GetFilteredRPRoutes.Replace("#TEMPTABLE#", tempTableName);

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
            return RetrieveData(input, createTempTableAction, CustomerRouteMapper);
        }

        private RPRoute CustomerRouteMapper(IDataReader reader)
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

        #region Queries

        private StringBuilder query_GetFilteredRPRoutes = new StringBuilder(@"IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                                                        BEGIN
                                                            SELECT [RoutingProductId]
                                                                  ,[SaleZoneId]
                                                                  ,[ExecutedRuleId]
                                                                  ,[OptionsDetailsBySupplier]
                                                                  ,[OptionsByPolicy]
                                                                  ,[IsBlocked]
                                                            INTO #TEMPTABLE# FROM [dbo].[ProductRoute] with(nolock)
                                                            Where #ROUTING_PRODUCT_IDS# AND #SALE_ZONE_IDS# 
                                                            END");

        #endregion
    }
}
