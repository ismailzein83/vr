using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePricingRuleTariffTarget : PricingRuleTariffTarget, ISalePricingRuleTarget
    {
        public int CustomerId { get; set; }

        public long SaleZoneId { get; set; }

        int? IRuleCustomerTarget.CustomerId
        {
            get { return this.CustomerId; }
        }

        long? IRuleSaleZoneTarget.SaleZoneId
        {
            get { return this.SaleZoneId; }
        }
    }
}
