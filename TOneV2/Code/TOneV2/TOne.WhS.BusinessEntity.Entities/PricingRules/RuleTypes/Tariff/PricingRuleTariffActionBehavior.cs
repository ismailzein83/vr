﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class PricingRuleTariffActionBehavior
    {
        public abstract void Execute(PricingRuleTariffActionSettings settings, PricingRuleTariffTarget target);
    }
}
