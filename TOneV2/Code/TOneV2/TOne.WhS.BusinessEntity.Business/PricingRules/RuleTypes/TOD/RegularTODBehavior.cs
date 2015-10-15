using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities.PricingRules.RuleTypes.TOD.Settings;

namespace TOne.WhS.BusinessEntity.Business.PricingRules.RuleTypes.TOD
{
    public class RegularTODBehavior :PricingRuleTODBehavior
    {
        public override void Execute(PricingRuleTODSettings settings, PricingRuleTODTarget target)
        {
            RegularTODSettings regularTODSettings = settings as RegularTODSettings;
        }
    }
}
