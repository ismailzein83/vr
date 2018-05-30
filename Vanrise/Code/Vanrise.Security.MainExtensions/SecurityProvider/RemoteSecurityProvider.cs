//using System;
//using Vanrise.Entities;
//using Vanrise.Common;
//using Vanrise.Common.Business;
//using Vanrise.Security.Entities;
//using Vanrise.Security.Business;

//namespace Vanrise.Security.MainExtensions.SecurityProvider
//{
//    public class RemoteSecurityProvider : SecurityProviderExtendedSettings
//    {
//        public override string AuthenticateUserEditor { get { return "vr-sec-securityprovider-authenticateuser-emailpassword"; } }

//        public override string FindUserEditor { get { return "vr-sec-securityprovider-finduser-remoteprovider"; } }

//        public override bool SupportPasswordManagement { get { return true; } }

//        public Guid VRConnectionId { get; set; }

//        public override SecurityProviderAuthenticateResult Authenticate(ISecurityProviderAuthenticateContext context)
//        {
//            var payload = context.Payload.CastWithValidate<EmailPasswordSecurityProviderAuthenticationPayload>("context.Payload");
//            CredentialsInput credentialsInput = new CredentialsInput() { Email = payload.Email, Password = payload.Password };
//            var vrConnection = new VRConnectionManager().GetVRConnection<VRInterAppRestConnection>(VRConnectionId);
//            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
//            AuthenticateOperationOutput<AuthenticationToken> authenticateOperationOutput = connectionSettings.AnonymousPost<CredentialsInput, AuthenticateOperationOutput<AuthenticationToken>>("/api/VR_Sec/Security/Authenticate", credentialsInput);

//            context.FailureMessage = authenticateOperationOutput.Message;
//            context.PasswordExpirationDaysLeft = authenticateOperationOutput.AuthenticationObject.PasswordExpirationDaysLeft;
//            context.AuthenticatedUser = new UserManager().GetUserbyEmail(payload.Email);

//            SecurityProviderAuthenticateResult authenticateResult;
//            switch (authenticateOperationOutput.Result)
//            {
//                case AuthenticateOperationResult.ActivationNeeded:
//                    authenticateResult = SecurityProviderAuthenticateResult.ActivationNeeded;
//                    break;
//                case AuthenticateOperationResult.UserNotExists:
//                    authenticateResult = SecurityProviderAuthenticateResult.UserNotExists;
//                    break;
//                case AuthenticateOperationResult.WrongCredentials:
//                    authenticateResult = SecurityProviderAuthenticateResult.WrongCredentials;
//                    break;
//                case AuthenticateOperationResult.PasswordExpired:
//                    authenticateResult = SecurityProviderAuthenticateResult.PasswordExpired;
//                    break;
//                case AuthenticateOperationResult.Inactive:
//                    authenticateResult = SecurityProviderAuthenticateResult.Inactive;
//                    break;
//                case AuthenticateOperationResult.Failed:
//                    authenticateResult = SecurityProviderAuthenticateResult.Failed;
//                    break;
//                case AuthenticateOperationResult.Succeeded:
//                    authenticateResult = SecurityProviderAuthenticateResult.Succeeded;
//                    break;
//                default: throw new NotSupportedException(string.Format("AuthenticateOperationResult {0} not supported.", authenticateOperationOutput));
//            }

//            return authenticateResult;
//        }

//        public override bool ResetPassword(ISecurityProviderResetPasswordContext context)
//        {
//            if (context.User == null)
//                return false;

//            ResetPasswordInput resetPasswordInput = new ResetPasswordInput() { UserId = context.User.UserId, Password = context.Password };

//            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();

//            UserManager userManager = new UserManager();
//            User loggedInUser = userManager.GetUserbyId(loggedInUserId);
//            string loggeInUserPassword = userManager.GetUserPassword(loggedInUserId);
//            CredentialsInput credentialsInput = new CredentialsInput() { Email = loggedInUser.Email, Password = loggeInUserPassword };

//            var vrConnection = new VRConnectionManager().GetVRConnection<VRInterAppRestConnection>(VRConnectionId);
//            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
//            UpdateOperationOutput<object> result = connectionSettings.Post<ResetPasswordInput, UpdateOperationOutput<object>>(credentialsInput, "/api/VR_Sec/Users/ResetPassword", resetPasswordInput);

//            context.ShowExactMessage = result.ShowExactMessage;
//            context.ValidationMessage = result.Message;
//            if (result.Result == UpdateOperationResult.Succeeded)
//                return true;
//            else
//                return false;
//        }

//        public override bool ActivatePassword(ISecurityProviderActivatePasswordContext context)
//        {
//            if (context.User == null)
//                return false;

//            ActivatePasswordInput activatePasswordInput = new ActivatePasswordInput() { Email = context.User.Email, Password = context.Password, TempPassword = context.TempPassword };

//            var vrConnection = new VRConnectionManager().GetVRConnection<VRInterAppRestConnection>(VRConnectionId);
//            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
//            UpdateOperationOutput<object> result = connectionSettings.AnonymousPost<ActivatePasswordInput, UpdateOperationOutput<object>>("/api/VR_Sec/Users/ActivatePassword", activatePasswordInput);

//            context.ShowExactMessage = result.ShowExactMessage;
//            context.ValidationMessage = result.Message;
//            if (result.Result == UpdateOperationResult.Succeeded)
//                return true;
//            else
//                return false;
//        }

//        public override bool ForgotPassword(ISecurityProviderForgotPasswordContext context)
//        {
//            if (context.User == null)
//                return false;

//            ForgotPasswordInput forgotPasswordInput = new ForgotPasswordInput() { Email = context.User.Email };

//            var vrConnection = new VRConnectionManager().GetVRConnection<VRInterAppRestConnection>(VRConnectionId);
//            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
//            UpdateOperationOutput<object> result = connectionSettings.AnonymousPost<ForgotPasswordInput, UpdateOperationOutput<object>>("/api/VR_Sec/Users/ForgotPassword", forgotPasswordInput);

//            context.ShowExactMessage = result.ShowExactMessage;
//            context.ValidationMessage = result.Message;
//            if (result.Result == UpdateOperationResult.Succeeded)
//                return true;
//            else
//                return false;
//        }

//        public override bool ChangePassword(ISecurityProviderChangePasswordContext context)
//        {
//            if (context.User == null)
//                return false;

//            string loggeInUserPassword = new UserManager().GetUserPassword(context.User.UserId);
//            CredentialsInput credentialsInput = new CredentialsInput() { Email = context.User.Email, Password = loggeInUserPassword };

//            ChangedPasswordObject changedPasswordObject = new ChangedPasswordObject() { NewPassword = context.NewPassword, OldPassword = context.OldPassword };

//            var vrConnection = new VRConnectionManager().GetVRConnection<VRInterAppRestConnection>(VRConnectionId);
//            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
//            UpdateOperationOutput<object> result = connectionSettings.Post<ChangedPasswordObject, UpdateOperationOutput<object>>(credentialsInput, "/api/VR_Sec/Security/ChangePassword", changedPasswordObject);

//            context.ShowExactMessage = result.ShowExactMessage;
//            context.ValidationMessage = result.Message;
//            if (result.Result == UpdateOperationResult.Succeeded)
//                return true;
//            else
//                return false;
//        }
//    }
//}