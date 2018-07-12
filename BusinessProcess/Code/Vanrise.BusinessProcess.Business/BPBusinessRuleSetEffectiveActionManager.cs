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

        public Vanrise.Entities.IDataRetrievalResult<BusinessRuleEffectiveActionDetail> GetFilteredBPBusinessRuleSetsEffectiveActions(Vanrise.Entities.DataRetrievalInput<BPBusinessRuleEffectiveActionQuery> input)
        {
            List<BPBusinessRuleEffectiveAction> allEffectiveActions = new List<BPBusinessRuleEffectiveAction>();
            if (input.Query.ParentBusinessRuleSetId.HasValue)
            {
                allEffectiveActions = GetParentEffectiveActions(input.Query.ParentBusinessRuleSetId.Value);
            }
            if (input.Query.BusinessRuleSetDefinitionId.HasValue)
            {
                var allBusinessRuleSets = GetCachedBPBusinessRuleSetsEffectiveActions();
                if (allBusinessRuleSets != null && allBusinessRuleSets.Count() > 0)
                {
                    allEffectiveActions = allBusinessRuleSets.GetRecord(input.Query.BusinessRuleSetDefinitionId.Value);
                }
            }
            else
            {
                BPBusinessRuleActionManager actionManager = new BPBusinessRuleActionManager();
                allEffectiveActions = actionManager.GetDefaultBusinessRulesActions(input.Query.BusinessProcessId).Values.ToList();
            }
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allEffectiveActions.ToBigResult(input, null, BPBusinessRuleSetEffectiveActionsDetailMapper));
        }
        public List<BPBusinessRuleEffectiveAction> GetParentEffectiveActions(int ruleSetId)
        {
            var allEffectiveActions = GetCachedBPBusinessRuleSetsEffectiveActions().GetRecord(ruleSetId);
            foreach (var effectiveAction in allEffectiveActions)
            {
                if (effectiveAction.IsInherited == false)
                    effectiveAction.IsInherited = true;
            }
            return allEffectiveActions;
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
                var parentRuleSet = ruleSetManager.GetBusinessRuleSetsByID(ruleSet.ParentId.Value);
                parentRuleSet.ThrowIfNull("parentRuleSet");
                var action =  parentRuleSet.Details.ActionDetails.First(x => x.BPBusinessRuleDefinitionId == ruleDefinitionId);
                action.ThrowIfNull("actionDefinition");
                return action.Settings.Action;
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
                IsInherited = bpBusinessRuleSetEffectiveAction.IsInherited,
                Description = ruleDefinitionManager.GetRuleDefinitionDescription(bpBusinessRuleSetEffectiveAction.RuleDefinitionId),
                ActionDescription = bpBusinessRuleSetEffectiveAction.Action.GetDescription(),
                ActionTypesIds = ruleDefinitionManager.GetBusinessRuleDefinitionById(bpBusinessRuleSetEffectiveAction.RuleDefinitionId).Settings.ActionTypes,
            };
        }
        private Dictionary<Guid, BPBusinessRuleEffectiveAction> UpdateEffectiveActions(int businessRuleSetId)
        {
            Dictionary<Guid, BPBusinessRuleEffectiveAction> copy = new Dictionary<Guid, BPBusinessRuleEffectiveAction>();

            BPBusinessRuleSetManager businessRuleSetManager = new BPBusinessRuleSetManager();
            var businessRuleSet = businessRuleSetManager.GetBusinessRuleSetsByID(businessRuleSetId);
            if (businessRuleSet.ParentId != null)
            {
                copy = UpdateEffectiveActions(businessRuleSet.ParentId.Value);
            }
            Dictionary<Guid, BPBusinessRuleEffectiveAction> copyFromPrevious = new Dictionary<Guid, BPBusinessRuleEffectiveAction>();
            Dictionary<Guid, BPBusinessRuleEffectiveAction> defaultActions = new Dictionary<Guid, BPBusinessRuleEffectiveAction>();
            BPBusinessRuleActionManager actionManager = new BPBusinessRuleActionManager();
            if (businessRuleSet.ParentId == null)
                defaultActions = actionManager.GetDefaultBusinessRulesActions(businessRuleSet.BPDefinitionId);
            else
                defaultActions = copy;
            foreach (var defaultAction in defaultActions)
            {
                var ruleEffectiveAction = new BPBusinessRuleEffectiveAction()
                {
                    RuleDefinitionId = defaultAction.Value.RuleDefinitionId,
                    Action = defaultAction.Value.Action,
                    IsInherited = true
                };
                copyFromPrevious.Add(defaultAction.Key, ruleEffectiveAction);
            }
            foreach (var action in businessRuleSet.Details.ActionDetails)
            {
                var effectiveAction = copyFromPrevious.GetRecord(action.BPBusinessRuleDefinitionId);
                if (copyFromPrevious != null)
                {
                    effectiveAction.IsInherited = false;
                    effectiveAction.Action = action.Settings.Action;
                }
            }
            return copyFromPrevious;
        }

        #endregion
    }
}
