using Retail.BusinessEntity.Business;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
namespace Retail.Teles.Business
{
    public class TelesAccountManager
    {
        AccountBEManager _accountBEManager = new AccountBEManager();

        public TelesAccountInfo GetAccountTelesInfo(Guid accountBEDefinitionId, long accountId, Guid vrConnectionId)
        {
            var userAccountInfo = _accountBEManager.GetExtendedSettings<UserAccountMappingInfo>(accountBEDefinitionId, accountId);
            if(userAccountInfo != null)
            {
                TelesAccountInfo telesAccountInfo = new TelesAccountInfo();
                if (userAccountInfo.TelesUserId != null && userAccountInfo.TelesSiteId != null && userAccountInfo.TelesEnterpriseId != null && userAccountInfo.TelesDomainId != null)
                {
                    TelesUserManager telesUserManager = new Business.TelesUserManager();
                    var user = telesUserManager.GetUser(vrConnectionId, userAccountInfo.TelesUserId);
                    if(user != null)
                    {
                        telesAccountInfo.FirstName = user.firstName;
                        telesAccountInfo.LastName = user.lastName;
                        telesAccountInfo.LoginName = user.loginName;
                        TelesSiteManager telesSiteManager = new TelesSiteManager();
                        telesAccountInfo.EnterpriseName = new TelesEnterpriseManager().GetEnterpriseName(vrConnectionId, userAccountInfo.TelesEnterpriseId, userAccountInfo.TelesDomainId);
                        telesAccountInfo.SiteName = telesSiteManager.GetSiteName(vrConnectionId, userAccountInfo.TelesSiteId);

                        var siteRoutinrGroups = telesSiteManager.GetSiteRoutingGroups(vrConnectionId, userAccountInfo.TelesSiteId);
                        if (siteRoutinrGroups != null)
                        {
                            var routingGroup = siteRoutinrGroups.FindRecord(x => x.id == user.routingGroupId);
                            if (routingGroup != null)
                            {
                                telesAccountInfo.RoutingGroupName = routingGroup.name;
                            }
                        }
                    }
                    return telesAccountInfo;
                }
            }
            return null;
        }
    }
}
