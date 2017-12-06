using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Security.Entities
{
    public class SecuritySettings : SettingData
    {
        public const string SETTING_TYPE = "VR_Sec_Settings";

        public MailMessageTemplateSettings MailMessageTemplateSettings { get; set; }

        PasswordSettings _passwordSettings;
        public PasswordSettings PasswordSettings
        {
            get
            {
                if (_passwordSettings == null)
                {
                    _passwordSettings = new PasswordSettings
                    {
                        PasswordLength = 6,
                        MaxPasswordLength = 8

                    };
                }
                return _passwordSettings;
            }
            set
            {
                _passwordSettings = value;
            }
        }

        public bool SendEmailNewUser { get; set; }

        public bool SendEmailOnResetPasswordByAdmin { get; set; }


    }

}
