using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public abstract class VRRuleManager<T, Q, S>
        where T : VRRule<S>
        where S : VRRuleSettings
        where Q : class
    {
        #region Ctor/Variables

        static CacheManager s_cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>();

        Guid _vrRuleDefinitionId;

        public VRRuleManager()
        {
            _vrRuleDefinitionId = GetVRRuleDefinitionId();
        }

        #endregion

        #region Public Methods

        public Vanrise.Entities.InsertOperationOutput<Q> AddVRRule(T vrRule)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<Q>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long vrRuleId;

            IVRRuleDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRRuleDataManager>();

            if (_dataManager.Insert(vrRule as VRRule, out vrRuleId))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                vrRule.VRRuleId = vrRuleId;
                insertOperationOutput.InsertedObject = VRRuleDetailMapper(vrRule as T);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<Q> UpdateVRRule(T vrRule)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<Q>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRRuleDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRRuleDataManager>();

            if (_dataManager.Update(vrRule as VRRule))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(_vrRuleDefinitionId);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRRuleDetailMapper(vrRule as T);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public T GetVRRule(long vrRuleId)
        {
            Dictionary<long, T> cachedVRRules = GetCachedVRRules();
            return cachedVRRules.GetRecord(vrRuleId);
        }

        private struct GetCachedVRRulesCacheName
        {
            public string VRRuleFQTN { get; set; }
            public string VRRuleSettingsFQTN { get; set; }
            public Guid VRRuleDefinitionId { get; set; }
        }
        public Dictionary<long, T> GetCachedVRRules()
        {
            var cacheName = new GetCachedVRRulesCacheName
            {
                VRRuleFQTN = typeof(T).AssemblyQualifiedName,
                VRRuleSettingsFQTN = typeof(S).AssemblyQualifiedName,
                VRRuleDefinitionId = _vrRuleDefinitionId
            };

            return s_cacheManager.GetOrCreateObject(cacheName, _vrRuleDefinitionId,
                () =>
                {
                    return GetCachedVRRules(_vrRuleDefinitionId).Values.Select(itm =>
                    {
                        var vrRuleAsR = Activator.CreateInstance<T>();
                        vrRuleAsR.VRRuleId = itm.VRRuleId;
                        vrRuleAsR.VRRuleDefinitionId = itm.VRRuleDefinitionId;
                        vrRuleAsR.Settings = itm.Settings as S;
                        return vrRuleAsR;
                    }).ToDictionary(itm => itm.VRRuleId, itm => itm);
                });
        }

        #endregion

        #region Private Methods

        private Dictionary<long, VRRule> GetCachedVRRules(Guid vrRuleDefinitionId)
        {
            return s_cacheManager.GetOrCreateObject("GetCachedVRRules", vrRuleDefinitionId,
                () =>
                {
                    IVRRuleDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRRuleDataManager>();
                    IEnumerable<VRRule> vrRules = _dataManager.GetVRRules(vrRuleDefinitionId);
                    return vrRules.ToDictionary(kvp => kvp.VRRuleId, kvp => kvp);
                });
        }

        #endregion

        #region Abstract Methods

        protected abstract Guid GetVRRuleDefinitionId();

        protected abstract Q VRRuleDetailMapper(T vrRule);

        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager<Guid>
        {
            IVRRuleDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRRuleDataManager>();
            ConcurrentDictionary<Guid, Object> _updateHandlesByBEDefinitionId = new ConcurrentDictionary<Guid, Object>();
            protected override bool ShouldSetCacheExpired(Guid vrRuleDefinitionId)
            {
                object _updateHandle;

                _updateHandlesByBEDefinitionId.TryGetValue(vrRuleDefinitionId, out _updateHandle);
                bool isCacheExpired = _dataManager.AreRulesUpdated(vrRuleDefinitionId, ref _updateHandle);
                _updateHandlesByBEDefinitionId.AddOrUpdate(vrRuleDefinitionId, _updateHandle, (key, existingHandle) => _updateHandle);

                return isCacheExpired;
            }
        }

        #endregion
    }
}