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
        public MailMessageTemplateSettings GetMailMessageTemplateSettings()
        {
            MailMessageTemplateSettings mailMessageTemplateSettings = GetSecuritySettings().MailMessageTemplateSettings;

            if (mailMessageTemplateSettings == null)
                throw new NullReferenceException("settingManager.MailMessageTemplateSettings NullReferenceException");

            return mailMessageTemplateSettings;
        }
        #endregion


        #region Private Methods
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
