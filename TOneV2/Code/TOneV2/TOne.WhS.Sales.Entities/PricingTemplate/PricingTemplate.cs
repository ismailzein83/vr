using System;
using System.Collections.Generic;

namespace TOne.WhS.Sales.Entities
{
    public class BasePricingTemplate
    {
        public int PricingTemplateId { get; set; }

        public string Name { get; set; }

        public PricingTemplateSettings Settings { get; set; }

        public DateTime CreatedTime { get; set; }
    }

    public class PricingTemplate : BasePricingTemplate
    {
        public int SellingNumberPlanId { get; set; }
    }

    public class PricingTemplateToEdit : BasePricingTemplate
    {
    }
}