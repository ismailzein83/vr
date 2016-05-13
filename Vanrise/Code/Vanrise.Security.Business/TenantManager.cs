using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class TenantManager
    {
        public Vanrise.Entities.IDataRetrievalResult<TenantDetail> GetFilteredTenants(Vanrise.Entities.DataRetrievalInput<TenantQuery> input)
        {
            UserManager userManager = new UserManager();
            var user = userManager.GetUserbyId(SecurityContext.Current.GetLoggedInUserId());
            var allItems = GetTenantsByTenantId(user.TenantId, true);

            Func<Tenant, bool> filterExpression = (itemObject) =>
                 (input.Query == null || input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 && (allItems != null && allItems.FirstOrDefault(itm => itm.TenantId == itemObject.TenantId) != null);

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, TenantDetailMapper));
        }
        /// <summary>
        /// If tenantId null, it returns all tenants. Otherwise it returns tenant related to the tenantId and all children under this tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public List<Tenant> GetTenantsByTenantId(int? tenantId, bool withRelatedTenants)
        {
            var tenants = GetCachedTenantList();

            if (!tenantId.HasValue)
                return tenants != null ? tenants.ToList() : null;

            List<Tenant> result = new List<Tenant>();
            if (withRelatedTenants)
            {
                foreach (Tenant tenant in tenants)
                {
                    if (IsMatching(tenant, tenants, tenantId.Value, true))
                    {
                        result.Add(tenant);
                    }
                }
            }
            else
            {
                result.Add(GetTenantbyId(tenantId.Value));
            }
            return result;
        }

        public IEnumerable<TenantInfo> GetTenantsInfo(TenantFilter filter)
        {
            UserManager userManager = new UserManager();
            var user = userManager.GetUserbyId(SecurityContext.Current.GetLoggedInUserId());
            var allItems = GetTenantsByTenantId(user.TenantId, true);

            if (filter == null )
                return allItems.MapRecords(TenantInfoMapper);


            List<TenantInfo> result = new List<TenantInfo>();
            foreach (Tenant item in allItems)
            {
                if (filter.Filters != null && filter.Filters.Count > 0 && filter.Filters.Where(itm => itm.IsExcluded(item)).Count() > 0)
                {
                    continue;
                }
                if (!filter.CanBeParentOfTenantId.HasValue || IsMatching(item, allItems, filter.CanBeParentOfTenantId.Value, false))
                {
                    result.Add(TenantInfoMapper(item));
                }

            }
            return result;
        }

        private bool IsMatching(Tenant tenant, IEnumerable<Tenant> tenants, int tenantId, bool couldBeExact)
        {
            if (tenant.TenantId == tenantId)
                return couldBeExact;

            if (!tenant.ParentTenantId.HasValue)
                return !couldBeExact;

            if (tenant.ParentTenantId.Value == tenantId)
                return couldBeExact;

            Tenant item = tenants.First(itm => itm.TenantId == tenant.ParentTenantId.Value);
            return IsMatching(item, tenants, tenantId, couldBeExact);
        }

        public Tenant GetTenantbyName(string name)
        {
            var tenants = GetCachedTenants();
            return tenants.FindRecord(itm => string.Compare(itm.Name, name, true) == 0);
        }

        public Tenant GetTenantbyId(int tenantId)
        {
            var tenants = GetCachedTenants();
            return tenants.GetRecord(tenantId);
        }

        public Vanrise.Entities.UpdateOperationOutput<TenantDetail> UpdateTenant(Tenant tenantObject)
        {
            UpdateOperationOutput<TenantDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<TenantDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            bool updateActionSucc;
            var cloudServiceProxy = GetCloudServiceProxy();
            if (cloudServiceProxy != null)
            {
                var output = cloudServiceProxy.UpdateTenantToApplication(new UpdateTenantToApplicationInput
                {
                    TenantId = tenantObject.TenantId
                });
                if (output.OperationOutput != null && output.OperationOutput.Result == UpdateOperationResult.Succeeded)
                {
                    updateActionSucc = true;
                }
                else
                {
                    updateActionSucc = false;
                }
            }
            else
            {
                ITenantDataManager dataManager = SecurityDataManagerFactory.GetDataManager<ITenantDataManager>();
                updateActionSucc = dataManager.UpdateTenant(tenantObject);
            }

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = TenantDetailMapper(tenantObject);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.InsertOperationOutput<TenantDetail> AddTenant(Tenant tenantObject)
        {
            InsertOperationOutput<TenantDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<TenantDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int tenantId = -1;
            bool insertActionSucc;
            var cloudServiceProxy = GetCloudServiceProxy();
            if (cloudServiceProxy != null)
            {
                var output = cloudServiceProxy.AddTenantToApplication(new AddTenantToApplicationInput
                {
                    Name = tenantObject.Name,
                    Settings = tenantObject.Settings,
                    ParentTenantId = tenantObject.ParentTenantId
                });
                if (output.OperationOutput != null && output.OperationOutput.Result == InsertOperationResult.Succeeded)
                {
                    insertActionSucc = true;
                    tenantObject = MapCloudTenantToTenant(output.OperationOutput.InsertedObject);
                }
                else
                {
                    insertActionSucc = false;
                }
            }
            else
            {
                ITenantDataManager dataManager = SecurityDataManagerFactory.GetDataManager<ITenantDataManager>();
                insertActionSucc = dataManager.AddTenant(tenantObject, out tenantId);
                tenantObject.TenantId = tenantId;

            }

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = TenantDetailMapper(tenantObject);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public string GetConnectionString(int tenantId, string connStringKey)
        {
            string cacheName = String.Format("TenantManager_GetConnectionString_{0}_{1}", tenantId, connStringKey);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    Tenant tenant = GetTenantbyId(tenantId);
                    if (tenant == null)
                        throw new NullReferenceException(String.Format("tenant '{0}'", tenantId));
                    return GetConnectionString(tenant, connStringKey);
                });
        }

        public CloudTenantOutput GetCloudTenantOutput(int tenantId, int applicationId)
        {
            var items = GetCachedCloudTenantOutput(applicationId);
            if (items != null)
            {
                return items.GetRecord(tenantId);
            }
            return null;
        }
        #region private methods
        private Dictionary<int, CloudTenantOutput> GetCachedCloudTenantOutput(int applicationId)
        {
            return CacheManagerFactory.GetCacheManager<CloudTenantCacheManager>().GetOrCreateObject(string.Format("GetCloudTenantsByAppId_{0}", applicationId),
               () =>
               {
                   IEnumerable<CloudTenantOutput> cloudTenants;
                   if (!TryGetCloudTenantsFromAuthServer(out cloudTenants))
                   {
                       return null;
                   }
                   return cloudTenants.ToDictionary(kvp => kvp.TenantId, kvp => kvp);
               });
        }

        private Dictionary<int, Tenant> GetCachedTenants()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetTenants",
               () =>
               {
                   IEnumerable<Tenant> tenants;
                   if (!TryGetTenantsFromAuthServer(out tenants))
                   {
                       ITenantDataManager dataManager = SecurityDataManagerFactory.GetDataManager<ITenantDataManager>();
                       tenants = dataManager.GetTenants();
                   }
                   return tenants != null ? tenants.ToDictionary(kvp => kvp.TenantId, kvp => kvp) : null;
               });
        }

        private IEnumerable<Tenant> GetCachedTenantList()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetTenantList",
               () =>
               {
                   IEnumerable<Tenant> tenants;
                   if (!TryGetTenantsFromAuthServer(out tenants))
                   {
                       ITenantDataManager dataManager = SecurityDataManagerFactory.GetDataManager<ITenantDataManager>();
                       tenants = dataManager.GetTenants();
                   }
                   return tenants;
               });
        }

        private string GetConnectionString(Tenant tenant, string connStringKey)
        {
            if (tenant.Settings != null && tenant.Settings.ConnectionStrings != null)
            {
                var tenantConnectionString = tenant.Settings.ConnectionStrings.FirstOrDefault(itm => itm.ConnectionStringKey == connStringKey);
                if (tenantConnectionString != null)
                    return tenantConnectionString.ConnectionString;
            }
            if (tenant.ParentTenantId.HasValue)
            {
                Tenant parentTenant = GetTenantbyId(tenant.ParentTenantId.Value);
                if (parentTenant == null)
                    throw new NullReferenceException(String.Format("tenant '{0}'", tenant.ParentTenantId.Value));
                return GetConnectionString(parentTenant, connStringKey);
            }
            return null;
        }

        private TenantDetail TenantDetailMapper(Tenant tenant)
        {
            return new TenantDetail()
            {
                Entity = tenant
            };
        }

        private TenantInfo TenantInfoMapper(Tenant tenant)
        {
            return new TenantInfo()
            {
                TenantId = tenant.TenantId,
                Name = tenant.Name
            };
        }

        private bool TryGetCloudTenantsFromAuthServer(out IEnumerable<CloudTenantOutput> tenants)
        {
            var cloudServiceProxy = GetCloudServiceProxy();
            if (cloudServiceProxy != null)
            {
                CloudTenantInput input = new CloudTenantInput();
                var output = cloudServiceProxy.GetCloudTenants(input);
                if (output != null && output.Count > 0)
                    tenants = output;
                else
                    tenants = null;
                return true;
            }
            else
            {
                tenants = null;
                return false;
            }
        }

        private bool TryGetTenantsFromAuthServer(out IEnumerable<Tenant> tenants)
        {
            var cloudServiceProxy = GetCloudServiceProxy();
            if (cloudServiceProxy != null)
            {
                GetApplicationTenantsInput input = new GetApplicationTenantsInput();
                var output = cloudServiceProxy.GetApplicationTenants(input);
                if (output != null && output.Tenants != null)
                    tenants = output.Tenants.Select(tenant => MapCloudTenantToTenant(tenant));
                else
                    tenants = null;
                return true;
            }
            else
            {
                tenants = null;
                return false;
            }
        }

        private Tenant MapCloudTenantToTenant(CloudApplicationTenant cloudApplicationTenant)
        {
            return new Tenant
            {
                TenantId = cloudApplicationTenant.Tenant.TenantId,
                Name = cloudApplicationTenant.Tenant.Name,
                ParentTenantId = cloudApplicationTenant.Tenant.ParentTenantId,
                Settings = cloudApplicationTenant.Tenant.Settings
            };
        }

        private ICloudService GetCloudServiceProxy()
        {
            var authServerManager = new CloudAuthServerManager();
            var authServer = authServerManager.GetAuthServer();
            if (authServer != null)
                return new CloudServiceProxy(authServer);
            else
                return null;
        }
        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ITenantDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<ITenantDataManager>();
            object _updateHandle;
            ICloudService _cloudServiceProxy = (new TenantManager()).GetCloudServiceProxy();

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                if (_cloudServiceProxy != null)
                {
                    var output = _cloudServiceProxy.CheckApplicationTenantsUpdated(new CheckApplicationTenantsUpdatedInput { LastReceivedUpdateInfo = _updateHandle });
                    if (output != null)
                        _updateHandle = output.LastUpdateInfo;
                    return output.Updated;
                }
                else
                    return _dataManager.AreTenantsUpdated(ref _updateHandle);
            }
        }

        private class CloudTenantCacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable
            {
                get { return true; }
            }
        }

        #endregion
    }
}