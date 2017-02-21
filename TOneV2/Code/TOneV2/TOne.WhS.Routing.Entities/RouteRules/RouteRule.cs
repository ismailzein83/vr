using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public struct RouteRuleIdentifier
    {
        public int CustomerId { get; set; }
        public string Code { get; set; }
    }

    public class RouteRule : Vanrise.Rules.BaseRule, IRuleCustomerCriteria, IRuleCodeCriteria, IRuleSaleZoneCriteria, IRuleRoutingProductCriteria, IDateEffectiveSettings
    {
        public string Name { get; set; }

        public RouteRuleCriteria Criteria { get; set; }

        public RouteRuleSettings Settings { get; set; }

        public string SourceId { get; set; }

        public CorrespondentType CorrespondentType { get { return Settings.CorrespondentType; } }

        public override bool IsAnyCriteriaExcluded(object target)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            RouteRuleTarget routeRuleTarget = target as RouteRuleTarget;
            if (routeRuleTarget == null)
                throw new Exception(String.Format("target is not of type RouteRuleTarget. it is of type '{0}'", target.GetType()));
            if (this.Criteria.ExcludedCodes != null && this.Criteria.ExcludedCodes.Contains(routeRuleTarget.Code))
                return true;

            if (routeRuleTarget.IsEffectiveInFuture)
            {
                ISaleZoneManager saleZoneManager = BEManagerFactory.GetManager<ISaleZoneManager>();
                SaleZone saleZone = saleZoneManager.GetSaleZone(routeRuleTarget.SaleZoneId);

                if (saleZone == null)
                    throw new NullReferenceException(string.Format("saleZone. routeRuleTarget.SaleZoneId: {0}", routeRuleTarget.SaleZoneId));

                if (this.EndEffectiveTime.HasValue && this.EndEffectiveTime.Value < saleZone.BED)
                    return true;

                if (saleZone.EED.HasValue && this.BeginEffectiveTime > saleZone.EED.Value)
                    return true;
            }
            return base.IsAnyCriteriaExcluded(target);
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
            get { return this.Criteria != null && this.Criteria.RoutingProductId.HasValue ? new List<int> { this.Criteria.RoutingProductId.Value } : null; }
        }

        public DateTime BED
        {
            get { return BeginEffectiveTime; }
        }

        public DateTime? EED
        {
            get { return EndEffectiveTime; }
        }
    }

    public interface ILinkedRouteRuleContext
    {
        List<RouteOption> RouteOptions { get; }
    }

    public class LinkedRouteRuleContext : ILinkedRouteRuleContext
    {
        public List<RouteOption> RouteOptions { get; set; }
    }
}
