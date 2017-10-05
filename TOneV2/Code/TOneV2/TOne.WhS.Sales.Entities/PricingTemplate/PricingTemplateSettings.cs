using System;
using System.Collections.Generic;

namespace TOne.WhS.Sales.Entities
{
    public class PricingTemplateSettings
    {
        public int CurrencyId { get; set; }

        public string Description { get; set; }

        public List<PricingTemplateRule> Rules { get; set; }
    }
}
