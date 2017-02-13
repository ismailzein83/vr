using Retail.BusinessEntity.Business;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
namespace Retail.Teles.Business
{
    public class TelesEnterpriseManager : IBusinessEntityManager
    {
        public IEnumerable<TelesEnterpriseInfo> GetEnterprisesInfo(int switchId, int domainId, TelesEnterpriseFilter filter)
        {
            var cachedEnterprises = GetCachedEnterprises(switchId, domainId);

            Func<TelesEnterpriseInfo, bool> filterFunc = null;
            if(filter != null)
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
            }

            if (cachedEnterprises == null)
                return null;
            return cachedEnterprises.FindAllRecords(filterFunc).OrderBy(x => x.Name);
        }
        public TelesEnterpriseInfo GetEnterprise(int switchId, int domainId,dynamic enterpriseId)
        {
            var cachedEnterprises = GetCachedEnterprises(switchId, domainId);
            var enterprise = cachedEnterprises.FindRecord(x => x.Key == enterpriseId);
            return enterprise.Value;
        }
        public string GetEnterpriseName(int switchId, int domainId, dynamic enterpriseId)
        {
            TelesEnterpriseInfo telesEnterpriseInfo = this.GetEnterprise(switchId,domainId, enterpriseId);
            return (telesEnterpriseInfo != null) ? telesEnterpriseInfo.Name : null;
        }
        public IEnumerable<dynamic> GetSites(int switchId, dynamic telesEnterpriseId)
        {
            var actionPath = string.Format("/domain/{0}/sub", telesEnterpriseId);
            List<dynamic> sites = TelesWebAPIClient.Get<List<dynamic>>(switchId, actionPath);
            return sites;
        }
        public IEnumerable<dynamic> GetUsers(int switchId, dynamic siteId)
        {
            var actionPath = string.Format("/domain/{0}/user", siteId);
            List<dynamic> sites = TelesWebAPIClient.Get<List<dynamic>>(switchId, actionPath);
            return sites;
        }
        public Dictionary<dynamic, dynamic> GetSiteRoutingGroups(int switchId, dynamic siteId)
        {
            var actionPath = string.Format("/domain/{0}/routGroup", siteId);
            List<dynamic> routingGroups = TelesWebAPIClient.Get<List<dynamic>>(switchId, actionPath);
            return routingGroups.ToDictionary(x => (dynamic)x.id, x => x);
        }
        public dynamic UpdateUser(int switchId, dynamic user)
        {
            var actionPath = string.Format("/user/{0}", user.id);
            return TelesWebAPIClient.Put<dynamic, dynamic>(switchId, actionPath, user);
        }
        public dynamic GetUser(int switchId, dynamic userId)
        {
            var actionPath = string.Format("/user/{0}", userId);
            return TelesWebAPIClient.Get<dynamic>(switchId, actionPath);
        }
        public bool MapEnterpriseToAccount(MapEnterpriseToAccountInput input)
        {
            AccountBEManager accountBEManager = new AccountBEManager();

            return accountBEManager.UpdateAccountExtendedSetting<EnterpriseAccountMappingInfo>(input.AccountBEDefinitionId, input.AccountId,
                new EnterpriseAccountMappingInfo { TelesEnterpriseId = input.TelesEnterpriseId });
        }


        public Dictionary<dynamic,long> GetCachedAccountsByEnterprises(Guid accountBEDefinitionId)
        {

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("GetCachedAccountsByEnterprises_{0}",accountBEDefinitionId), () =>
               {
                   var accountBEManager = new AccountBEManager();
                   var cashedAccounts = accountBEManager.GetAccounts(accountBEDefinitionId);
                   Dictionary<dynamic, long> accountsByEnterprises = null;
                   foreach(var item in cashedAccounts)
                   {
                       var enterpriseAccountMappingInfo = accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(item.Value);
                       if (enterpriseAccountMappingInfo !=  null)
                       {
                           if (accountsByEnterprises == null)
                               accountsByEnterprises = new Dictionary<dynamic, long>();
                           accountsByEnterprises.Add(enterpriseAccountMappingInfo.TelesEnterpriseId,item.Key);
                       }
                   }
                   return accountsByEnterprises;
               });
        }
        public Dictionary<long, dynamic> GetCachedEnterprisesByAccounts(Guid accountBEDefinitionId)
        {

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("GetCachedEnterprisesByAccounts_{0}", accountBEDefinitionId), () =>
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
                        enterprisesByAccounts.Add( item.Key , enterpriseAccountMappingInfo.TelesEnterpriseId);
                    }
                }
                return enterprisesByAccounts;
            });
        }

        #region Private Classes
        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            object _updateHandle;

            public override bool IsCacheExpired(object parameter, ref DateTime? lastCheckTime)
            {
                if (lastCheckTime.HasValue)
                {
                    if (lastCheckTime.Value < DateTime.Now.AddMinutes(-5))
                        return true;
                }
                return false;
            }
        }
        #endregion

        #region Private Methods
        private Dictionary<dynamic, TelesEnterpriseInfo> GetCachedEnterprises(int switchId, int domainId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("GetCachedEnterprisesInfo_{0}_{1}", switchId, domainId),
               () =>
               {
                   var actionPath = string.Format("/domain/{0}/sub", domainId);
                   List<dynamic> enterprises = TelesWebAPIClient.Get<List<dynamic>>(switchId, actionPath);
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
                   return telesEnterpriseInfo.ToDictionary(x=>x.TelesEnterpriseId,x=>x);
               });
        }

        #endregion

        #region IBusinessEntityManager
        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var telesBEDefinitionSettings = context.EntityDefinition.Settings as TelesEnterpriseBEDefinitionSettings;

            var cachedEnterprises = GetCachedEnterprises(telesBEDefinitionSettings.SwitchId, telesBEDefinitionSettings.DomainId);
            if (cachedEnterprises != null)
                return cachedEnterprises.Select(itm => itm as dynamic).ToList();
            else
                return null;
        }
        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            var telesBEDefinitionSettings = context.EntityDefinition.Settings as TelesEnterpriseBEDefinitionSettings;
            return GetEnterprise(telesBEDefinitionSettings.SwitchId, telesBEDefinitionSettings.DomainId, context.EntityId);
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            var telesBEDefinitionSettings = context.EntityDefinition.Settings as TelesEnterpriseBEDefinitionSettings;
            return GetEnterpriseName(telesBEDefinitionSettings.SwitchId, telesBEDefinitionSettings.DomainId, context.EntityId);
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
