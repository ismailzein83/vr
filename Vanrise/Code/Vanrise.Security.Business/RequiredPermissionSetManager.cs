using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
using Vanrise.Common;

namespace Vanrise.Security.Business
{
    public class RequiredPermissionSetManager : IRequiredPermissionSetManager, Vanrise.Common.ISecurityRequiredPermissionSetManager
    {
        #region Public Methods
        public int GetRequiredPermissionSetId(string module, RequiredPermissionSettings requiredPermission)
        {
            string requiredPermissionString = SecurityManager.RequiredPermissionsToString(requiredPermission.Entries);
            return GetRequiredPermissionSetId(module, requiredPermissionString);
        }

        public int GetRequiredPermissionSetId(string module, string requiredPermissionString)
        {
            int id;
            ReqPermDicKey dicKey = new ReqPermDicKey
            {
                Module = module,
                RequiredPermissionString = requiredPermissionString
            };
            if (!s_requiredPermissionSetIds.TryGetValue(dicKey, out id))
            {
                lock (s_requiredPermissionSetIds)
                {
                    if (s_requiredPermissionSetIds.Count == 0)
                        LoadAllRequiredPermissionSets();
                    if (!s_requiredPermissionSetIds.TryGetValue(dicKey, out id))
                    {
                        IRequiredPermissionSetDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IRequiredPermissionSetDataManager>();
                        id = dataManager.AddIfNotExists(module, requiredPermissionString);
                        s_requiredPermissionSetIds.Add(dicKey, id);
                    }
                }
            }
            return id;
        }

        public RequiredPermissionSet GetRequiredPermissionSet(int requiredPermissionSetId)
        {
            var requiredPermissionSet = GetCachedRequiredPermissionSets().GetRecord(requiredPermissionSetId);
            if(requiredPermissionSet == null)//handle the case where the entry is available in database and the cache is older
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                requiredPermissionSet = GetCachedRequiredPermissionSets().GetRecord(requiredPermissionSetId);
            }
            return requiredPermissionSet;
        }

        public List<RequiredPermissionSet> GetModuleRequiredPermissionSets(string moduleName)
        {
            List<RequiredPermissionSet> moduleRequiredPermissionSets = GetRequiredPermissionSetsByModule().GetRecord(moduleName);
            return moduleRequiredPermissionSets != null ? moduleRequiredPermissionSets : new List<RequiredPermissionSet>();
        }

        private struct IsUserGrantedAllModulePermissionSetsCacheName
        {
            public int UserId { get; set; }

            public string ModuleName { get; set; }
        }

        public bool IsUserGrantedAllModulePermissionSets(int userId, string moduleName, out List<int> grantedPermissionSetIds)
        {
            var cacheName = new IsUserGrantedAllModulePermissionSetsCacheName
            {
                UserId = userId,
                ModuleName = moduleName
            };
            UserModulePermissionSets userModulePermissionSets = Vanrise.Caching.CacheManagerFactory.GetCacheManager<WithDependenciesCacheManager>().GetOrCreateObject(
                cacheName,
                () =>
                {
                    List<RequiredPermissionSet> moduleRequiredPermissionSets = GetModuleRequiredPermissionSets(moduleName);
                    SecurityManager securityManager = new SecurityManager();
                    List<int> grantedPermissionSetIds_Internal = moduleRequiredPermissionSets.Where(prm => securityManager.IsAllowed(prm.RequiredPermissionString, userId)).Select(prm => prm.RequiredPermissionSetId).ToList();
                    return new UserModulePermissionSets
                    {
                        IsGrantedAllPermissionSets = grantedPermissionSetIds_Internal.Count == moduleRequiredPermissionSets.Count,
                        GrantedPermissionSetIds = grantedPermissionSetIds_Internal
                    };
                });
            grantedPermissionSetIds = userModulePermissionSets.GrantedPermissionSetIds;
            return userModulePermissionSets.IsGrantedAllPermissionSets;
        }

        public bool IsUserGrantedAnyModulePermissionSet(int userId, string moduleName)
        {
            List<int> grantedPermissionSetIds;
            bool isUserGrantedAllModulePermissionSets = IsUserGrantedAllModulePermissionSets(userId, moduleName, out grantedPermissionSetIds);
            return isUserGrantedAllModulePermissionSets || (grantedPermissionSetIds != null && grantedPermissionSetIds.Count > 0);
        }

        public bool IsCurrentUserGrantedAllModulePermissionSets(string moduleName, out List<int> grantedPermissionSetIds)
        {
            return IsUserGrantedAllModulePermissionSets(SecurityContext.Current.GetLoggedInUserId(), moduleName, out grantedPermissionSetIds);
        }

        public bool IsCurrentUserGrantedAnyModulePermissionSet(string moduleName)
        {
            return IsUserGrantedAnyModulePermissionSet(SecurityContext.Current.GetLoggedInUserId(), moduleName);
        }
        
        #endregion

        #region Private Methods

        private Dictionary<int, RequiredPermissionSet> GetCachedRequiredPermissionSets()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedRequiredPermissionSets",
                () =>
                {
                    IRequiredPermissionSetDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IRequiredPermissionSetDataManager>();
                    return dataManager.GetAll().ToDictionary(itm => itm.RequiredPermissionSetId, itm => itm);
                });
        }

        private Dictionary<string, List<RequiredPermissionSet>> GetRequiredPermissionSetsByModule()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRequiredPermissionSetsByModule",
                () =>
                {
                    Dictionary<int, RequiredPermissionSet> allPermissionSets = GetCachedRequiredPermissionSets();
                    Dictionary<string, List<RequiredPermissionSet>> requiredPermissionSetsByModule = new Dictionary<string, List<RequiredPermissionSet>>();
                    foreach(var prm in allPermissionSets.Values)
                    {
                        requiredPermissionSetsByModule.GetOrCreateItem(prm.Module).Add(prm);
                    }
                    return requiredPermissionSetsByModule;
                });
        }

        private void LoadAllRequiredPermissionSets()
        {
            IRequiredPermissionSetDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IRequiredPermissionSetDataManager>();
            List<RequiredPermissionSet> allRequiredPermissionSets = dataManager.GetAll();
            if (allRequiredPermissionSets != null)
            {
                foreach (var requiredPermissionSet in allRequiredPermissionSets)
                {
                    ReqPermDicKey dicKey = new ReqPermDicKey
                    {
                        Module = requiredPermissionSet.Module,
                        RequiredPermissionString = requiredPermissionSet.RequiredPermissionString
                    };
                    s_requiredPermissionSetIds.Add(dicKey, requiredPermissionSet.RequiredPermissionSetId);
                }
            }
        }

        private struct ReqPermDicKey
        {
            public string Module { get; set; }

            public string RequiredPermissionString { get; set; }
        }

        static Dictionary<ReqPermDicKey, int> s_requiredPermissionSetIds = new Dictionary<ReqPermDicKey, int>();

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRequiredPermissionSetDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IRequiredPermissionSetDataManager>();
            object _updateHandle;
            
            protected override bool ShouldSetCacheExpired()
            {
               return _dataManager.AreRequiredPermissionSetsUpdated(ref _updateHandle);
            }
        }

        private class WithDependenciesCacheManager: Vanrise.Caching.BaseCacheManager
        {
            DateTime? _requiredPermissionSetCacheLastCheck;
            DateTime? _permissionCacheLastCheck;
            protected override bool ShouldSetCacheExpired()
            {
                return Vanrise.Caching.CacheManagerFactory.GetCacheManager<RequiredPermissionSetManager.CacheManager>().IsCacheExpired(ref _requiredPermissionSetCacheLastCheck)
                    |
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<PermissionManager.CacheManager>().IsCacheExpired(ref _permissionCacheLastCheck);
            }
        }

        private class UserModulePermissionSets
        {
            public bool IsGrantedAllPermissionSets { get; set; }

            public List<int> GrantedPermissionSetIds { get; set; }
        }

        #endregion
    }
}
