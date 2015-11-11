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

            target.TotalAmount = 0;
            Decimal accountedDuration = (Decimal)target.DurationInSeconds;
            if (FirstPeriod > 0)
            {
                FirstPeriod = FirstPeriod;
                Decimal firstPeriodRate = (FirstPeriodRate > 0) ? (Decimal)FirstPeriodRate : target.Rate;
                target.TotalAmount = firstPeriodRate;
                accountedDuration -= (Decimal)FirstPeriod;
                    accountedDuration = Math.Max(0, accountedDuration);
            }
            if (FractionUnit > 0)
            {
                FractionUnit = (byte)FractionUnit;
                accountedDuration = Math.Ceiling(accountedDuration / FractionUnit) * FractionUnit;
                target.TotalAmount += Math.Ceiling(accountedDuration / FractionUnit) *  target.Rate;// 60
            }
            else
                target.TotalAmount += (accountedDuration * target.Rate) / 60;

            target.TotalAmount += (Decimal)CallFee;
        }
    }
}
