using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public class SaleEntityRouteRuleExecutionContext : ISaleEntityRouteRuleExecutionContext
    {
        RouteRule _routeRule;
        internal List<RouteOptionRuleTarget> _options = new List<RouteOptionRuleTarget>();
        HashSet<int> _filteredSupplierIds;
        Vanrise.Rules.RuleTree[] _ruleTreesForRouteOptions;
        List<SupplierCodeMatchWithRate> _validSupplierCodeMatches;

        public RouteRule RouteRule { get { return _routeRule; } }
        public int? NumberOfOptions { get; internal set; }
        public HashSet<int> SaleZoneServiceList { get; internal set; }
        public string SaleZoneServiceIds { get; internal set; }
        public RoutingDatabase RoutingDatabase { get; internal set; }
        public bool KeepBackupsForRemovedOptions { get; internal set; }

        internal List<SupplierCodeMatchWithRate> SupplierCodeMatches { private get; set; }
        internal SupplierCodeMatchWithRateBySupplier SupplierCodeMatchBySupplier { private get; set; }

        public SaleEntityRouteRuleExecutionContext(RouteRule routeRule, Vanrise.Rules.RuleTree[] ruleTreesForRouteOptions)
        {
            _ruleTreesForRouteOptions = ruleTreesForRouteOptions;
            _routeRule = routeRule;
            SupplierFilterSettings supplierFilterSettings = new SupplierFilterSettings { RoutingProductId = routeRule.Criteria.GetRoutingProductId() };
            _filteredSupplierIds = SupplierGroupContext.GetFilteredSupplierIds(supplierFilterSettings);
        }


        public ReadOnlyCollection<RouteOptionRuleTarget> GetOptions()
        {
            return _options.AsReadOnly();
        }

        public SupplierCodeMatchWithRate GetSupplierCodeMatch(int supplierId)
        {
            if (_filteredSupplierIds == null || _filteredSupplierIds.Contains(supplierId))
            {
                if (this.SupplierCodeMatchBySupplier != null)
                {
                    SupplierCodeMatchWithRate supplierCodeMatch;
                    if (this.SupplierCodeMatchBySupplier.TryGetValue(supplierId, out supplierCodeMatch))
                        return supplierCodeMatch;
                }
            }
            return null;
        }

        public RouteOptionRuleTarget BuildRouteOptionRuleTarget(RouteRuleTarget routeRuleTarget, IRouteOptionSettings option, List<IRouteBackupOptionSettings> backups)
        {
            RouteOptionRuleTarget routeOptionRuleTarget = null;
            var optionSupplierCodeMatchWithRate = GetSupplierCodeMatch(option.SupplierId);
            if (optionSupplierCodeMatchWithRate != null)
            {
                routeOptionRuleTarget = Helper.CreateRouteOptionRuleTarget(routeRuleTarget, optionSupplierCodeMatchWithRate, option);
                if (backups != null && backups.Count > 0)
                {
                    routeOptionRuleTarget.Backups = new List<RouteBackupOptionRuleTarget>();
                    foreach (IRouteBackupOptionSettings backup in backups)
                    {
                        SupplierCodeMatchWithRate backupOptionSupplierCodeMatch = GetSupplierCodeMatch(backup.SupplierId);
                        if (backupOptionSupplierCodeMatch == null)
                            continue;

                        var backupOption = Helper.CreateRouteBackupOptionRuleTarget(routeRuleTarget, backupOptionSupplierCodeMatch, backup);
                        routeOptionRuleTarget.Backups.Add(backupOption);
                    }
                    if (routeOptionRuleTarget.Backups.Count == 0)
                        routeOptionRuleTarget.Backups.Add(Helper.CreateRouteBackupOptionRuleTargetFromOption(routeOptionRuleTarget));
                }
            }
            else if (KeepBackupsForRemovedOptions && backups != null && backups.Count > 0)
            {
                List<RouteBackupOptionRuleTarget> routeBackupOptionRuleTargets = new List<RouteBackupOptionRuleTarget>();
                foreach (IRouteBackupOptionSettings backup in backups)
                {
                    SupplierCodeMatchWithRate backupOptionSupplierCodeMatch = GetSupplierCodeMatch(backup.SupplierId);
                    if (backupOptionSupplierCodeMatch == null)
                        continue;

                    var backupOption = Helper.CreateRouteBackupOptionRuleTarget(routeRuleTarget, backupOptionSupplierCodeMatch, backup);
                    routeBackupOptionRuleTargets.Add(backupOption);
                }
                if (routeBackupOptionRuleTargets.Count > 0)
                {
                    var firstBackup = routeBackupOptionRuleTargets.First();

                    routeOptionRuleTarget = Helper.CreateRouteOptionRuleTargetFromBackup(firstBackup, option);
                    var otherBackups = routeBackupOptionRuleTargets.FindAllRecords(itm => itm != firstBackup);
                    if (otherBackups != null && otherBackups.Count() > 0)
                        routeOptionRuleTarget.Backups.AddRange(otherBackups);
                    else
                        routeOptionRuleTarget.Backups.Add(firstBackup);
                }
            }

            if (routeOptionRuleTarget != null && routeOptionRuleTarget.Backups.Count == 0)
                routeOptionRuleTarget.Backups = null;

            return routeOptionRuleTarget;
        }

        public List<SupplierCodeMatchWithRate> GetFilteredSuppliersCodeMatches()
        {
            if (_validSupplierCodeMatches == null)
            {
                if (_filteredSupplierIds == null)
                    _validSupplierCodeMatches = this.SupplierCodeMatches;
                else
                {
                    _validSupplierCodeMatches = new List<SupplierCodeMatchWithRate>();
                    if (this.SupplierCodeMatches != null)
                    {
                        foreach (var supplierCodeMatch in this.SupplierCodeMatches)
                        {
                            if (_filteredSupplierIds.Contains(supplierCodeMatch.CodeMatch.SupplierId))
                            {
                                _validSupplierCodeMatches.Add(supplierCodeMatch);
                            }
                        }
                    }
                }
            }
            return _validSupplierCodeMatches;
        }


        public List<SupplierCodeMatchWithRate> GetAllSuppliersCodeMatches()
        {
            return this.SupplierCodeMatches;
        }

        internal void CheckRouteOptionRule(BaseRouteOptionRuleTarget targetOption, RouteRule routeRule)
        {
            var routeOptionRule = GetRouteOptionRule(targetOption);

            if (routeOptionRule != null)
            {
                targetOption.ExecutedRuleId = routeOptionRule.RuleId;
                RouteOptionRuleExecutionContext routeOptionRuleExecutionContext = new RouteOptionRuleExecutionContext() { SaleZoneServiceIds = SaleZoneServiceIds, RouteRule = routeRule };
                routeOptionRule.Settings.Execute(routeOptionRuleExecutionContext, targetOption);
            }
        }

        internal RouteOption CreateOptionFromTarget(RouteOptionRuleTarget targetOption, out bool allItemsBlocked)
        {
            allItemsBlocked = true;
            RouteOption routeOption = new RouteOption
            {
                SupplierId = targetOption.SupplierId,
                SupplierCode = targetOption.SupplierCode,
                SupplierZoneId = targetOption.SupplierZoneId,
                SupplierRate = targetOption.SupplierRate,
                Percentage = targetOption.Percentage,
                IsBlocked = targetOption.BlockOption,
                ExecutedRuleId = targetOption.ExecutedRuleId,
                ExactSupplierServiceIds = targetOption.ExactSupplierServiceIds,
                NumberOfTries = targetOption.NumberOfTries
            };

            if (!routeOption.IsBlocked)
                allItemsBlocked = false;

            if (targetOption.Backups != null)
            {
                routeOption.Backups = new List<RouteBackupOption>();
                foreach (RouteBackupOptionRuleTarget backup in targetOption.Backups)
                {
                    RouteBackupOption backupOption = new RouteBackupOption()
                    {
                        SupplierId = backup.SupplierId,
                        SupplierCode = backup.SupplierCode,
                        SupplierZoneId = backup.SupplierZoneId,
                        SupplierRate = backup.SupplierRate,
                        IsBlocked = backup.BlockOption,
                        ExecutedRuleId = backup.ExecutedRuleId,
                        ExactSupplierServiceIds = backup.ExactSupplierServiceIds,
                        NumberOfTries = backup.NumberOfTries
                    };

                    routeOption.Backups.Add(backupOption);

                    if (!backupOption.IsBlocked)
                        allItemsBlocked = false;
                }
            }

            ////These values are set to default because route options are cached based on route rule; 
            ////so each time we reset these values to default because they are related to the execution of route option rule and not to route rule
            //targetOption.ExecutedRuleId = null;
            //targetOption.BlockOption = false;
            //targetOption.FilterOption = false;
            return routeOption;
        }

        private RouteOptionRule GetRouteOptionRule(BaseRouteOptionRuleTarget targetOption)
        {
            if (_ruleTreesForRouteOptions != null)
            {
                foreach (var ruleTree in _ruleTreesForRouteOptions)
                {
                    var matchRule = ruleTree.GetMatchRule(targetOption) as RouteOptionRule;
                    if (matchRule != null)
                        return matchRule;
                }
            }
            return null;
        }
    }
}