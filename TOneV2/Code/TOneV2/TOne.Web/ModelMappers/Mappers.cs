using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.LCR.Entities;
using TOne.Web.Models;

namespace TOne.Web.ModelMappers
{
    public static class  Mappers
    {
        public static RouteRuleSummaryModel MapRouteRule(RouteRule route)
        {
            return new RouteRuleSummaryModel
            {
                RouteRuleId = route.RouteRuleId ,
                CarrierAccountDescription = route.CarrierAccountSet.Description,
                CodeSetDescription = route.CodeSet.Description,
                ActionDescription = (route.ActionData!=null)?route.ActionData.GetType().Name : "Route Block" ,
                Type = route.Type ,
                TypeDescription = route.Type.ToString() ,
                BeginEffectiveDate = route.BeginEffectiveDate,
                EndEffectiveDate = route.EndEffectiveDate,
                Reason = route.Reason
               
            };
        }

        public static IEnumerable<RouteRuleSummaryModel> MapRouteRules(IEnumerable<RouteRule> rules)
        {
            List<RouteRuleSummaryModel> models = new List<RouteRuleSummaryModel>();
            if (rules != null )
                foreach (var rule in rules)
                {
                    models.Add(MapRouteRule(rule));
                }
            return models;
        }

    }
}