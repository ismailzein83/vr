using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class CloudAuthServerManager
    {
        public const string SecurityTokenAppIdsExtensionName = "CloudAuthServer_TokenExtension";

        public bool HasAccessToCurrentApp(SecurityToken securityToken)
        {
            var authServer = GetAuthServer();
            if (authServer == null)
                return true;
            Object userApplicationIds;
            if(securityToken.Extensions.TryGetValue(SecurityTokenAppIdsExtensionName, out userApplicationIds))
            {
                IEnumerable<int> applicationIds = userApplicationIds as IEnumerable<int>;
                if (applicationIds != null && applicationIds.Contains(authServer.Settings.CurrentApplicationId))
                    return true;
            }
            return false;
        }

        public CloudAuthServer GetAuthServer()
        {
            return null;
            //if (System.Web.HttpContext.Current.Request.Url.Port != 8787)
            //{
            //    return new CloudAuthServer
            //    {
            //        ApplicationIdentification = new CloudApplicationIdentification
            //        {
            //            IdentificationKey = "Application 1"
            //        },
            //        Settings = new CloudAuthServerSettings
            //        {
            //            AuthenticationCookieName = "Cloud-AuthServer-CookieName",
            //            CloudServiceHTTPHeaderName = "Vanrise_CloudApplicationIdentification",
            //            InternalURL = "http://localhost:8787",
            //            OnlineURL = "http://localhost:8787/Security/Login",
            //            TokenDecryptionKey = "CloudSecretKey",
            //            CurrentApplicationId = 2
            //        }
            //    };
            //}
            //else
            //    return null;
        }
    }
}
