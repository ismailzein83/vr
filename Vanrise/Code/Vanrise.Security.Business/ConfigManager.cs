using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Security.Entities;
using Vanrise.Common;

namespace Vanrise.Security.Business
{
    public class ConfigManager
    {
        #region Public Methods
        public Guid GetNewUserId()
        {
            MailMessageTemplateSettings mailMessageTemplateSettings = GetMailMessageTemplateSettings();

            Guid newUserId = mailMessageTemplateSettings.NewUserId;

            if (newUserId == default(Guid))
                throw new NullReferenceException("settingManager.MailMessageTemplateSettings.NewUserId NullReferenceException");

            return newUserId;
        }
        public Guid GetResetPasswordId()
        {
            MailMessageTemplateSettings mailMessageTemplateSettings = GetMailMessageTemplateSettings();

            Guid resetPasswordId = mailMessageTemplateSettings.ResetPasswordId;

            if (resetPasswordId == default(Guid))
                throw new NullReferenceException("settingManager.MailMessageTemplateSettings.resetPasswordId NullReferenceException");

            return resetPasswordId;
        }
        public Guid GetForgotPasswordId()
        {
            MailMessageTemplateSettings mailMessageTemplateSettings = GetMailMessageTemplateSettings();

            Guid forgotPasswordId = mailMessageTemplateSettings.ForgotPasswordId;

            if (forgotPasswordId == default(Guid))
                throw new NullReferenceException("settingManager.MailMessageTemplateSettings.forgotPasswordId NullReferenceException");

            return forgotPasswordId;
        }

        public bool ShouldSendEmailOnNewUser()
        {
            return GetSecuritySettings().SendEmailNewUser;
        }


        public int GetPasswordLength()
        {
            return GetPasswordSettings().PasswordLength;
        }

        public int GetMaxPasswordLength()
        {
            return GetPasswordSettings().MaxPasswordLength;
        }

        public int GetMaxRecordsCount()
        {
            return GetPasswordSettings().UserPasswordHistoryCount;
        }
        public int GetMaxFailedTries()
        {
            return GetPasswordSettings().MaximumUserLoginTries;
        }
        public int GetLockForMinutes()
        {
            return GetPasswordSettings().MinutesToLock;
        }
        public TimeSpan? GetFailedInterval()
        {
            return GetPasswordSettings().FailedInterval;
        }
        public Guid? GetNotificationMailTemplateId()
        {
            return GetPasswordSettings().NotificationMailTemplateId;
        }
        public bool GetExactExceptionMessage()
        {
            return GetAPISettings().ExactExceptionMessage;
        }

        public PasswordComplexity? GetPasswordComplexity()
        {
            return GetPasswordSettings().PasswordComplexity;
        }

        public bool ShouldSendEmailOnResetPasswordByAdmin()
        {
            return GetSecuritySettings().SendEmailOnResetPasswordByAdmin;
        }

        public int GetSessionExpirationInMinutes()
        {
            return GetSecuritySettings().SessionExpirationInMinutes;
        }

        #endregion


        #region Private Methods
        private MailMessageTemplateSettings GetMailMessageTemplateSettings()
        {
            MailMessageTemplateSettings mailMessageTemplateSettings = GetSecuritySettings().MailMessageTemplateSettings;

            mailMessageTemplateSettings.ThrowIfNull("mailMessageTemplateSettings");

            return mailMessageTemplateSettings;
        }

        private PasswordSettings GetPasswordSettings()
        {
            PasswordSettings passwordSettings = GetSecuritySettings().PasswordSettings;

            passwordSettings.ThrowIfNull("passwordSettings");

            return passwordSettings;
        }

        private APISettings GetAPISettings()
        {
            APISettings apiSettings = GetSecuritySettings().APISettings;

            apiSettings.ThrowIfNull("apiSettings");

            return apiSettings;
        }
        private SecuritySettings GetSecuritySettings()
        {
            SettingManager settingManager = new SettingManager();
            SecuritySettings securitySettings = settingManager.GetSetting<SecuritySettings>(SecuritySettings.SETTING_TYPE);

            securitySettings.ThrowIfNull("securitySettings");

            return securitySettings;
        }
        #endregion
    }
}
