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
            var appUserManager = new CloudApplicationUserManager();
            var userApps = appUserManager.GetUserApplications(context.Token.UserId);
            if(userApps != null && userApps.Count > 0)
            {
                context.Token.Extensions.Add(CloudAuthServerManager.SecurityTokenAppIdsExtensionName, 
                    userApps.Where(itm => itm.Settings != null && itm.Settings.Status == UserStatus.Active).Select(itm => itm.ApplicationId).ToList());
            }            
        }
    }
}
