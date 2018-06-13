using System;
using Vanrise.Common;
using Vanrise.Security.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vanrise.Common.Business;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Vanrise.Security.MainExtensions.SecurityProvider
{
    public class LocalSecurityProvider : SecurityProviderExtendedSettings
    {
        public override string AuthenticateUserEditor { get { return "vr-sec-securityprovider-authenticateuser-emailpassword"; } }

        public override string FindUserEditor { get { return "vr-sec-securityprovider-finduser-localprovider"; } }

        public override bool SupportPasswordManagement { get { return true; } }

        public override bool PasswordCheckRequired { get { return true; } }

        public override SecurityProviderAuthenticateResult Authenticate(ISecurityProviderAuthenticateContext context)
        {
            var payload = context.Payload.CastWithValidate<EmailPasswordSecurityProviderAuthenticationPayload>("context.Payload");

            UserManager manager = new UserManager();
            User user = manager.GetUserbyEmail(payload.Email);
            if (user == null)
                return SecurityProviderAuthenticateResult.UserNotExists;

            var configManager = new Security.Business.ConfigManager();

            TimeSpan? disableTillTime;
            if (manager.IsUserDisabledTill(user, out disableTillTime))
            {
                context.FailureMessage = string.Format("User is locked, try after {0} minutes, {1} seconds", disableTillTime.Value.Minutes, disableTillTime.Value.Seconds);
                return SecurityProviderAuthenticateResult.Inactive;
            }

            if (configManager.GetFailedInterval().HasValue)
            {
                DateTime now = DateTime.Now;
                UserFailedLoginManager userFailedLoginManager = new UserFailedLoginManager();
                var failedLogins = userFailedLoginManager.GetUserFailedLoginByUserId(user.UserId, now.AddMinutes(-1), now.AddTicks(configManager.GetFailedInterval().Value.Ticks));
                if (failedLogins.Count() == configManager.GetMaxFailedTries())
                {
                    DateTime disableTill = now.AddMinutes(configManager.GetLockForMinutes());
                    var disableTillTimeSpan = disableTill - now;

                    SecurityManager.SendNotificationMail(user, configManager);

                    if (manager.UpdateDisableTill(user.UserId, disableTill))
                    {
                        VRActionLogger.Current.LogObjectCustomAction(UserManager.UserLoggableEntity.Instance, "Login", false, user, "User is locked after multiple failed logins");
                        context.FailureMessage = string.Format("User is locked, try after {0} minutes, {1} seconds", disableTillTimeSpan.Minutes, disableTillTimeSpan.Seconds);
                        return SecurityProviderAuthenticateResult.Inactive;
                    }
                }
            }

            context.AuthenticatedUser = user;

            DateTime passwordChangeTime;
            string loggedInUserPassword = manager.GetUserPassword(user.UserId, out passwordChangeTime);

            if (HashingUtility.VerifyHash(payload.Password, "", loggedInUserPassword))
            {
                int? passwordExpirationDaysLeft;
                bool isPasswordExpired = Helper.CheckIfPasswordExpired(user, passwordChangeTime, out passwordExpirationDaysLeft);
                context.PasswordExpirationDaysLeft = passwordExpirationDaysLeft;

                if (isPasswordExpired)
                {
                    VRActionLogger.Current.LogObjectCustomAction(UserManager.UserLoggableEntity.Instance, "Login", false, user, "User password is expired");
                    return SecurityProviderAuthenticateResult.PasswordExpired;
                }

                return SecurityProviderAuthenticateResult.Succeeded;
            }

            string loggedInUserTempPassword = manager.GetUserTempPassword(user.UserId);
            if (HashingUtility.VerifyHash(payload.Password, "", loggedInUserTempPassword))
            {
                VRActionLogger.Current.LogObjectCustomAction(UserManager.UserLoggableEntity.Instance, "Login", false, user, "Try login with activation needed");
                return SecurityProviderAuthenticateResult.ActivationNeeded;
            }

            int addedId;
            new UserFailedLoginManager().AddUserFailedLogin(new UserFailedLogin { FailedResultId = (int)AuthenticateOperationResult.WrongCredentials, UserId = user.UserId }, out addedId);
            VRActionLogger.Current.LogObjectCustomAction(UserManager.UserLoggableEntity.Instance, "Login", false, user, "Try login with wrong credentials");
            return SecurityProviderAuthenticateResult.WrongCredentials;
        }

        public override bool ResetPassword(ISecurityProviderResetPasswordContext context)
        {
            if (context.User == null)
                return false;

            int lastModifiedBy = SecurityContext.Current.GetLoggedInUserId();

            string validationMessage;
            if (!new SecurityManager().DoesPasswordMeetRequirement(context.Password, out validationMessage))
            {
                context.ValidationMessage = validationMessage;
                return false;
            }

            string hashedPassword = HashingUtility.ComputeHash(context.Password, "", null);

            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            bool updateActionSucc = dataManager.ResetPassword(context.User.UserId, hashedPassword, lastModifiedBy);

            if (updateActionSucc)
            {
                new UserPasswordHistoryManager().AddPasswordHistory(context.User.UserId, hashedPassword, true);
                var configManager = new Security.Business.ConfigManager();
                if (configManager.ShouldSendEmailOnResetPasswordByAdmin())
                {
                    Task taskSendMail = new Task(() =>
                    {
                        try
                        {
                            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                            objects.Add("User", context.User);
                            objects.Add("Password", context.Password);

                            Guid resetPasswordId = configManager.GetResetPasswordId();
                            new VRMailManager().SendMail(resetPasswordId, objects);
                        }
                        catch (Exception ex)
                        {
                            LoggerFactory.GetExceptionLogger().WriteException(ex);
                        }
                    });
                    taskSendMail.Start();
                }
            }
            return updateActionSucc;
        }

        public override bool ActivatePassword(ISecurityProviderActivatePasswordContext context)
        {
            if (context.User == null)
                return false;

            int lastModifiedBy = context.User.UserId;

            SecurityManager securityManager = new SecurityManager();
            string validationMessage;
            if (!securityManager.DoesPasswordMeetRequirement(context.Password, out validationMessage))
            {
                context.ValidationMessage = validationMessage;
                return false;
            }

            if (securityManager.IsPasswordSame(context.User.UserId, context.Password, out validationMessage))
            {
                context.ValidationMessage = validationMessage;
                context.ShowExactMessage = true;
                return false;
            }

            string loggedInUserTempPassword = new UserManager().GetUserTempPassword(context.User.UserId);
            if (!HashingUtility.VerifyHash(context.TempPassword, "", loggedInUserTempPassword))
                return false;

            string hashedPassword = HashingUtility.ComputeHash(context.Password, "", null);

            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            bool updateActionSucc = dataManager.ActivatePassword(context.User.Email, hashedPassword, lastModifiedBy);

            if (updateActionSucc)
                new UserPasswordHistoryManager().AddPasswordHistory(context.User.UserId, hashedPassword, false);

            return updateActionSucc;
        }

        public override bool ForgotPassword(ISecurityProviderForgotPasswordContext context)
        {
            if (context.User == null)
                return false;

            int lastModifiedBy = context.User.UserId;

            PasswordGenerator pwdGenerator = new PasswordGenerator();
            string pwd = pwdGenerator.Generate();

            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();

            TimeSpan s_tempPasswordValidity;
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["TempPasswordValidity"], out s_tempPasswordValidity))
                s_tempPasswordValidity = new TimeSpan(1, 0, 0);

            bool updateActionSucc = dataManager.UpdateTempPasswordByEmail(context.User.Email, HashingUtility.ComputeHash(pwd, "", null), DateTime.Now.Add(s_tempPasswordValidity), lastModifiedBy);
            if (updateActionSucc)
            {
                Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                objects.Add("User", context.User);
                objects.Add("Password", pwd);

                Guid forgotPasswordId = new Security.Business.ConfigManager().GetForgotPasswordId();

                VRMailManager vrMailManager = new VRMailManager();
                vrMailManager.SendMail(forgotPasswordId, objects);
            }

            return updateActionSucc;
        }

        public override bool ChangePassword(ISecurityProviderChangePasswordContext context)
        {
            if (context.User == null)
                return false;

            int lastModifiedBy = context.User.UserId;

            SecurityManager securityManager = new SecurityManager();
            string validationMessage;
            if (!securityManager.DoesPasswordMeetRequirement(context.NewPassword, out validationMessage))
            {
                context.ValidationMessage = validationMessage;
                return false;
            }

            if (securityManager.IsPasswordSame(context.User.UserId, context.NewPassword, out validationMessage))
            {
                context.ValidationMessage = validationMessage;
                return false;
            }

            string currentUserPassword = new UserManager().GetUserPassword(context.User.UserId);

            bool changePasswordActionSucc = false;
            bool oldPasswordIsCorrect = HashingUtility.VerifyHash(context.OldPassword, "", currentUserPassword);
            if (oldPasswordIsCorrect)
            {
                string encryptedNewPassword = HashingUtility.ComputeHash(context.NewPassword, "", null);

                IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                changePasswordActionSucc = dataManager.ChangePassword(context.User.UserId, encryptedNewPassword, lastModifiedBy);

                if (changePasswordActionSucc)
                    new UserPasswordHistoryManager().AddPasswordHistory(context.User.UserId, encryptedNewPassword, false);
            }

            return changePasswordActionSucc;
        }

        public override PasswordValidationInfo GetPasswordValidationInfo(ISecurityProviderGetPasswordValidationInfoContext context)
        {
            var configManager = new Security.Business.ConfigManager();

            StringBuilder msg = new StringBuilder("Password must meet complexity requirements:");

            int passwordLength = configManager.GetPasswordLength();
            int maxPasswordLength = configManager.GetMaxPasswordLength();
            var complexity = configManager.GetPasswordComplexity();

            msg.Append(string.Format("<br> - Length must be at least {0} characters.", passwordLength));
            msg.Append(string.Format("<br> - Length must be at most {0} characters.", maxPasswordLength));

            if (complexity.HasValue)
            {
                msg.Append(string.Format("<br> - Passwords must contain characters from {0} of the following four categories:", complexity.Value == PasswordComplexity.Medium ? "two" : "three"));
                msg.Append("<ul><li>Uppercase characters of European languages (A through Z).</li>");
                msg.Append("<li>Lowercase characters of European languages (a through z).</li>");
                msg.Append("<li>Base 10 digits (0 through 9).</li>");
                msg.Append("<li>Nonalphanumeric characters:  ~,!,@,#,$,%,^,&,*,?,_,~,-,£,(,).</li></ul>");
            }

            return new PasswordValidationInfo()
            {
                RequirementsMessage = msg.ToString(),
                RequiredPassword = !configManager.ShouldSendEmailOnNewUser()
            };
        }
    }
}