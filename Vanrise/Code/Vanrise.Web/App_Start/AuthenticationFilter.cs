using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Vanrise.Web.App_Start
{
    public class AuthenticationFilter : System.Web.Http.Filters.AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            Vanrise.Entities.IsAnonymousAttribute att = actionContext.ActionDescriptor.GetCustomAttributes<Vanrise.Entities.IsAnonymousAttribute>().FirstOrDefault();

            if (att == null)
            {
                string token = HttpContext.Current.Request.Headers[Vanrise.Security.Business.SecurityContext.SECURITY_TOKEN_NAME];

                if (token == null)
                {
                    actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }
                else
                {
                    string decryptedText = String.Empty;
                    try
                    {
                        decryptedText = Common.Cryptography.Decrypt(token, ConfigurationManager.AppSettings[Vanrise.Security.Business.SecurityContext.SECURITY_ENCRYPTION_SECRETE_KEY]);
                    }
                    catch
                    {

                    }
                    
                    if (decryptedText == String.Empty)
                    {
                        actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                    }
                }
            }

            base.OnAuthorization(actionContext);
        }
    }
}