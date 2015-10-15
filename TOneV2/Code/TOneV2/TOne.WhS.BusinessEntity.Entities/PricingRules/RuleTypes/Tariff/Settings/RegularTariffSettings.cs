using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities.PricingRules.RuleTypes.Tariff.Settings
{
    public class RegularTariffSettings : PricingRuleTariffSettings
    {
        public Decimal CallFee { get; set; }

        public int FirstPeriod { get; set; }

        public Decimal FirstPeriodRate { get; set; }

        public int FractionUnit { get; set; }
    }
}
