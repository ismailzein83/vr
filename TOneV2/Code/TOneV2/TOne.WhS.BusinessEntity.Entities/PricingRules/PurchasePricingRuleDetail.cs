using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
   public class PurchasePricingRuleDetail 
    {
        public PurchasePricingRule Entity { get; set; }
        public string RuleTypeName { get; set; }

        public string SuppliersWithZones
        {
            get
            {
                if (this.Entity.Criteria != null && this.Entity.Criteria.SuppliersWithZonesGroupSettings != null)
                    return this.Entity.Criteria.SuppliersWithZonesGroupSettings.GetDescription(this.Entity.GetSuppliersWithZonesGroupContext());

                return string.Empty;
            }
        }
    }
}
