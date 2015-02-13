using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.LCR.Entities
{
    public enum CarrierAccountSelectionType { ByCustomer, BySupplier, ByCustomerRoutingPool }

    public class RoutingRule
    {
        public int RoutingRuleId { get; set; }

        public CarrierAccountSelectionType CarrierAccountSelectionType { get; set; }

        public MultipleSelection<string> CarrierAccountIDs { get; set; }

        public int CustomerRoutingPoolId { get; set; }
                
        public BaseCodeSet CodeSet { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public BaseRouteRuleAction RuleAction { get; set; }

        public string Reason { get; set; }
    }
}