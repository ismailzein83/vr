using Retail.BusinessEntity.Business;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Entities;
using Retail.Teles.Business.AccountBEActionTypes;
using System.Security.Policy;

namespace Retail.Teles.Business
{
    public class TelesEnterpriseManager : IBusinessEntityManager
    {
        #region Public Methods
        AccountBEManager _accountBEManager = new AccountBEManager();

        public IEnumerable<TelesEnterpriseInfo> GetEnterprisesInfo(Guid vrConnectionId, TelesEnterpriseFilter filter)
        {
            var cachedEnterprises = GetCachedEnterprises(vrConnectionId, false);

            Func<TelesEnterpriseInfo, bool> filterFunc = null;
            if (filter != null)
            {
                filterFunc = (telesEnterpriseInfo) =>
                {

                    if (filter.Filters != null)
                    {
                        var context = new TelesEnterpriseFilterContext() { EnterpriseId = telesEnterpriseInfo.TelesEnterpriseId, AccountBEDefinitionId = filter.AccountBEDefinitionId };
                        if (filter.Filters.Any(x => x.IsExcluded(context)))
                            return false;
                    }
                    return true;
                };
                return cachedEnterprises.FindAllRecords(filterFunc).OrderBy(x => x.Name);

            }

            return cachedEnterprises.Values.OrderBy(x => x.Name);
        }
        public TelesEnterpriseInfo GetEnterprise(Guid vrConnectionId, dynamic enterpriseId)
        {
            var cachedEnterprises = GetCachedEnterprises(vrConnectionId, false);
            TelesEnterpriseInfo enterpriseInfo;
            cachedEnterprises.TryGetValue(enterpriseId, out enterpriseInfo);
            return enterpriseInfo;
        }

        public long CreateEnterprise(Guid vrConnectionId, string centrexFeatSet, Retail.Teles.Business.Provisioning.ProvisionAccountRuntimeSettings.Enterprise request)
        {
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);
            var actionPath = string.Format("/domain/{0}?centrexFeatSet={1}", telesRestConnection.DefaultDomainId, centrexFeatSet);
            VRWebAPIResponse<string> response = telesRestConnection.Post<Retail.Teles.Business.Provisioning.ProvisionAccountRuntimeSettings.Enterprise, string>(actionPath, request, true);
            response.Headers.Location.ThrowIfNull("response.Headers", response.Headers);
            var enterpriseId = response.Headers.Location.Segments.Last();
            enterpriseId.ThrowIfNull("enterpriseId", enterpriseId);
            return Convert.ToInt64(enterpriseId);
        }
        public long CreateSite(Guid vrConnectionId, dynamic enterpriseId, string centrexFeatSet, Retail.Teles.Business.Provisioning.ProvisionAccountRuntimeSettings.Site request)
        {
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);
            var actionPath = string.Format("/domain/{0}?centrexFeatSet={1}", enterpriseId, centrexFeatSet);
            VRWebAPIResponse<string> response = telesRestConnection.Post<Retail.Teles.Business.Provisioning.ProvisionAccountRuntimeSettings.Site, string>(actionPath, request, true);
            response.Headers.Location.ThrowIfNull("response.Headers", response.Headers);
            var siteId = response.Headers.Location.Segments.Last();
            siteId.ThrowIfNull("siteId", siteId);
            return Convert.ToInt64(siteId);
        }
        public long CreateScreenedNumber(Guid vrConnectionId, dynamic siteId, Retail.Teles.Business.Provisioning.ProvisionAccountRuntimeSettings.ScreenedNumber request)
        {
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);
            var actionPath = string.Format("/domain/{0}/screenNum", siteId);
            VRWebAPIResponse<string> response = telesRestConnection.Post<Retail.Teles.Business.Provisioning.ProvisionAccountRuntimeSettings.ScreenedNumber, string>(actionPath, request, true);
            var screenedNumberId = response.Headers.Location.Segments.Last();
            screenedNumberId.ThrowIfNull("screenedNumberId", screenedNumberId);
            return Convert.ToInt64(screenedNumberId);
        }
        public string GetEnterpriseName(Guid vrConnectionId, dynamic enterpriseId)
        {
            var cachedEnterprises = GetCachedEnterprises(vrConnectionId, true);
            if (cachedEnterprises != null)
            {
                TelesEnterpriseInfo enterpriseInfo;
                if (cachedEnterprises.TryGetValue(enterpriseId, out enterpriseInfo))
                    return enterpriseInfo.Name;
                else
                    return null;
            }
            else
            {
                return string.Format("{0} (Name unavailable)", enterpriseId);
            }
        }
        public IEnumerable<dynamic> GetSites(Guid vrConnectionId, dynamic telesEnterpriseId)
        {
            var actionPath = string.Format("/domain/{0}/sub", telesEnterpriseId);
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);

            List<dynamic> sites = telesRestConnection.Get<List<dynamic>>(actionPath);
            return sites;
        }
        public IEnumerable<dynamic> GetUsers(Guid vrConnectionId, dynamic siteId)
        {
            var actionPath = string.Format("/domain/{0}/user", siteId);
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);
            List<dynamic> sites = telesRestConnection.Get<List<dynamic>>(actionPath);
            return sites;
        }
        public Dictionary<dynamic, dynamic> GetSiteRoutingGroups(Guid vrConnectionId, dynamic siteId)
        {
            var actionPath = string.Format("/domain/{0}/routGroup", siteId);
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);

            List<dynamic> routingGroups = telesRestConnection.Get<List<dynamic>>(actionPath);
            return routingGroups.ToDictionary(x => (dynamic)x.id, x => x);
        }
        public dynamic UpdateUser(Guid vrConnectionId, dynamic user)
        {
            var actionPath = string.Format("/user/{0}", user.id);
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);

            return telesRestConnection.Put<dynamic, dynamic>(actionPath, user);
        }
        public dynamic GetUser(Guid vrConnectionId, dynamic userId)
        {
            var actionPath = string.Format("/user/{0}", userId);
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);

            return telesRestConnection.Get<dynamic>(actionPath);
        }
        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> MapEnterpriseToAccount(MapEnterpriseToAccountInput input)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool result = TryMapEnterpriseToAccount(input.AccountBEDefinitionId, input.AccountId, input.TelesEnterpriseId);
            if (result)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                _accountBEManager.TrackAndLogObjectCustomAction(input.AccountBEDefinitionId, input.AccountId, "Map To Teles Enterprise", null,null);
                updateOperationOutput.UpdatedObject = _accountBEManager.GetAccountDetail(input.AccountBEDefinitionId, input.AccountId);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;

        }
        public bool TryMapEnterpriseToAccount(Guid accountBEDefinitionId, long accountId, dynamic telesEnterpriseId, ProvisionStatus? status = null)
        {

            EnterpriseAccountMappingInfo enterpriseAccountMappingInfo = new EnterpriseAccountMappingInfo { TelesEnterpriseId = telesEnterpriseId, Status = status };

            var result = _accountBEManager.UpdateAccountExtendedSetting<EnterpriseAccountMappingInfo>(accountBEDefinitionId, accountId,
                enterpriseAccountMappingInfo);
            if (result)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            return result;

        }
        public bool TryMapSiteToAccount(Guid accountBEDefinitionId, long accountId, dynamic telesSiteId, ProvisionStatus? status = null)
        {

            SiteAccountMappingInfo siteAccountMappingInfo = new SiteAccountMappingInfo { TelesSiteId = telesSiteId, Status = status };

            return _accountBEManager.UpdateAccountExtendedSetting<SiteAccountMappingInfo>(accountBEDefinitionId, accountId,
                siteAccountMappingInfo);
        }
        public Dictionary<dynamic, long> GetCachedAccountsByEnterprises(Guid accountBEDefinitionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<AccountBEManager.CacheManager>().GetOrCreateObject(string.Format("GetCachedAccountsByEnterprises_{0}", accountBEDefinitionId), accountBEDefinitionId, () =>
            {
                var accountBEManager = new AccountBEManager();
                var cashedAccounts = accountBEManager.GetAccounts(accountBEDefinitionId);
                Dictionary<dynamic, long> accountsByEnterprises = null;
                foreach (var item in cashedAccounts)
                {
                    var enterpriseAccountMappingInfo = accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(item.Value);
                    if (enterpriseAccountMappingInfo != null)
                    {
                        if (accountsByEnterprises == null)
                            accountsByEnterprises = new Dictionary<dynamic, long>();
                        accountsByEnterprises.Add(enterpriseAccountMappingInfo.TelesEnterpriseId, item.Key);
                    }
                }
                return accountsByEnterprises;
            });
        }
        public Dictionary<long, dynamic> GetCachedEnterprisesByAccounts(Guid accountBEDefinitionId)
        {

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<AccountBEManager.CacheManager>().GetOrCreateObject(string.Format("GetCachedEnterprisesByAccounts_{0}", accountBEDefinitionId), accountBEDefinitionId, () =>
            {
                var accountBEManager = new AccountBEManager();
                var cashedAccounts = accountBEManager.GetAccounts(accountBEDefinitionId);
                Dictionary<long, dynamic> enterprisesByAccounts = null;
                foreach (var item in cashedAccounts)
                {
                    var enterpriseAccountMappingInfo = accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(item.Value);
                    if (enterpriseAccountMappingInfo != null)
                    {
                        if (enterprisesByAccounts == null)
                            enterprisesByAccounts = new Dictionary<long, dynamic>();
                        enterprisesByAccounts.Add(item.Key, enterpriseAccountMappingInfo.TelesEnterpriseId);
                    }
                }
                return enterprisesByAccounts;
            });
        }
     
        public bool DoesUserHaveExecutePermission(Guid accountBEDefinitionId)
        {
            var accountDefinitionActions = new AccountBEDefinitionManager().GetAccountActionDefinitions(accountBEDefinitionId);
            foreach (var a in accountDefinitionActions)
            {
                var settings = a.ActionDefinitionSettings as MappingTelesAccountActionSettings;
                if (settings != null)
                    return settings.DoesUserHaveExecutePermission();
            }
            return false;
        }
        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable
            {
                get
                {
                    return true;
                }
            }
            protected override bool UseCentralizedCacheRefresher
            {
                get
                {
                    return true;
                }
            }
            
        }

        private class CachedEnterprisesInfo
        {
            public bool IsValid { get; set; }

            public Dictionary<dynamic, TelesEnterpriseInfo> EnterpriseInfos { get; set; }
        }

        #endregion

        #region Private Methods
        private Dictionary<dynamic, TelesEnterpriseInfo> GetCachedEnterprises(Guid vrConnectionId, bool handleTelesNotAvailable)
        {
            CachedEnterprisesInfo enterpriseInfos = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("GetCachedEnterprisesInfo_{0}", vrConnectionId),
               () =>
               {
                   try
                   {
                       TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);

                       var actionPath = string.Format("/domain/{0}/sub", telesRestConnection.DefaultDomainId);
                       List<dynamic> enterprises = telesRestConnection.Get<List<dynamic>>(actionPath);
                       List<TelesEnterpriseInfo> telesEnterpriseInfo = new List<TelesEnterpriseInfo>();
                       if (enterprises != null)
                       {
                           foreach (var enterprise in enterprises)
                           {
                               telesEnterpriseInfo.Add(new TelesEnterpriseInfo
                               {
                                   Name = enterprise.name,
                                   TelesEnterpriseId = enterprise.id.Value
                               });
                           }
                       }
                       return new CachedEnterprisesInfo
                       {
                           EnterpriseInfos = telesEnterpriseInfo.ToDictionary(x => x.TelesEnterpriseId, x => x),
                           IsValid = true
                       };
                   }
                   catch (Exception ex)//handle the case where Teles API is not available
                   {
                       LoggerFactory.GetExceptionLogger().WriteException(ex);
                       return new CachedEnterprisesInfo
                       {
                           IsValid = false
                       };
                   }
               });
            if (enterpriseInfos.IsValid)
            {
                return enterpriseInfos.EnterpriseInfos;
            }
            else
            {
                if (handleTelesNotAvailable)
                    return null;
                else
                    throw new VRBusinessException("Cannot connect to Teles API");
            }
        }
      
        private TelesRestConnection GetTelesRestConnection(Guid vrConnectionId)
        {
            VRConnectionManager vrConnectionManager = new VRConnectionManager();
            VRConnection vrConnection = vrConnectionManager.GetVRConnection<TelesRestConnection>(vrConnectionId);
            return vrConnection.Settings.CastWithValidate<TelesRestConnection>("telesRestConnection", vrConnectionId);
        }  
        #endregion

        #region IBusinessEntityManager

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var telesBEDefinitionSettings = context.EntityDefinition.Settings.CastWithValidate<TelesEnterpriseBEDefinitionSettings>("context.EntityDefinition.Settings");

            var cachedEnterprises = GetCachedEnterprises(telesBEDefinitionSettings.VRConnectionId, false);

            return cachedEnterprises.Values.Select(itm => itm as dynamic).ToList();
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            var telesBEDefinitionSettings = context.EntityDefinition.Settings as TelesEnterpriseBEDefinitionSettings;
            return GetEnterprise(telesBEDefinitionSettings.VRConnectionId, context.EntityId);
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            var telesBEDefinitionSettings = context.EntityDefinition.Settings as TelesEnterpriseBEDefinitionSettings;
            return GetEnterpriseName(telesBEDefinitionSettings.VRConnectionId, context.EntityId);
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var telesEnterpriseInfo = context.Entity as TelesEnterpriseInfo;
            return telesEnterpriseInfo.TelesEnterpriseId;
        }

        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion  
    }
}
