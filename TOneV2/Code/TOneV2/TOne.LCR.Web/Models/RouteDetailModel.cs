using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.LCR.Entities;

namespace TOne.LCR.Web.Models
{
    public class RouteDetailModel
    {

        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int SaleZoneId { get; set; }
        public decimal Rate { get; set; }
        public short ServicesFlag { get; set; }
        public string Code { get; set; }
        public string ZoneName { get; set; }
        public List<OptionsModel> Options { get; set; }
        public Nullable<RouteRuleActionType> ActionType { get; set; }
        public Nullable<int> RouteRuleId { get; set; }
        public IEnumerable<RouteRuleSummaryModel> Rules { get; set; }

    }
}