﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.BusinessEntity.Business;
using TOne.LCR.Entities;
using TOne.LCR.Web.Models;

namespace TOne.LCR.Web.ModelMappers
{
    public static class Mappers
    {
        public static RouteRuleSummaryModel MapRouteRule(RouteRule route)
        {
            BusinessEntity.Business.BusinessEntityInfoManager beManager = new BusinessEntity.Business.BusinessEntityInfoManager();
            return new RouteRuleSummaryModel
            {
                RouteRuleId = route.RouteRuleId,
                CarrierAccountDescription = route.CarrierAccountSet.GetDescription(beManager),
                CodeSetDescription = route.CodeSet.GetDescription(beManager),
                ActionDescription = (route.ActionData != null) ? route.ActionData.GetDescription(beManager) : "Route Block",
                Type = route.Type,
                TypeDescription = route.Type.ToString(),
                BeginEffectiveDate = route.BeginEffectiveDate,
                EndEffectiveDate = route.EndEffectiveDate,
                Reason = route.Reason
            };
        }

        public static IEnumerable<RouteRuleSummaryModel> MapRouteRules(IEnumerable<RouteRule> rules)
        {
            List<RouteRuleSummaryModel> models = new List<RouteRuleSummaryModel>();
            if (rules != null)
                foreach (var rule in rules)
                {
                    models.Add(MapRouteRule(rule));
                }
            return models;
        }

        public static RouteDetailModel MapRouteDetail(RouteDetail routeDetail)
        {
            BusinessEntityInfoManager infoManager = new BusinessEntityInfoManager();
            return new RouteDetailModel()
            {
                Code = routeDetail.Code,
                CustomerID = routeDetail.CustomerID,
                Rate = routeDetail.Rate,
                SaleZoneId = routeDetail.SaleZoneId,
                ZoneName = infoManager.GetZoneName(routeDetail.SaleZoneId),
                ServicesFlag = routeDetail.ServicesFlag,
                CustomerName = infoManager.GetCarrirAccountName(routeDetail.CustomerID)
            };
        }

        public static IEnumerable<RouteDetailModel> MapRouteDetails(IEnumerable<RouteDetail> routes)
        {
            List<RouteDetailModel> routeDetails = new List<RouteDetailModel>();
            if (routes != null)
            {
                foreach (var route in routes)
                {
                    routeDetails.Add(MapRouteDetail(route));
                }
            }
            return routeDetails;
        }
    }
}