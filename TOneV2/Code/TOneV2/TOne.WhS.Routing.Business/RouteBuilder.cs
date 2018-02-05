﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public class RouteBuilder
    {
        #region Variables/Ctor

        Vanrise.Rules.RuleTree[] _ruleTreesForCustomerRoutes;
        Dictionary<int, Vanrise.Rules.RuleTree[]> _ruleTreesForRoutingProducts = new Dictionary<int, Vanrise.Rules.RuleTree[]>();
        Vanrise.Rules.RuleTree[] _ruleTreesForRouteOptions;
        private Dictionary<int, CarrierAccount> _carrierAccounts;

        public RouteBuilder(RoutingProcessType processType)
        {
            switch (processType)
            {
                case RoutingProcessType.CustomerRoute: _ruleTreesForRouteOptions = new RouteOptionRuleManager().GetRuleTreesByPriorityForCustomerRoutes(); break;

                case RoutingProcessType.RoutingProductRoute: _ruleTreesForRouteOptions = new RouteOptionRuleManager().GetRuleTreesByPriorityForProductRoutes(); break;

                default: throw new Exception(string.Format("Unsupported RoutingProcessType: {0}", processType));
            }
        }

        #endregion

        #region Public Methods

        public IEnumerable<CustomerRoute> BuildRoutes(IBuildCustomerRoutesContext context, string routeCode)
        {
            _carrierAccounts = new CarrierAccountManager().GetCachedCarrierAccounts();

            CodeGroupManager codeGroupeManager = new CodeGroupManager();
            CodeGroup codeGroup = codeGroupeManager.GetMatchCodeGroup(routeCode);

            CustomerCountryManager customerCountryManager = new CustomerCountryManager();

            HashSet<int> countryIdsHavingParentCode = codeGroupeManager.GetCGsParentCountries().GetRecord(codeGroup.Code);
            bool isCountryIdsHavingParentCodeNotEmpty = countryIdsHavingParentCode != null && countryIdsHavingParentCode.Count > 0;

            List<CustomerRoute> customerRoutes = new List<CustomerRoute>();

            if (context.SaleZoneDefintions != null)
            {
                Dictionary<RouteRule, List<RouteOptionRuleTarget>> optionsByRules = new Dictionary<RouteRule, List<RouteOptionRuleTarget>>();
                RouteRuleManager routeRuleManager = new RouteRuleManager();

                foreach (var saleZoneDefintion in context.SaleZoneDefintions)
                {
                    HashSet<int> soldCustomers = new HashSet<int>();
                    List<CustomerZoneDetail> matchCustomerZoneDetails = null;

                    if (context.CustomerZoneDetails != null && context.CustomerZoneDetails.TryGetValue(saleZoneDefintion.SaleZoneId, out matchCustomerZoneDetails))
                    {
                        foreach (var customerZoneDetail in matchCustomerZoneDetails)
                        {
                            soldCustomers.Add(customerZoneDetail.CustomerId);

                            var routeRuleTarget = new RouteRuleTarget
                            {
                                CustomerId = customerZoneDetail.CustomerId,
                                Code = routeCode,
                                SaleZoneId = saleZoneDefintion.SaleZoneId,
                                RoutingProductId = customerZoneDetail.RoutingProductId,
                                SaleRate = customerZoneDetail.EffectiveRateValue,
                                EffectiveOn = context.EntitiesEffectiveOn,
                                IsEffectiveInFuture = context.EntitiesEffectiveInFuture
                            };

                            var routeRule = GetRouteRule(routeRuleTarget, null);
                            if (routeRule != null)
                            {
                                bool createCustomerRoute = true;

                                if (createCustomerRoute)
                                {
                                    CustomerRoute route = ExecuteRule<CustomerRoute>(optionsByRules, routeCode, saleZoneDefintion, customerZoneDetail, context.SupplierCodeMatches, context.SupplierCodeMatchesBySupplier,
                                        routeRuleTarget, routeRule, context.RoutingDatabase);

                                    route.CustomerId = customerZoneDetail.CustomerId;
                                    route.VersionNumber = context.VersionNumber;
                                    customerRoutes.Add(route);
                                }
                            }
                            else
                            {
                                throw new NullReferenceException(string.Format("Missing Default Route Rule. Route Code: {0}. Customer Id: {1}. Sale Zone Id: {2}. Effective On:{3}.", routeCode, customerZoneDetail.CustomerId,
                                    saleZoneDefintion.SaleZoneId, context.EntitiesEffectiveInFuture ? "Future" : context.EntitiesEffectiveOn.Value.ToString()));
                            }
                        }
                    }

                    if (context.IsFullRouteBuild)
                    {
                        if (soldCustomers.Count != context.ActiveRoutingCustomerInfos.Count())
                        {
                            foreach (RoutingCustomerInfo routingCustomerInfo in context.ActiveRoutingCustomerInfos)
                            {
                                if (saleZoneDefintion.SellingNumberPlanId != routingCustomerInfo.SellingNumberPlanId || soldCustomers.Contains(routingCustomerInfo.CustomerId))
                                    continue;

                                CheckAndAddRouteToUnratedZone(context, routingCustomerInfo.CustomerId, customerCountryManager, customerRoutes, countryIdsHavingParentCode, routeCode, saleZoneDefintion, codeGroup.CountryId,
                                    isCountryIdsHavingParentCodeNotEmpty, context.VersionNumber);
                            }
                        }
                    }
                }
            }

            return customerRoutes;
        }

        private void CheckAndAddRouteToUnratedZone(IBuildCustomerRoutesContext context, int customerId, CustomerCountryManager customerCountryManager, List<CustomerRoute> customerRoutes,
            HashSet<int> countryIdsHavingParentCode, string routeCode, SaleZoneDefintion saleZoneDefintion, int routeCodeCountryId, bool isCountryIdsHavingParentCodeNotEmpty, int versionNumber)
        {
            HashSet<int> soldCountries = context.CustomerCountries.GetOrCreateItem(customerId, () =>
            {
                var customerCountryIds = customerCountryManager.GetCustomerCountryIds(customerId, context.EntitiesEffectiveOn, context.EntitiesEffectiveInFuture);
                return customerCountryIds != null ? customerCountryIds.ToHashSet() : null;
            });

            if (soldCountries == null)
                return;

            if (soldCountries.Contains(routeCodeCountryId) || (isCountryIdsHavingParentCodeNotEmpty && countryIdsHavingParentCode.Any(soldCountries.Contains)))
            {
                customerRoutes.Add(new CustomerRoute() { Code = routeCode, CorrespondentType = CorrespondentType.Other, CustomerId = customerId, SaleZoneId = saleZoneDefintion.SaleZoneId, VersionNumber = versionNumber });
            }
        }

        public IEnumerable<RPRoute> BuildRoutes(IBuildRoutingProductRoutesContext context, long saleZoneId, bool includeBlockedSupplierZones)
        {
            List<RPRoute> routes = new List<RPRoute>();

            if (context.RoutingProducts != null)
            {
                RouteRuleManager routeRuleManager = new RouteRuleManager();

                foreach (var routingProduct in context.RoutingProducts)
                {
                    RouteRuleTarget routeRuleTarget = new RouteRuleTarget
                    {
                        RoutingProductId = routingProduct.RoutingProductId,
                        SaleZoneId = saleZoneId,
                        EffectiveOn = context.EntitiesEffectiveOn,
                        IsEffectiveInFuture = context.EntitiesEffectiveInFuture
                    };
                    if (routingProduct.Settings == null)
                        throw new NullReferenceException(string.Format("routingProduct.Settings of Routing Product Id: {0}", routingProduct.RoutingProductId));

                    HashSet<int> saleZoneServiceIds = routingProduct.Settings.GetZoneServices(saleZoneId);

                    var routeRule = GetRouteRule(routeRuleTarget, routingProduct.RoutingProductId);
                    if (routeRule != null)
                    {
                        RPRoute route = ExecuteRule(routingProduct.RoutingProductId, saleZoneId, saleZoneServiceIds, context, routeRuleTarget, routeRule, includeBlockedSupplierZones, context.RoutingDatabase);
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

        private RouteRule GetRouteRule(RouteRuleTarget routeRuleTarget, int? routingProductId)
        {
            Vanrise.Rules.RuleTree[] ruleTrees = null;
            if (!routingProductId.HasValue)
            {
                if (_ruleTreesForCustomerRoutes == null)
                    _ruleTreesForCustomerRoutes = new RouteRuleManager().GetRuleTreesByPriority(null);
                ruleTrees = _ruleTreesForCustomerRoutes;
            }
            else
            {
                if (!_ruleTreesForRoutingProducts.TryGetValue(routingProductId.Value, out ruleTrees))
                {
                    lock (_ruleTreesForRoutingProducts)
                    {
                        ruleTrees = _ruleTreesForRoutingProducts.GetOrCreateItem(routingProductId.Value, () => new RouteRuleManager().GetRuleTreesByPriority(routingProductId));
                    }
                }
            }
            if (ruleTrees != null)
            {
                foreach (var ruleTree in ruleTrees)
                {
                    var matchRule = ruleTree.GetMatchRule(routeRuleTarget) as RouteRule;
                    if (matchRule != null)
                        return matchRule;
                }
            }
            return null;
        }

        private T ExecuteRule<T>(Dictionary<RouteRule, List<RouteOptionRuleTarget>> optionsByRules, string routeCode, SaleZoneDefintion saleZoneDefintion, CustomerZoneDetail customerZoneDetail, List<SupplierCodeMatchWithRate> supplierCodeMatches,
            SupplierCodeMatchWithRateBySupplier supplierCodeMatchBySupplier, RouteRuleTarget routeRuleTarget, RouteRule routeRule, RoutingDatabase routingDatabase) where T : BaseRoute
        {
            ConfigManager configManager = new ConfigManager();
            bool keepBackupsForRemovedOptions = configManager.GetCustomerRouteBuildKeepBackUpsForRemovedOptions();

            SaleEntityRouteRuleExecutionContext routeRuleExecutionContext = new SaleEntityRouteRuleExecutionContext(routeRule, _ruleTreesForRouteOptions);
            int numberOfOptionsInSettings = configManager.GetCustomerRouteBuildNumberOfOptions();
            routeRuleExecutionContext.NumberOfOptions = numberOfOptionsInSettings;
            routeRuleExecutionContext.SupplierCodeMatches = supplierCodeMatches;
            routeRuleExecutionContext.SupplierCodeMatchBySupplier = supplierCodeMatchBySupplier;
            routeRuleExecutionContext.SaleZoneServiceList = customerZoneDetail.SaleZoneServiceIds;//used for service matching
            routeRuleExecutionContext.SaleZoneServiceIds = customerZoneDetail.SaleZoneServiceIds != null ? string.Join(",", customerZoneDetail.SaleZoneServiceIds) : null;// used for market price
            routeRuleExecutionContext.RoutingDatabase = routingDatabase;
            routeRuleExecutionContext.KeepBackupsForRemovedOptions = keepBackupsForRemovedOptions;

            T route = Activator.CreateInstance<T>();
            route.Code = routeCode;
            route.SaleZoneId = saleZoneDefintion.SaleZoneId;
            route.ExecutedRuleId = routeRule.RuleId;
            route.Rate = customerZoneDetail.EffectiveRateValue;
            route.SaleZoneServiceIds = customerZoneDetail.SaleZoneServiceIds;
            route.CorrespondentType = routeRule.CorrespondentType;

            int? maxNumberOfOptions = routeRule.Settings.GetMaxNumberOfOptions(routeRuleExecutionContext);

            if (routeRule.Settings.UseOrderedExecution)
            {
                List<RouteOptionRuleTarget> routeOptionRuleTargets = CloneRouteOptionRuleTargets(optionsByRules.GetOrCreateItem(routeRule, () =>
                    {
                        return routeRule.Settings.GetOrderedOptions(routeRuleExecutionContext, routeRuleTarget);
                    }));

                if (routeOptionRuleTargets != null)
                {
                    int optionsAdded = 0;
                    route.Options = new List<RouteOption>();
                    bool isValidOption;
                    CarrierAccount customer = _carrierAccounts.GetRecord(customerZoneDetail.CustomerId);
                    foreach (RouteOptionRuleTarget targetOption in routeOptionRuleTargets)
                    {
                        targetOption.RouteTarget = routeRuleTarget;

                        CheckRouteOptionRuleTarget(targetOption, customerZoneDetail.CustomerId, keepBackupsForRemovedOptions, (optionTarget) =>
                        {
                            var supplier = _carrierAccounts.GetRecord(optionTarget.SupplierId);
                            if (supplier.CarrierProfileId == customer.CarrierProfileId)
                                return false;

                            routeRule.Settings.CheckOptionFilter(routeRuleExecutionContext, routeRuleTarget, optionTarget);
                            if (optionTarget.FilterOption)
                                return false;

                            routeRuleExecutionContext.CheckRouteOptionRule(optionTarget, routeRule);
                            if (optionTarget.FilterOption)
                                return false;

                            return true;

                        }, out isValidOption);

                        if (!isValidOption)
                            continue;

                        bool allItemsBlocked;
                        RouteOption routeOption = routeRuleExecutionContext.CreateOptionFromTarget(targetOption, out allItemsBlocked);
                        route.Options.Add(routeOption);

                        if (!allItemsBlocked)
                            optionsAdded++;

                        if (maxNumberOfOptions.HasValue && maxNumberOfOptions == optionsAdded)
                            break;

                    }
                }

                FinalizeRouteOptionContext finalizeRouteOptionContext = new FinalizeRouteOptionContext() { NumberOfOptionsInSettings = numberOfOptionsInSettings, RouteOptions = route.Options };
                route.Options = routeRule.Settings.GetFinalOptions(finalizeRouteOptionContext);

                routeRule.Settings.ApplyOptionsPercentage(route.Options);
            }
            else
            {
                routeRule.Settings.ExecuteForSaleEntity(routeRuleExecutionContext, routeRuleTarget);
                if (routeRuleExecutionContext._options != null)
                {
                    //route.OptionsToInsert = routeRuleExecutionContext._options.Select(itm => new RouteOptionToInsert(itm)).ToList();
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
            }
            route.IsBlocked = routeRuleTarget.BlockRoute;
            return route;
        }

        private List<RouteOptionRuleTarget> CloneRouteOptionRuleTargets(List<RouteOptionRuleTarget> routeOptionRuleTargets)
        {
            if (routeOptionRuleTargets == null)
                return null;

            List<RouteOptionRuleTarget> clonedRouteOptionRuleTargets = new List<RouteOptionRuleTarget>();
            foreach (RouteOptionRuleTarget routeOptionRuleTarget in routeOptionRuleTargets)
                clonedRouteOptionRuleTargets.Add(routeOptionRuleTarget.CloneObject());

            return clonedRouteOptionRuleTargets;
        }

        private void CheckRouteOptionRuleTarget(RouteOptionRuleTarget targetOption, int customerId, bool keepBackupsForRemovedOptions,
            Func<BaseRouteOptionRuleTarget, bool> isOptionValid, out bool isValidOption)
        {
            isValidOption = true;
            if (isOptionValid(targetOption))
            {
                if (targetOption.Backups != null && targetOption.Backups.Count > 0)
                {
                    if (!targetOption.BlockOption || keepBackupsForRemovedOptions)
                    {
                        var backups = targetOption.Backups.FindAllRecords(itm => isOptionValid(itm));
                        if (backups != null && backups.Count() > 0)
                            targetOption.Backups = backups.ToList();
                        else
                            targetOption.Backups = new List<RouteBackupOptionRuleTarget>() { Helper.CreateRouteBackupOptionRuleTargetFromOption(targetOption) };
                    }
                    else
                    {
                        targetOption.Backups = null;
                    }
                }
            }
            else
            {
                if (keepBackupsForRemovedOptions && targetOption.Backups != null && targetOption.Backups.Count > 0)
                {
                    var backups = targetOption.Backups.FindAllRecords(itm => isOptionValid(itm));
                    if (backups != null && backups.Count() > 0)
                    {
                        var firstBackup = backups.First();
                        var otherBackups = targetOption.Backups.FindAllRecords(itm => itm != firstBackup);
                        targetOption = Helper.CreateRouteOptionRuleTargetFromBackup(firstBackup, targetOption.OptionSettings as IRouteOptionSettings);

                        if (otherBackups != null && otherBackups.Count() > 0)
                            targetOption.Backups = new List<RouteBackupOptionRuleTarget>(otherBackups);
                        else
                            targetOption.Backups = new List<RouteBackupOptionRuleTarget>() { firstBackup};
                    }
                    else
                    {
                        isValidOption = false;
                    }
                }
                else
                {
                    isValidOption = false;
                }
            }
        }

        private RPRoute ExecuteRule(int routingProductId, long saleZoneId, HashSet<int> saleZoneServiceIds, IBuildRoutingProductRoutesContext context, RouteRuleTarget routeRuleTarget, RouteRule routeRule,
            bool includeBlockedSupplierZones, RoutingDatabase routingDatabase)
        {
            RPRouteRuleExecutionContext routeRuleExecutionContext = new RPRouteRuleExecutionContext(routeRule, _ruleTreesForRouteOptions);
            routeRuleExecutionContext.SupplierCodeMatches = context.SupplierCodeMatches;
            routeRuleExecutionContext.SupplierCodeMatchesBySupplier = context.SupplierCodeMatchesBySupplier;
            routeRuleExecutionContext.SaleZoneServiceIds = saleZoneServiceIds;
            routeRuleExecutionContext.RoutingDatabase = routingDatabase;

            routeRule.Settings.CreateSupplierZoneOptionsForRP(routeRuleExecutionContext, routeRuleTarget);
            RPRoute route = new RPRoute
            {
                RoutingProductId = routingProductId,
                SaleZoneId = saleZoneId,
                SaleZoneServiceIds = saleZoneServiceIds,
                ExecutedRuleId = routeRule.RuleId,
                IsBlocked = routeRuleTarget.BlockRoute,
                OptionsDetailsBySupplier = new Dictionary<int, RPRouteOptionSupplier>(),
                RPOptionsByPolicy = new Dictionary<Guid, IEnumerable<RPRouteOption>>()
            };

            var routeOptionRuleTargets = routeRuleExecutionContext.GetSupplierZoneOptions();
            if (routeOptionRuleTargets != null)
            {
                RPRouteOptionSupplier optionSupplierDetails = null;

                Dictionary<int, List<DateTime?>> supplierZoneEEDsBySupplier = new Dictionary<int, List<DateTime?>>();

                foreach (var routeOptionRuleTarget in routeOptionRuleTargets)
                {
                    List<DateTime?> supplierZoneEEDs = supplierZoneEEDsBySupplier.GetOrCreateItem(routeOptionRuleTarget.SupplierId);
                    supplierZoneEEDs.Add(routeOptionRuleTarget.SupplierRateEED);

                    if (!route.OptionsDetailsBySupplier.TryGetValue(routeOptionRuleTarget.SupplierId, out optionSupplierDetails))
                    {
                        optionSupplierDetails = new RPRouteOptionSupplier
                        {
                            SupplierId = routeOptionRuleTarget.SupplierId,
                            SupplierZones = new List<RPRouteOptionSupplierZone>(),
                            NumberOfBlockedZones = 0,
                            NumberOfUnblockedZones = 0,
                            Percentage = routeOptionRuleTarget.Percentage,
                            SupplierServiceWeight = routeOptionRuleTarget.SupplierServiceWeight
                        };

                        route.OptionsDetailsBySupplier.Add(routeOptionRuleTarget.SupplierId, optionSupplierDetails);
                    }

                    if (routeOptionRuleTarget.BlockOption)
                        optionSupplierDetails.NumberOfBlockedZones++;
                    else
                        optionSupplierDetails.NumberOfUnblockedZones++;

                    var optionSupplierZone = new RPRouteOptionSupplierZone
                    {
                        SupplierZoneId = routeOptionRuleTarget.SupplierZoneId,
                        //SupplierCode = routeOptionRuleTarget.SupplierCode,
                        SupplierRate = routeOptionRuleTarget.SupplierRate,
                        IsBlocked = routeOptionRuleTarget.BlockOption,
                        ExecutedRuleId = routeOptionRuleTarget.ExecutedRuleId,
                        ExactSupplierServiceIds = routeOptionRuleTarget.ExactSupplierServiceIds,
                        SupplierRateId = routeOptionRuleTarget.SupplierRateId
                    };

                    optionSupplierDetails.SupplierZones.Add(optionSupplierZone);
                }

                foreach (var supplierZoneToRPOptionPolicy in context.SupplierZoneToRPOptionPolicies)
                {
                    List<RPRouteOption> rpRouteOptions = new List<RPRouteOption>();
                    foreach (var item in route.OptionsDetailsBySupplier.Values)
                    {
                        List<DateTime?> supplierZoneEEDs = supplierZoneEEDsBySupplier.GetOrCreateItem(item.SupplierId);
                        bool supplierZoneMatchHasClosedRate = supplierZoneEEDs != null && supplierZoneEEDs.FirstOrDefault(itm => itm.HasValue) != null;

                        SupplierZoneToRPOptionPolicyExecutionContext supplierZoneToRPOptionPolicyExecutionContext = new SupplierZoneToRPOptionPolicyExecutionContext
                        {
                            SupplierOptionDetail = item,
                            IncludeBlockedSupplierZones = includeBlockedSupplierZones
                        };
                        supplierZoneToRPOptionPolicy.Execute(supplierZoneToRPOptionPolicyExecutionContext);
                        rpRouteOptions.Add(new RPRouteOption
                            {
                                SupplierId = item.SupplierId,
                                SupplierRate = supplierZoneToRPOptionPolicyExecutionContext.EffectiveRate,
                                SaleZoneId = saleZoneId,
                                SupplierStatus = item.SupplierStatus,
                                Percentage = item.Percentage,
                                SupplierServiceWeight = item.SupplierServiceWeight,
                                SupplierZoneMatchHasClosedRate = supplierZoneMatchHasClosedRate
                            });
                    }

                    IEnumerable<RPRouteOption> rpRouteOptionsAsEnumerable = rpRouteOptions;
                    routeRule.Settings.ApplyRuleToRPOptions(routeRuleExecutionContext, ref rpRouteOptionsAsEnumerable);
                    if (rpRouteOptionsAsEnumerable != null && rpRouteOptionsAsEnumerable.ToList().Count > 0)
                    {
                        if (!includeBlockedSupplierZones)
                        {
                            IEnumerable<RPRouteOption> unblockedRouteOptions = rpRouteOptionsAsEnumerable.FindAllRecords(itm => itm.SupplierStatus != SupplierStatus.Block);
                            IEnumerable<RPRouteOption> blockedRouteOptions = rpRouteOptionsAsEnumerable.FindAllRecords(itm => itm.SupplierStatus == SupplierStatus.Block);

                            if (unblockedRouteOptions == null)
                                rpRouteOptionsAsEnumerable = blockedRouteOptions;
                            else if (blockedRouteOptions == null)
                                rpRouteOptionsAsEnumerable = unblockedRouteOptions;
                            else
                                rpRouteOptionsAsEnumerable = unblockedRouteOptions.Union(blockedRouteOptions);
                        }
                        route.RPOptionsByPolicy.Add(supplierZoneToRPOptionPolicy.ConfigId, rpRouteOptionsAsEnumerable);
                    }
                }
            }
            return route;
        }

        private class FinalizeRouteOptionContext : IFinalizeRouteOptionContext
        {
            public List<RouteOption> RouteOptions { get; set; }

            public int NumberOfOptionsInSettings { get; set; }
        }

        #endregion
    }
}