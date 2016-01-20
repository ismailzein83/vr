using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class SellingRule : Vanrise.Rules.BaseRule, IRuleCustomerCriteria, IRuleSaleZoneCriteria
    {
        public string Description { get; set; }

        public SellingRuleCriteria Criteria { get; set; }

        public SellingRuleSettings Settings { get; set; }

        public ISaleZoneGroupContext GetSaleZoneGroupContext()
        {
            ISaleZoneGroupContext context = ContextFactory.CreateContext<ISaleZoneGroupContext>();
            context.FilterSettings = new SaleZoneFilterSettings
            {
                RoutingProductId = this.Criteria.SellingProductId
            };
            return context;
        }
        
        public ICustomerGroupContext GetCustomerGroupContext()
        {
            ICustomerGroupContext context = ContextFactory.CreateContext<ICustomerGroupContext>();
            context.FilterSettings = new CustomerFilterSettings
            {
            };
            return context;
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

        public IEnumerable<long> SaleZoneIds
        {
            get
            {
                if (this.Criteria != null && this.Criteria.SaleZoneGroupSettings != null)
                    return GetSaleZoneGroupContext().GetGroupZoneIds(this.Criteria.SaleZoneGroupSettings);
                else
                    return null;
            }
        }
    }
}
