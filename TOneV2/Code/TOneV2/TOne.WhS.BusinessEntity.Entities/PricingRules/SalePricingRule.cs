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

        public ISaleZoneGroupContext GetSaleZoneGroupContext()
        {
            ISaleZoneGroupContext saleZoneGroupContext = ContextFactory.CreateContext<ISaleZoneGroupContext>();
            saleZoneGroupContext.FilterSettings = new SaleZoneFilterSettings
            {
            };
            return saleZoneGroupContext;
        }

        public ICustomerGroupContext GetCustomerGroupContext()
        {
            ICustomerGroupContext customerGroupContext = ContextFactory.CreateContext<ICustomerGroupContext>();
            customerGroupContext.FilterSettings = new CustomerFilterSettings
            {
            };
            return customerGroupContext;
        }

        IEnumerable<int> IRuleCustomerCriteria.CustomerIds
        {
            get
            {
                if (this.Criteria != null && this.Criteria.CustomerGroupSettings != null)
                    return this.GetCustomerGroupContext().GetGroupCustomerIds(this.Criteria.CustomerGroupSettings);
                else
                    return null;
            }
        }

        IEnumerable<long> IRuleSaleZoneCriteria.SaleZoneIds
        {
            get
            {
                if (this.Criteria != null && this.Criteria.SaleZoneGroupSettings != null)
                {
                    return GetSaleZoneGroupContext().GetGroupZoneIds(this.Criteria.SaleZoneGroupSettings);
                }
                else
                {
                    return null;
                }
            }
        }
        
    }
}
