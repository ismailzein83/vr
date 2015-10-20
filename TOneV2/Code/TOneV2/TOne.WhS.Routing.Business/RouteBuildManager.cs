using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business.RouteRules;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

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

        public void  BuildCustomerZoneRates(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture, Action<CustomerZoneRate> onCustomerZoneRateAvailable)
        {            
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
                    onCustomerZoneRateAvailable(customerZoneRate);
                }
            }
        }

        public void BuildSupplierZoneRates(DateTime? effectiveOn, bool isEffectiveInFuture, Action<SupplierZoneRate> onSupplierZoneRateAvailable)
        {
            SupplierRateManager supplierRateManager = new SupplierRateManager();
            var supplierRates = supplierRateManager.GetRates(effectiveOn, isEffectiveInFuture);
            SupplierPriceListManager supplierPriceListManager = new SupplierPriceListManager();
            PurchasePricingRuleManager purchasePricingRuleManager = new PurchasePricingRuleManager();
            if(supplierRates != null)
            {
                foreach(var supplierRate in supplierRates)
                {
                    var priceList = supplierPriceListManager.GetPriceList(supplierRate.PriceListId);
                    PurchasePricingRulesInput purchasePricingRulesInput = new PurchasePricingRulesInput
                    {
                        SupplierId = priceList.SupplierId,
                        SupplierZoneId = supplierRate.ZoneId,
                        Rate = supplierRate,
                        EffectiveOn = effectiveOn,
                        IsEffectiveInFuture = isEffectiveInFuture
                    };
                    var pricingRulesResult = purchasePricingRuleManager.ApplyPricingRules(purchasePricingRulesInput);

                    SupplierZoneRate supplierZoneRate = new SupplierZoneRate
                    {
                        SupplierId = priceList.SupplierId,
                        SupplierZoneId = supplierRate.ZoneId,
                        Rate = pricingRulesResult != null ? pricingRulesResult.Rate : supplierRate.NormalRate
                    };
                    onSupplierZoneRateAvailable(supplierZoneRate);
                }
            }
        }
        
        public void BuildCodeMatches(IEnumerable<SaleCode> saleCodes, IEnumerable<SupplierCode> supplierCodes, Action<CodeMatches> onCodeMatchesAvailable)
        {
            List<SaleCodeIterator> saleCodeIterators;
            HashSet<string> distinctSaleCodes;
            List<SupplierCodeIterator> supplierCodeIterators;
            HashSet<string> distinctSupplierCodes;
            StructuresSaleCodes(saleCodes, out saleCodeIterators, out distinctSaleCodes);
            StructuresSupplierCodes(supplierCodes, out supplierCodeIterators, out distinctSupplierCodes);

            foreach(string code in distinctSupplierCodes)
            {
                BuildAndAddCodeMatches(code, false, saleCodeIterators, supplierCodeIterators, onCodeMatchesAvailable);
            }
            foreach (string code in distinctSaleCodes)
            {
                if (distinctSupplierCodes.Contains(code))
                    continue;
                BuildAndAddCodeMatches(code, true, saleCodeIterators, supplierCodeIterators, onCodeMatchesAvailable);
            }
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

        SalePriceListRatesByOwner GetRatesByOwner(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            SaleRateManager saleRateManager = new SaleRateManager();
            var rates = saleRateManager.GetRates(effectiveOn, isEffectiveInFuture);
            throw new NotImplementedException();
        }


        private void BuildAndAddCodeMatches(string code, bool isSaleCode, List<SaleCodeIterator> saleCodeIterators, List<SupplierCodeIterator> supplierCodeIterators, Action<CodeMatches> onCodeMatchesAvailable)
        {
            List<SaleCodeMatch> saleCodeMatches = new List<SaleCodeMatch>();
            foreach (var saleCodeIterator in saleCodeIterators)
            {
                SaleCode matchSaleCode = isSaleCode ? saleCodeIterator.CodeIterator.GetExactMatch(code) : saleCodeIterator.CodeIterator.GetLongestMatch(code);
                if (matchSaleCode != null)
                    saleCodeMatches.Add(new SaleCodeMatch
                    {
                        SaleCode = matchSaleCode.Code,
                        SaleZoneId = matchSaleCode.ZoneId,
                        SellingNumberPlanId = saleCodeIterator.SellingNumberPlanId
                    });
            }
            if (saleCodeMatches.Count > 0)
            {
                SupplierCodeMatchBySupplier supplierCodeMatches = new SupplierCodeMatchBySupplier();
                foreach (var supplierCodeIterator in supplierCodeIterators)
                {
                    SupplierCode matchSupplierCode = supplierCodeIterator.CodeIterator.GetLongestMatch(code);
                    if (matchSupplierCode != null)
                        supplierCodeMatches.Add(supplierCodeIterator.SupplierId,
                            new List<SupplierCodeMatch> 
                            { 
                                new  SupplierCodeMatch
                                {
                                    SupplierId = supplierCodeIterator.SupplierId,
                                    SupplierCode = matchSupplierCode.Code,
                                    SupplierZoneId = matchSupplierCode.ZoneId
                                }
                            });
                }
                CodeMatches codeMatches = new CodeMatches
                {
                    Code = code,
                    SaleCodeMatches = saleCodeMatches,
                    SupplierCodeMatches = supplierCodeMatches
                };
                onCodeMatchesAvailable(codeMatches);
            }
        }

        void StructuresSaleCodes(IEnumerable<SaleCode> saleCodes, out List<SaleCodeIterator> saleCodeIterators, out HashSet<string> distinctSaleCodes)
        {
            saleCodeIterators = new List<SaleCodeIterator>();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            distinctSaleCodes = new HashSet<string>();
            Dictionary<int, List<SaleCode>> saleCodesBySellingNumberPlan = new Dictionary<int, List<SaleCode>>();
            foreach (var saleCode in saleCodes)
            {
                distinctSaleCodes.Add(saleCode.Code);
                SaleZone saleZone = saleZoneManager.GetSaleZone(saleCode.ZoneId);
                List<SaleCode> currentSaleCodes = saleCodesBySellingNumberPlan.GetOrCreateItem(saleZone.SellingNumberPlanId);
                currentSaleCodes.Add(saleCode);
            }
            foreach (var saleCodeEntry in saleCodesBySellingNumberPlan)
            {
                SaleCodeIterator codeIterator = new SaleCodeIterator
                {
                    SellingNumberPlanId = saleCodeEntry.Key,
                    CodeIterator = new CodeIterator<SaleCode>(saleCodeEntry.Value)
                };
                saleCodeIterators.Add(codeIterator);
            }
        }

        void StructuresSupplierCodes(IEnumerable<SupplierCode> supplierCodes, out List<SupplierCodeIterator> supplierCodeIterators, out HashSet<string> distinctSupplierCodes)
        {
            supplierCodeIterators = new List<SupplierCodeIterator>();
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
            distinctSupplierCodes = new HashSet<string>();
            Dictionary<int, List<SupplierCode>> supplierCodesBySupplier = new Dictionary<int, List<SupplierCode>>();
            foreach (var supplierCode in supplierCodes)
            {
                distinctSupplierCodes.Add(supplierCode.Code);
                SupplierZone supplierZone = supplierZoneManager.GetSupplierZone(supplierCode.ZoneId);
                List<SupplierCode> currentSupplierCodes = supplierCodesBySupplier.GetOrCreateItem(supplierZone.SupplierId);
                currentSupplierCodes.Add(supplierCode);
            }
            foreach (var supplierCodeEntry in supplierCodesBySupplier)
            {
                SupplierCodeIterator codeIterator = new SupplierCodeIterator
                {
                    SupplierId = supplierCodeEntry.Key,
                    CodeIterator = new CodeIterator<SupplierCode>(supplierCodeEntry.Value)
                };
                supplierCodeIterators.Add(codeIterator);
            }
        }

        #endregion

        #region Private Classes

        private class SaleCodeIterator
        {
            public int SellingNumberPlanId { get; set; }

            public CodeIterator<SaleCode> CodeIterator { get; set; }
        }

        private class SupplierCodeIterator
        {
            public int SupplierId { get; set; }

            public CodeIterator<SupplierCode> CodeIterator { get; set; }
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
