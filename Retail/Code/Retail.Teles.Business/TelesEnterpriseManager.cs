using Retail.BusinessEntity.Business;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business
{
    public class TelesEnterpriseManager
    {
        public IEnumerable<TelesEnterpriseInfo> GetEnterprisesInfo(int switchId, int domainId, TelesEnterpriseFilter filter)
        {
            var actionPath = string.Format("/domain/{0}/sub",domainId);
            List<dynamic> enterprises = TelesWebAPIClient.Get<List<dynamic>>(switchId, actionPath);
            List<TelesEnterpriseInfo> telesEnterpriseInfo = new List<TelesEnterpriseInfo>();
            if(enterprises != null)
            {
                foreach(var enterprise in enterprises)
                {
                    telesEnterpriseInfo.Add(new TelesEnterpriseInfo
                    {
                        Name = enterprise.name,
                        TelesEnterpriseId = enterprise.id
                    });
                }
            }
            return telesEnterpriseInfo;
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
        public bool MapEnterpriseToAccount(MapEnterpriseToAccountInput input)
        {
            AccountBEManager accountBEManager = new AccountBEManager();

            return accountBEManager.UpdateAccountExtendedSetting<InterpriseAccountMappingInfo>(input.AccountBEDefinitionId, input.AccountId,
                new InterpriseAccountMappingInfo { TelesEnterpriseId = input.TelesEnterpriseId });
        }
    }
}
