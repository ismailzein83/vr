using System;
using System.Collections.Generic;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class PricingTemplateRule
    {
        public List<CountryPricingTemplate> Countries { get; set; }

        public List<long> ZoneIds { get; set; }

        //public List<ZonePricingTemplate> Zones { get; set; }

        public List<RatePricingTemplate> Rates { get; set; }

        //public List<int> Services { get; set; }

        public bool IsZoneMatching(IPricingTemplateRuleContext context)
        {
            context.ThrowIfNull("context");

            bool result;

            if (this.Countries != null)
            {
                result = Countries.FindRecord(itm => itm.CountryId == context.CountryId && (itm.ExcludedZoneIds == null || !itm.ExcludedZoneIds.Contains(context.SaleZoneId))) != null;
                if (result)
                    return true;
            }

            if (this.ZoneIds != null)
            {
                result = ZoneIds.Contains(context.SaleZoneId);
                if (result)
                    return true;
            }

            return false;
        }
    }

    public interface IPricingTemplateRuleContext
    {
        long SaleZoneId { get; }
        int CountryId { get; }
    }

    public class PricingTemplateRuleContext : IPricingTemplateRuleContext
    {
        public long SaleZoneId { get; set; }
        public int CountryId { get; set; }
    }
}