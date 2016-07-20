using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
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

        //private readonly ICarrierAccountDataManager _carrierAccountDataManager;
        //private readonly IZoneDataManager _zoneDataManager;

        public OldRoutingDataManager()
        {

        }
        const string query = @"";

        #region Mobile
        public List<RouteInfo> GetRoutes(char showBlocksChar, char? isBlockChar, int topValue, int from, int to, string customerId, string supplierId, string code, string zone)
        {
            FlaggedServiceManager flaggedServiceManager = new FlaggedServiceManager();
            ZoneManager zoneManager = new ZoneManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            Dictionary<int, BusinessEntity.Entities.Zone> Zones = zoneManager.GetAllZones();
            List<RouteInfo> routesInfo = new List<RouteInfo>();
            ExecuteReaderSP("LCR.sp_Route_GetRoutes", (reader) =>
            {
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        FlaggedServiceInput input = new FlaggedServiceInput() { FlaggedServiceID = (Int16)reader["OurServicesFlag"] };
                        flaggedServiceManager.AssignFlaggedServiceInfo(input);
                        RouteInfo routeInfo = new RouteInfo();
                        routeInfo.RouteID = (int)reader["RouteID"];
                        routeInfo.CustomerID = carrierAccountManager.GetCarrierAccount(reader["CustomerID"] as string);
                        routeInfo.Code = reader["Code"] as string;
                        routeInfo.OurZoneID = Zones.Where(z => z.Key == (int)reader["OurZoneID"]).FirstOrDefault().Value;
                        routeInfo.OurServicesFlag = new FlaggedService() { FlaggedServiceID = input.FlaggedServiceID, ServiceColor = input.FlaggedServiceColor, Symbol = input.FlaggedServiceSymbol };
                        routeInfo.OurActiveRate = (float)Math.Round((float)reader["OurActiveRate"], 4);
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
                        FlaggedServiceInput input = new FlaggedServiceInput() { FlaggedServiceID = (Int16)reader["SupplierServicesFlag"] };
                        flaggedServiceManager.AssignFlaggedServiceInfo(input);
                        RouteOptionInfo routeOption = new RouteOptionInfo();
                        routeOption.RouteID = (int)reader["RouteID"];
                        routeOption.SupplierID = carrierAccountManager.GetCarrierAccount(reader["SupplierID"] as string);
                        routeOption.SupplierZoneID = Zones.Where(z => z.Key == (int)reader["SupplierZoneID"]).FirstOrDefault().Value;
                        routeOption.SupplierServicesFlag = new FlaggedService() { FlaggedServiceID = input.FlaggedServiceID, ServiceColor = input.FlaggedServiceColor, Symbol = input.FlaggedServiceSymbol };
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
