using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public enum SecurityProviderAuthenticateResult { Succeeded = 0, Failed = 1, Inactive = 2, WrongCredentials = 3, UserNotExists = 4, ActivationNeeded = 5, PasswordExpired = 6 }

    public class SecurityProvider
    {
        public Guid SecurityProviderId { get; set; }

        public string Name { get; set; }

        public SecurityProviderSettings Settings { get; set; }
    }

    public class SecurityProviderSettings
    {
        public SecurityProviderExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class SecurityProviderExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string FindUserEditor { get; set; }

        public virtual string AuthenticateUserEditor { get; set; }

        public abstract SecurityProviderAuthenticateResult Authenticate(ISecurityProviderAuthenticateContext context);

        public virtual bool SupportPasswordManagement { get { return false; } }

        public virtual bool PasswordCheckRequired { get { return false; } }

        public virtual bool ResetPassword(ISecurityProviderResetPasswordContext context) { throw new NotImplementedException(); }

        public virtual bool ActivatePassword(ISecurityProviderActivatePasswordContext context) { throw new NotImplementedException(); }

        public virtual bool ForgotPassword(ISecurityProviderForgotPasswordContext context) { throw new NotImplementedException(); }

        public virtual bool ChangePassword(ISecurityProviderChangePasswordContext context) { throw new NotImplementedException(); }

        public virtual PasswordValidationInfo GetPasswordValidationInfo(ISecurityProviderGetPasswordValidationInfoContext context) { throw new NotImplementedException(); }

        public virtual ApplicationRedirectInput GetApplicationRedirectInput(ISecurityProviderGetApplicationRedirectInputContext context) { throw new NotImplementedException(); }

        public virtual bool ValidateSecurityToken(ISecurityProviderValidateSecurityTokenContext context) { throw new NotImplementedException(); }

        public virtual void OnBeforeSave(ISecurityProviderOnBeforeSaveContext context) { }

        public virtual IEnumerable<RegisteredApplicationInfo> GetRemoteRegisteredApplicationsInfo(ISecurityProviderGetRemoteRegisteredApplicationsInfoContext context) { return null; }
    }

    public abstract class SecurityProviderAuthenticationPayload
    {

    }

    public class EmailPasswordSecurityProviderAuthenticationPayload : SecurityProviderAuthenticationPayload
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public interface ISecurityProviderAuthenticateContext
    {
        SecurityProviderAuthenticationPayload Payload { get; }

        User AuthenticatedUser { set; }

        string FailureMessage { set; }

        int? PasswordExpirationDaysLeft { set; }
    }

    public class SecurityProviderAuthenticateContext : ISecurityProviderAuthenticateContext
    {
        public SecurityProviderAuthenticationPayload Payload { get; set; }

        public User AuthenticatedUser { get; set; }

        public string FailureMessage { get; set; }

        public int? PasswordExpirationDaysLeft { get; set; }
    }

    public interface ISecurityProviderResetPasswordContext
    {
        User User { get; }
        string Password { get; }
        string ValidationMessage { set; }
        bool ShowExactMessage { set; }
    }

    public class SecurityProviderResetPasswordContext : ISecurityProviderResetPasswordContext
    {
        public User User { get; set; }

        public string Password { get; set; }

        public string ValidationMessage { get; set; }

        public bool ShowExactMessage { get; set; }
    }

    public interface ISecurityProviderActivatePasswordContext
    {
        User User { get; }
        string Password { get; }
        string TempPassword { get; }
        string ValidationMessage { set; }
        bool ShowExactMessage { set; }
    }

    public class SecurityProviderActivatePasswordContext : ISecurityProviderActivatePasswordContext
    {
        public User User { get; set; }
        public string Password { get; set; }
        public string TempPassword { get; set; }
        public string ValidationMessage { get; set; }
        public bool ShowExactMessage { get; set; }
    }

    public interface ISecurityProviderForgotPasswordContext
    {
        User User { get; }
        string ValidationMessage { set; }
        bool ShowExactMessage { set; }
    }

    public class SecurityProviderForgotPasswordContext : ISecurityProviderForgotPasswordContext
    {
        public User User { get; set; }
        public string ValidationMessage { get; set; }
        public bool ShowExactMessage { get; set; }
    }

    public interface ISecurityProviderChangePasswordContext
    {
        bool PasswordExpired { get; }
        User User { get; }
        string NewPassword { get; }
        string OldPassword { get; }
        string ValidationMessage { set; }
        bool ShowExactMessage { set; }
    }

    public class SecurityProviderChangePasswordContext : ISecurityProviderChangePasswordContext
    {
        public bool PasswordExpired { get; set; }
        public User User { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
        public string ValidationMessage { get; set; }
        public bool ShowExactMessage { get; set; }
    }

    public interface ISecurityProviderGetPasswordValidationInfoContext
    {

    }

    public class SecurityProviderGetPasswordValidationInfoContext : ISecurityProviderGetPasswordValidationInfoContext
    {

    }

    public interface ISecurityProviderGetApplicationRedirectInputContext
    {
        string Token { get; }

        string Email { get; }
    }

    public class SecurityProviderGetApplicationRedirectInputContext : ISecurityProviderGetApplicationRedirectInputContext
    {
        public string Token { get; set; }

        public string Email { get; set; }
    }

    public interface ISecurityProviderValidateSecurityTokenContext
    {
        string Token { get; }

        Guid ApplicationId { get; }
    }

    public class SecurityProviderValidateSecurityTokenContext : ISecurityProviderValidateSecurityTokenContext
    {
        public string Token { get; set; }

        public Guid ApplicationId { get; set; }
    }

    public interface ISecurityProviderOnBeforeSaveContext
    {
        SecurityProviderExtendedSettings PreviousSettings { get; }

        string ErrorMessage { set; }
    }

    public class SecurityProviderOnBeforeSaveContext : ISecurityProviderOnBeforeSaveContext
    {
        public SecurityProviderExtendedSettings PreviousSettings { get; set; }

        public string ErrorMessage { get; set; }
    }

    public interface ISecurityProviderGetRemoteRegisteredApplicationsInfoContext
    {
        string SerializedFilter { get; }
    }

    public class SecurityProviderGetRemoteRegisteredApplicationsInfoContext : ISecurityProviderGetRemoteRegisteredApplicationsInfoContext
    {
        public string SerializedFilter { get; set; }
    }
}