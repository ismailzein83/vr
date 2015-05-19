using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;
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
        public IEnumerable<RouteDetail> GetRoutesDetail(string customerIds, string code, string zoneIds, int fromRow, int toRow, bool isDescending, string orderBy)
        {
            return GetPagedItemsText<RouteDetail>(string.Format(query_GetRoutes, GetConditions(customerIds, code, zoneIds)), RouteDetailMapper, null, fromRow, toRow, isDescending, orderBy);
        }

        private string GetConditions(string customerIds, string code, string zoneIds)
        {
            StringBuilder condition = new StringBuilder();
            if (!string.IsNullOrEmpty(customerIds))
            {
                condition.Append(" and ");
                condition.Append("CustomerID in (" + customerIds.Split(',').Select(s => "'" + s + "'").Aggregate((i, j) => i + "," + j) + ")");
            }

            if (!string.IsNullOrEmpty(zoneIds))
            {
                condition.Append(" and ");
                condition.Append("SaleZoneId in (" + zoneIds.Split(',').Select(s => s).Aggregate((i, j) => i + "," + j) + ")");
            }

            if (!string.IsNullOrEmpty(code))
            {
                condition.Append(" and ");
                condition.Append("Code like '" + code + "%'");
            }

            return condition.ToString();
        }

        public static string Serialize(RouteOptions routeOptions)
        {
            //return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.None, s_Settings);
            byte[] serialized = Vanrise.Common.ProtoBufSerializer.Serialize<RouteOptions>(routeOptions);
            return Convert.ToBase64String(serialized);
        }

        //string Serialize3(RouteOptions routeOptions)
        //{
        //    MemoryStream
        //    //ProtoBuf.Serializer.Serialize<RouteOptions>()
        //}

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

        const string query_GetRoutes = @" 
                                         select r.CustomerId,
                                                r.Code,
                                                r.SaleZoneId,
                                                r.OurActiveRate,
                                                r.ServicesFlag,
                                                r.Options
                                                From Route where 1 = 1  {0}
                                       ";

        #endregion



    }
}
