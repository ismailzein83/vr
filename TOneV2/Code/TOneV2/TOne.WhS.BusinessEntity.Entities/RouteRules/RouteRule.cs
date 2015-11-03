using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteRule : Vanrise.Rules.BaseRule, IRuleCustomerCriteria, IRuleCodeCriteria, IRuleSaleZoneCriteria, IRuleRoutingProductCriteria
    {
        public RouteRuleCriteria Criteria { get; set; }        

        public RouteRuleSettings Settings { get; set; }

        public string Description { get; set; }

        public override bool IsAnyCriteriaExcluded(object target)
        {
            IRuleCodeTarget ruleCodeTarget = target as IRuleCodeTarget;
            if (this.Criteria.ExcludedCodes != null && this.Criteria.ExcludedCodes.Contains(ruleCodeTarget.Code))
                return true;
            return false;
        }

        IEnumerable<long> IRuleSaleZoneCriteria.SaleZoneIds
        {
            get
            {
                if(this.Criteria != null && this.Criteria.SaleZoneGroupSettings != null)
                {
                    return GetSaleZoneGroupContext().GetGroupZoneIds(this.Criteria.SaleZoneGroupSettings);
                }
                else
                {
                    return null;
                }
            }
        }
        
        public ISaleZoneGroupContext GetSaleZoneGroupContext()
        {
            ISaleZoneGroupContext saleZoneGroupContext = ContextFactory.CreateContext<ISaleZoneGroupContext>();
            saleZoneGroupContext.RoutingProductId = this.Criteria.RoutingProductId;
            return saleZoneGroupContext;
        }


        IEnumerable<CodeCriteria> IRuleCodeCriteria.CodeCriterias
        {
            get { return this.Criteria != null && this.Criteria.CodeCriteriaGroupSettings != null ? this.Criteria.CodeCriteriaGroupSettings.GetCodeCriterias(null) : null; }
        }

        IEnumerable<int> IRuleCustomerCriteria.CustomerIds
        {
            get { return this.Criteria != null && this.Criteria.CustomerGroupSettings != null ? this.Criteria.CustomerGroupSettings.GetCustomerIds(null) : null; }
        }

        IEnumerable<int> IRuleRoutingProductCriteria.RoutingProductIds
        {
            get { return this.Criteria != null && this.Criteria.RoutingProductId.HasValue ? new List<int> { this.Criteria.RoutingProductId.Value} : null; }
        }
    }
}
