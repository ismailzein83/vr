using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRuleManager : Vanrise.Rules.RuleManager<RouteRule>
    {        
        public RouteRule GetMatchRule(RouteRuleTarget target)
        {
            var ruleTrees = GetRuleTreesByPriority();
            if (ruleTrees != null)
            {
                foreach (var ruleTree in ruleTrees)
                {
                    var matchRule = ruleTree.GetMatchRule(target) as RouteRule;
                    if (matchRule != null)
                        return matchRule;
                }
            }
            return null;
        }

        Vanrise.Rules.RuleTree[] GetRuleTreesByPriority()
        {
            return GetCachedOrCreate("GetRuleTreesByPriority",
                () =>
                {
                    List<Vanrise.Rules.RuleTree> ruleTrees = new List<Vanrise.Rules.RuleTree>();
                    var structureBehaviors = GetRuleStructureBehaviors();
                    var routeRuleTypes = GetRouteRuleTypesTemplates();

                    int? currentPriority = null;
                    List<Vanrise.Rules.BaseRule> currentRules = null;
                    foreach (var ruleType in routeRuleTypes.OrderBy(itm => GetRuleTypePriority(itm)))
                    {
                        int priority = GetRuleTypePriority(ruleType);
                        if (currentPriority == null || currentPriority.Value != priority)
                        { 
                            if (currentRules != null && currentRules.Count > 0)
                                ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
                            currentPriority = priority;
                            currentRules = new List<Vanrise.Rules.BaseRule>();
                        }
                        var ruleTypeRules = GetFilteredRules(itm => itm.Settings.ConfigId == ruleType.TemplateConfigID);
                        if (ruleTypeRules != null)
                            currentRules.AddRange(ruleTypeRules);
                    }
                    if (currentRules != null && currentRules.Count > 0)
                        ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
                    return ruleTrees.ToArray();
                });
        }

        int GetRuleTypePriority(TemplateConfig ruleTypeConfig)
        {
            return ruleTypeConfig.Settings != null ? (ruleTypeConfig.Settings as RouteRuleTypeSettings).Priority : int.MaxValue;
        }

        public List<Vanrise.Entities.TemplateConfig> GetRouteRuleTypesTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.RouteRuleConfigType);
        }

        IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetRuleStructureBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            ruleStructureBehaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorByCode());
            ruleStructureBehaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorBySaleZone());
            ruleStructureBehaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorByCustomer());
            ruleStructureBehaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorByRoutingProduct());
            return ruleStructureBehaviors;
        }

        public Vanrise.Entities.IDataRetrievalResult<RouteRule> GetFilteredRouteRules(Vanrise.Entities.DataRetrievalInput<RouteRuleQuery> input)
        {
            IRouteRuleDataManager dataManager = BEDataManagerFactory.GetDataManager<IRouteRuleDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredRouteRules(input));
        }

        public RouteRule GetRouteRule(int routeRuleId)
        {
            IRouteRuleDataManager dataManager = BEDataManagerFactory.GetDataManager<IRouteRuleDataManager>();
            return dataManager.GetRouteRule(routeRuleId);
        }


        public TOne.Entities.InsertOperationOutput<RouteRule> AddRouteRule(RouteRule routeRule)
        {
            TOne.Entities.InsertOperationOutput<RouteRule> insertOperationOutput = new TOne.Entities.InsertOperationOutput<RouteRule>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int routeRuleId = -1;

            IRouteRuleDataManager dataManager = BEDataManagerFactory.GetDataManager<IRouteRuleDataManager>();
            bool insertActionSucc = dataManager.Insert(routeRule, out routeRuleId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                routeRule.RouteRuleId = routeRuleId;
                insertOperationOutput.InsertedObject = routeRule;
            }

            return insertOperationOutput;
        }

        public TOne.Entities.UpdateOperationOutput<RouteRule> UpdateRouteRule(RouteRule routeRule)
        {
            IRouteRuleDataManager dataManager = BEDataManagerFactory.GetDataManager<IRouteRuleDataManager>();

            bool updateActionSucc = dataManager.Update(routeRule);
            TOne.Entities.UpdateOperationOutput<RouteRule> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<RouteRule>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = routeRule;
            }

            return updateOperationOutput;
        }

        public TOne.Entities.DeleteOperationOutput<object> DeleteRouteRule(int routeRuleId)
        {
            IRouteRuleDataManager dataManager = BEDataManagerFactory.GetDataManager<IRouteRuleDataManager>();

            TOne.Entities.DeleteOperationOutput<object> deleteOperationOutput = new TOne.Entities.DeleteOperationOutput<object>();
            deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;

            bool deleteActionSucc = dataManager.Delete(routeRuleId);

            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }


        public List<Vanrise.Entities.TemplateConfig> GetCodeCriteriaGroupTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CodeCriteriaGroupConfigType);
        }
    }
}
