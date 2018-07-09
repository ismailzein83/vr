using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using System.Linq;
using Vanrise.Common;
using System;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPBusinessRuleActionManager
    {
        #region public methods
        public BPBusinessRuleAction GetBusinessRuleAction(Guid bpBusinessRuleDefinitionId)
        {
            return GetCachedBPBusinessRuleActionsByDefinition().GetRecord(bpBusinessRuleDefinitionId);
        }
        public BusinessRuleAction GetEffectiveRuleAction (Guid bpBusinessRuleDefinitionId,int? businessRuleSetId)
        {
            if (!businessRuleSetId.HasValue)
            {
                var action = GetBusinessRuleAction(bpBusinessRuleDefinitionId);
                if (action == null)
                    throw new VRBusinessException("no default actions are defined for business rules");
                return action.Details.Settings.Action;
            }
            else
            {
                BPBusinessRuleSetEffectiveActionManager effectiveActionManager = new BPBusinessRuleSetEffectiveActionManager();
                var effectiveAction = effectiveActionManager.GetBusinessRuleEffectiveAction(bpBusinessRuleDefinitionId, businessRuleSetId.Value);
                return effectiveAction.Action;
            }
        }
        public Dictionary<Guid, BPBusinessRuleEffectiveAction> GetDefaultBusinessRulesActions(Guid businessProcessId)
        {
            Dictionary<Guid, BPBusinessRuleEffectiveAction> defaultActions = new Dictionary<Guid, BPBusinessRuleEffectiveAction>();
            BPBusinessRuleDefinitionManager bpDefinitionManager = new BPBusinessRuleDefinitionManager();
            var allActions = GetCachedBPBusinessRuleActionsByDefinition();
            var allDefinitions = bpDefinitionManager.GetAllBusinessRuleDefinitions();
            foreach (var definition in allDefinitions)
            {
                if(definition.BPDefintionId == businessProcessId)
                {
                    BPBusinessRuleAction action = allActions.GetRecord(definition.BPBusinessRuleDefinitionId);
                    if (action == null)
                        throw new VRBusinessException("BPBusinessRuleAction");
                    var effectiveAction = new BPBusinessRuleEffectiveAction()
                    {
                        RuleDefinitionId = definition.BPBusinessRuleDefinitionId,
                        //check action
                        Action = action.Details.Settings.Action,
                        IsInherited = false
                    };
                    defaultActions.Add(definition.BPBusinessRuleDefinitionId, effectiveAction);
                }
            }
            return defaultActions;
        }
       

        #endregion

        #region private methods

        private Dictionary<Guid, BPBusinessRuleAction> GetCachedBPBusinessRuleActionsByDefinition()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBPBusinessRuleActions",
            () =>
            {
                IBPBusinessRuleActionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPBusinessRuleActionDataManager>();
                var data = dataManager.GetBPBusinessRuleActions();
                return data.ToDictionary(cn => cn.Details.BPBusinessRuleDefinitionId, cn => cn);
            });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBPBusinessRuleActionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPBusinessRuleActionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreBPBusinessRuleActionsUpdated(ref _updateHandle);
            }
        }
        #endregion
    }
}