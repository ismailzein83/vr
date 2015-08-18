using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.Data.SQL;
using TOne.LCR.Entities.Routing;

namespace TOne.LCR.Data.SQL
{
    public class OldRoutingDataManager : BaseTOneDataManager, IOldRoutingDataManager
    {
        public List<Entities.Routing.RouteInfo> GetRoutes(string customerId, string target, Entities.Routing.TargetType targetType, int routesCount, int lcrCount)
        {
            return new List<Entities.Routing.RouteInfo>();
        }

        private readonly ICarrierAccountDataManager _carrierAccountDataManager;
        private readonly IZoneDataManager _zoneDataManager;

        public OldRoutingDataManager()
        {
            _carrierAccountDataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            _zoneDataManager = BEDataManagerFactory.GetDataManager<IZoneDataManager>();
        }
        const string query = @"";

        #region Mobile
        public List<RouteInfo> GetRoutes(char showBlocksChar, char? isBlockChar, int topValue, int from, int to, string customerId, string supplierId, string code, string zone)
        {
            Dictionary<int, BusinessEntity.Entities.Zone> Zones = _zoneDataManager.GetAllZones();
            List<RouteInfo> routesInfo = new List<RouteInfo>();
            ExecuteReaderSP("LCR.sp_Route_GetRoutes", (reader) =>
            {
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        RouteInfo routeInfo = new RouteInfo();
                        routeInfo.RouteID = (int)reader["RouteID"];
                        routeInfo.CustomerID = _carrierAccountDataManager.GetCarrierAccount(reader["CustomerID"] as string);
                        routeInfo.Code = reader["Code"] as string;
                        routeInfo.OurZoneID = Zones.Where(z => z.Key == (int)reader["OurZoneID"]).FirstOrDefault().Value;
                        routeInfo.OurServicesFlag = (short)reader["OurServicesFlag"];
                        routeInfo.OurActiveRate = (float)reader["OurActiveRate"];
                        routeInfo.State = (byte)reader["State"] == 0 ? RouteState.Blocked : RouteState.Enabled;
                        routeInfo.Updated = (DateTime)reader["Updated"];
                        routeInfo.IsBlockAffected = (bool)reader["IsBlockAffected"];
                        routeInfo.IsSpecialRequestAffected = (bool)reader["IsSpecialRequestAffected"];
                        routeInfo.IsToDAffected = (bool)reader["IsToDAffected"];
                        routeInfo.IsOverrideAffected = (bool)reader["IsOverrideAffected"];
                        routeInfo.IsOptionBlock = (bool)reader["IsOptionBlock"];
                        routeInfo.SuppliersInfo = new List<RouteOptionInfo>();
                        routesInfo.Add(routeInfo);
                    }
                }
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        RouteOptionInfo routeOption = new RouteOptionInfo();
                        routeOption.RouteID = (int)reader["RouteID"];
                        routeOption.SupplierID = _carrierAccountDataManager.GetCarrierAccount(reader["SupplierID"] as string);
                        routeOption.SupplierZoneID = Zones.Where(z => z.Key == (int)reader["SupplierZoneID"]).FirstOrDefault().Value;
                        routeOption.SupplierServicesFlag = (Int16)reader["SupplierServicesFlag"];
                        routeOption.SupplierActiveRate = (float)reader["SupplierActiveRate"];
                        routeOption.Priority = (byte)reader["Priority"];
                        routeOption.NumberOfTries = (byte)reader["NumberOfTries"];
                        routeOption.Percentage = (byte?)reader["Percentage"];
                        routeOption.State = (byte)reader["State"] == 0 ? RouteState.Blocked : RouteState.Enabled;
                        routesInfo.Where(r => r.RouteID == routeOption.RouteID).FirstOrDefault().SuppliersInfo.Add(routeOption);
                    }
                }
            }, from, to, topValue, isBlockChar, showBlocksChar, zone, code, supplierId, customerId);
            return routesInfo;
        }
        RouteInfo RouteInfoMapper(IDataReader reader)
        {
            RouteInfo module = new RouteInfo
            {

            };
            return module;
        }
        #endregion
    }
}
