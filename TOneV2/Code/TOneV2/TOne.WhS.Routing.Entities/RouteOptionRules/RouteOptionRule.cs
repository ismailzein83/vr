using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteOptionRule : Vanrise.Rules.BaseRule, IRuleSupplierCriteria, IRuleSupplierZoneCriteria, IRuleCustomerCriteria, IRuleCodeCriteria, IRuleSaleZoneCriteria, IRuleRoutingProductCriteria
    {
        public string Name { get; set; }

        public RouteOptionRuleCriteria Criteria { get; set; }

        public RouteOptionRuleSettings Settings { get; set; }

        public override bool IsAnyCriteriaExcluded(object target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            RouteOptionRuleTarget routeOptionRuleTarget = target as RouteOptionRuleTarget;
            if (routeOptionRuleTarget == null)
                throw new Exception(String.Format("target is not of type RouteOptionRuleTarget. it is of type '{0}'", target.GetType()));

            if (this.Criteria.ExcludedCodes != null && this.Criteria.ExcludedCodes.Contains(routeOptionRuleTarget.RouteTarget.Code))
                return true;

            if (routeOptionRuleTarget.IsEffectiveInFuture)
            {
                ISupplierZoneManager supplierZoneManager = BEManagerFactory.GetManager<ISupplierZoneManager>();
                SupplierZone supplierZone = supplierZoneManager.GetSupplierZone(routeOptionRuleTarget.SupplierZoneId);

                if (this.EndEffectiveTime.HasValue && this.EndEffectiveTime.Value < supplierZone.BED)
                    return true;

                if (supplierZone.EED.HasValue && this.BeginEffectiveTime > supplierZone.EED.Value)
                    return true;

                ISaleZoneManager saleZoneManager = BEManagerFactory.GetManager<ISaleZoneManager>();
                SaleZone saleZone = saleZoneManager.GetSaleZone(routeOptionRuleTarget.RouteTarget.SaleZoneId);

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
                        return suppliersWithZones.SelectMany(itm => itm.SupplierZoneIds != null ? itm.SupplierZoneIds : new List<long>());
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
    }
}
