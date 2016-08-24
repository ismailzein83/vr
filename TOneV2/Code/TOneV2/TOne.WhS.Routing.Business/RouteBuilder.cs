﻿using System;
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
                foreach (var saleCodeMatch in context.SaleCodeMatches)
                {
                    List<CustomerZoneDetail> matchCustomerZoneDetails;
                    if (context.CustomerZoneDetails.TryGetValue(saleCodeMatch.SaleZoneId, out matchCustomerZoneDetails))
                    {
                        foreach (var customerZoneDetail in matchCustomerZoneDetails)
                        {
                            var routeRuleTarget = new RouteRuleTarget
                            {
                                CustomerId = customerZoneDetail.CustomerId,
                                Code = routeCode,
                                SaleZoneId = saleCodeMatch.SaleZoneId,
                                RoutingProductId = customerZoneDetail.RoutingProductId,
                                SaleRate = customerZoneDetail.EffectiveRateValue,
                                EffectiveOn = context.EntitiesEffectiveOn,
                                IsEffectiveInFuture = context.EntitiesEffectiveInFuture
                            };
                            var routeRule = routeRuleManager.GetMatchRule(routeRuleTarget, null);

                            if (routeRule != null)
                            {
                                bool createCustomerRoute = true;

                                // CheckSellingProductRoute(out createCustomerRoute, context, routeCode, sellingProductRoutesDic, routeRuleManager, saleCodeMatch, customerZoneDetail, routeRuleTarget, routeRule);

                                if (createCustomerRoute)
                                {
                                    CustomerRoute route = ExecuteRule<CustomerRoute>(routeCode, saleCodeMatch, customerZoneDetail, context.SupplierCodeMatches, context.SupplierCodeMatchesBySupplier, routeRuleTarget, routeRule);
                                    route.CustomerId = customerZoneDetail.CustomerId;
                                    customerRoutes.Add(route);
                                }
                            }
                            else
                                throw new NullReferenceException(string.Format("Missing Default Route Rule. Route Code: {0}. Customer Id: {1}. Sale Zone Id: {2}. Effective On:{3}.", routeCode, customerZoneDetail.CustomerId, saleCodeMatch.SaleZoneId, context.EntitiesEffectiveInFuture ? "Future" : context.EntitiesEffectiveOn.Value.ToString()));
                        }
                    }
                }
            }
            sellingProductRoutes = sellingProductRoutesDic.Values;
            return customerRoutes;
        }

        public IEnumerable<RPRoute> BuildRoutes(IBuildRoutingProductRoutesContext context, long saleZoneId)
        {
            List<RPRoute> routes = new List<RPRoute>();

            if (context.RoutingProductIds != null)
            {
                RouteRuleManager routeRuleManager = new RouteRuleManager();
                foreach (var routingProductId in context.RoutingProductIds)
                {
                    RouteRuleTarget routeRuleTarget = new RouteRuleTarget
                    {
                        RoutingProductId = routingProductId,
                        SaleZoneId = saleZoneId,
                        EffectiveOn = context.EntitiesEffectiveOn,
                        IsEffectiveInFuture = context.EntitiesEffectiveInFuture
                    };
                    var routeRule = routeRuleManager.GetMatchRule(routeRuleTarget, routingProductId);
                    if (routeRule != null)
                    {
                        RPRoute route = ExecuteRule(routingProductId, saleZoneId, context, routeRuleTarget, routeRule);
                        routes.Add(route);
                    }
                    //Removed after discussion with Sari
                    //else
                    //    throw new NullReferenceException(string.Format("Missing Default Route Rule. Routing Product Id: {0}. Sale Zone Id: {1}. Effective On: {2}.", routingProductId, saleZoneId, context.EntitiesEffectiveInFuture ? "Future" : context.EntitiesEffectiveOn.Value.ToString()));
                }
            }

            return routes;
        }

        #endregion

        #region Private Methods

        //private void CheckSellingProductRoute(out bool createCustomerRoute, IBuildCustomerRoutesContext context, string routeCode, Dictionary<int, SellingProductRoute> sellingProductRoutesDic, RouteRuleManager routeRuleManager, SaleCodeMatch saleCodeMatch, CustomerZoneDetail customerZoneDetail, RouteRuleTarget routeRuleTarget, RouteRule routeRule)
        //{
        //    createCustomerRoute = true;
        //    //if same rule and rate is inherited from Pricing, then it should be same route as pricing product
        //    if (routeRule.Criteria.RoutingProductId.HasValue && customerZoneDetail.RateSource == SalePriceListOwnerType.SellingProduct)
        //    {
        //        createCustomerRoute = false;
        //        SellingProductRoute sellingProductRoute;
        //        if (!sellingProductRoutesDic.TryGetValue(customerZoneDetail.SellingProductId, out sellingProductRoute))
        //        {
        //            var sellingProductRouteRuleTarget = new RouteRuleTarget
        //            {
        //                Code = routeCode,
        //                SaleZoneId = saleCodeMatch.SaleZoneId,
        //                RoutingProductId = customerZoneDetail.RoutingProductId,
        //                SaleRate = customerZoneDetail.EffectiveRateValue,
        //                EffectiveOn = context.EntitiesEffectiveOn,
        //                IsEffectiveInFuture = context.EntitiesEffectiveInFuture
        //            };
        //            var sellingProductRouteRule = routeRuleManager.GetMatchRule(sellingProductRouteRuleTarget);
        //            if (sellingProductRouteRule != null)
        //            {
        //                SellingProductRoute route = ExecuteRule<SellingProductRoute>(routeCode, saleCodeMatch, customerZoneDetail, context.SupplierCodeMatches, context.SupplierCodeMatchesBySupplier, sellingProductRouteRuleTarget, sellingProductRouteRule);
        //                route.SellingProductId = customerZoneDetail.SellingProductId;
        //                sellingProductRoutesDic.Add(customerZoneDetail.SellingProductId, route);
        //            }
        //        }
        //        if (sellingProductRoute == null)
        //            createCustomerRoute = true;
        //        else
        //        {
        //            //check if any option has a rule specific for the customer
        //            if (sellingProductRoute.Options != null)
        //            {
        //                RouteOptionRuleManager routeOptionRuleManager = new RouteOptionRuleManager();
        //                foreach (var option in sellingProductRoute.Options)
        //                {
        //                    RouteOptionRuleTarget routeOptionRuleTarget = new RouteOptionRuleTarget
        //                    {
        //                        SupplierId = option.SupplierId,
        //                        SupplierCode = option.SupplierCode,
        //                        SupplierZoneId = option.SupplierZoneId,
        //                        EffectiveOn = context.EntitiesEffectiveOn,
        //                        RouteTarget = routeRuleTarget,
        //                        SupplierRate = option.SupplierRate
        //                    };
        //                    var matchOptionRule = routeOptionRuleManager.GetMatchRule(routeOptionRuleTarget);
        //                    int? matchOptionRuleId = matchOptionRule != null ? matchOptionRule.RuleId : default(int?);
        //                    if (matchOptionRuleId != option.ExecutedRuleId)
        //                    {
        //                        createCustomerRoute = true;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        private T ExecuteRule<T>(string routeCode, SaleCodeMatch saleCodeMatch, CustomerZoneDetail customerZoneDetail, List<SupplierCodeMatchWithRate> supplierCodeMatches, SupplierCodeMatchWithRateBySupplier supplierCodeMatchBySupplier, RouteRuleTarget routeRuleTarget, RouteRule routeRule)
            where T : BaseRoute
        {
            ConfigManager configManager = new ConfigManager();
            SaleEntityRouteRuleExecutionContext routeRuleExecutionContext = new SaleEntityRouteRuleExecutionContext(routeRule);
            routeRuleExecutionContext.NumberOfOptions = configManager.GetRouteBuildNumberOfOptions();
            routeRuleExecutionContext.SupplierCodeMatches = supplierCodeMatches;
            routeRuleExecutionContext.SupplierCodeMatchBySupplier = supplierCodeMatchBySupplier;

            routeRule.Settings.ExecuteForSaleEntity(routeRuleExecutionContext, routeRuleTarget);
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

        private RPRoute ExecuteRule(int routingProductId, long saleZoneId, IBuildRoutingProductRoutesContext context, RouteRuleTarget routeRuleTarget, RouteRule routeRule)
        {
            RPRouteRuleExecutionContext routeRuleExecutionContext = new RPRouteRuleExecutionContext(routeRule);
            routeRuleExecutionContext.SupplierCodeMatches = context.SupplierCodeMatches;
            routeRuleExecutionContext.SupplierCodeMatchesBySupplier = context.SupplierCodeMatchesBySupplier;

            routeRule.Settings.CreateSupplierZoneOptionsForRP(routeRuleExecutionContext, routeRuleTarget);
            RPRoute route = new RPRoute
            {
                RoutingProductId = routingProductId,
                SaleZoneId = saleZoneId,
                ExecutedRuleId = routeRule.RuleId,
                IsBlocked = routeRuleTarget.BlockRoute,
                OptionsDetailsBySupplier = new Dictionary<int, RPRouteOptionSupplier>(),
                RPOptionsByPolicy = new Dictionary<int, IEnumerable<RPRouteOption>>()
            };
            var routeOptionRuleTargets = routeRuleExecutionContext.GetSupplierZoneOptions();
            if (routeOptionRuleTargets != null)
            {
                foreach (var routeOptionRuleTarget in routeOptionRuleTargets)
                {
                    RPRouteOptionSupplier optionSupplierDetails;
                    if (!route.OptionsDetailsBySupplier.TryGetValue(routeOptionRuleTarget.SupplierId, out optionSupplierDetails))
                    {
                        optionSupplierDetails = new RPRouteOptionSupplier
                        {
                            SupplierId = routeOptionRuleTarget.SupplierId,
                            SupplierZones = new List<RPRouteOptionSupplierZone>()
                        };
                        route.OptionsDetailsBySupplier.Add(routeOptionRuleTarget.SupplierId, optionSupplierDetails);
                    }
                    var optionSupplierZone = new RPRouteOptionSupplierZone
                    {
                        SupplierZoneId = routeOptionRuleTarget.SupplierZoneId,
                        SupplierCode = routeOptionRuleTarget.SupplierCode,
                        SupplierRate = routeOptionRuleTarget.SupplierRate,
                        IsBlocked = routeOptionRuleTarget.BlockOption,
                        ExecutedRuleId = routeOptionRuleTarget.ExecutedRuleId
                    };
                    optionSupplierDetails.SupplierZones.Add(optionSupplierZone);

                }

                foreach (var supplierZoneToRPOptionPolicy in context.SupplierZoneToRPOptionPolicies)
                {
                    List<RPRouteOption> rpRouteOptions = new List<RPRouteOption>();
                    foreach (var optionSupplierDetails in route.OptionsDetailsBySupplier.Values)
                    {
                        SupplierZoneToRPOptionPolicyExecutionContext supplierZoneToRPOptionPolicyExecutionContext = new SupplierZoneToRPOptionPolicyExecutionContext
                        {
                            SupplierOptionDetail = optionSupplierDetails
                        };
                        supplierZoneToRPOptionPolicy.Execute(supplierZoneToRPOptionPolicyExecutionContext);
                        rpRouteOptions.Add(new RPRouteOption
                            {
                                SupplierId = optionSupplierDetails.SupplierId,
                                SupplierRate = supplierZoneToRPOptionPolicyExecutionContext.EffectiveRate,
                                SaleZoneId = saleZoneId
                            });
                    }
                    IEnumerable<RPRouteOption> rpRouteOptionsAsEnumerable = rpRouteOptions;
                    routeRule.Settings.ApplyRuleToRPOptions(routeRuleExecutionContext, ref rpRouteOptionsAsEnumerable);
                    if (rpRouteOptionsAsEnumerable != null && rpRouteOptionsAsEnumerable.ToList().Count > 0)
                        route.RPOptionsByPolicy.Add(supplierZoneToRPOptionPolicy.ConfigId, rpRouteOptionsAsEnumerable);
                }
            }
            return route;
        }

        #endregion
    }

}
