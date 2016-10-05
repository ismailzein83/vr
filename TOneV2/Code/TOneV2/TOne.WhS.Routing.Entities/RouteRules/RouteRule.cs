using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteRule : Vanrise.Rules.BaseRule, IRuleCustomerCriteria, IRuleCodeCriteria, IRuleSaleZoneCriteria, IRuleRoutingProductCriteria
    {
        public string SourceId { get; set; }
        public RouteRuleCriteria Criteria { get; set; }        

        public RouteRuleSettings Settings { get; set; }

        public string Name { get; set; }

        public override bool IsAnyCriteriaExcluded(object target)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            IRuleCodeTarget ruleCodeTarget = target as IRuleCodeTarget;
            if (ruleCodeTarget == null)
                throw new Exception(String.Format("target is not of type IRuleCodeTarget. it is of type '{0}'", target.GetType()));
            if (this.Criteria.ExcludedCodes != null && this.Criteria.ExcludedCodes.Contains(ruleCodeTarget.Code))
                return true;
            return false;
        }

        public ISaleZoneGroupContext GetSaleZoneGroupContext()
        {
            ISaleZoneGroupContext context = ContextFactory.CreateContext<ISaleZoneGroupContext>();
            context.FilterSettings = new SaleZoneFilterSettings
            {
                RoutingProductId = this.Criteria.RoutingProductId
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

        public ICodeCriteriaGroupContext GetCodeCriteriaGroupContext()
        {
            ICodeCriteriaGroupContext context = ContextFactory.CreateContext<ICodeCriteriaGroupContext>();
            return context;
        }


        IEnumerable<CodeCriteria> IRuleCodeCriteria.CodeCriterias
        {
            get
            {
                if (this.Criteria != null && this.Criteria.CodeCriteriaGroupSettings != null)
                {
                    return GetCodeCriteriaGroupContext().GetGroupCodeCriterias(this.Criteria.CodeCriteriaGroupSettings);
                }
                else
                {
                    return null;
                }
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

        IEnumerable<int> IRuleRoutingProductCriteria.RoutingProductIds
        {
            get { return this.Criteria != null && this.Criteria.RoutingProductId.HasValue ? new List<int> { this.Criteria.RoutingProductId.Value} : null; }
        }
    }
}
