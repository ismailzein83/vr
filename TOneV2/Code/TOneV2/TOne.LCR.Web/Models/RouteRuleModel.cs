using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.LCR.Entities;

namespace TOne.LCR.Web.Models
{
    public class RouteRuleSummaryModel
    {
        public int RouteRuleId { get; set; }

        public string CarrierAccountDescription { get; set; }

        public string CodeSetDescription { get; set; }

        public string ActionDescription { get; set; }

        public RouteRuleType Type{ get; set; }

        public string TypeDescription { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public string Reason { get; set; }
    }
}