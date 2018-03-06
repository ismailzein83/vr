using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Security.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Security")]
    public class SecurityController : Vanrise.Web.Base.BaseAPIController
    {
        SecurityManager _manager;
        public SecurityController()
        {
            _manager = new SecurityManager();
        }

        [IsAnonymous]
        [HttpPost]
        [Route("Authenticate")]
        public AuthenticateOperationOutput<AuthenticationToken> Authenticate(CredentialsInput credentialsObject)
        {
            SecurityManager manager = new SecurityManager();
            return manager.Authenticate(credentialsObject.Email, credentialsObject.Password);
        }

        [IsAnonymous]
        [HttpPost]
        [Route("TryRenewCurrentSecurityToken")]
        public TryRenewCurrentSecurityTokenOutput TryRenewCurrentSecurityToken()
        {
            AuthenticationToken newAuthenticationToken;
            bool isSucceeded = _manager.TryRenewCurrentAuthenticationToken(out newAuthenticationToken);
            return new TryRenewCurrentSecurityTokenOutput
            {
                IsSucceeded = isSucceeded,
                NewAuthenticationToken = newAuthenticationToken
            };
        }

        [HttpPost]
        [Route("ChangePassword")]
        public Vanrise.Entities.UpdateOperationOutput<object> ChangePassword(ChangedPasswordObject changedPasswordObject)
        {          
            SecurityManager manager = new SecurityManager();
            return manager.ChangePassword(changedPasswordObject.OldPassword,changedPasswordObject.NewPassword);
        }

        [HttpGet]
        [Route("IsAllowed")]
        public bool IsAllowed(string requiredPermissions)
        {
            return SecurityContext.Current.IsAllowed(requiredPermissions);
        }

        [HttpGet]
        [Route("HasPermissionToActions")]
        public bool HasPermissionToActions(string systemActionNames)
        {
            return SecurityContext.Current.HasPermissionToActions(systemActionNames);
        }

        [IsAnonymous]
        [HttpGet]
        [Route("HasAuthServer")]
        public bool HasAuthServer()
        {
            CloudAuthServerManager manager = new CloudAuthServerManager();
            return manager.HasAuthServer();
        }

        [IsAnonymous]
        [HttpGet]
        [Route("GetPasswordValidationInfo")]
        public PasswordValidationInfo GetPasswordValidationInfo()
        {
            SecurityManager manager = new SecurityManager();
            return manager.GetPasswordValidationInfo();
        }
    }

    public class TryRenewCurrentSecurityTokenOutput
    {
        public bool IsSucceeded { get; set; }

        public AuthenticationToken NewAuthenticationToken { get; set; }
    }
}