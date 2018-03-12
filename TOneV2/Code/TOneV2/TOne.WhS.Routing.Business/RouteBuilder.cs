using System;
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
            SaleZoneManager saleZoneManager = new SaleZoneManager();

            HashSet<int> countryIdsHavingParentCode = codeGroupeManager.GetCGsParentCountries().GetRecord(codeGroup.Code);
            bool isCountryIdsHavingParentCodeNotEmpty = countryIdsHavingParentCode != null && countryIdsHavingParentCode.Count > 0;

            List<CustomerRoute> customerRoutes = new List<CustomerRoute>();

            bool keepBackupsForRemovedOptions = new ConfigManager().GetCustomerRouteBuildKeepBackUpsForRemovedOptions();

            if (context.SaleZoneDefintions != null)
            {
                Dictionary<RouteRule, List<RouteOptionRuleTarget>> optionsByRules = new Dictionary<RouteRule, List<RouteOptionRuleTarget>>();
                RouteRuleManager routeRuleManager = new RouteRuleManager();

                foreach (var saleZoneDefintion in context.SaleZoneDefintions)
                {
                    SaleZone saleZone = saleZoneManager.GetSaleZone(saleZoneDefintion.SaleZoneId);
                    saleZone.ThrowIfNull("saleZone", saleZoneDefintion.SaleZoneId);

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
                                CountryId = saleZone.CountryId,
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
                                    CustomerZoneDetailData customerZoneDetailData = new CustomerZoneDetailData()
                                    {
                                        CustomerId = customerZoneDetail.CustomerId,
                                        EffectiveRateValue = customerZoneDetail.EffectiveRateValue,
                                        SaleZoneId = customerZoneDetail.SaleZoneId,
                                        SaleZoneServiceIds = customerZoneDetail.SaleZoneServiceIds
                                    };
                                    CustomerRoute route = ExecuteRule<CustomerRoute>(optionsByRules, routeCode, saleZoneDefintion, customerZoneDetailData, context.SupplierCodeMatches, context.SupplierCodeMatchesBySupplier,
                                        routeRuleTarget, routeRule, context.RoutingDatabase, RoutingProcessType.CustomerRoute, keepBackupsForRemovedOptions);

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

        public IEnumerable<RPRoute> BuildRoutes(IBuildRoutingProductRoutesContext context, long saleZoneId)
        {
            List<RPRoute> routes = new List<RPRoute>();
            SaleZone saleZone = new SaleZoneManager().GetSaleZone(saleZoneId);
            saleZone.ThrowIfNull("saleZone", saleZoneId);

            if (context.RoutingProducts != null)
            {
                RouteRuleManager routeRuleManager = new RouteRuleManager();

                foreach (var routingProduct in context.RoutingProducts)
                {
                    RouteRuleTarget routeRuleTarget = new RouteRuleTarget
                    {
                        RoutingProductId = routingProduct.RoutingProductId,
                        SaleZoneId = saleZoneId,
                        CountryId = saleZone.CountryId,
                        EffectiveOn = context.EntitiesEffectiveOn,
                        IsEffectiveInFuture = context.EntitiesEffectiveInFuture
                    };

                    if (routingProduct.Settings == null)
                        throw new NullReferenceException(string.Format("routingProduct.Settings of Routing Product Id: {0}", routingProduct.RoutingProductId));

                    HashSet<int> saleZoneServiceIds = routingProduct.Settings.GetZoneServices(saleZoneId);

                    var routeRule = GetRouteRule(routeRuleTarget, routingProduct.RoutingProductId);
                    if (routeRule != null)
                    {
                        RPRoute route = ExecuteRule(routingProduct.RoutingProductId, saleZoneId, saleZoneServiceIds, context, routeRuleTarget, routeRule, context.RoutingDatabase);
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

        public T ExecuteRule<T>(Dictionary<RouteRule, List<RouteOptionRuleTarget>> optionsByRules, string routeCode, SaleZoneDefintion saleZoneDefintion, CustomerZoneDetailData customerZoneDetailData, List<SupplierCodeMatchWithRate> supplierCodeMatches,
            SupplierCodeMatchWithRateBySupplier supplierCodeMatchBySupplier, RouteRuleTarget routeRuleTarget, RouteRule routeRule, RoutingDatabase routingDatabase, RoutingProcessType processType, bool keepBackupsForRemovedOptions) where T : BaseRoute
        {
            ConfigManager configManager = new ConfigManager();
            int numberOfOptionsInSettings = configManager.GetCustomerRouteBuildNumberOfOptions();

            SaleEntityRouteRuleExecutionContext routeRuleExecutionContext = new SaleEntityRouteRuleExecutionContext(routeRule, _ruleTreesForRouteOptions);
            routeRuleExecutionContext.NumberOfOptions = numberOfOptionsInSettings;
            routeRuleExecutionContext.SupplierCodeMatches = supplierCodeMatches;
            routeRuleExecutionContext.SupplierCodeMatchBySupplier = supplierCodeMatchBySupplier;
            routeRuleExecutionContext.SaleZoneServiceList = customerZoneDetailData.SaleZoneServiceIds; //used for service matching
            routeRuleExecutionContext.SaleZoneServiceIds = customerZoneDetailData.SaleZoneServiceIds != null ? string.Join(",", customerZoneDetailData.SaleZoneServiceIds) : null; //used for market price
            routeRuleExecutionContext.RoutingDatabase = routingDatabase;
            routeRuleExecutionContext.KeepBackupsForRemovedOptions = keepBackupsForRemovedOptions;

            T route = Activator.CreateInstance<T>();
            route.Code = routeCode;
            route.SaleZoneId = saleZoneDefintion.SaleZoneId;
            route.ExecutedRuleId = routeRule.RuleId;
            route.Rate = customerZoneDetailData.EffectiveRateValue;
            route.SaleZoneServiceIds = customerZoneDetailData.SaleZoneServiceIds;
            route.CorrespondentType = routeRule.CorrespondentType;

            int? maxNumberOfOptions = null;
            switch (processType)
            {
                case RoutingProcessType.CustomerRoute: maxNumberOfOptions = routeRule.Settings.GetMaxNumberOfOptions(routeRuleExecutionContext); break;
                case RoutingProcessType.RoutingProductRoute: maxNumberOfOptions = null; break;
            }


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
                    CarrierAccount customer = customerZoneDetailData.CustomerId.HasValue ? _carrierAccounts.GetRecord(customerZoneDetailData.CustomerId.Value) : null;

                    foreach (RouteOptionRuleTarget targetOption in routeOptionRuleTargets)
                    {
                        targetOption.RouteTarget = routeRuleTarget;

                        List<RouteOptionRuleTarget> processedTargetOptions = ProcessRouteOptionRuleTarget(targetOption, keepBackupsForRemovedOptions, (optionTarget) =>
                        {
                            var supplier = _carrierAccounts.GetRecord(optionTarget.SupplierId);
                            if (customer != null && supplier.CarrierProfileId == customer.CarrierProfileId)
                                return false;

                            routeRule.Settings.CheckOptionFilter(routeRuleExecutionContext, routeRuleTarget, optionTarget);
                            if (optionTarget.FilterOption)
                                return false;

                            routeRuleExecutionContext.CheckRouteOptionRule(optionTarget, routeRule);
                            if (optionTarget.FilterOption)
                                return false;

                            return true;
                        });

                        if (processedTargetOptions == null || processedTargetOptions.Count == 0)
                            continue;

                        foreach (RouteOptionRuleTarget processedTargetOption in processedTargetOptions)
                        {
                            bool allItemsBlocked;
                            RouteOption routeOption = routeRuleExecutionContext.CreateOptionFromTarget(processedTargetOption, out allItemsBlocked);
                            route.Options.Add(routeOption);

                            if (!allItemsBlocked)
                                optionsAdded++;
                        }

                        if (maxNumberOfOptions.HasValue && maxNumberOfOptions == optionsAdded)
                            break;
                    }
                }

                FinalizeRouteOptionContext finalizeRouteOptionContext = new FinalizeRouteOptionContext() { NumberOfOptionsInSettings = maxNumberOfOptions, RouteOptions = route.Options };
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

        private List<RouteOptionRuleTarget> ProcessRouteOptionRuleTarget(RouteOptionRuleTarget targetOption, bool keepBackupsForRemovedOptions, Func<BaseRouteOptionRuleTarget, bool> isOptionValid)
        {
            List<RouteOptionRuleTarget> results = new List<RouteOptionRuleTarget>();
            List<RouteBackupOptionRuleTarget> backups = targetOption.Backups;
            var currentOption = targetOption;

            if (isOptionValid(targetOption))
            {
                currentOption.Backups = null;

                results.Add(currentOption);

                bool createNewOption = false;
                if (targetOption.BlockOption)
                {
                    if (!keepBackupsForRemovedOptions)
                        return results;

                    createNewOption = true;
                }

                if (backups != null && backups.Count > 0)
                {
                    currentOption.Backups = new List<RouteBackupOptionRuleTarget>();
                    foreach (RouteBackupOptionRuleTarget backup in backups)
                    {
                        if (!isOptionValid(backup))
                            continue;

                        if (backup.BlockOption || !createNewOption)
                        {
                            currentOption.Backups.Add(backup);
                        }
                        else
                        {
                            currentOption = TOne.WhS.Routing.Entities.Helper.CreateRouteOptionRuleTargetFromBackup(backup, targetOption.Percentage);
                            createNewOption = false;
                            results.Add(currentOption);
                        }
                    }

                    if (currentOption.Backups.Count == 0)
                        currentOption.Backups.Add(TOne.WhS.Routing.Entities.Helper.CreateRouteBackupOptionRuleTargetFromOption(currentOption));
                }
            }
            else
            {
                if (!keepBackupsForRemovedOptions || targetOption.Backups == null || targetOption.Backups.Count == 0)
                    return null;

                bool createNewOption = true;

                RouteBackupOptionRuleTarget previousBackup = null;
                foreach (RouteBackupOptionRuleTarget backup in targetOption.Backups)
                {
                    if (!isOptionValid(backup))
                        continue;

                    if (createNewOption)
                    {
                        if (previousBackup == null || (previousBackup.BlockOption && !backup.BlockOption))
                            createNewOption = true;
                        else
                            createNewOption = false;

                        if (createNewOption)
                        {
                            currentOption = TOne.WhS.Routing.Entities.Helper.CreateRouteOptionRuleTargetFromBackup(backup, targetOption.Percentage);
                            results.Add(currentOption);
                        }
                        else
                        {
                            currentOption.Backups.Add(backup);
                        }
                    }
                    else
                    {
                        currentOption.Backups.Add(backup);
                    }

                    previousBackup = backup;
                }

                if (currentOption != null && (currentOption.Backups == null || currentOption.Backups.Count == 0))
                    currentOption.Backups = new List<RouteBackupOptionRuleTarget>() { TOne.WhS.Routing.Entities.Helper.CreateRouteBackupOptionRuleTargetFromOption(currentOption) };
            }

            return results;
        }

        private RPRoute ExecuteRule(int routingProductId, long saleZoneId, HashSet<int> saleZoneServiceIds, IBuildRoutingProductRoutesContext context, RouteRuleTarget routeRuleTarget, RouteRule routeRule, RoutingDatabase routingDatabase)
        {
            bool keepBackupsForRemovedOptions = new ConfigManager().GetProductRouteBuildKeepBackUpsForRemovedOptions();

            RPRouteRuleExecutionContext routeRuleExecutionContext = new RPRouteRuleExecutionContext(routeRule, _ruleTreesForRouteOptions);
            routeRuleExecutionContext.SupplierCodeMatches = context.SupplierCodeMatches;
            routeRuleExecutionContext.SupplierCodeMatchesBySupplier = context.SupplierCodeMatchesBySupplier;
            routeRuleExecutionContext.SaleZoneServiceIds = saleZoneServiceIds;
            routeRuleExecutionContext.RoutingDatabase = routingDatabase;
            routeRuleExecutionContext.KeepBackupsForRemovedOptions = keepBackupsForRemovedOptions;

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

            var routeOptionRuleTargetsBySupplier = routeRuleExecutionContext.GetSupplierZoneOptions();
            if (routeOptionRuleTargetsBySupplier != null)
            {
                RPRouteOptionSupplier optionSupplierDetails = null;

                HashSet<int> supplierHavingZonesWithEED = new HashSet<int>();

                foreach (var routeOptionRuleTargetKvp in routeOptionRuleTargetsBySupplier)
                {
                    int supplierId = routeOptionRuleTargetKvp.Key;
                    List<RouteOptionRuleTarget> routeOptionRuleTargets = routeOptionRuleTargetKvp.Value;

                    if (!routeOptionRuleTargets.Any())
                        continue;

                    RouteOptionRuleTarget firstRouteOptionRuleTarget = routeOptionRuleTargets[0];
                    if (firstRouteOptionRuleTarget.SupplierRateEED.HasValue)
                        supplierHavingZonesWithEED.Add(supplierId);

                    optionSupplierDetails = new RPRouteOptionSupplier
                    {
                        SupplierId = supplierId,
                        SupplierZones = new List<RPRouteOptionSupplierZone>(),
                        NumberOfBlockedZones = 0,
                        NumberOfUnblockedZones = 0,
                        Percentage = firstRouteOptionRuleTarget.Percentage,
                        SupplierServiceWeight = firstRouteOptionRuleTarget.SupplierServiceWeight
                    };
                    route.OptionsDetailsBySupplier.Add(supplierId, optionSupplierDetails);

                    AddSupplierZone(optionSupplierDetails, firstRouteOptionRuleTarget);

                    for (var x = 1; x < routeOptionRuleTargets.Count; x++)
                    {
                        var currentRouteOptionRuleTarget = routeOptionRuleTargets[x];
                        AddSupplierZone(optionSupplierDetails, currentRouteOptionRuleTarget);
                    }
                }

                foreach (var supplierZoneToRPOptionPolicy in context.SupplierZoneToRPOptionPolicies)
                {
                    List<RPRouteOption> rpRouteOptions = new List<RPRouteOption>();
                    foreach (var item in route.OptionsDetailsBySupplier.Values)
                    {
                        bool supplierZoneMatchHasClosedRate = supplierHavingZonesWithEED.Contains(item.SupplierId);

                        SupplierZoneToRPOptionPolicyExecutionContext supplierZoneToRPOptionPolicyExecutionContext = new SupplierZoneToRPOptionPolicyExecutionContext { SupplierOptionDetail = item };
                        supplierZoneToRPOptionPolicy.Execute(supplierZoneToRPOptionPolicyExecutionContext);

                        rpRouteOptions.Add(new RPRouteOption
                        {
                            SupplierId = item.SupplierId,
                            SaleZoneId = saleZoneId,
                            SupplierRate = supplierZoneToRPOptionPolicyExecutionContext.EffectiveRate,
                            Percentage = item.Percentage,
                            SupplierServiceWeight = item.SupplierServiceWeight,
                            SupplierZoneMatchHasClosedRate = supplierZoneMatchHasClosedRate,
                            SupplierStatus = item.SupplierStatus,
                            IsForced = item.IsForced
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

        private void AddSupplierZone(RPRouteOptionSupplier optionSupplierDetails, RouteOptionRuleTarget currentRouteOptionRuleTarget)
        {
            if (currentRouteOptionRuleTarget.BlockOption)
                optionSupplierDetails.NumberOfBlockedZones++;
            else
                optionSupplierDetails.NumberOfUnblockedZones++;

            var optionSupplierZone = new RPRouteOptionSupplierZone
            {
                SupplierZoneId = currentRouteOptionRuleTarget.SupplierZoneId,
                SupplierRateId = currentRouteOptionRuleTarget.SupplierRateId,
                SupplierRate = currentRouteOptionRuleTarget.SupplierRate,
                ExecutedRuleId = currentRouteOptionRuleTarget.ExecutedRuleId,
                ExactSupplierServiceIds = currentRouteOptionRuleTarget.ExactSupplierServiceIds,
                IsBlocked = currentRouteOptionRuleTarget.BlockOption,
                IsForced = currentRouteOptionRuleTarget.IsForced
            };

            optionSupplierDetails.SupplierZones.Add(optionSupplierZone);
        }

        private class FinalizeRouteOptionContext : IFinalizeRouteOptionContext
        {
            public List<RouteOption> RouteOptions { get; set; }

            public int? NumberOfOptionsInSettings { get; set; }
        }

        #endregion
    }
}