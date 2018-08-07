using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPBusinessRuleSetEffectiveActionManager
    {
        #region public Methods

        public IEnumerable<BusinessRuleEffectiveActionDetail> GetBPBusinessRuleSetsEffectiveActions(BPBusinessRuleEffectiveActionQuery input)
        {
            List<BPBusinessRuleEffectiveAction> allEffectiveActions = new List<BPBusinessRuleEffectiveAction>();
            if (input.ParentBusinessRuleSetId.HasValue)
            {
                allEffectiveActions= GetParentRuleSetEffectiveActions(input.ParentBusinessRuleSetId.Value);
            }
            if (input.BusinessRuleSetDefinitionId.HasValue)
            {
                var allBusinessRuleSets = GetCachedBPBusinessRuleSetsEffectiveActions();
                if (allBusinessRuleSets != null && allBusinessRuleSets.Count() > 0)
                {
                    allEffectiveActions= allBusinessRuleSets.GetRecord(input.BusinessRuleSetDefinitionId.Value);
                }
            }
            else if (!input.ParentBusinessRuleSetId.HasValue && !input.BusinessRuleSetDefinitionId.HasValue)
            {
                BPBusinessRuleActionManager actionManager = new BPBusinessRuleActionManager();
                allEffectiveActions= actionManager.GetDefaultBusinessRulesActions(input.BusinessProcessId).Values.ToList();
            }
            return allEffectiveActions.MapRecords(BPBusinessRuleSetEffectiveActionsDetailMapper);
        }
        public List<BPBusinessRuleEffectiveAction> GetParentRuleSetEffectiveActions(int ruleSetId)
        {
            var allEffectiveActions = GetCachedBPBusinessRuleSetsEffectiveActions().GetRecord(ruleSetId);
            return allEffectiveActions;
        }
        public List<BPBusinessRuleActionDetails> GetRuleSetEffectiveActions(int bpBusinessRuleSetId)
        {
            BPBusinessRuleSetManager businessRuleSetManager = new BPBusinessRuleSetManager();
            var businessRuleSet = businessRuleSetManager.GetBusinessRuleSetsByID(bpBusinessRuleSetId);
            businessRuleSet.ThrowIfNull("businessRuleSet");
            businessRuleSet.Details.ThrowIfNull("businessRuleSet.Details");
            return businessRuleSet.Details.ActionDetails;
        }
        public BusinessRuleAction GetParentAction (int businessRuleSetId,Guid ruleDefinitionId)
        {
            BPBusinessRuleActionManager actionManager = new BPBusinessRuleActionManager ();
            BPBusinessRuleSetManager ruleSetManager = new BPBusinessRuleSetManager();
            var ruleSet = ruleSetManager.GetBusinessRuleSetsByID(businessRuleSetId);
            ruleSet.ThrowIfNull("ruleSet");
            if (ruleSet.ParentId == null)
            {
                var defaultAction = actionManager.GetBusinessRuleAction(ruleDefinitionId);
                defaultAction.ThrowIfNull("defaultAction");
                return defaultAction.Details.Settings.Action;
            }
            else
            {
                var allEffectiveActions = GetCachedBPBusinessRuleSetsEffectiveActions().GetRecord(ruleSet.ParentId.Value);
                return allEffectiveActions.FirstOrDefault(x => x.RuleDefinitionId == ruleDefinitionId).Action;
            }
        }
        public string GetParentActionDescription (int ruleSetId,Guid ruleDefinitionId)
        {
            var parentAction = GetParentAction(ruleSetId, ruleDefinitionId);
            parentAction.ThrowIfNull("parentAction");
            return parentAction.GetDescription();
        }
        public BPBusinessRuleEffectiveAction GetBusinessRuleEffectiveAction(Guid businessRuleDefinitionId, int? businessRuleSetId)
        {
            if (businessRuleSetId.HasValue)
            {
                var actions = GetCachedBPBusinessRuleSetsEffectiveActions().GetRecord(businessRuleSetId.Value);
                if (actions == null || actions.Count() == 0)
                    throw new VRBusinessException("No default actions are defined for business rules");
                return actions.First(x => x.RuleDefinitionId == businessRuleDefinitionId);
            }
            else
            {
                BPBusinessRuleDefinitionManager definitionManager = new BPBusinessRuleDefinitionManager();
                BPBusinessRuleActionManager actionManager = new BPBusinessRuleActionManager();
                var ruleDefinition = definitionManager.GetBusinessRuleDefinitionById(businessRuleDefinitionId);
                ruleDefinition.ThrowIfNull("Rule Definition");
                var allEffectiveActions = actionManager.GetDefaultBusinessRulesActions(ruleDefinition.BPDefintionId).Values.ToList();
                return allEffectiveActions.First(x => x.RuleDefinitionId == businessRuleDefinitionId);
            }
        }
        public IEnumerable<BPBusinessRuleActionType> GetRuleActionsExtensionConfigs(ActionTypesInfoFilter actionTypeFilter)
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            var customObjects = new List<object>();
            if (actionTypeFilter.Filters != null)
            {
                foreach (IActionTypesFilter actionType in actionTypeFilter.Filters)
                    customObjects.Add(null);
            }
            Func<BPBusinessRuleActionType, bool> filterPredicate = (actionType) =>
         {
             if (actionTypeFilter.Filters != null)
             {
                 for (int i = 0; i < actionTypeFilter.Filters.Count(); i++)
                 {
                     var actionTypeContext = new ActionTypeFilterContext() { ActionTypeId = actionType.ExtensionConfigurationId, CustomData = customObjects[i] };
                     bool filterResult = actionTypeFilter.Filters.ElementAt(i).IsExcluded(actionTypeContext);
                     customObjects[i] = actionTypeContext.CustomData;
                     if (filterResult)
                         return false;
                 }
             }
             return true;
         };
            return extensionConfiguration.GetExtensionConfigurations<BPBusinessRuleActionType>(BPBusinessRuleActionType.EXTENSION_TYPE).FindAllRecords(filterPredicate);
        }
        #endregion

        #region Private Methods

        private Dictionary<int, List<BPBusinessRuleEffectiveAction>> GetCachedBPBusinessRuleSetsEffectiveActions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<BPBusinessRuleSetManager.CacheManager>().GetOrCreateObject("GetCachedBPBusinessRuleSetsEffectiveActions",
            () =>
            {
                Dictionary<int, List<BPBusinessRuleEffectiveAction>> cachedBusinessRuleActionsByRuleSetId = new Dictionary<int, List<BPBusinessRuleEffectiveAction>>();
                BPBusinessRuleSetManager businessRuleSetManager = new BPBusinessRuleSetManager();
                var businessRuleSets = businessRuleSetManager.GetAllBusinessRuleSets();
                foreach (var businesssRuleSet in businessRuleSets)
                {
                    BPBusinessRuleActionManager actionManager = new BPBusinessRuleActionManager();
                    var effectiveActions = actionManager.GetDefaultBusinessRulesActions(businesssRuleSet.BPDefinitionId);
                  var ruleSetEffectiveActions =  UpdateEffectiveActions(businesssRuleSet.BPBusinessRuleSetId);
                  cachedBusinessRuleActionsByRuleSetId.Add(businesssRuleSet.BPBusinessRuleSetId, ruleSetEffectiveActions.Values.ToList());
                }
                return cachedBusinessRuleActionsByRuleSetId;
            });
        }
        private BusinessRuleEffectiveActionDetail BPBusinessRuleSetEffectiveActionsDetailMapper(BPBusinessRuleEffectiveAction bpBusinessRuleSetEffectiveAction)
        {
            BPBusinessRuleDefinitionManager ruleDefinitionManager = new BPBusinessRuleDefinitionManager();
            return new BusinessRuleEffectiveActionDetail()
            {
                Entity = bpBusinessRuleSetEffectiveAction,
                RuleDefinitionId = bpBusinessRuleSetEffectiveAction.RuleDefinitionId,
                Description = ruleDefinitionManager.GetRuleDefinitionDescription(bpBusinessRuleSetEffectiveAction.RuleDefinitionId),
                Title = ruleDefinitionManager.GetRuleDefinitionTitle(bpBusinessRuleSetEffectiveAction.RuleDefinitionId),
                ActionDescription = bpBusinessRuleSetEffectiveAction.Action.GetDescription(),
                ActionTypesIds = ruleDefinitionManager.GetBusinessRuleDefinitionById(bpBusinessRuleSetEffectiveAction.RuleDefinitionId).Settings.ActionTypes,
            };
        }
        private Dictionary<Guid, BPBusinessRuleEffectiveAction> UpdateEffectiveActions(int businessRuleSetId)
        {
            Dictionary<Guid, BPBusinessRuleEffectiveAction> actions = null;

            BPBusinessRuleSetManager businessRuleSetManager = new BPBusinessRuleSetManager();
            var businessRuleSet = businessRuleSetManager.GetBusinessRuleSetsByID(businessRuleSetId);
            if (businessRuleSet.ParentId != null)
            {
                actions = UpdateEffectiveActions(businessRuleSet.ParentId.Value);
            }

            if (actions == null)
            {
                BPBusinessRuleActionManager actionManager = new BPBusinessRuleActionManager();
                actions = actionManager.GetDefaultBusinessRulesActions(businessRuleSet.BPDefinitionId);
            }

            Dictionary<Guid, BPBusinessRuleEffectiveAction> actionsCopy = new Dictionary<Guid, BPBusinessRuleEffectiveAction>();

            foreach (var action in actions)
            {
                var ruleEffectiveAction = new BPBusinessRuleEffectiveAction()
                {
                    RuleDefinitionId = action.Value.RuleDefinitionId,
                    Action = action.Value.Action,
                };
                actionsCopy.Add(action.Key, ruleEffectiveAction);
            }

            foreach (var action in businessRuleSet.Details.ActionDetails)
            {
                var effectiveAction = actionsCopy.GetRecord(action.BPBusinessRuleDefinitionId);
                if (effectiveAction != null)
                {
                    effectiveAction.Action = action.Settings.Action;
                }
            }
            return actionsCopy;
        }
      


        #endregion
    }
}
