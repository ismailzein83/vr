using System;
using System.Collections.Generic;

namespace TOne.WhS.Sales.Entities
{
    public class PricingTemplate
    {
        public int PricingTemplateID { get; set; }

        public string Name { get; set; }

        public PricingTemplateSettings Settings { get; set; }

        public DateTime CreatedTime { get; set; }
    }

    public class PricingTemplateSettings
    {
        public int CurrencyId { get; set; }

        public string Description { get; set; }

        public List<PricingTemplateRule> Rules { get; set; }
    }
}