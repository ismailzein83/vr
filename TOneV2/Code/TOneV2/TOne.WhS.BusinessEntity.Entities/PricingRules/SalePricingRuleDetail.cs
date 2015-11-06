using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePricingRuleDetail 
    {
        public SalePricingRule Entity { get; set; }
        public string RuleTypeName { get; set; }
        public string Customers
        {
            get
            {
                if (this.Entity.Criteria != null && this.Entity.Criteria.CustomerGroupSettings != null)
                    return this.Entity.Criteria.CustomerGroupSettings.GetDescription(this.Entity.GetCustomerGroupContext());

                return string.Empty;
            }
        }

        public string SaleZones
        {
            get
            {
                if (this.Entity.Criteria != null && this.Entity.Criteria.SaleZoneGroupSettings != null)
                    return this.Entity.Criteria.SaleZoneGroupSettings.GetDescription(this.Entity.GetSaleZoneGroupContext());

                return string.Empty;
            }
        }
    }
}
