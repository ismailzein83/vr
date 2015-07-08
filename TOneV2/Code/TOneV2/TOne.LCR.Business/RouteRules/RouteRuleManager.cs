using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Data;
using TOne.LCR.Entities;
namespace TOne.LCR.Business
{
    public class RouteRuleManager
    {
        public ActionExecutionPath<BaseRouteAction> GetRouteActionExecutionPath()
        {
            return _routeActionExecutionPath;
        }

        static ActionExecutionPath<BaseRouteAction> _routeActionExecutionPath = BuildRouteActionExecutionPath();

        private static ActionExecutionPath<BaseRouteAction> BuildRouteActionExecutionPath()
        {
            ActionExecutionStep<BaseRouteAction> blockStep = new ActionExecutionStep<BaseRouteAction> { Action = new BlockRouteAction(), IsEndAction = true };
            ActionExecutionStep<BaseRouteAction> overrideStep = new ActionExecutionStep<BaseRouteAction> { Action = new OverrideRouteAction(), IsEndAction = true };
            //ActionExecutionStep<BaseRouteAction> buildLCRStep = new ActionExecutionStep<BaseRouteAction> { Action = new BuildLCRRouteAction() };
            ActionExecutionStep<BaseRouteAction> priorityStep = new ActionExecutionStep<BaseRouteAction> { Action = new PriorityRouteAction() };
            ActionExecutionStep<BaseRouteAction> getTopOptionsStep = new ActionExecutionStep<BaseRouteAction> { Action = new GetTopOptionsRouteAction() };
            ActionExecutionStep<BaseRouteAction> blockSupplierStep = new ActionExecutionStep<BaseRouteAction> { Action = new BlockSuppliersRouteAction() };
            ActionExecutionStep<BaseRouteAction> checkNoOptionsStep = new ActionExecutionStep<BaseRouteAction> { Action = new CheckNoOptionsRouteAction() };
            ActionExecutionStep<BaseRouteAction> applyPercentageStep = new ActionExecutionStep<BaseRouteAction> { Action = new ApplyPercentageRouteAction() };

            ActionExecutionPath<BaseRouteAction> executionPath = new ActionExecutionPath<BaseRouteAction> { FirstStep = blockStep };
            blockStep.NextStep = overrideStep;
            overrideStep.NextStep = priorityStep;
            priorityStep.NextStep = getTopOptionsStep;
            getTopOptionsStep.NextStep = blockSupplierStep;
            blockSupplierStep.NextStep = checkNoOptionsStep;
            checkNoOptionsStep.NextStep = applyPercentageStep;
            return executionPath;
        }

        public ActionExecutionPath<BaseRouteOptionAction> GetRouteOptionActionExecutionPath()
        {
            return _routeOptionActionExecutionPath;
        }

        public TOne.Entities.InsertOperationOutput<RouteRule> InsertRouteRule(RouteRule rule)
        {

            TOne.Entities.InsertOperationOutput<RouteRule> insertOperationOutput = new TOne.Entities.InsertOperationOutput<RouteRule>();

            int ruleId = -1;

            IRouteRulesDataManager dataManager = LCRDataManagerFactory.GetDataManager<IRouteRulesDataManager>();
            bool insertActionSucc = dataManager.InsertRouteRule(rule, out ruleId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                rule.RouteRuleId = ruleId;
                insertOperationOutput.InsertedObject = rule;
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            return insertOperationOutput;
        }

        public TOne.Entities.UpdateOperationOutput<RouteRule> UpdateRouteRule(RouteRule rule)
        {
            IRouteRulesDataManager dataManager = LCRDataManagerFactory.GetDataManager<IRouteRulesDataManager>();
            TOne.Entities.UpdateOperationOutput<RouteRule> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<RouteRule>();
            bool updateActionSucc = dataManager.UpdateRouteRule(rule);

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = rule;
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            return updateOperationOutput;
        }

        public TOne.Entities.DeleteOperationOutput<RouteRule> DeleteRouteRule(int ruleId)
        {
            IRouteRulesDataManager dataManager = LCRDataManagerFactory.GetDataManager<IRouteRulesDataManager>();
            TOne.Entities.DeleteOperationOutput<RouteRule> deleteOperationOutput = new TOne.Entities.DeleteOperationOutput<RouteRule>();
            bool updateActionSucc = dataManager.DeleteRouteRule(ruleId);
            if (updateActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }
            else
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;
            return deleteOperationOutput;
        }

        public List<RouteRule> GetAllRouteRule()
        {
            IRouteRulesDataManager dataManager = LCRDataManagerFactory.GetDataManager<IRouteRulesDataManager>();
            return dataManager.GetAllRouteRule();
        }

        public IEnumerable<RouteRule> GetFilteredRouteRules(List<string> ruleTypes, List<int> zoneIds, string code, List<string> customerIds, int fromRow, int toRow, bool both)
        {
            IEnumerable<RouteRule> rules = GetAllRouteRule();
            if (rules != null)
            {
                Func<RouteRule, bool> filter = (rule) =>
                    {
                        if (ruleTypes != null && ruleTypes.Count > 0 && !ruleTypes.Contains(rule.ActionData.GetType().Name))
                            return false;

                        if (customerIds != null && customerIds.Count > 0)
                        {
                            bool anyFound = false;
                            foreach (var customerId in customerIds)
                            {
                                if (rule.CarrierAccountSet.IsAccountIdIncluded(customerId))
                                    anyFound = true;
                            }
                            if (!anyFound)
                                return false;
                        }

                        var codeSetMatch = rule.CodeSet.GetMatch();

                        if (!String.IsNullOrEmpty(code))
                        {
                            bool isFound = false;
                            if (codeSetMatch.IsMatchingAllZones
                                ||
                                (
                                    codeSetMatch.MatchCodes != null
                                    &&
                                    (codeSetMatch.MatchCodes.ContainsKey(code)
                                    ||
                                    codeSetMatch.MatchCodes.Any(itm => itm.Value && code.StartsWith(itm.Key))
                                    )
                                 )
                                )
                                isFound = true;
                            if ((!isFound || rule.CodeSet.IsCodeExcluded(code)) && both)
                                return false;
                            if (!both && isFound)
                                return true;
                        }

                        if (zoneIds != null && zoneIds.Count > 0)
                        {
                            bool anyFound = false;
                            foreach (var zoneId in zoneIds)
                            {
                                if ((codeSetMatch.MatchZoneIds != null && codeSetMatch.MatchZoneIds.Contains(zoneId))
                                    ||
                                (codeSetMatch.IsMatchingAllZones && !rule.CodeSet.IsZoneExcluded(zoneId)))
                                {
                                    anyFound = true;
                                    break;
                                }
                            }
                            if (!anyFound)
                                return false;
                        }

                        return true;
                    };
                if (fromRow != 0 && toRow != 0)
                    rules = fromRow == 1 ? rules.Where(filter).Take(toRow) : rules.Where(filter).Skip(fromRow - 1).Take(toRow - fromRow + 1);
                else
                    rules = rules.Where(filter);


            }
            return rules;
        }

        public RouteRule GetRouteRuleDetails(int RouteRuleId)
        {
            IRouteRulesDataManager dataManager = LCRDataManagerFactory.GetDataManager<IRouteRulesDataManager>();
            return dataManager.GetRouteRuleDetails(RouteRuleId);
        }

        static ActionExecutionPath<BaseRouteOptionAction> _routeOptionActionExecutionPath = BuildRouteOptionActionExecutionPath();

        private static ActionExecutionPath<BaseRouteOptionAction> BuildRouteOptionActionExecutionPath()
        {
            //ActionExecutionStep<BaseRouteOptionAction> checkRateStep = new ActionExecutionStep<BaseRouteOptionAction> { Action = new CheckRateOptionAtion(), IsEndAction = true };
            ActionExecutionStep<BaseRouteOptionAction> blockStep = new ActionExecutionStep<BaseRouteOptionAction> { Action = new BlockRouteOptionAction(), IsEndAction = true };

            ActionExecutionPath<BaseRouteOptionAction> executionPath = new ActionExecutionPath<BaseRouteOptionAction> { FirstStep = blockStep };
            //checkRateStep.NextStep = blockStep;

            return executionPath;
        }


        //public RuleActionExecutionPath GetExecutionPath()
        //{
        //    return _executionPath;
        //}

        //static RuleActionExecutionPath _executionPath = BuildExecutionPath();

        //private static RuleActionExecutionPath BuildExecutionPath()
        //{
        //    RuleActionExecutionStep blockStep = new RuleActionExecutionStep { Action = new BlockRouteAction(), IsEndAction = true };
        //    RuleActionExecutionStep overrideStep = new RuleActionExecutionStep { Action = new OverrideRouteAction(), IsEndAction = true };
        //    RuleActionExecutionStep buildLCRStep = new RuleActionExecutionStep { Action = new BuildLCRRouteAction() };
        //    RuleActionExecutionStep priorityStep = new RuleActionExecutionStep { Action = new PriorityRouteAction() };
        //    RuleActionExecutionStep getTopOptionsStep = new RuleActionExecutionStep { Action = new GetTopOptionsRouteAction() };
        //    RuleActionExecutionStep checkNoOptionsStep = new RuleActionExecutionStep { Action = new CheckNoOptionsRouteAction() };
        //    RuleActionExecutionStep applyPercentageStep = new RuleActionExecutionStep { Action = new ApplyPercentageRouteAction() };

        //    RuleActionExecutionPath executionPath = new RuleActionExecutionPath { FirstStep = blockStep };
        //    blockStep.NextStep = overrideStep;
        //    overrideStep.NextStep = buildLCRStep;
        //    buildLCRStep.NextStep = priorityStep;
        //    priorityStep.NextStep = getTopOptionsStep;  
        //    getTopOptionsStep.NextStep = checkNoOptionsStep;
        //    checkNoOptionsStep.NextStep = applyPercentageStep;
        //    return executionPath;
        //}

    }
}
