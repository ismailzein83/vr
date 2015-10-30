using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PurchasePricingRuleRateTypeTarget : PricingRuleRateTypeTarget, IPurchasePricingRuleTarget
    {
        public int SupplierId { get; set; }

        public long SupplierZoneId { get; set; }

        int? IRuleSupplierTarget.SupplierId
        {
            get { return this.SupplierId; }
        }

        long? IRuleSupplierZoneTarget.SupplierZoneId
        {
            get { return this.SupplierZoneId; }
        }
    }
}
