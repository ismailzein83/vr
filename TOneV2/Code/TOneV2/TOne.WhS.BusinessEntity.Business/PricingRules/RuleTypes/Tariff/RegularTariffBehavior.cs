using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities.PricingRules.RuleTypes.Tariff.Settings;

namespace TOne.WhS.BusinessEntity.Business.PricingRules.RuleTypes.Tariff
{
    public class RegularTariffBehavior : PricingRuleTariffActionBehavior
    {
        public override void Execute(PricingRuleTariffActionSettings settings, PricingRuleTariffTarget target)
        {
            RegularTariffSettings regularTariffSettings = settings as RegularTariffSettings;
        }
    }
}
