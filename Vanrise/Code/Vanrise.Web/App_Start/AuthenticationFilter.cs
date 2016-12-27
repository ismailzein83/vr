using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Vanrise.Common;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Web.App_Start
{
    public class AuthenticationFilter : System.Web.Http.Filters.AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (NeedsAuthentication(actionContext))
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
                        InvalidAccess invalidAccess;
                        string decryptionKey = (new SecurityManager()).GetTokenDecryptionKey();
                        string decryptedToken = Common.Cryptography.Decrypt(encryptedToken, decryptionKey);
                        string errorMessage;
                        if (decryptedToken != String.Empty)
                        {
                            SecurityToken securityToken = Serializer.Deserialize<SecurityToken>(decryptedToken);
                            if (securityToken == null)
                            {
                                actionContext.Response = Utils.CreateResponseMessage(System.Net.HttpStatusCode.Unauthorized, "Invalid Token");
                            }
                            else if (!(new SecurityManager()).CheckTokenAccess(securityToken, out errorMessage, out invalidAccess))
                            {
                                System.Net.HttpStatusCode httpStatusCode;
                                switch (invalidAccess)
                                {
                                    case InvalidAccess.LicenseExpired: httpStatusCode = System.Net.HttpStatusCode.PaymentRequired; break;
                                    case InvalidAccess.TokenExpired:
                                    case InvalidAccess.UnauthorizeAccess:
                                    default: httpStatusCode = System.Net.HttpStatusCode.Unauthorized; break;

                                }
                                actionContext.Response = Utils.CreateResponseMessage(httpStatusCode, errorMessage);
                            }
                        }
                        else
                        {
                            actionContext.Response = Utils.CreateResponseMessage(System.Net.HttpStatusCode.Unauthorized, "Invalid Token");
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerFactory.GetExceptionLogger().WriteException(ex);
                        actionContext.Response = Utils.CreateResponseMessage(System.Net.HttpStatusCode.Unauthorized, "Invalid Token");
                    }
                }
            }

            base.OnAuthorization(actionContext);
        }

        private bool NeedsAuthentication(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            Vanrise.Entities.IsAnonymousAttribute isAnonymosAttribute = actionContext.ActionDescriptor.GetCustomAttributes<Vanrise.Entities.IsAnonymousAttribute>().FirstOrDefault();
            if (isAnonymosAttribute != null)
                return false;
            Vanrise.Entities.IsInternalAPIAttribute isInternalAPI = actionContext.ActionDescriptor.GetCustomAttributes<Vanrise.Entities.IsInternalAPIAttribute>().FirstOrDefault();
            if (isInternalAPI != null && ConfigurationManager.AppSettings["IsInternalAPIApplication"] == "true")
                return false;
            return true;
        }
    }
}