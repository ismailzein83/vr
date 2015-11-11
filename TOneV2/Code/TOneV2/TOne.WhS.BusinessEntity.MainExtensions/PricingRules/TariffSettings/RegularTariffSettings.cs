using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.PricingRules.TariffSettings
{
    public class RegularTariffSettings : PricingRuleTariffSettings
    {
        public Decimal CallFee { get; set; }

        public int FirstPeriod { get; set; }

        public Decimal FirstPeriodRate { get; set; }

        public int FractionUnit { get; set; }

        public override void Execute(IPricingRuleTariffContext context, PricingRuleTariffTarget target)
        {
            target.EffectiveRate = target.Rate;
            target.TotalAmount = target.Rate * (target.DurationInSeconds / 60);
        }
    }
}
