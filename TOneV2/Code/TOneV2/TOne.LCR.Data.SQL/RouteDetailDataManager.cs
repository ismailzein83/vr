using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;
using TOne.LCR.Entities.Routing;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace TOne.LCR.Data.SQL
{
    public class RouteDetailDataManager : RoutingDataManager, IRouteDetailDataManager
    {
        public List<RouteDetail> GetRoutesDetail(string customerId, string code, int? ourZoneId)
        {
            List<RouteDetail> routesDetails = new List<RouteDetail>();
            routesDetails = GetItemsSP("sp_Route_GetRoutes", RouteDetailMapper, customerId, code, ourZoneId);
            return routesDetails;
        }
        public IEnumerable<RouteDetail> GetRoutesDetail(IEnumerable<string> customerIds, string code, IEnumerable<int> zoneIds, int fromRow, int toRow, bool isDescending, string orderBy)
        {
            StringBuilder filterQuery = new StringBuilder();
            RouteDetailFilter filter = new RouteDetailFilter()
            {
                Code = new List<string>(),
                CustomerIds = customerIds.ToList(),
                ZoneIds = zoneIds.ToList()
            };
            filter.Code.Add(code);
            AddFilterToQuery(filter, filterQuery);
            return GetPagedItemsText<RouteDetail>(string.Format(query_GetRoutes, filterQuery.ToString()), RouteDetailMapper, null, fromRow, toRow, isDescending, orderBy);
        }

        void AddFilter<T>(StringBuilder whereBuilder, IEnumerable<T> values, string column)
        {
            if (values != null && values.Count() > 0)
            {
                if (typeof(T) == typeof(string))
                    whereBuilder.AppendFormat("AND {0} IN ('{1}')", column, String.Join("', '", values));
                else
                    whereBuilder.AppendFormat("AND {0} IN ({1})", column, String.Join(", ", values));
            }
        }

        private void AddFilterToQuery(RouteDetailFilter filter, StringBuilder whereBuilder)
        {

            AddFilter(whereBuilder, filter.CustomerIds, "CustomerID");
            AddFilter(whereBuilder, filter.Code, "Code");
            AddFilter(whereBuilder, filter.ZoneIds, "OurZoneID");
        }

        public static string Serialize(RouteOptions routeOptions)
        {
            //return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.None, s_Settings);
            byte[] serialized = Vanrise.Common.ProtoBufSerializer.Serialize<RouteOptions>(routeOptions);
            return Convert.ToBase64String(serialized);
        }

        string Serialize2(RouteOptions routeOptions)
        {
            StringBuilder str = new StringBuilder();
            str.Append(routeOptions.IsBlock);
            str.Append("|");
            if (routeOptions.SupplierOptions != null)
            {
                foreach (var o in routeOptions.SupplierOptions)
                {
                    str.AppendFormat("{0}~{1}~{2}~{3}~{4}~{5}~{6}|", o.SupplierId, o.SupplierZoneId, o.Rate,
                        o.Setting != null ? o.Setting.Percentage : null,
                        o.Setting != null ? (object)o.Setting.Priority : null, o.Setting != null ? (object)o.Setting.IsBlocked : null,
                        o.ServicesFlag);
                }
            }
            return str.ToString();
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRouteToStream(RouteDetail routeDetail, object stream)
        {
            StreamForBulkInsert streamForBulkInsert = stream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3: 0.00000}^{4}^{5}", routeDetail.CustomerID, routeDetail.Code, routeDetail.SaleZoneId, routeDetail.Rate, routeDetail.ServicesFlag,
                           routeDetail.Options != null ? Serialize2(routeDetail.Options) : null);
        }

        public object FinishDBApplyStream(object stream)
        {
            StreamForBulkInsert streamForBulkInsert = stream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "Route",
                Stream = streamForBulkInsert,
                TabLock = true,
                FieldSeparator = '^'
            };
        }

        public void ApplyRouteDetailsToDB(Object preparedRouteDetails)
        {
            InsertBulkToTable(preparedRouteDetails as BaseBulkInsertInfo);
        }

        public void CreateIndexesOnTable()
        {
            ExecuteNonQueryText(query_CreateIndexesOnTable, null);
        }
        RouteDetail RouteDetailMapper(IDataReader reader)
        {
            return new RouteDetail
               {
                   Code = reader["Code"] as string,
                   CustomerID = reader["CustomerID"] as string,
                   Rate = GetReaderValue<decimal>(reader, "OurActiveRate"),
                   SaleZoneId = GetReaderValue<int>(reader, "OurZoneId"),
                   ServicesFlag = GetReaderValue<short>(reader, "OurServicesFlag")//,
                   //Options = reader["Options"] != null ? (RouteOptions)Serializer.Deserialize<RouteOptions>(reader["Options"] as string) : null
               };

        }

        public void UpdateRoutes(IEnumerable<RouteDetail> routeDetails)
        {
            DataTable dtRoutes = GetRouteDetailTypeTable(routeDetails);
            ExecuteNonQueryText(query_UpdateRoutes, (cmd) =>
            {
                var dtPrm = new SqlParameter("@RouteTable", SqlDbType.Structured);
                dtPrm.TypeName = "RouteType";
                dtPrm.Value = dtRoutes;
                cmd.Parameters.Add(dtPrm);
            });
        }

        DataTable GetRouteDetailTypeTable(IEnumerable<RouteDetail> routeDetails)
        {
            DataTable routesDataTable = new DataTable();
            routesDataTable.Columns.Add("Code", typeof(String));
            routesDataTable.Columns.Add("CustomerId", typeof(String));
            routesDataTable.Columns.Add("Rate", typeof(Decimal));
            routesDataTable.Columns.Add("Options", typeof(String));
            routesDataTable.BeginLoadData();
            foreach (var route in routeDetails)
            {
                DataRow dr = routesDataTable.NewRow();
                dr["Code"] = route.Code;
                dr["CustomerId"] = route.CustomerID;
                dr["Rate"] = route.Rate;
                dr["Options"] = route.Options != null ? Serialize2(route.Options) : null;
                routesDataTable.Rows.Add(dr);
            }
            routesDataTable.EndLoadData();
            return routesDataTable;
        }

        #region Queries

        const string query_CreateIndexesOnTable = @"   CREATE NONCLUSTERED INDEX [IX_Route_Code] ON [Route] 
		                                                (
			                                                [Code] ASC
		                                                )
                                                       CREATE NONCLUSTERED INDEX [IX_Route_CustomerId] ON [Route] 
		                                                (
			                                                [CustomerId] ASC
		                                                )
                                                         CREATE NONCLUSTERED INDEX [IX_Route_OurZoneId] ON [Route] 
		                                                (
			                                                [OurZoneId] ASC
		                                                )";

        const string query_GetRoutes = @"Select r.CustomerId,
                                                r.Code,
                                                r.SaleZoneId,
                                                r.OurActiveRate,
                                                r.ServicesFlag,
                                                r.Options
                                                From Route where 1 = 1  {0}";

        const string query_UpdateRoutes = @"
                                            UPDATE r
                                              SET r.OurActiveRate = rt.Rate,
	                                              r.Options = rt.Options
                                              FROM [Route] AS r
                                              INNER JOIN @RouteTable AS rt  ON r.Code = rt.Code and r.CustomerId = rt.CustomerId";

        #endregion






    }
}
