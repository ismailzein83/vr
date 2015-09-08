using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.LCR.Entities.Routing;
using TOne.Web.Online.Models;

namespace TOne.Web.Online.ModelMappers
{
    public class RouteModelMapper
    {
        private static RouteModel MapRoutingModel(RouteInfo routeInfo)
        {
            return new RouteModel
            {
                Code = routeInfo.Code,
                CustomerID = routeInfo.CustomerID != null ? routeInfo.CustomerID.CarrierAccountId : "",
                CustomerName = routeInfo.CustomerID != null ? routeInfo.CustomerID.ProfileName + (!string.IsNullOrEmpty(routeInfo.CustomerID.NameSuffix) ? "[" + routeInfo.CustomerID.NameSuffix + "]" : "") : "",
                OurActiveRate = routeInfo.OurActiveRate,
                OurZoneID = routeInfo.OurZoneID != null ? routeInfo.OurZoneID.ZoneId : 0,
                OurZoneName = routeInfo.OurZoneID != null ? routeInfo.OurZoneID.Name : "",
                RouteID = routeInfo.RouteID,
                SuppliersInfo = routeInfo.SuppliersInfo != null && routeInfo.SuppliersInfo.Count > 0 ? MapOptionModels(routeInfo.SuppliersInfo) : null
            };
        }
        private static RouteOptionModel MapRoutingOptionModel(RouteOptionInfo routeOptionInfo)
        {
            return new RouteOptionModel
            {
                FlaggedServiceID = routeOptionInfo.SupplierServicesFlag.FlaggedServiceID,
                Symbol = routeOptionInfo.SupplierServicesFlag.Symbol,
                ServiceColor = routeOptionInfo.SupplierServicesFlag.ServiceColor,
                SupplierInfoString = routeOptionInfo.SupplierInfoString
            };
        }

        public static List<RouteModel> MapRouteModel(List<RouteInfo> routeInfos)
        {
            List<RouteModel> models = new List<RouteModel>();
            if (routeInfos != null && routeInfos.Count > 0)
                foreach (var routeInfo in routeInfos)
                {
                    models.Add(MapRoutingModel(routeInfo));
                }
            return models;
        }

        private static List<RouteOptionModel> MapOptionModels(List<RouteOptionInfo> options)
        {
            List<RouteOptionModel> result = new List<RouteOptionModel>();
            foreach (var option in options)
            {
                result.Add(MapRoutingOptionModel(option));
            }
            return result;
        }

    }
}