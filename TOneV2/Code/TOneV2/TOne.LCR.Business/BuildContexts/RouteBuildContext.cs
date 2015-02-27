using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{    
    public class RouteBuildContext : IRouteBuildContext
    {
        #region ctor/Local Variables

        RouteDetail _route;
        SingleDestinationRoutesBuildContext _parentContext;
        Dictionary<Type, RouteRuleMatchFinder> _routeRuleFindersByActionDataType;        
        Dictionary<string, CodeMatch> _suppliersCodeMatches;
        private SupplierZoneRates _supplierZoneRates;

        bool _removeBlockedOptions = true;

        internal RouteBuildContext(RouteDetail route, SingleDestinationRoutesBuildContext parentContext, Dictionary<string, CodeMatch> suppliersCodeMatches,
            SupplierZoneRates supplierZoneRates, Dictionary<Type, RouteRuleMatchFinder> routeRuleFindersByActionDataType)
        {
            _route = route;
            _parentContext = parentContext;
            _suppliersCodeMatches = suppliersCodeMatches;
            _supplierZoneRates = supplierZoneRates;
            _routeRuleFindersByActionDataType = routeRuleFindersByActionDataType;
        }

        #endregion

        #region Public Properties

        public RouteDetail Route
        {
            get
            {
                return _route;
            }
        }

        public Dictionary<string, CodeMatch> SuppliersCodeMatches
        {
            get
            {
                return _suppliersCodeMatches;
            }
        }

        public SupplierZoneRates SupplierZoneRates
        {
            get
            {
                return _supplierZoneRates;
            }
        }

        #endregion

        #region Private/Internal Methods

        internal void BuildRoute()
        {
            RouteRuleManager ruleManager = new RouteRuleManager();
            RuleActionExecutionPath executionPath = ruleManager.GetExecutionPath();
            RuleActionExecutionStep currentStep = executionPath.FirstStep;
            do
            {
                Type actionDataType = currentStep.Action.GetActionDataType();
                if (actionDataType == null)
                {
                    RouteActionResult actionResult = currentStep.Action.Execute(this, null);
                    RuleActionExecutionStep nextStep;
                    if (CheckActionResult(actionResult, executionPath, currentStep, out nextStep))
                        currentStep = nextStep;
                }
                else
                {
                    RouteRuleMatchFinder ruleFinder;
                    if (_routeRuleFindersByActionDataType.TryGetValue(actionDataType, out ruleFinder))
                    {
                        ruleFinder.GoToStart();
                        BaseRouteRule rule;
                        bool done = false;
                        while (!done && ruleFinder.GetNext(out rule))
                        {
                            if (rule.CarrierAccountSet.IsAccountIdIncluded(_route.CustomerID))
                            {
                                RouteActionResult actionResult = currentStep.Action.Execute(this, (rule as CustomerRouteRule).ActionData);
                                RuleActionExecutionStep nextStep;
                                if (CheckActionResult(actionResult, executionPath, currentStep, out nextStep))
                                {
                                    done = true;
                                    currentStep = nextStep;
                                }
                            }
                        }
                    }
                    else
                    {
                        currentStep = currentStep.NextStep;
                    }
                }
            }
            while (currentStep != null);
        }

        private static bool CheckActionResult(RouteActionResult actionResult, RuleActionExecutionPath executionPath, RuleActionExecutionStep currentStep, out RuleActionExecutionStep nextStep)
        {
            if(actionResult == null)
            {
                nextStep = currentStep.NextStep;
                return true;
            }
            if (!actionResult.IsInvalid)
            {
                if (actionResult.NextActionType != null)
                {
                    if (!executionPath.GetAllSteps().Steps.TryGetValue(actionResult.NextActionType, out nextStep))
                        throw new Exception(String.Format("Route Action Type '{0}' not found in execution path", actionResult.NextActionType));
                }
                else if (currentStep.IsEndAction)
                    nextStep = null;
                else
                    nextStep = currentStep.NextStep;
                return true;
            }
            else
            {
                nextStep = currentStep.NextStep;
                return false;
            }
        }

        private List<BaseRouteOptionFilter> GetRouteOptionFilters()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Public Methods

        public void BuildLCR()
        {
            _route.Options = new RouteOptions();
            _route.Options.SupplierOptions = _parentContext.GetLCROptions(_route.ServicesFlag).ToList();
        }

        public void ApplyOptionsFilter(int? nbOfOptions, bool onlyImportantFilters)
        {
            int maxOptions = nbOfOptions.HasValue ? nbOfOptions.Value : int.MaxValue;
            if (_route.Options != null && _route.Options.SupplierOptions != null)
            {
                List<BaseRouteOptionFilter> optionFilters = GetRouteOptionFilters();
                if (optionFilters != null)
                {
                    List<RouteSupplierOption> optionsToRemove = new List<RouteSupplierOption>();
                    int validOptions = 0;
                    foreach (var option in _route.Options.SupplierOptions)
                    {
                        bool isValid = true;
                        foreach (var filter in optionFilters)
                        {
                            var filterResult = filter.Execute(null, option, _route);
                            if (filterResult != null)
                            {
                                if (filterResult.Notify)
                                    ;//TODO add to notification
                                if (filterResult.BlockOption || filterResult.RemoveOption)
                                {
                                    isValid = false;

                                    if (filterResult.RemoveOption
                                        || (filterResult.BlockOption && _removeBlockedOptions))
                                        optionsToRemove.Add(option);
                                    else if (filterResult.BlockOption)
                                        option.IsBlocked = true;

                                    break;
                                }
                            }
                        }
                        if (isValid)
                            validOptions++;
                        if (validOptions == maxOptions)
                            break;
                    }
                    foreach (var optionToRemove in optionsToRemove)
                        _route.Options.SupplierOptions.Remove(optionToRemove);
                }
            }
        }

        public bool TryBuildSupplierOption(string supplierId, short? percentage, out RouteSupplierOption routeOption)
        {
            CodeMatch supplierCodeMatch;
            if (_suppliersCodeMatches.TryGetValue(supplierId, out supplierCodeMatch))
            {
                ZoneRates zoneRates;
                if (_supplierZoneRates.SuppliersZonesRates.TryGetValue(supplierId, out zoneRates))
                {
                    RateInfo rate;
                    if (zoneRates.ZonesRates.TryGetValue(supplierCodeMatch.SupplierZoneId, out rate))
                    {
                        routeOption = new RouteSupplierOption
                        {
                            SupplierId = supplierId,
                            Percentage = percentage,
                            SupplierZoneId = supplierCodeMatch.SupplierZoneId,
                            Rate = rate.Rate,
                            ServicesFlag = rate.ServicesFlag
                        };
                        return true;
                    }
                }
            }
            routeOption = null;
            return false;
        }

        public void BlockRoute()
        {
            _route.Options = new RouteOptions();
            _route.Options.IsBlock = true;
        }

        #endregion
    }
}
