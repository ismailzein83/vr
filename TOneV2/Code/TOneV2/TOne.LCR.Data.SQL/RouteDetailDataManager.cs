using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;
using Vanrise.Data.SQL;

namespace TOne.LCR.Data.SQL
{
    public class RouteDetailDataManager : RoutingDataManager, IRouteDetailDataManager
    {
        



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
                    str.AppendFormat("{0}^{1}^{2}^{3}^{4}^{5}^{6}|", o.SupplierId, o.SupplierZoneId, o.Rate, o.Percentage, o.Priority, o.IsBlocked, o.ServicesFlag);
                }
            }
            return str.ToString();
        }

        public class DBApplyPrepareInfo
        {
            public string FilePath { get; set; }
            public System.IO.StreamWriter StreamWriter { get; set; }
        }

        public object InitialiazeStreamForDBApply()
        {
            string filePath = GetFilePathForBulkInsert();
            return new DBApplyPrepareInfo
            {
                FilePath = filePath,
                StreamWriter = new System.IO.StreamWriter(filePath)
            };
        }

        public void WriteRouteToStream(RouteDetail routeDetail, object stream)
        {
            DBApplyPrepareInfo prepareInfo = stream as DBApplyPrepareInfo;
            prepareInfo.StreamWriter.WriteLine("{0}^{1}^{2}^{3: 0.00000}^{4}^{5}", routeDetail.CustomerID, routeDetail.Code, routeDetail.SaleZoneId, routeDetail.Rate, routeDetail.ServicesFlag,
                           routeDetail.Options != null ? Serialize(routeDetail.Options) : null);
        }

        public object FinishDBApplyStream(object stream)
        {
            DBApplyPrepareInfo prepareInfo = stream as DBApplyPrepareInfo;
            prepareInfo.StreamWriter.Close();
            prepareInfo.StreamWriter.Dispose();
            return new BulkInsertInfo
            {
                TableName = "Route",
                DataFilePath = prepareInfo.FilePath,
                TabLock = true,
                FieldSeparator = '^'
            };
        }

        public object PrepareRouteDetailsForDBApply(List<RouteDetail> routeDetails)
        {
            System.IO.StreamWriter wr = null;
            string filePath = GetFilePathForBulkInsert();
            try
            {
                using (wr = new System.IO.StreamWriter(filePath))
                {
                    foreach (var rd in routeDetails)
                    {
                        wr.WriteLine("{0}^{1}^{2}^{3: 0.00000}^{4}^{5}", rd.CustomerID, rd.Code, rd.SaleZoneId, rd.Rate, rd.ServicesFlag,
                            rd.Options != null ? Serialize(rd.Options) : null);
                    }
                }
            }
            finally
            {
                if (wr != null)
                    wr.Dispose();
            }
            
            return new BulkInsertInfo
            {
                TableName = "Route",
                DataFilePath = filePath,
                TabLock = true,
                FieldSeparator = '^'
            };
        }

        public void ApplyRouteDetailsToDB(Object preparedRouteDetails)
        {
            InsertBulkToTable(preparedRouteDetails as BulkInsertInfo);
        }

        public void CreateIndexesOnTable()
        {
            throw new NotImplementedException();
        }

        #region Queries

        const string query_CreateIndexesOnTable = @"";

        #endregion
    }
}
