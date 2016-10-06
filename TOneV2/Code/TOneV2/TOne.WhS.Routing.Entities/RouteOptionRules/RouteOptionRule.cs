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
        public RouteOptionRuleCriteria Criteria { get; set; }

        public RouteOptionRuleSettings Settings { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public override bool IsAnyCriteriaExcluded(object target)
        {
            IRuleCodeTarget ruleCodeTarget = target as IRuleCodeTarget;
            if (this.Criteria.ExcludedCodes != null && this.Criteria.ExcludedCodes.Contains(ruleCodeTarget.Code))
                return true;
            return false;
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
                        ISupplierZoneManager supplierZoneManager = BEManagerFactory.GetManager<ISupplierZoneManager>();
                        return suppliersWithZones.SelectMany(itm => itm.SupplierZoneIds != null ? itm.SupplierZoneIds : supplierZoneManager.GetSupplierZoneIdsByDates(itm.SupplierId, BeginEffectiveTime, EndEffectiveTime));
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

        public override void RefreshRuleState(Vanrise.Rules.IRefreshRuleStateContext context)
        {
            this.Settings.RefreshState(context);
        }
    }
}
