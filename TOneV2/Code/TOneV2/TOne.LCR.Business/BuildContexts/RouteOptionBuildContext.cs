using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class RouteOptionBuildContext : IRouteOptionBuildContext
    {
        #region ctor/Local Variables

        RouteSupplierOption _routeOption;
        RouteBuildContext _parentContext;
        bool _removeBlockedOptions = true;

        internal RouteOptionBuildContext(RouteSupplierOption routeOption, RouteBuildContext parentContext)
        {
            _routeOption = routeOption;
            _parentContext = parentContext;
        }

        #endregion

        #region Public Properties

        public RouteSupplierOption RouteOption
        {
            get
            {
                return _routeOption;
            }
        }

        public RouteDetail Route
        {
            get
            {
                return _parentContext.Route;
            }
        }

        #endregion

        #region Private/Internal Methods

        internal void ExecuteOptionActions(bool onlyImportantFilters, ActionExecutionPath<BaseRouteOptionAction> executionPath,
            List<RouteSupplierOption> optionsToRemove, RouteSupplierOption option, out bool isOptionRemovedFromRoute)
        {
            isOptionRemovedFromRoute = false;
            ActionExecutionStep<BaseRouteOptionAction> currentStep = executionPath.FirstStep;
            do
            {
                if (onlyImportantFilters && !currentStep.Action.IsImportant)
                    currentStep = currentStep.NextStep;

                Type actionDataType = currentStep.Action.GetActionDataType();
                if (actionDataType == null)
                {
                    RouteOptionActionResult actionResult = currentStep.Action.Execute(this, null);
                    ActionExecutionStep<BaseRouteOptionAction> nextStep;
                    if (CheckActionResult(actionResult, option, optionsToRemove, executionPath, currentStep, out nextStep, ref isOptionRemovedFromRoute))
                        currentStep = nextStep;
                }
                else
                {
                    RouteRulesByActionDataType optionRules;
                    bool done = false;
                    if (_parentContext.RouteOptionsRules != null 
                        && _parentContext.RouteOptionsRules.Rules.TryGetValue(option.SupplierId, out optionRules))
                    {
                        RouteRuleMatches ruleMatches;
                        if (optionRules.Rules.TryGetValue(actionDataType, out ruleMatches))
                        {
                            RouteRuleMatchFinder ruleFinder = new RouteRuleMatchFinder(this.Route.Code, option.SupplierZoneId, ruleMatches);
                            ruleFinder.GoToStart();
                            RouteRule rule;

                            while (!done && ruleFinder.GetNext(out rule))
                            {
                                RouteOptionActionResult actionResult = currentStep.Action.Execute(this, rule.ActionData);
                                ActionExecutionStep<BaseRouteOptionAction> nextStep;
                                if (CheckActionResult(actionResult, option, optionsToRemove, executionPath, currentStep, out nextStep, ref isOptionRemovedFromRoute))
                                {
                                    done = true;
                                    currentStep = nextStep;
                                }
                            }
                        }
                    }
                    if (!done)
                        currentStep = currentStep.NextStep;
                }
            }
            while (currentStep != null);
        }

        private bool CheckActionResult(RouteOptionActionResult actionResult, RouteSupplierOption option, List<RouteSupplierOption> optionsToRemove, ActionExecutionPath<BaseRouteOptionAction> executionPath,
            ActionExecutionStep<BaseRouteOptionAction> currentStep, out ActionExecutionStep<BaseRouteOptionAction> nextStep, ref bool isOptionRemovedFromRoute)
        {
            if (actionResult == null)
            {
                nextStep = currentStep.NextStep;
                return true;
            }
            if (!actionResult.IsInvalid)
            {
                //if (filterResult.Notify)
                //    ;//TODO add to notification
                if (actionResult.DontMatchRoute)
                {
                    nextStep = null;
                    return false;
                }
                else if (actionResult.BlockOption || actionResult.RemoveOption)
                {
                    if (actionResult.RemoveOption
                            || (actionResult.BlockOption && _removeBlockedOptions))
                        optionsToRemove.Add(option);
                    else if (actionResult.BlockOption)
                        option.IsBlocked = true;
                    isOptionRemovedFromRoute = true;
                    nextStep = null;
                }
                else
                {
                    if (currentStep.IsEndAction)
                        nextStep = null;
                    else
                        nextStep = currentStep.NextStep;
                }

                return true;
            }
            else
            {
                nextStep = null;
                return false;
            }
        }

        #endregion

    }
}
