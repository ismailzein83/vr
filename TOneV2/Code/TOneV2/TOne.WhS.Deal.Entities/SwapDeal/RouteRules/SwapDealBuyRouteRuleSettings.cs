using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealBuyRouteRuleSettings : Vanrise.Entities.VRRuleSettings
    {
        public string Description { get; set; }

        public int SwapDealId { get; set; }

        public List<long> SupplierZoneIds { get; set; }

        public SwapDealBuyRouteRuleExtendedSettings ExtendedSettings { get; set; }
    }
}
