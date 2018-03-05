using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public struct RouteRuleIdentifier
    {
        public int CustomerId { get; set; }
        public string Code { get; set; }

        public override int GetHashCode()
        {
            return CustomerId.GetHashCode() + Code.GetHashCode();
        }
    }

    public class RouteRule : Vanrise.Rules.BaseRule, IRuleCustomerCriteria, IRuleCodeCriteria, IRuleSaleZoneCriteria, IRuleRoutingProductCriteria, IDateEffectiveSettings, IRuleCountryCriteria
    {
        public string Name { get; set; }

        public BaseRouteRuleCriteria Criteria { get; set; }

        public RouteRuleSettings Settings { get; set; }

        public string SourceId { get; set; }

        public CorrespondentType CorrespondentType { get { return Settings.CorrespondentType; } }

        public override bool HasAdditionalInformation { get { return true; } }

        public override bool IsAnyCriteriaExcluded(object target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            RouteRuleTarget routeRuleTarget = target as RouteRuleTarget;
            if (routeRuleTarget == null)
                throw new Exception(String.Format("target is not of type RouteRuleTarget. it is of type '{0}'", target.GetType()));

            if (this.Criteria != null)
            {
                RoutingExcludedDestinations routingExcludedDestinations = this.Criteria.GetExcludedDestinations();
                if (routingExcludedDestinations != null)
                {
                    RoutingExcludedDestinationContext context = new RoutingExcludedDestinationContext(routeRuleTarget.Code, base.RuleId);
                    if (routingExcludedDestinations.IsExcludedDestination(context))
                        return true;
                }
            }

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

        public override void UpdateAdditionalInformation(Vanrise.Rules.BaseRule existingRule, ref Vanrise.Rules.AdditionalInformation additionalInformation)
        {
            RouteRuleAdditionalInformation routeRuleAdditionalInformation = new RouteRuleAdditionalInformation();
            if (additionalInformation != null)
            {
                RouteRuleAdditionalInformation existingAdditionalInformation = additionalInformation.CastWithValidate<RouteRuleAdditionalInformation>("RouteRuleAdditionalInformation");
                routeRuleAdditionalInformation.CriteriaHasChanged = existingAdditionalInformation.CriteriaHasChanged;
                routeRuleAdditionalInformation.SettingsHasChanged = existingAdditionalInformation.SettingsHasChanged;
            }

            if (existingRule != null)
            {
                RouteRule existingRouteRule = existingRule.CastWithValidate<RouteRule>("RouteRule", existingRule.RuleId);

                string existingBaseRouteRuleCriteria = Vanrise.Common.Serializer.Serialize(existingRouteRule.Criteria);
                string newBaseRouteRuleCriteria = Vanrise.Common.Serializer.Serialize(this.Criteria);
                if (string.Compare(existingBaseRouteRuleCriteria, newBaseRouteRuleCriteria) != 0)
                    routeRuleAdditionalInformation.CriteriaHasChanged = true;

                string existingSettings = Vanrise.Common.Serializer.Serialize(existingRouteRule.Settings);
                string newSettings = Vanrise.Common.Serializer.Serialize(this.Settings);
                if (string.Compare(existingSettings, newSettings) != 0)
                    routeRuleAdditionalInformation.SettingsHasChanged = true;
            }
            else
            {
                routeRuleAdditionalInformation.CriteriaHasChanged = true;
                routeRuleAdditionalInformation.SettingsHasChanged = true;
            }

            additionalInformation = routeRuleAdditionalInformation;
        }

        public ISaleZoneGroupContext GetSaleZoneGroupContext()
        {
            ISaleZoneGroupContext context = ContextFactory.CreateContext<ISaleZoneGroupContext>();
            context.FilterSettings = new SaleZoneFilterSettings
            {
                RoutingProductId = this.Criteria.GetRoutingProductId()
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

        public ICountryCriteriaGroupContext GetCountryCriteriaGroupContext()
        {
            return new CountryCriteriaGroupContext();
        }

        IEnumerable<CodeCriteria> IRuleCodeCriteria.CodeCriterias
        {
            get
            {
                if (this.Criteria == null)
                    return null;

                CodeCriteriaGroupSettings codeCriteriaGroupSettings = this.Criteria.GetCodeCriteriaGroupSettings();
                if (codeCriteriaGroupSettings == null)
                    return null;

                return GetCodeCriteriaGroupContext().GetGroupCodeCriterias(codeCriteriaGroupSettings);
            }
        }

        IEnumerable<long> IRuleSaleZoneCriteria.SaleZoneIds
        {
            get
            {
                if (this.Criteria == null)
                    return null;

                SaleZoneGroupSettings saleZoneGroupSettings = this.Criteria.GetSaleZoneGroupSettings();
                if (saleZoneGroupSettings == null)
                    return null;

                return GetSaleZoneGroupContext().GetGroupZoneIds(saleZoneGroupSettings);
            }
        }

        IEnumerable<int> IRuleCountryCriteria.CountryIds
        {
            get
            {
                if (this.Criteria == null)
                    return null;

                CountryCriteriaGroupSettings countryCriteriaGroupSettings = this.Criteria.GetCountryCriteriaGroupSettings();
                if (countryCriteriaGroupSettings == null)
                    return null;

                return countryCriteriaGroupSettings.GetCountryIds(GetCountryCriteriaGroupContext());
            }
        }

        IEnumerable<int> IRuleCustomerCriteria.CustomerIds
        {
            get
            {
                if (this.Criteria == null)
                    return null;

                CustomerGroupSettings customerGroupSettings = this.Criteria.GetCustomerGroupSettings();
                if (customerGroupSettings == null)
                    return null;

                return this.GetCustomerGroupContext().GetGroupCustomerIds(customerGroupSettings);
            }
        }

        IEnumerable<int> IRuleRoutingProductCriteria.RoutingProductIds
        {
            get
            {
                if (this.Criteria == null)
                    return null;

                int? routingProductId = this.Criteria.GetRoutingProductId();
                if (!routingProductId.HasValue)
                    return null;

                return new List<int> { routingProductId.Value };
            }
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

    public class RouteRuleAdditionalInformation : Vanrise.Rules.AdditionalInformation
    {
        public bool CriteriaHasChanged { get; set; }

        public bool SettingsHasChanged { get; set; }
    }
}
