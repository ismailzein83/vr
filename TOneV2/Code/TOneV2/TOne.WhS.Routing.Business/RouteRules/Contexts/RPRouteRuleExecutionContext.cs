using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RPRouteRuleExecutionContext : IRPRouteRuleExecutionContext
    {
        RouteRule _routeRule;
        internal Dictionary<int, List<RouteOptionRuleTarget>> _supplierZoneOptions = new Dictionary<int, List<RouteOptionRuleTarget>>();
        HashSet<int> _filteredSupplierIds;
        Vanrise.Rules.RuleTree[] _ruleTreesForRouteOptions;
        List<SupplierCodeMatchWithRate> _validSupplierCodeMatches;

        public RouteRule RouteRule { get { throw new NotImplementedException(); } }
        public HashSet<int> SaleZoneServiceIds { get; set; }
        public RoutingDatabase RoutingDatabase { get; set; }
        internal List<SupplierCodeMatchWithRate> SupplierCodeMatches { private get; set; }
        internal SupplierCodeMatchesWithRateBySupplier SupplierCodeMatchesBySupplier { private get; set; }

        public RPRouteRuleExecutionContext(RouteRule routeRule, Vanrise.Rules.RuleTree[] ruleTreesForRouteOptions)
        {
            _routeRule = routeRule;
            _ruleTreesForRouteOptions = ruleTreesForRouteOptions;

            SupplierFilterSettings supplierFilterSettings = new SupplierFilterSettings { RoutingProductId = routeRule.Criteria.GetRoutingProductId() };
            _filteredSupplierIds = TOne.WhS.BusinessEntity.Business.SupplierGroupContext.GetFilteredSupplierIds(supplierFilterSettings);
        }

        public bool TryAddSupplierZoneOption(RouteOptionRuleTarget optionTarget)
        {
            var routeOptionRule = GetRouteOptionRule(optionTarget);
            if (routeOptionRule != null)
            {
                optionTarget.ExecutedRuleId = routeOptionRule.RuleId;
                RouteOptionRuleExecutionContext routeOptionRuleExecutionContext = new RouteOptionRuleExecutionContext() { SaleZoneServiceIds = string.Join<int>(",", SaleZoneServiceIds.ToList()), RouteRule = _routeRule };
                routeOptionRule.Settings.Execute(routeOptionRuleExecutionContext, optionTarget);

                if (optionTarget.FilterOption)
                    return false;
            }
            List<RouteOptionRuleTarget> optionRuleTargets = _supplierZoneOptions.GetOrCreateItem(optionTarget.SupplierId);
            optionRuleTargets.Add(optionTarget);
            return true;
        }

        public void CreateSupplierZoneOptionsForRP(RouteRuleTarget target, Action<HashSet<int>, RouteRuleTarget, BaseRouteOptionRuleTarget> filterOption,
            IRouteOptionSettings optionSettings, List<IRouteBackupOptionSettings> backupsSettings, HashSet<int> addedSuppliers)
        {
            bool optionAdded = false;

            if (addedSuppliers.Contains(optionSettings.SupplierId))
            {
                if (!optionSettings.Percentage.HasValue)
                    return;

                AddPercentageOption(optionSettings.SupplierId, optionSettings.Percentage.Value);
                return;
            }

            List<SupplierCodeMatchWithRate> optionSupplierCodeMatches = GetSupplierCodeMatches(optionSettings.SupplierId);
            if (optionSupplierCodeMatches != null)
            {
                foreach (var supplierCodeMatch in optionSupplierCodeMatches)
                {
                    var option = Helper.CreateRouteOptionRuleTarget(target, supplierCodeMatch, optionSettings);
                    filterOption(SaleZoneServiceIds, target, option);
                    if (option.FilterOption)
                        continue;

                    if (TryAddSupplierZoneOption(option) && !option.BlockOption)
                    {
                        optionAdded = true;
                        addedSuppliers.Add(option.SupplierId);
                    }

                }
            }

            if (optionAdded || backupsSettings == null)
                return;

            bool backupAdded = false;
            foreach (IRouteBackupOptionSettings backup in backupsSettings)
            {
                if (addedSuppliers.Contains(backup.SupplierId))
                {
                    if (!optionSettings.Percentage.HasValue)
                        return;

                    AddPercentageOption(backup.SupplierId, optionSettings.Percentage.Value);
                    return;
                }

                List<SupplierCodeMatchWithRate> backupSupplierCodeMatches = GetSupplierCodeMatches(backup.SupplierId);
                if (backupSupplierCodeMatches == null)
                    continue;

                foreach (var supplierCodeMatch in backupSupplierCodeMatches)
                {
                    var backupOption = Helper.CreateRouteOptionRuleTarget(target, supplierCodeMatch, backup, optionSettings.Percentage);
                    filterOption(SaleZoneServiceIds, target, backupOption);
                    if (backupOption.FilterOption)
                        continue;

                    if (TryAddSupplierZoneOption(backupOption) && !backupOption.BlockOption)
                    {
                        backupAdded = true;
                        addedSuppliers.Add(backupOption.SupplierId);
                    }
                }

                if (backupAdded)
                    return;
            }
        }

        private void AddPercentageOption(int supplierId, int percentage)
        {
            List<RouteOptionRuleTarget> optionRuleTargets = _supplierZoneOptions.GetRecord(supplierId);
            foreach (RouteOptionRuleTarget optionRuleTarget in optionRuleTargets)
            {
                if (optionRuleTarget.Percentage.HasValue)
                    optionRuleTarget.Percentage += percentage;
                else
                    optionRuleTarget.Percentage = percentage;
            }
        }

        internal Dictionary<int, List<RouteOptionRuleTarget>> GetSupplierZoneOptions()
        {
            return _supplierZoneOptions;
        }

        public List<SupplierCodeMatchWithRate> GetAllSuppliersCodeMatches()
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

        public List<SupplierCodeMatchWithRate> GetSupplierCodeMatches(int supplierId)
        {
            if (_filteredSupplierIds == null || _filteredSupplierIds.Contains(supplierId))
            {
                if (this.SupplierCodeMatchesBySupplier != null)
                {
                    List<SupplierCodeMatchWithRate> supplierCodeMatches;
                    if (this.SupplierCodeMatchesBySupplier.TryGetValue(supplierId, out supplierCodeMatches))
                        return supplierCodeMatches;
                }
            }
            return null;
        }

        private RouteOptionRule GetRouteOptionRule(RouteOptionRuleTarget targetOption)
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
