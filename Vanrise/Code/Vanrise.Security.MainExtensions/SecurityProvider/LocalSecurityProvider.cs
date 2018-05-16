using System;
using Vanrise.Common;
using Vanrise.Security.Entities;
using Vanrise.Security.Business;

namespace Vanrise.Security.MainExtensions.SecurityProvider
{
    public class LocalSecurityProvider : SecurityProviderExtendedSettings
    {
        public override SecurityProviderAuthenticateResult Authenticate(ISecurityProviderAuthenticateContext context)
        {
            EmailPasswordSecurityProviderAuthenticationPayload payload = context.Payload as EmailPasswordSecurityProviderAuthenticationPayload;
            payload.ThrowIfNull("payload");

            UserManager manager = new UserManager();
            User user = manager.GetUserbyEmail(payload.Email);
            if (user == null)
                return SecurityProviderAuthenticateResult.UserNotExists;

            context.AuthenticatedUser = user;

            DateTime passwordChangeTime;
            string loggedInUserPassword = manager.GetUserPassword(user.UserId, out passwordChangeTime);

            if (HashingUtility.VerifyHash(payload.Password, "", loggedInUserPassword))
                return SecurityProviderAuthenticateResult.Succeeded;

            string loggedInUserTempPassword = manager.GetUserTempPassword(user.UserId);
            if (HashingUtility.VerifyHash(payload.Password, "", loggedInUserTempPassword))
                return SecurityProviderAuthenticateResult.ActivationNeeded;

            return SecurityProviderAuthenticateResult.WrongCredentials;
        }
    }
}