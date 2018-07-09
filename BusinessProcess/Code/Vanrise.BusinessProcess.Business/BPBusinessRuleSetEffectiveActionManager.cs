using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.BusinessProcess.Business
{
    public class BPBusinessRuleSetEffectiveActionManager
    {
        #region public Methods

        public Vanrise.Entities.IDataRetrievalResult<BusinessRuleEffectiveActionDetail> GetFilteredBPBusinessRuleSetsEffectiveActions(Vanrise.Entities.DataRetrievalInput<BPBusinessRuleEffectiveActionQuery> input)
        {
            List<BPBusinessRuleEffectiveAction> allEffectiveActions = new List<BPBusinessRuleEffectiveAction>();
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
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<Vanrise.Runtime.Business.SchedulerTaskManager.CacheManager>().GetOrCreateObject("GetCachedBPBusinessRuleSetsEffectiveActions",
            () =>
            {
                Dictionary<int, List<BPBusinessRuleEffectiveAction>> cachedBusinessRuleActionsByRuleSetId = new Dictionary<int, List<BPBusinessRuleEffectiveAction>>();
                BPBusinessRuleSetManager businessRuleSetManager = new BPBusinessRuleSetManager();
                var businessRuleSets = businessRuleSetManager.GetAllBusinessRuleSets();
                foreach (var businesssRuleSet in businessRuleSets)
                {
                    BPBusinessRuleActionManager actionManager = new BPBusinessRuleActionManager();
                    var effectiveActions = actionManager.GetDefaultBusinessRulesActions(businesssRuleSet.BPDefinitionId);
                    UpdateEffectiveActions(effectiveActions, businesssRuleSet.BPBusinessRuleSetId);
                    cachedBusinessRuleActionsByRuleSetId.Add(businesssRuleSet.BPBusinessRuleSetId, effectiveActions.Values.ToList());
                }
                return cachedBusinessRuleActionsByRuleSetId;
            });
        }
        private BusinessRuleEffectiveActionDetail BPBusinessRuleSetEffectiveActionsDetailMapper(BPBusinessRuleEffectiveAction bpBusinessRuleSetEffectiveAction)
        {
            BPBusinessRuleDefinitionManager ruleDefinitionManager = new BPBusinessRuleDefinitionManager();
            return new BusinessRuleEffectiveActionDetail()
            {
                RuleDefinitionId = bpBusinessRuleSetEffectiveAction.RuleDefinitionId,
                IsOverriden = bpBusinessRuleSetEffectiveAction.IsInherited,
                Description = ruleDefinitionManager.GetRuleDefinitionDescription(bpBusinessRuleSetEffectiveAction.RuleDefinitionId),
                ActionDescription = bpBusinessRuleSetEffectiveAction.Action.GetDescription(),
                ActionTypesIds = ruleDefinitionManager.GetBusinessRuleDefinitionById(bpBusinessRuleSetEffectiveAction.RuleDefinitionId).Settings.ActionTypes
            };
        }
        private void UpdateEffectiveActions(Dictionary<Guid, BPBusinessRuleEffectiveAction> effectiveActions, int businessRuleSetId)
        {
            BPBusinessRuleSetManager businessRuleSetManager = new BPBusinessRuleSetManager();
            var businessRuleSet = businessRuleSetManager.GetBusinessRuleSetsByID(businessRuleSetId);
            if (businessRuleSet.ParentId != null)
            {
                UpdateEffectiveActions(effectiveActions, businessRuleSet.ParentId.Value);
            }
            foreach (var action in businessRuleSet.Details.ActionDetails)
            {
                var effectiveAction = effectiveActions.GetRecord(action.BPBusinessRuleDefinitionId);
                effectiveAction.IsInherited = true;
                effectiveAction.Action = action.Settings.Action;
            }
        }

        #endregion
    }
}
