using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class PricingRulesInput
    {
        public IRate Rate { get; set; }

        public int DurationInSeconds { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsEffectiveInFuture { get; set; }
    }

    public class SalePricingRulesInput : PricingRulesInput
    {
        public int CustomerId { get; set; }

        public int SellingProductId { get; set; }

        public long SaleZoneId { get; set; }
    }

    public class PurchasePricingRulesInput : PricingRulesInput
    {
        public int SupplierId { get; set; }

        public long SupplierZoneId { get; set; }
    }
}
