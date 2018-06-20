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
        [Route("Authenticate2")]
        public AuthenticateOperationOutput<AuthenticationToken> Authenticate2(AuthenticateInput authenticateInput)
        {
            SecurityManager manager = new SecurityManager();
            return manager.Authenticate(authenticateInput.SecurityProviderId, authenticateInput.Payload);
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
            return manager.ChangePassword(changedPasswordObject.OldPassword, changedPasswordObject.NewPassword);
        }

        [IsAnonymous]
        [HttpPost]
        [Route("ChangeExpiredPassword")]
        public Vanrise.Entities.UpdateOperationOutput<object> ChangeExpiredPassword(ChangeExpiredPasswordInput changeExpiredPasswordObject)
        {
            SecurityManager manager = new SecurityManager();
            return manager.ChangeExpiredPassword(changeExpiredPasswordObject.Email, changeExpiredPasswordObject.OldPassword, changeExpiredPasswordObject.Password);
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
        [Route("GetPasswordValidationInfo")]
        public PasswordValidationInfo GetPasswordValidationInfo(Guid? securityProviderId = null)
        {
            SecurityManager manager = new SecurityManager();
            return manager.GetPasswordValidationInfo(securityProviderId);
        }

        [IsAnonymous]
        [HttpPost]
        [Route("GetRemotePasswordValidationInfo")]
        public PasswordValidationInfo GetRemotePasswordValidationInfo(RemotePasswordValidationInfoInput input)
        {
            SecurityManager manager = new SecurityManager();
            return manager.GetRemotePasswordValidationInfo(input.VRConnectionId, input.SecurityProviderId);
        }

        [HttpGet]
        [Route("RedirectToApplication")]
        public ApplicationRedirectOutput RedirectToApplication(string applicationURL)
        {
            SecurityManager manager = new SecurityManager();
            return manager.RedirectToApplication(applicationURL);
        }

        [IsAnonymous]
        [HttpPost]
        [Route("TryGenerateToken")]
        public ApplicationRedirectOutput TryGenerateToken(ApplicationRedirectInput input)
        {
            return _manager.TryGenerateToken(input);
        }

        [IsAnonymous]
        [HttpPost]
        [Route("ValidateSecurityToken")]
        public bool ValidateSecurityToken(ValidateSecurityTokenInput input)
        {
            return _manager.ValidateSecurityToken(input);
        }
    }

    public class TryRenewCurrentSecurityTokenOutput
    {
        public bool IsSucceeded { get; set; }

        public AuthenticationToken NewAuthenticationToken { get; set; }
    }

    public class AuthenticateInput
    {
        public Guid SecurityProviderId { get; set; }
        public SecurityProviderAuthenticationPayload Payload { get; set; }
    }

    public class RemotePasswordValidationInfoInput
    {
        public Guid? SecurityProviderId { get; set; }
        public Guid VRConnectionId { get; set; }
    }
}