using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class ConfigManager
    {
        #region Public Methods
        public Guid GetNewUserId()
        {
            MailMessageTemplateSettings mailMessageTemplateSettings = GetMailMessageTemplateSettings();

            Guid newUserId = mailMessageTemplateSettings.NewUserId;

            if (newUserId == null)
                throw new NullReferenceException("settingManager.MailMessageTemplateSettings.NewUserId NullReferenceException");

            return newUserId;
        }
        public Guid GetResetPasswordId()
        {
            MailMessageTemplateSettings mailMessageTemplateSettings = GetMailMessageTemplateSettings();

            Guid resetPasswordId = mailMessageTemplateSettings.ResetPasswordId;

            if (resetPasswordId == null)
                throw new NullReferenceException("settingManager.MailMessageTemplateSettings.resetPasswordId NullReferenceException");

            return resetPasswordId;
        }
        public Guid GetForgotPasswordId()
        {
            MailMessageTemplateSettings mailMessageTemplateSettings = GetMailMessageTemplateSettings();

            Guid forgotPasswordId = mailMessageTemplateSettings.ForgotPasswordId;

            if (forgotPasswordId == null)
                throw new NullReferenceException("settingManager.MailMessageTemplateSettings.forgotPasswordId NullReferenceException");

            return forgotPasswordId;
        }

        public bool ShouldSendEmailOnNewUser()
        {
            return GetSecuritySettings().SendEmailNewUser;
        }

        public bool ShouldSendEmailOnResetPasswordByAdmin()
        {
            return GetSecuritySettings().SendEmailOnResetPasswordByAdmin;
        }

        #endregion


        #region Private Methods
        private MailMessageTemplateSettings GetMailMessageTemplateSettings()
        {
            MailMessageTemplateSettings mailMessageTemplateSettings = GetSecuritySettings().MailMessageTemplateSettings;

            if (mailMessageTemplateSettings == null)
                throw new NullReferenceException("settingManager.MailMessageTemplateSettings NullReferenceException");

            return mailMessageTemplateSettings;
        }
        private SecuritySettings GetSecuritySettings()
        {
            SettingManager settingManager = new SettingManager();
            SecuritySettings securitySettings = settingManager.GetSetting<SecuritySettings>(SecuritySettings.SETTING_TYPE);

            if (securitySettings == null)
                throw new NullReferenceException("settingManager NullReferenceException");

            return securitySettings;
        }
        #endregion
    }
}
