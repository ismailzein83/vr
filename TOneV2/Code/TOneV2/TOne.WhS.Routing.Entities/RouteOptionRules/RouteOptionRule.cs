using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Entities
{
    public struct RouteOptionRuleIdentifier
    {
        public int CustomerId { get; set; }

        public string Code { get; set; }

        public int SupplierId { get; set; }

        public long SupplierZoneId { get; set; }
    }

    public class RouteOptionRule : Vanrise.Rules.BaseRule, IRuleSupplierCriteria, IRuleSupplierZoneCriteria, IRuleCustomerCriteria, IRuleCodeCriteria, IRuleSaleZoneCriteria, IRuleRoutingProductCriteria, IDateEffectiveSettings
    {
        public string Name { get; set; }

        public RouteOptionRuleCriteria Criteria { get; set; }

        public RouteOptionRuleSettings Settings { get; set; }

        public override bool HasAdditionalInformation { get { return true; } }

        public override void UpdateAdditionalInformation(Vanrise.Rules.BaseRule existingRule, ref Vanrise.Rules.AdditionalInformation additionalInformation)
        {
            RouteOptionRuleAdditionalInformation routeOptionRuleAdditionalInformation;
            if (additionalInformation != null)
                routeOptionRuleAdditionalInformation = additionalInformation.CastWithValidate<RouteOptionRuleAdditionalInformation>("additionalInformation");
            else
                routeOptionRuleAdditionalInformation = new RouteOptionRuleAdditionalInformation();

            if (existingRule != null)
            {
                RouteOptionRule existingRouteOptionRule = existingRule.CastWithValidate<RouteOptionRule>("existingRule", existingRule.RuleId);

                string existingBaseRouteOptionRuleCriteria = Vanrise.Common.Serializer.Serialize(existingRouteOptionRule.Criteria);
                string newBaseRouteOptionRuleCriteria = Vanrise.Common.Serializer.Serialize(this.Criteria);
                if (string.Compare(existingBaseRouteOptionRuleCriteria, newBaseRouteOptionRuleCriteria) != 0)
                    routeOptionRuleAdditionalInformation.CriteriaHasChanged = true;

                string existingSettings = Vanrise.Common.Serializer.Serialize(existingRouteOptionRule.Settings);
                string newSettings = Vanrise.Common.Serializer.Serialize(this.Settings);
                if (string.Compare(existingSettings, newSettings) != 0)
                    routeOptionRuleAdditionalInformation.SettingsHasChanged = true;
            }
            else
            {
                routeOptionRuleAdditionalInformation.CriteriaHasChanged = true;
                routeOptionRuleAdditionalInformation.SettingsHasChanged = true;
            }

            additionalInformation = routeOptionRuleAdditionalInformation;
        }

        public override bool IsAnyCriteriaExcluded(object target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            RouteOptionRuleTarget routeOptionRuleTarget = target as RouteOptionRuleTarget;
            if (routeOptionRuleTarget == null)
                throw new Exception(String.Format("target is not of type RouteOptionRuleTarget. it is of type '{0}'", target.GetType()));

            if (this.Criteria != null && this.Criteria.ExcludedDestinations != null)
            {
                RoutingExcludedDestinationContext context = new RoutingExcludedDestinationContext() { Code = routeOptionRuleTarget.RouteTarget.Code };
                if (this.Criteria.ExcludedDestinations.IsExcludedDestination(context))
                    return true;
            }

            if (routeOptionRuleTarget.IsEffectiveInFuture)
            {
                ISupplierZoneManager supplierZoneManager = BEManagerFactory.GetManager<ISupplierZoneManager>();
                SupplierZone supplierZone = supplierZoneManager.GetSupplierZone(routeOptionRuleTarget.SupplierZoneId);

                if (supplierZone == null)
                    throw new NullReferenceException(string.Format("supplierZone. routeOptionRuleTarget.SupplierZoneId: {0}", routeOptionRuleTarget.SupplierZoneId));


                if (this.EndEffectiveTime.HasValue && this.EndEffectiveTime.Value < supplierZone.BED)
                    return true;

                if (supplierZone.EED.HasValue && this.BeginEffectiveTime > supplierZone.EED.Value)
                    return true;

                ISaleZoneManager saleZoneManager = BEManagerFactory.GetManager<ISaleZoneManager>();
                SaleZone saleZone = saleZoneManager.GetSaleZone(routeOptionRuleTarget.RouteTarget.SaleZoneId);

                if (saleZone == null)
                    throw new NullReferenceException(string.Format("saleZone. routeOptionRuleTarget.RouteTarget.SaleZoneId: {0}", routeOptionRuleTarget.RouteTarget.SaleZoneId));

                if (this.EndEffectiveTime.HasValue && this.EndEffectiveTime.Value < saleZone.BED)
                    return true;

                if (saleZone.EED.HasValue && this.BeginEffectiveTime > saleZone.EED.Value)
                    return true;
            }
            return base.IsAnyCriteriaExcluded(target);
        }

        public ISaleZoneGroupContext GetSaleZoneGroupContext()
        {
            ISaleZoneGroupContext saleZoneGroupContext = ContextFactory.CreateContext<ISaleZoneGroupContext>();
            saleZoneGroupContext.FilterSettings = new SaleZoneFilterSettings
            {
                RoutingProductId = this.Criteria.RoutingProductId
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

        public ICodeCriteriaGroupContext GetCodeCriteriaGroupContext()
        {
            ICodeCriteriaGroupContext context = ContextFactory.CreateContext<ICodeCriteriaGroupContext>();
            return context;
        }

        public ISuppliersWithZonesGroupContext GetSuppliersWithZonesGroupContext()
        {
            ISuppliersWithZonesGroupContext groupContext = ContextFactory.CreateContext<ISuppliersWithZonesGroupContext>();
            groupContext.FilterSettings = new SupplierFilterSettings
            {
                RoutingProductId = this.Criteria.RoutingProductId
            };
            return groupContext;
        }

        public override void RefreshRuleState(Vanrise.Rules.IRefreshRuleStateContext context)
        {
            this.Settings.RefreshState(context);
        }


        IEnumerable<int> IRuleSupplierCriteria.SupplierIds
        {
            get
            {
                if (this.Criteria != null && this.Criteria.SuppliersWithZonesGroupSettings != null)
                {
                    var suppliersWithZones = this.GetSuppliersWithZonesGroupContext().GetSuppliersWithZones(this.Criteria.SuppliersWithZonesGroupSettings);
                    if (suppliersWithZones != null)
                        return suppliersWithZones.Select(itm => itm.SupplierId);
                }
                return null;
            }
        }

        IEnumerable<long> IRuleSupplierZoneCriteria.SupplierZoneIds
        {
            get
            {
                if (this.Criteria != null && this.Criteria.SuppliersWithZonesGroupSettings != null)
                {
                    var suppliersWithZones = this.GetSuppliersWithZonesGroupContext().GetSuppliersWithZones(this.Criteria.SuppliersWithZonesGroupSettings);
                    if (suppliersWithZones != null)
                    {
                        bool hasSupplierWithSpecificZones = false;
                        bool hasSupplierWithAllZones = false;

                        List<long> suppliersZoneIds = new List<long>();
                        foreach (var supplierWithZones in suppliersWithZones)
                        {
                            if (supplierWithZones.SupplierZoneIds == null || supplierWithZones.SupplierZoneIds.Count == 0)
                            {
                                hasSupplierWithAllZones = true;
                            }
                            else
                            {
                                suppliersZoneIds.AddRange(supplierWithZones.SupplierZoneIds);
                                hasSupplierWithSpecificZones = true;
                            }

                            if (hasSupplierWithAllZones && hasSupplierWithSpecificZones)
                                throw new Exception(string.Format("Rule Id:{0} contains Suppliers with All and Specific Zones.", RuleId));
                        }
                        return hasSupplierWithAllZones ? null : suppliersZoneIds;
                    }
                }
                return null;
            }
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

    public interface ILinkedRouteOptionRuleContext
    {
    }

    public class LinkedRouteOptionRuleContext : ILinkedRouteOptionRuleContext
    {
    }

    public class RouteOptionRuleAdditionalInformation : Vanrise.Rules.AdditionalInformation
    {
        public bool CriteriaHasChanged { get; set; }

        public bool SettingsHasChanged { get; set; }
    }
}
