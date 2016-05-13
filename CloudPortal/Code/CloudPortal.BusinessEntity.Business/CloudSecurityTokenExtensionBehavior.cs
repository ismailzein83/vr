using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace CloudPortal.BusinessEntity.Business
{
    public class CloudSecurityTokenExtensionBehavior : SecurityTokenExtensionBehavior
    {
        public override void AddExtensionsToToken(ISecurityTokenExtensionContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (context.Token == null)
                throw new ArgumentNullException("context.Token");
            var appUserManager = new CloudApplicationUserManager();
            var appTenantManager = new CloudApplicationTenantManager();
            var userApps = appUserManager.GetUserApplications(context.Token.UserId);
            if(userApps != null && userApps.Count > 0)
            {
                context.Token.AccessibleCloudApplications = new List<SecurityTokenCloudApplication>();
                foreach (var appUser in userApps.Where(itm => itm.Settings != null && itm.Settings.Status == UserStatus.Active))
                {
                    var cloudApplicationTenant = appTenantManager.GetApplicationTenantById(appUser.CloudApplicationTenantID);
                    context.Token.AccessibleCloudApplications.Add(new SecurityTokenCloudApplication { ApplicationId = cloudApplicationTenant.ApplicationId });
                }
            }            
        }
    }
}
