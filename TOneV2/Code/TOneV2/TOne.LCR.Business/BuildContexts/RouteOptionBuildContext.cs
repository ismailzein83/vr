using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class RouteOptionBuildContext : IRouteOptionBuildContext, IDisposable
    {
        #region ctor/Local Variables

        RouteSupplierOption _routeOption;
        RouteBuildContext _parentContext;
        bool _removeBlockedOptions = false;

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

        internal void ExecuteOptionActions(ActionExecutionPath<BaseRouteOptionAction> executionPath, out bool removeOption)
        {
            removeOption = false;
            ActionExecutionStep<BaseRouteOptionAction> currentStep = executionPath.FirstStep;
            while (currentStep != null)
            {

                Type actionDataType = currentStep.Action.GetActionDataType();
                if (actionDataType == null)
                {
                    RouteOptionActionResult actionResult = currentStep.Action.Execute(this, null);
                    ActionExecutionStep<BaseRouteOptionAction> nextStep;
                    if (CheckActionResult(actionResult, executionPath, currentStep, out nextStep, out removeOption))
                        currentStep = nextStep;
                    else
                        currentStep = currentStep.NextStep;
                }
                else
                {
                    RouteRulesByActionDataType optionRules;
                    bool done = false;
                    if (_parentContext.RouteOptionsRules != null
                        && _parentContext.RouteOptionsRules.TryGetValue(_routeOption.SupplierId, out optionRules))
                    {
                        RouteRuleMatches ruleMatches;
                        if (optionRules.TryGetValue(actionDataType, out ruleMatches))
                        {
                            RouteRuleMatchFinder ruleFinder = new RouteRuleMatchFinder(this.Route.Code, _routeOption.SupplierZoneId, ruleMatches);
                            ruleFinder.GoToStart();
                            RouteRule rule;

                            while (!done && ruleFinder.GetNext(out rule))
                            {
                                RouteOptionActionResult actionResult = currentStep.Action.Execute(this, rule.ActionData);
                                ActionExecutionStep<BaseRouteOptionAction> nextStep;
                                if (CheckActionResult(actionResult, executionPath, currentStep, out nextStep, out removeOption))
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

            if (_routeOption.Setting != null && _routeOption.Setting.IsBlocked && _removeBlockedOptions)
                removeOption = true;
        }

        private bool CheckActionResult(RouteOptionActionResult actionResult, ActionExecutionPath<BaseRouteOptionAction> executionPath,
            ActionExecutionStep<BaseRouteOptionAction> currentStep, out ActionExecutionStep<BaseRouteOptionAction> nextStep, out bool removeOption)
        {
            removeOption = false;
            nextStep = null;
            if (actionResult != null && (actionResult.IsInvalid || actionResult.NotMatchingTheRule))
            {
                return false;
            }
            else
            {
                bool nextStepSet = false;
                if (actionResult != null)
                {
                    if (actionResult.BlockOption || actionResult.RemoveOption)
                    {
                        if (actionResult.RemoveOption)
                            removeOption = true;
                        else if (actionResult.BlockOption)
                        {
                            if (_routeOption.Setting == null)
                                _routeOption.Setting = new OptionSetting();
                            _routeOption.Setting.IsBlocked = true;
                        }
                        nextStep = null;
                        nextStepSet = true;
                    }
                }

                if (!nextStepSet)
                {
                    if (currentStep.IsEndAction)
                        nextStep = null;
                    else
                        nextStep = currentStep.NextStep;
                }

                return true;
            }
        }

        #endregion


        public void Dispose()
        {
        }
    }
}
