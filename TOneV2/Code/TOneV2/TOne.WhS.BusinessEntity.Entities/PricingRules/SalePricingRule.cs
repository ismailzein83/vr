using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePricingRule : BasePricingRule, IRuleCustomerCriteria, IRuleSaleZoneCriteria
    {
        public SalePricingRuleCriteria Criteria { get; set; }

        public IEnumerable<int> CustomerIds
        {
            get { return this.Criteria != null && this.Criteria.CustomerGroupSettings != null ? this.Criteria.CustomerGroupSettings.GetCustomerIds(null) : null; }
        }

        public IEnumerable<long> SaleZoneIds
        {
            get { return this.Criteria != null && this.Criteria.SaleZoneGroupSettings != null ? this.Criteria.SaleZoneGroupSettings.GetZoneIds(null) : null; }
        }
    }
}
