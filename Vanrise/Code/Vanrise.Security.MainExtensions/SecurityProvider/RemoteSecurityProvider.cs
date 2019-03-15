﻿using System;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Security.Entities;
using Vanrise.Security.Business;
using System.Collections.Generic;

namespace Vanrise.Security.MainExtensions.SecurityProvider
{
    public class RemoteSecurityProvider : SecurityProviderExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("04C515AB-5A8F-4C3B-A8F1-A4CE98518D06"); } }
        public override string AuthenticateUserEditor { get { return "vr-sec-securityprovider-authenticateuser-emailpassword"; } }

        public override string FindUserEditor { get { return "vr-sec-securityprovider-finduser-remoteprovider"; } }

        public override bool SupportPasswordManagement { get { return true; } }

        public Guid VRConnectionId { get; set; }

        public Guid ApplicationId { get; set; }

        public override SecurityProviderAuthenticateResult Authenticate(ISecurityProviderAuthenticateContext context)
        {
            var payload = context.Payload.CastWithValidate<EmailPasswordSecurityProviderAuthenticationPayload>("context.Payload");
            CredentialsInput credentialsInput = new CredentialsInput() { Email = payload.Email, Password = payload.Password };
            var vrConnection = new VRConnectionManager().GetVRConnection<VRInterAppRestConnection>(VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            AuthenticateOperationOutput<AuthenticationToken> authenticateOperationOutput = connectionSettings.AnonymousPost<CredentialsInput, AuthenticateOperationOutput<AuthenticationToken>>("/api/VR_Sec/Security/Authenticate", credentialsInput);

            context.FailureMessage = authenticateOperationOutput.Message;
            context.AuthenticatedUser = new UserManager().GetUserbyEmail(payload.Email);

            if (authenticateOperationOutput.AuthenticationObject != null)
                context.PasswordExpirationDaysLeft = authenticateOperationOutput.AuthenticationObject.PasswordExpirationDaysLeft;

            SecurityProviderAuthenticateResult authenticateResult;
            switch (authenticateOperationOutput.Result)
            {
                case AuthenticateOperationResult.ActivationNeeded:
                    authenticateResult = SecurityProviderAuthenticateResult.ActivationNeeded;
                    break;
                case AuthenticateOperationResult.WrongCredentials:
                    authenticateResult = SecurityProviderAuthenticateResult.WrongCredentials;
                    break;
                case AuthenticateOperationResult.PasswordExpired:
                    authenticateResult = SecurityProviderAuthenticateResult.PasswordExpired;
                    break;
                case AuthenticateOperationResult.Inactive:
                    authenticateResult = SecurityProviderAuthenticateResult.Inactive;
                    break;
                case AuthenticateOperationResult.Failed:
                    authenticateResult = SecurityProviderAuthenticateResult.Failed;
                    break;
                case AuthenticateOperationResult.Succeeded:
                    authenticateResult = SecurityProviderAuthenticateResult.Succeeded;
                    break;
                default: throw new NotSupportedException(string.Format("AuthenticateOperationResult {0} not supported.", authenticateOperationOutput));
            }

            return authenticateResult;
        }

        public override bool ResetPassword(ISecurityProviderResetPasswordContext context)
        {
            if (context.User == null)
                return false;

            EmailResetPasswordInput emailResetPasswordInput = new EmailResetPasswordInput() { Email = context.User.Email, Password = context.Password };

            var vrConnection = new VRConnectionManager().GetVRConnection<VRInterAppRestConnection>(VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            UpdateOperationOutput<object> result = connectionSettings.Post<EmailResetPasswordInput, UpdateOperationOutput<object>>("/api/VR_Sec/Users/ResetPasswordByEmail", emailResetPasswordInput);

            context.ShowExactMessage = result.ShowExactMessage;
            context.ValidationMessage = result.Message;
            if (result.Result == UpdateOperationResult.Succeeded)
                return true;
            else
                return false;
        }

        public override bool ActivatePassword(ISecurityProviderActivatePasswordContext context)
        {
            if (context.User == null)
                return false;

            ActivatePasswordInput activatePasswordInput = new ActivatePasswordInput() { Email = context.User.Email, Password = context.Password, TempPassword = context.TempPassword };

            var vrConnection = new VRConnectionManager().GetVRConnection<VRInterAppRestConnection>(VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            UpdateOperationOutput<object> result = connectionSettings.AnonymousPost<ActivatePasswordInput, UpdateOperationOutput<object>>("/api/VR_Sec/Users/ActivatePassword", activatePasswordInput);

            context.ShowExactMessage = result.ShowExactMessage;
            context.ValidationMessage = result.Message;
            if (result.Result == UpdateOperationResult.Succeeded)
                return true;
            else
                return false;
        }

        public override bool ForgotPassword(ISecurityProviderForgotPasswordContext context)
        {
            if (context.User == null)
                return false;

            ForgotPasswordInput forgotPasswordInput = new ForgotPasswordInput() { Email = context.User.Email };

            var vrConnection = new VRConnectionManager().GetVRConnection<VRInterAppRestConnection>(VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            UpdateOperationOutput<object> result = connectionSettings.AnonymousPost<ForgotPasswordInput, UpdateOperationOutput<object>>("/api/VR_Sec/Users/ForgotPassword", forgotPasswordInput);

            context.ShowExactMessage = result.ShowExactMessage;
            context.ValidationMessage = result.Message;
            if (result.Result == UpdateOperationResult.Succeeded)
                return true;
            else
                return false;
        }

        public override bool ChangePassword(ISecurityProviderChangePasswordContext context)
        {
            if (context.User == null)
                return false;

            string loggeInUserPassword = context.OldPassword;

            var vrConnection = new VRConnectionManager().GetVRConnection<VRInterAppRestConnection>(VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            UpdateOperationOutput<object> result;
            if (context.PasswordExpired)
            {
                ChangeExpiredPasswordInput changeExpiredPasswordInput = new ChangeExpiredPasswordInput() { Email = context.User.Email, OldPassword = context.OldPassword, Password = context.NewPassword };
                result = connectionSettings.AnonymousPost<ChangeExpiredPasswordInput, UpdateOperationOutput<object>>("/api/VR_Sec/Security/ChangeExpiredPassword", changeExpiredPasswordInput);
            }
            else
            {
                CredentialsInput credentialsInput = new CredentialsInput() { Email = context.User.Email, Password = loggeInUserPassword };
                ChangedPasswordObject changedPasswordObject = new ChangedPasswordObject() { NewPassword = context.NewPassword, OldPassword = context.OldPassword };
                result = connectionSettings.Post<ChangedPasswordObject, UpdateOperationOutput<object>>(credentialsInput, "/api/VR_Sec/Security/ChangePassword", changedPasswordObject);
            }

            context.ShowExactMessage = result.ShowExactMessage;
            context.ValidationMessage = result.Message;
            if (result.Result == UpdateOperationResult.Succeeded)
                return true;
            else
                return false;
        }

        public override PasswordValidationInfo GetPasswordValidationInfo(ISecurityProviderGetPasswordValidationInfoContext context)
        {
            var vrConnection = new VRConnectionManager().GetVRConnection<VRInterAppRestConnection>(VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings.Get<PasswordValidationInfo>("/api/VR_Sec/Security/GetPasswordValidationInfo");
        }

        public override ApplicationRedirectInput GetApplicationRedirectInput(ISecurityProviderGetApplicationRedirectInputContext context)
        {
            return new ApplicationRedirectInput() { ApplicationId = this.ApplicationId, Token = context.Token, Email = context.Email };
        }

        public override bool ValidateSecurityToken(ISecurityProviderValidateSecurityTokenContext context)
        {
            var vrConnection = new VRConnectionManager().GetVRConnection<VRInterAppRestConnection>(VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            ValidateSecurityTokenInput input = new ValidateSecurityTokenInput() { ApplicationId = context.ApplicationId, Token = context.Token };
            return connectionSettings.AnonymousPost<ValidateSecurityTokenInput, bool>("/api/VR_Sec/Security/ValidateSecurityToken", input);
        }

        public override void OnBeforeSave(ISecurityProviderOnBeforeSaveContext context)
        {
            if (context.PreviousSettings != null)
            {
                RemoteSecurityProvider previousRemoteSecurityProvider = context.PreviousSettings as RemoteSecurityProvider;
                if (previousRemoteSecurityProvider != null)
                {
                    if (this.VRConnectionId == previousRemoteSecurityProvider.VRConnectionId)
                    {
                        this.ApplicationId = previousRemoteSecurityProvider.ApplicationId;
                        return;
                    }
                }
            }

            var vrConnection = new VRConnectionManager().GetVRConnection<VRInterAppRestConnection>(VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
                        
            string baseURL = VRWebContext.GetCurrentRequestBaseURL();

            ProductInfo productInfo = new Common.Business.ConfigManager().GetProductInfo();
            RegisterApplicationInput input = new RegisterApplicationInput() { ApplicationName = productInfo.ProductName, ApplicationURL = baseURL };

            RegisterApplicationOutput output = connectionSettings.Post<RegisterApplicationInput, RegisterApplicationOutput>("/api/VR_Sec/RegisteredApplication/RegisterApplication", input);
            if (output == null)
                context.ErrorMessage = "Unable to register application";
            else
                this.ApplicationId = output.ApplicationId;
        }

        public override IEnumerable<RegisteredApplicationInfo> GetRemoteRegisteredApplicationsInfo(ISecurityProviderGetRemoteRegisteredApplicationsInfoContext context)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(this.VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            IEnumerable<RegisteredApplicationInfo> result = connectionSettings.Get<IEnumerable<RegisteredApplicationInfo>>(string.Format("/api/VR_Sec/RegisteredApplication/GetRegisteredApplicationsInfo?filter={0}", context.SerializedFilter));
            if (result == null)
                return null;

            return result.FindAllRecords(itm => itm.ApplicationId != this.ApplicationId);
        }

        public override bool ValidateApplication(IValidateApplicationContext context)
        {
            if (context.ApplicationId != this.ApplicationId)
                return false;

            return true;
        }

        public override string GetApplicationURL(GetApplicationURLContext context)
        {
            var vrConnection = new VRConnectionManager().GetVRConnection<VRInterAppRestConnection>(VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings.BaseURL;
        }
    }
}