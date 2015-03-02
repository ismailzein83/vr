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

        internal RouteBuildContext(RouteDetail route, SingleDestinationRoutesBuildContext parentContext)
        {
            _route = route;
            _parentContext = parentContext;
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
                return _parentContext.SuppliersCodeMatches;
            }
        }

        public SupplierZoneRates SupplierZoneRates
        {
            get
            {
                return _parentContext.SupplierRates;
            }
        }

        public RouteOptionRulesBySupplier RouteOptionsRules
        {
            get
            {
                return _parentContext.RouteOptionsRules;
            }
        }
        
        #endregion

        #region Private/Internal Methods

        internal void BuildRoute()
        {
            RouteRuleManager ruleManager = new RouteRuleManager();
            ActionExecutionPath<BaseRouteAction> executionPath = ruleManager.GetRouteActionExecutionPath();
            ActionExecutionStep<BaseRouteAction> currentStep = executionPath.FirstStep;
            object nextActionData = null;
            do
            {
                Type actionDataType = currentStep.Action.GetActionDataType();
                if (actionDataType == null || nextActionData != null)
                {
                    var actionData = nextActionData;
                    nextActionData = null;
                    RouteActionResult actionResult = currentStep.Action.Execute(this, actionData);
                    ActionExecutionStep<BaseRouteAction> nextStep;
                    if (CheckActionResult(actionResult, executionPath, currentStep, out nextStep, out nextActionData))
                        currentStep = nextStep;
                }
                else
                {
                    RouteRuleMatchFinder ruleFinder;
                    if (_parentContext.RouteRuleFindersByActionDataType.TryGetValue(actionDataType, out ruleFinder))
                    {
                        ruleFinder.GoToStart();
                        RouteRule rule;
                        bool done = false;
                        while (!done && ruleFinder.GetNext(out rule))
                        {
                            if (rule.CarrierAccountSet.IsAccountIdIncluded(_route.CustomerID))
                            {
                                RouteActionResult actionResult = currentStep.Action.Execute(this, rule.ActionData);
                                ActionExecutionStep<BaseRouteAction> nextStep;
                                if (CheckActionResult(actionResult, executionPath, currentStep, out nextStep, out nextActionData))
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

        private bool CheckActionResult(RouteActionResult actionResult, ActionExecutionPath<BaseRouteAction> executionPath, ActionExecutionStep<BaseRouteAction> currentStep, out ActionExecutionStep<BaseRouteAction> nextStep, out object nextActionData)
        {
            nextActionData = null;
            if (actionResult == null)
            {
                nextStep = currentStep.NextStep;
                return true;
            }
            if (!actionResult.IsInvalid)
            {
                if (actionResult.NextActionType != null)
                {
                    nextStep = executionPath.GetStep(actionResult.NextActionType);
                    if (nextStep == null)
                        throw new Exception(String.Format("Route Action Type '{0}' not found in execution path", actionResult.NextActionType));
                    nextActionData = actionResult.NextActionData;
                }
                else if (currentStep.IsEndAction)
                    nextStep = null;
                else
                    nextStep = currentStep.NextStep;
                return true;
            }
            else
            {
                nextStep = null;
                return false;
            }
        }

        #endregion

        #region Public Methods

        public bool TryBuildSupplierOption(string supplierId, short? percentage, out RouteSupplierOption routeOption)
        {
            CodeMatch supplierCodeMatch;
            if (_parentContext.SuppliersCodeMatches.TryGetValue(supplierId, out supplierCodeMatch))
            {
                ZoneRates zoneRates;
                if (_parentContext.SupplierRates.SuppliersZonesRates.TryGetValue(supplierId, out zoneRates))
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

        public void BuildLCR()
        {
            _route.Options = new RouteOptions();
            _route.Options.SupplierOptions = _parentContext.GetLCROptions(_route.ServicesFlag).ToList();
        }

        public void ExecuteOptionsActions(int? nbOfOptions, bool onlyImportantFilters)
        {
            int maxOptions = nbOfOptions.HasValue ? nbOfOptions.Value : int.MaxValue;
            if (_route.Options != null && _route.Options.SupplierOptions != null)
            {
                RouteRuleManager ruleManager = new RouteRuleManager();
                var executionPath = ruleManager.GetRouteOptionActionExecutionPath();
                List<RouteSupplierOption> optionsToRemove = new List<RouteSupplierOption>();
                int validOptions = 0;
                foreach (var option in _route.Options.SupplierOptions)
                {
                    RouteOptionBuildContext optionBuildContext = new RouteOptionBuildContext(option, this);
                    bool isOptionRemovedFromRoute;
                    optionBuildContext.ExecuteOptionActions(onlyImportantFilters, executionPath, optionsToRemove, option, out isOptionRemovedFromRoute);
                    
                    if (!isOptionRemovedFromRoute)
                        validOptions++;
                    if (validOptions == maxOptions)
                    {
                        int removeStartIndex = _route.Options.SupplierOptions.IndexOf(option) + 1;
                        if (_route.Options.SupplierOptions.Count > removeStartIndex)
                            _route.Options.SupplierOptions.RemoveRange(removeStartIndex, _route.Options.SupplierOptions.Count - removeStartIndex);
                        break;
                    }
                }

                foreach (var optionToRemove in optionsToRemove)
                {
                    _route.Options.SupplierOptions.Remove(optionToRemove);
                }                
            }
        }

        public void BlockRoute()
        {
            _route.Options = new RouteOptions();
            _route.Options.IsBlock = true;
        }

        #endregion
    }
}
