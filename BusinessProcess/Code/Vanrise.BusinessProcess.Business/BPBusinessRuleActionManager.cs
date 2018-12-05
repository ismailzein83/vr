﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
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
        public BPBusinessRuleEffectiveAction GetEffectiveRuleAction(Guid bpBusinessRuleDefinitionId, int? businessRuleSetId)
        {
            if (!businessRuleSetId.HasValue)
            {
                var action = GetBusinessRuleAction(bpBusinessRuleDefinitionId);
                if (action == null)
                    throw new VRBusinessException(string.Format("No default actions are defined for business rules:{0}", bpBusinessRuleDefinitionId));
                return new BPBusinessRuleEffectiveAction
                {
                    RuleDefinitionId = bpBusinessRuleDefinitionId,
                    Action = action.Details.Settings.Action,
                    Disabled = action.Details.Settings.Disabled
                };
            }
            else
            {
                BPBusinessRuleSetEffectiveActionManager effectiveActionManager = new BPBusinessRuleSetEffectiveActionManager();
                var effectiveAction = effectiveActionManager.GetBusinessRuleEffectiveAction(bpBusinessRuleDefinitionId, businessRuleSetId.Value);
                if (effectiveAction == null)
                    throw new VRBusinessException(string.Format("No default actions are defined for business rules:{0}", bpBusinessRuleDefinitionId));
                return effectiveAction;
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
                if (definition.BPDefintionId == businessProcessId)
                {
                    BPBusinessRuleAction action = allActions.GetRecord(definition.BPBusinessRuleDefinitionId);
                    if (action == null)
                        throw new VRBusinessException("BPBusinessRuleAction");
                    var effectiveAction = new BPBusinessRuleEffectiveAction()
                    {
                        RuleDefinitionId = definition.BPBusinessRuleDefinitionId,
                        //check action
                        Disabled = action.Details.Settings.Disabled,
                        Action = action.Details.Settings.Action,
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
            protected override bool UseCentralizedCacheRefresher { get { return true; } }

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return base.ShouldSetCacheExpired(parameter);
            }
        }
        #endregion
    }
}