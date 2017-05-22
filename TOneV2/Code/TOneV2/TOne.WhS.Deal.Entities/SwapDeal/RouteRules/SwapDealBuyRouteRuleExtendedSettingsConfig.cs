using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealBuyRouteRuleExtendedSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_Deal_SwapDealBuyRouteRuleExtendedSettings";

        public string Editor { get; set; }
    }
}
