using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Vanrise.Common;
using Vanrise.Security.Entities;

namespace Vanrise.Web.App_Start
{
    public class AuthenticationFilter : System.Web.Http.Filters.AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            Vanrise.Entities.IsAnonymousAttribute att = actionContext.ActionDescriptor.GetCustomAttributes<Vanrise.Entities.IsAnonymousAttribute>().FirstOrDefault();

            if (att == null)
            {
                string encryptedToken = HttpContext.Current.Request.Headers[Vanrise.Security.Business.SecurityContext.SECURITY_TOKEN_NAME];

                if (encryptedToken == null)
                {
                    actionContext.Response = Utils.CreateResponseMessage(System.Net.HttpStatusCode.Unauthorized, "Could not find a token to authorize this request");
                }
                else
                {
                    try
                    {
                        string decryptedToken = Common.Cryptography.Decrypt(encryptedToken, ConfigurationManager.AppSettings[Vanrise.Security.Business.SecurityContext.SECURITY_ENCRYPTION_SECRETE_KEY]);
                        if (decryptedToken != String.Empty)
                        {
                            SecurityToken securityToken = Serializer.Deserialize<SecurityToken>(decryptedToken);
                            if (securityToken == null || securityToken.UserId <= 0)
                            {
                                actionContext.Response = Utils.CreateResponseMessage(System.Net.HttpStatusCode.Unauthorized, "Invalid Identity");
                            }
                            else if (securityToken.ExpiresAt < DateTime.Now)
                            {
                                actionContext.Response = Utils.CreateResponseMessage(System.Net.HttpStatusCode.Unauthorized, "Token Expired");
                            }
                        }
                        else
                        {
                            actionContext.Response = Utils.CreateResponseMessage(System.Net.HttpStatusCode.Unauthorized, "Invalid Token");
                        }
                    }
                    catch
                    {
                        actionContext.Response = Utils.CreateResponseMessage(System.Net.HttpStatusCode.Unauthorized, "Invalid Token");
                    }
                }
            }

            base.OnAuthorization(actionContext);
        }
    }
}