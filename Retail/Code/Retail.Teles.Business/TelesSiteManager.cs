using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Business.AccountBEActionTypes;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Retail.Teles.Business
{
    public class TelesSiteManager
    { 
        
        #region Public Methods
        AccountBEManager _accountBEManager = new AccountBEManager();
        public IEnumerable<TelesEnterpriseSiteInfo> GetEnterpriseSitesInfo(Guid vrConnectionId, long enterpriseId, TelesEnterpriseSiteFilter filter)
        {
            var cachedSites = GetEnterpriseSitesInfoByEnterpiseId(vrConnectionId, enterpriseId);

            Func<TelesEnterpriseSiteInfo, bool> filterFunc = null;
            if (filter != null)
            {
                filterFunc = (telesEnterpriseInfo) =>
                {
                    return true;
                };
                return cachedSites.FindAllRecords(filterFunc).OrderBy(x => x.Name);
            }
            return cachedSites.Values.OrderBy(x => x.Name);
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
        public IEnumerable<dynamic> GetSites(Guid vrConnectionId, dynamic telesEnterpriseId)
        {
            var actionPath = string.Format("/domain/{0}/sub", telesEnterpriseId);
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);

            List<dynamic> sites = telesRestConnection.Get<List<dynamic>>(actionPath);
            return sites;
        }
        public dynamic GetSite(Guid vrConnectionId, dynamic telesSiteId)
        {
            var actionPath = string.Format("/domain/{0}", telesSiteId);
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);
            return telesRestConnection.Get<dynamic>(actionPath);
        }
        public Dictionary<dynamic, dynamic> GetSiteRoutingGroups(Guid vrConnectionId, dynamic siteId)
        {
            var actionPath = string.Format("/domain/{0}/routGroup", siteId);
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);

            List<dynamic> routingGroups = telesRestConnection.Get<List<dynamic>>(actionPath);
            return routingGroups.ToDictionary(x => (dynamic)x.id, x => x);
        }
        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> MapSiteToAccount(MapSiteToAccountInput input)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool result = TryMapSiteToAccount(input.AccountBEDefinitionId, input.AccountId, input.TelesSiteId);
            if (result)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                _accountBEManager.TrackAndLogObjectCustomAction(input.AccountBEDefinitionId, input.AccountId, "Map To Teles Site", null, null);
                updateOperationOutput.UpdatedObject = _accountBEManager.GetAccountDetail(input.AccountBEDefinitionId, input.AccountId);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;

        }
        public bool TryMapSiteToAccount(Guid accountBEDefinitionId, long accountId, dynamic telesSiteId, ProvisionStatus? status = null)
        {

            SiteAccountMappingInfo siteAccountMappingInfo = new SiteAccountMappingInfo { TelesSiteId = telesSiteId, Status = status };

            return _accountBEManager.UpdateAccountExtendedSetting<SiteAccountMappingInfo>(accountBEDefinitionId, accountId,
                siteAccountMappingInfo);
        }
        public bool DoesUserHaveExecutePermission(Guid accountBEDefinitionId)
        {
            var accountDefinitionActions = new AccountBEDefinitionManager().GetAccountActionDefinitions(accountBEDefinitionId);
            foreach (var a in accountDefinitionActions)
            {
                var settings = a.ActionDefinitionSettings as MappingTelesSiteActionSettings;
                if (settings != null)
                    return settings.DoesUserHaveExecutePermission();
            }
            return false;
        }
        public string GetSiteName(Guid vrConnectionId, dynamic telesSiteId)
        {
            var site = GetSite(vrConnectionId, telesSiteId);
            if (site == null)
                return null;
            return site.name;
        }
        public Dictionary<dynamic, long> GetCachedAccountsBySites(Guid accountBEDefinitionId, int enterpriseId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<AccountBEManager.CacheManager>().GetOrCreateObject(string.Format("GetCachedAccountsBySites_{0}", accountBEDefinitionId), accountBEDefinitionId, () =>
            {
                var accountBEManager = new AccountBEManager();
                TelesEnterpriseManager telesEnterpriseManager = new Business.TelesEnterpriseManager();
                Dictionary<dynamic, long> accountsBySite = null;
                var accountEnterprises = telesEnterpriseManager.GetCachedAccountsByEnterprises(accountBEDefinitionId);
                if(accountEnterprises != null)
                {
                   long accountId = -1;
                   if(accountEnterprises.TryGetValue(enterpriseId, out accountId))
                   {
                       var cashedAccounts = accountBEManager.GetChildAccounts(accountBEDefinitionId, accountId,false);
                       foreach (var item in cashedAccounts)
                       {
                           var siteAccountMappingInfo = accountBEManager.GetExtendedSettings<SiteAccountMappingInfo>(item);
                           if (siteAccountMappingInfo != null)
                           {
                               if (accountsBySite == null)
                                   accountsBySite = new Dictionary<dynamic, long>();
                               accountsBySite.Add(siteAccountMappingInfo.TelesSiteId, item.AccountId);
                           }
                       }
                   }
                }
                return accountsBySite;
            });
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

        #endregion

        #region Private Methods
        private Dictionary<dynamic, TelesEnterpriseSiteInfo> GetEnterpriseSitesInfoByEnterpiseId(Guid vrConnectionId, long enterpriseId)
        {
            var sites = GetSites(vrConnectionId, enterpriseId);
            List<TelesEnterpriseSiteInfo> telesEnterpriseSitesInfo = new List<TelesEnterpriseSiteInfo>();
            if (sites != null)
            {
                foreach (var site in sites)
                {
                    telesEnterpriseSitesInfo.Add(new TelesEnterpriseSiteInfo
                    {
                        Name = site.name,
                        TelesSiteId = site.id.Value
                    });
                }
            }
            return telesEnterpriseSitesInfo.ToDictionary(x => x.TelesSiteId, x => x);
        }

        private TelesRestConnection GetTelesRestConnection(Guid vrConnectionId)
        {
            VRConnectionManager vrConnectionManager = new VRConnectionManager();
            VRConnection vrConnection = vrConnectionManager.GetVRConnection<TelesRestConnection>(vrConnectionId);
            return vrConnection.Settings.CastWithValidate<TelesRestConnection>("telesRestConnection", vrConnectionId);
        }
        #endregion
    }
}
