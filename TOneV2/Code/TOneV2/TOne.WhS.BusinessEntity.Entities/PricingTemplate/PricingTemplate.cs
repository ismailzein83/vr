using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum MarginRateCalculationMethodType
    {
        Cost = 0,
        RPRouteOption = 1,
        Supplier = 2
    }

    public class PricingTemplate
    {
        public int PricingTemplateID { get; set; }

        public string Name { get; set; }

        public int CurrencyId { get; set; }

        public string Description { get; set; }

        public List<PricingTemplateRule> Rules { get; set; }
    }

    public class CountryPricingTemplate
    {
        public int CountryId { get; set; }

        public List<long> ExcludedZoneIds { get; set; }
    }

    public class ZonePricingTemplate
    {
        public int CountryId { get; set; }

        public List<long> IncludedZoneIds { get; set; }
    }

    public class RatePricingTemplate
    {
        public decimal FromRate { get; set; }
        public decimal ToRate { get; set; }
        public List<int> Services { get; set; }
        public decimal? MinRate { get; set; }
        public decimal? MaxMinRate { get; set; }

        public decimal Margin { get; set; }

        public decimal MarginPercentage { get; set; }

        public MarginRateCalculationMethodType Type { get; set; }

        public Guid CostCalculationMethodConfigId { get; set; }

        public int RPRouteOptionNumber { get; set; }

        public int SupplierId { get; set; }
    }

    public class PricingTemplateRule
    {
        public List<CountryPricingTemplate> Countries { get; set; }

        public List<ZonePricingTemplate> Zones { get; set; }

        public List<RatePricingTemplate> Rates { get; set; }
    }
}
