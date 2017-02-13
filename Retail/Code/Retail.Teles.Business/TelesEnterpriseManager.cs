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
            if (cachedEnterprises == null)
                return null;
            return cachedEnterprises.Values;
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
                               TelesEnterpriseId = enterprise.id
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
