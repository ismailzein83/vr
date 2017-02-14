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
namespace Retail.Teles.Business
{
    public class TelesEnterpriseManager : IBusinessEntityManager
    {
        public IEnumerable<TelesEnterpriseInfo> GetEnterprisesInfo(Guid vrConnectionId, TelesEnterpriseFilter filter)
        {
            var cachedEnterprises = GetCachedEnterprises(vrConnectionId);

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
        public TelesEnterpriseInfo GetEnterprise(Guid vrConnectionId, dynamic enterpriseId)
        {
            var cachedEnterprises = GetCachedEnterprises(vrConnectionId);
            var enterprise = cachedEnterprises.FindRecord(x => x.Key == enterpriseId);
            return enterprise.Value;
        }
        public string GetEnterpriseName(Guid vrConnectionId, dynamic enterpriseId)
        {
            TelesEnterpriseInfo telesEnterpriseInfo = this.GetEnterprise(vrConnectionId, enterpriseId);
            return (telesEnterpriseInfo != null) ? telesEnterpriseInfo.Name : null;
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
            AccountBEManager accountBEManager = new AccountBEManager();
            bool result = accountBEManager.UpdateAccountExtendedSetting<EnterpriseAccountMappingInfo>(input.AccountBEDefinitionId, input.AccountId,
                new EnterpriseAccountMappingInfo { TelesEnterpriseId = input.TelesEnterpriseId });
            if (result)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = accountBEManager.GetAccountDetail(input.AccountBEDefinitionId, input.AccountId);
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            } else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
           
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
        private Dictionary<dynamic, TelesEnterpriseInfo> GetCachedEnterprises(Guid vrConnectionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("GetCachedEnterprisesInfo_{0}", vrConnectionId),
               () =>
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
                   return telesEnterpriseInfo.ToDictionary(x=>x.TelesEnterpriseId,x=>x);
               });
        }
        private TelesRestConnection GetTelesRestConnection(Guid vrConnectionId)
        {
            VRConnectionManager vrConnectionManager = new VRConnectionManager();
            VRConnection vrConnection = vrConnectionManager.GetVRConnection<TelesRestConnection>(vrConnectionId);
            var telesRestConnection = vrConnection.Settings as TelesRestConnection;
            if (telesRestConnection == null)
                throw new NullReferenceException("telesRestConnection");
            return telesRestConnection;
        }
        #endregion

        #region IBusinessEntityManager
        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var telesBEDefinitionSettings = context.EntityDefinition.Settings as TelesEnterpriseBEDefinitionSettings;

            var cachedEnterprises = GetCachedEnterprises(telesBEDefinitionSettings.VRConnectionId);
            if (cachedEnterprises != null)
                return cachedEnterprises.Select(itm => itm as dynamic).ToList();
            else
                return null;
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
