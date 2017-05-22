using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealBuyRouteRuleSettings : Vanrise.Entities.VRRuleSettings
    {
        //public override Guid VRRuleSettingsConfigId { get { return new Guid("5CCCA10A-4980-4F2A-8004-8F7198377FB7"); } }

        public string Description { get; set; }

        public int SwapDealId { get; set; }

        public List<long> SupplierZoneIds { get; set; }

        public SwapDealBuyRouteRuleExtendedSettings ExtendedSettings { get; set; }
    }
}
