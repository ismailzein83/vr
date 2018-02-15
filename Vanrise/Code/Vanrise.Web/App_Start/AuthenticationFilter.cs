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
            try
            {
                if (NeedsAuthentication(actionContext))
                {
                    SecurityToken securityToken;
                    if(!SecurityContext.TryGetSecurityToken(out securityToken))
                    {
                        actionContext.Response = Utils.CreateResponseMessage(System.Net.HttpStatusCode.Unauthorized, "Could not find a token to authorize this request");
                    }
                    else
                    {
                        InvalidAccess invalidAccess;
                        string errorMessage;
                        if (!(new SecurityManager()).CheckTokenAccess(securityToken, out errorMessage, out invalidAccess))
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
                }
            }
            catch (Exception ex)
            {
                Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
                throw;
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