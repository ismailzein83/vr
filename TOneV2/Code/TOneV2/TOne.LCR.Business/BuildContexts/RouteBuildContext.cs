using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{    
    public class RouteBuildContext : IRouteBuildContext, IDisposable
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
                    if (_parentContext.RouteRuleFindersByActionDataType != null 
                        && _parentContext.RouteRuleFindersByActionDataType.TryGetValue(actionDataType, out ruleFinder))
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
            if (_parentContext.CodeMatchesBySupplierId.TryGetValue(supplierId, out supplierCodeMatch))
            {
                ZoneRates zoneRates;
                if (_parentContext.SupplierRates.SuppliersZonesRates.TryGetValue(supplierId, out zoneRates))
                {
                    RateInfo rate;
                    if (zoneRates.ZonesRates.TryGetValue(supplierCodeMatch.SupplierZoneId, out rate))
                    {
                        routeOption = new RouteSupplierOption(supplierId, supplierCodeMatch.SupplierZoneId, rate.Rate, rate.ServicesFlag);
                        routeOption.Percentage = percentage;
                        return true;
                    }
                }
            }
            routeOption = null;
            return false;
        }

        public RouteSupplierOption GetNextOptionInLCR()
        {
            return _parentContext.GetNextOptionInLCR();
        }

        //public void BuildLCR()
        //{
        //    _route.Options = new RouteOptions();
        //    _route.Options.SupplierOptions = _parentContext.GetLCROptions(_route.ServicesFlag).ToList();
        //}

        //public bool TryAddOptionFromLCR(out RouteSupplierOption option)
        //{
        //    if(_route.Options == null || _route.Options.SupplierOptions == null)
        //    {
        //        _route.Options = new RouteOptions();
        //        _route.Options.SupplierOptions = new List<RouteSupplierOption>();
        //    }
        //    option = _parentContext.GetNextOptionInLCR();
        //    if (option != null)
        //    {
        //        _route.Options.SupplierOptions.Add(option);
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        public void ExecuteOptionsActions(bool retrieveFromLCR, int? nbOfOptions, bool onlyImportantFilters)
        {
            int maxOptions = nbOfOptions.HasValue ? nbOfOptions.Value : int.MaxValue;

            RouteRuleManager ruleManager = new RouteRuleManager();
            var executionPath = ruleManager.GetRouteOptionActionExecutionPath();
            //List<RouteSupplierOption> optionsToRemove = new List<RouteSupplierOption>();
            int validOptions = 0;
            Queue<RouteSupplierOption> qInitialOptions = new Queue<RouteSupplierOption>();
            if (_route.Options != null && _route.Options.SupplierOptions != null)
            {
                foreach (var o in _route.Options.SupplierOptions)
                    qInitialOptions.Enqueue(o);
            }

            _route.Options = new RouteOptions();
            _route.Options.SupplierOptions = new List<RouteSupplierOption>();
            do
            {
                RouteSupplierOption current = null;
                if (qInitialOptions.Count > 0)
                    current = qInitialOptions.Dequeue();
                else if (retrieveFromLCR)
                    current = _parentContext.GetNextOptionInLCR();

                if (current == null)
                    break;

                if (!onlyImportantFilters && current.Rate > _route.Rate)
                    break;

                using (RouteOptionBuildContext optionBuildContext = new RouteOptionBuildContext(current, this))
                {
                    bool removeOption;
                    optionBuildContext.ExecuteOptionActions(onlyImportantFilters, executionPath, current, out removeOption);
                    if (!removeOption && !current.IsBlocked)
                        validOptions++;
                    if (!removeOption)
                        _route.Options.SupplierOptions.Add(current);
                }
            }
            while (validOptions < maxOptions);
        }

        public void BlockRoute()
        {
            _route.Options = new RouteOptions();
            _route.Options.IsBlock = true;
        }

        #endregion

        public void Dispose()
        {
        }
    }
}
