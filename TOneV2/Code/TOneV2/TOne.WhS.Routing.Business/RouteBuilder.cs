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
    public class RouteBuilder
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

       
        public IEnumerable<CustomerRoute> BuildRoutes(IBuildCustomerRoutesContext context, string routeCode, out IEnumerable<SellingProductRoute> sellingProductRoutes)
        {
            List<CustomerRoute> customerRoutes = new List<CustomerRoute>();
            Dictionary<int, SellingProductRoute> sellingProductRoutesDic = new Dictionary<int, SellingProductRoute>();

            if (context.SaleCodeMatches != null && context.CustomerZoneDetails != null)
            {                
                RouteRuleManager routeRuleManager = new RouteRuleManager();
                foreach(var saleCodeMatch in context.SaleCodeMatches)
                {
                    List<CustomerZoneDetail> matchCustomerZoneDetails;
                    if(context.CustomerZoneDetails.TryGetValue(saleCodeMatch.SaleZoneId, out matchCustomerZoneDetails))
                    {
                        foreach(var customerZoneDetail in matchCustomerZoneDetails)
                        {
                            var routeRuleTarget = new RouteRuleTarget
                            {
                                CustomerId = customerZoneDetail.CustomerId,
                                Code = routeCode,
                                SaleZoneId = saleCodeMatch.SaleZoneId,
                                RoutingProductId = customerZoneDetail.RoutingProductId,
                                SaleRate = customerZoneDetail.EffectiveRateValue,
                                EffectiveOn = context.EntitiesEffectiveOn
                            };
                            var routeRule = routeRuleManager.GetMatchRule(routeRuleTarget);

                            if(routeRule != null)
                            {
                                bool createCustomerRoute = true;

                               // CheckSellingProductRoute(out createCustomerRoute, context, routeCode, sellingProductRoutesDic, routeRuleManager, saleCodeMatch, customerZoneDetail, routeRuleTarget, routeRule);

                                if(createCustomerRoute)
                                {
                                    CustomerRoute route = ExecuteRule<CustomerRoute>(routeCode, saleCodeMatch, customerZoneDetail, context.SupplierCodeMatches, context.SupplierCodeMatchesBySupplier, routeRuleTarget, routeRule);
                                    route.CustomerId = customerZoneDetail.CustomerId;
                                    customerRoutes.Add(route);
                                }
                            }
                        }
                    }
                }
            }
            sellingProductRoutes = sellingProductRoutesDic.Values;
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
                        RoutingProductRoute route = ExecuteRule(routingProductId, saleZoneId, context.SupplierCodeMatches, context.SupplierCodeMatchesBySupplier, routeRuleTarget, routeRule);
                        routes.Add(route);
                    }
                }
            }

            return routes;
        }

        #endregion

        #region Private Methods

        private void CheckSellingProductRoute(out bool createCustomerRoute, IBuildCustomerRoutesContext context, string routeCode, Dictionary<int, SellingProductRoute> sellingProductRoutesDic, RouteRuleManager routeRuleManager, SaleCodeMatch saleCodeMatch, CustomerZoneDetail customerZoneDetail, RouteRuleTarget routeRuleTarget, RouteRule routeRule)
        {
            createCustomerRoute = true;
            //if same rule and rate is inherited from Pricing, then it should be same route as pricing product
            if (routeRule.Criteria.RoutingProductId.HasValue && customerZoneDetail.RateSource == SalePriceListOwnerType.SellingProduct)
            {
                createCustomerRoute = false;
                SellingProductRoute sellingProductRoute;
                if (!sellingProductRoutesDic.TryGetValue(customerZoneDetail.SellingProductId, out sellingProductRoute))
                {
                    var sellingProductRouteRuleTarget = new RouteRuleTarget
                    {
                        Code = routeCode,
                        SaleZoneId = saleCodeMatch.SaleZoneId,
                        RoutingProductId = customerZoneDetail.RoutingProductId,
                        SaleRate = customerZoneDetail.EffectiveRateValue,
                        EffectiveOn = context.EntitiesEffectiveOn,
                        IsEffectiveInFuture = context.EntitiesEffectiveInFuture
                    };
                    var sellingProductRouteRule = routeRuleManager.GetMatchRule(sellingProductRouteRuleTarget);
                    if (sellingProductRouteRule != null)
                    {
                        SellingProductRoute route = ExecuteRule<SellingProductRoute>(routeCode, saleCodeMatch, customerZoneDetail, context.SupplierCodeMatches, context.SupplierCodeMatchesBySupplier, sellingProductRouteRuleTarget, sellingProductRouteRule);
                        route.SellingProductId = customerZoneDetail.SellingProductId;
                        sellingProductRoutesDic.Add(customerZoneDetail.SellingProductId, route);
                    }
                }
                if (sellingProductRoute == null)
                    createCustomerRoute = true;
                else
                {
                    //check if any option has a rule specific for the customer
                    if (sellingProductRoute.Options != null)
                    {
                        RouteOptionRuleManager routeOptionRuleManager = new RouteOptionRuleManager();
                        foreach (var option in sellingProductRoute.Options)
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

        private T ExecuteRule<T>(string routeCode, SaleCodeMatch saleCodeMatch, CustomerZoneDetail customerZoneDetail, List<SupplierCodeMatchWithRate> supplierCodeMatches, SupplierCodeMatchWithRateBySupplier supplierCodeMatchBySupplier, RouteRuleTarget routeRuleTarget, RouteRule routeRule)
            where T : BaseRoute
        {
            RouteRuleExecutionContext routeRuleExecutionContext = new RouteRuleExecutionContext(routeRule);
            routeRuleExecutionContext.NumberOfOptions = 5;
            routeRuleExecutionContext.SupplierCodeMatches = supplierCodeMatches;
            routeRuleExecutionContext.SupplierCodeMatchBySupplier = supplierCodeMatchBySupplier;

            routeRule.Settings.Execute(routeRuleExecutionContext, routeRuleTarget);
            T route = Activator.CreateInstance<T>();
            route.Code = routeCode;
            route.SaleZoneId = saleCodeMatch.SaleZoneId;
            route.ExecutedRuleId = routeRule.RuleId;
            route.Rate = customerZoneDetail.EffectiveRateValue;
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

        private RoutingProductRoute ExecuteRule(int routingProductId, long saleZoneId, List<SupplierCodeMatchWithRate> supplierCodeMatches, SupplierCodeMatchWithRateBySupplier supplierCodeMatchBySupplier, RouteRuleTarget routeRuleTarget, RouteRule routeRule)
        {
            RouteRuleExecutionContext routeRuleExecutionContext = new RouteRuleExecutionContext(routeRule);
            routeRuleExecutionContext.SupplierCodeMatches = supplierCodeMatches;
            routeRuleExecutionContext.SupplierCodeMatchBySupplier = supplierCodeMatchBySupplier;

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

    
}
