using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business.RouteRules;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RouteBuildManager
    {
        #region Public Methods

        public IEnumerable<CarrierAccount> GetCustomersForRouting()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CarrierAccount> GetSuppliersForRouting()
        {
            throw new NotImplementedException();
        }

        public List<CustomerZoneRate> GetCustomerZoneRates(int customerId, DateTime effectiveOn, bool isEffectiveInFuture)
        {
            List<CustomerZoneRate> customerZoneRates = new List<CustomerZoneRate>();

            CustomerZoneManager customerZoneManager = new CustomerZoneManager();
            CustomerZones customerZones = customerZoneManager.GetCustomerZones(customerId, effectiveOn, isEffectiveInFuture);
            
            SalePriceListRatesByOwner ratesByOwner = GetRatesByOwner(effectiveOn, isEffectiveInFuture);

            SalePriceListRatesByZone customerRates;
            ratesByOwner.RatesByCustomers.TryGetValue(customerId, out customerRates);

            CustomerPricingProductManager customerSellingProductManager = new CustomerPricingProductManager();
            CustomerPricingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, effectiveOn, isEffectiveInFuture);
            SalePriceListRatesByZone sellingProductRates = null;
            if (customerSellingProduct != null)
                ratesByOwner.RatesByPricingProduct.TryGetValue(customerSellingProduct.PricingProductId, out sellingProductRates);

            SalePricingRuleManager salePricingRuleManager = new SalePricingRuleManager();
            foreach(var customerZone in customerZones.Zones)
            {
                bool isPricingProductRate = false;
                SalePriceListRate zoneRate;
                if (!customerRates.TryGetValue(customerZone.ZoneId, out zoneRate))
                {
                    if (sellingProductRates != null)
                    {
                        if (sellingProductRates.TryGetValue(customerZone.ZoneId, out zoneRate))
                            isPricingProductRate = true;
                    }
                }
                if(zoneRate != null)
                {
                    SalePricingRulesInput salePricingRulesInput = new SalePricingRulesInput
                    {
                        CustomerId = customerId,
                        SaleZoneId = customerZone.ZoneId,
                        Rate = zoneRate,
                        EffectiveOn = effectiveOn,
                        IsEffectiveInFuture = isEffectiveInFuture
                    };
                    var pricingRulesResult = salePricingRuleManager.ApplyPricingRules(salePricingRulesInput);

                    CustomerZoneRate customerZoneRate = new CustomerZoneRate
                    {
                        CustomerId = customerId,
                        RoutingProductId = zoneRate.RoutingProductId,
                        SellingProductId = isPricingProductRate ? customerSellingProduct.PricingProductId : (int?)null,
                        SaleZoneId = customerZone.ZoneId,
                        Rate = pricingRulesResult != null ? pricingRulesResult.Rate : zoneRate.NormalRate
                    };
                    customerZoneRates.Add(customerZoneRate);
                }
            }

            return customerZoneRates;
        }

        SalePriceListRatesByOwner GetRatesByOwner(DateTime effectiveOn, bool isEffectiveInFuture)
        {
            SaleRateManager saleRateManager = new SaleRateManager();
            var rates = saleRateManager.GetRates(effectiveOn, isEffectiveInFuture);
            throw new NotImplementedException();
        }

        public IEnumerable<CustomerRoute> BuildRoutes(IBuildCustomerRoutesContext context, string routeCode, out IEnumerable<PricingProductRoute> pricingProductRoutes)
        {
            List<CustomerRoute> customerRoutes = new List<CustomerRoute>();
            Dictionary<int, PricingProductRoute> pricingProductRoutesDic = new Dictionary<int, PricingProductRoute>();

            if (context.SaleCodeMatches != null && context.CustomerZoneRates != null)
            {                
                RouteRuleManager routeRuleManager = new RouteRuleManager();
                foreach(var saleCodeMatch in context.SaleCodeMatches)
                {
                    List<CustomerZoneRate> matchCustomerZoneRates;
                    if(context.CustomerZoneRates.TryGetValue(saleCodeMatch.SaleZoneId, out matchCustomerZoneRates))
                    {
                        foreach(var customerZoneRate in matchCustomerZoneRates)
                        {
                            var routeRuleTarget = new RouteRuleTarget
                            {
                                CustomerId = customerZoneRate.CustomerId,
                                Code = routeCode,
                                SaleZoneId = saleCodeMatch.SaleZoneId,
                                RoutingProductId = customerZoneRate.RoutingProductId,
                                SaleRate = customerZoneRate.Rate,
                                EffectiveOn = context.EntitiesEffectiveOn
                            };
                            var routeRule = routeRuleManager.GetMatchRule(routeRuleTarget);

                            if(routeRule != null)
                            {
                                bool createCustomerRoute;

                                CheckPricingProductRoute(out createCustomerRoute, context, routeCode, pricingProductRoutesDic, routeRuleManager, saleCodeMatch, customerZoneRate, routeRuleTarget, routeRule);

                                if(createCustomerRoute)
                                {
                                    CustomerRoute route = ExecuteRule<CustomerRoute>(routeCode, saleCodeMatch, customerZoneRate, context.SupplierCodeMatches, context.SupplierZoneRates, routeRuleTarget, routeRule);
                                    route.CustomerId = customerZoneRate.CustomerId;
                                    customerRoutes.Add(route);
                                }
                            }
                        }
                    }
                }
            }
            pricingProductRoutes = pricingProductRoutesDic.Values;
            return customerRoutes;
        }

        public IEnumerable<RoutingProductRoute> BuildRoutes(IBuildRoutingProductRoutesContext context, long saleZoneId)
        {
            List<RoutingProductRoute> routes = new List<RoutingProductRoute>();

            if (context.RoutingProductIds != null)
            {
                RouteRuleManager routeRuleManager = new RouteRuleManager();
                foreach (var routingProductId in context.RoutingProductIds)
                {
                    RouteRuleTarget routeRuleTarget = new RouteRuleTarget
                    {
                        RoutingProductId = routingProductId,
                        SaleZoneId = saleZoneId,
                        EffectiveOn = context.EntitiesEffectiveOn
                    };
                    var routeRule = routeRuleManager.GetMatchRule(routeRuleTarget);
                    if (routeRule != null)
                    {
                        RoutingProductRoute route = ExecuteRule(routingProductId, saleZoneId, context.SupplierCodeMatches, context.SupplierZoneRates, routeRuleTarget, routeRule);
                        routes.Add(route);
                    }
                }
            }

            return routes;
        }

        #endregion

        #region Private Methods

        private void CheckPricingProductRoute(out bool createCustomerRoute, IBuildCustomerRoutesContext context, string routeCode, Dictionary<int, PricingProductRoute> pricingProductRoutesDic, RouteRuleManager routeRuleManager, SaleCodeMatch saleCodeMatch, CustomerZoneRate customerZoneRate, RouteRuleTarget routeRuleTarget, RouteRule routeRule)
        {
            createCustomerRoute = true;
            //if same rule and rate is inherited from Pricing, then it should be same route as pricing product
            if (routeRule.Criteria.RoutingProductId.HasValue && customerZoneRate.SellingProductId.HasValue)
            {
                createCustomerRoute = false;
                PricingProductRoute pricingProductRoute;
                if (!pricingProductRoutesDic.TryGetValue(customerZoneRate.SellingProductId.Value, out pricingProductRoute))
                {
                    var pricingProductRouteRuleTarget = new RouteRuleTarget
                    {
                        Code = routeCode,
                        SaleZoneId = saleCodeMatch.SaleZoneId,
                        RoutingProductId = customerZoneRate.RoutingProductId,
                        SaleRate = customerZoneRate.Rate,
                        EffectiveOn = context.EntitiesEffectiveOn
                    };
                    var pricingProductRouteRule = routeRuleManager.GetMatchRule(pricingProductRouteRuleTarget);
                    if (pricingProductRouteRule != null)
                    {
                        PricingProductRoute route = ExecuteRule<PricingProductRoute>(routeCode, saleCodeMatch, customerZoneRate, context.SupplierCodeMatches, context.SupplierZoneRates, pricingProductRouteRuleTarget, pricingProductRouteRule);
                        route.PricingProductId = customerZoneRate.SellingProductId.Value;
                        pricingProductRoutesDic.Add(customerZoneRate.SellingProductId.Value, route);
                    }
                }
                if (pricingProductRoute == null)
                    createCustomerRoute = true;
                else
                {
                    //check if any option has a rule specific for the customer
                    if (pricingProductRoute.Options != null)
                    {
                        RouteOptionRuleManager routeOptionRuleManager = new RouteOptionRuleManager();
                        foreach (var option in pricingProductRoute.Options)
                        {
                            RouteOptionRuleTarget routeOptionRuleTarget = new RouteOptionRuleTarget
                            {
                                SupplierId = option.SupplierId,
                                SupplierCode = option.SupplierCode,
                                SupplierZoneId = option.SupplierZoneId,
                                EffectiveOn = context.EntitiesEffectiveOn,
                                RouteTarget = routeRuleTarget,
                                SupplierRate = option.SupplierRate
                            };
                            var matchOptionRule = routeOptionRuleManager.GetMatchRule(routeOptionRuleTarget);
                            int? matchOptionRuleId = matchOptionRule != null ? matchOptionRule.RuleId : default(int?);
                            if (matchOptionRuleId != option.ExecutedRuleId)
                            {
                                createCustomerRoute = true;
                                break;
                            }
                        }
                    }
                }
            }                
        }

        private T ExecuteRule<T>(string routeCode, SaleCodeMatch saleCodeMatch, CustomerZoneRate customerZoneRate, SupplierCodeMatchBySupplier supplierCodeMatches, SupplierZoneRatesByZone supplierZoneRates, RouteRuleTarget routeRuleTarget, RouteRule routeRule)
            where T : BaseRoute
        {
            RouteRuleExecutionContext routeRuleExecutionContext = new RouteRuleExecutionContext(routeRule);
            routeRuleExecutionContext.NumberOfOptions = 5;
            routeRuleExecutionContext.SupplierCodeMatches = supplierCodeMatches;
            routeRuleExecutionContext.SupplierZoneRates = supplierZoneRates;

            routeRule.Settings.Execute(routeRuleExecutionContext, routeRuleTarget);
            T route = Activator.CreateInstance<T>();
            route.Code = routeCode;
            route.SaleZoneId = saleCodeMatch.SaleZoneId;
            route.ExecutedRuleId = routeRule.RuleId;
            route.Rate = customerZoneRate.Rate;
            route.IsBlocked = routeRuleTarget.BlockRoute;

            if (routeRuleExecutionContext._options != null)
            {
                route.Options = new List<RouteOption>();
                foreach (var targetOption in routeRuleExecutionContext._options)
                {
                    RouteOption routeOption = new RouteOption
                    {
                        SupplierId = targetOption.SupplierId,
                        SupplierCode = targetOption.SupplierCode,
                        SupplierZoneId = targetOption.SupplierZoneId,
                        SupplierRate = targetOption.SupplierRate,
                        Percentage = targetOption.Percentage,
                        IsBlocked = targetOption.BlockOption,
                        ExecutedRuleId = targetOption.ExecutedRuleId
                    };
                    route.Options.Add(routeOption);
                }
            }
            return route;
        }

        private RoutingProductRoute ExecuteRule(int routingProductId, long saleZoneId, SupplierCodeMatchBySupplier supplierCodeMatches, SupplierZoneRatesByZone supplierZoneRates, RouteRuleTarget routeRuleTarget, RouteRule routeRule)
        {
            RouteRuleExecutionContext routeRuleExecutionContext = new RouteRuleExecutionContext(routeRule);
            routeRuleExecutionContext.SupplierCodeMatches = supplierCodeMatches;
            routeRuleExecutionContext.SupplierZoneRates = supplierZoneRates;

            routeRule.Settings.Execute(routeRuleExecutionContext, routeRuleTarget);
            RoutingProductRoute route = new RoutingProductRoute
            {
                RoutingProductId = routingProductId,
                SaleZoneId = saleZoneId,
                ExecutedRuleId = routeRule.RuleId,
                IsBlocked = routeRuleTarget.BlockRoute
            };
            if (routeRuleExecutionContext._options != null)
            {
                route.Options = new List<RoutingProductRouteOption>();
                foreach (var targetOption in routeRuleExecutionContext._options)
                {
                    RoutingProductRouteOption routeOption = new RoutingProductRouteOption
                    {
                        SupplierId = targetOption.SupplierId,
                        SupplierCode = targetOption.SupplierCode,
                        SupplierZoneId = targetOption.SupplierZoneId,
                        SupplierRate = targetOption.SupplierRate,
                        Percentage = targetOption.Percentage,
                        IsBlocked = targetOption.BlockOption,
                        ExecutedRuleId = targetOption.ExecutedRuleId
                    };
                    route.Options.Add(routeOption);
                }
            }
            return route;
        }

        #endregion
    }

    public class SalePriceListRatesByOwner
    {
        public Dictionary<int, SalePriceListRatesByZone> RatesByCustomers { get; set; }

        public Dictionary<int, SalePriceListRatesByZone> RatesByPricingProduct { get; set; }
    }

    public class SalePriceListRatesByZone : Dictionary<long, SalePriceListRate>
    {

    }
}
