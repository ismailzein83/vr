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

        SecurityProviderConfigurationSettings _securityProviderSettings;
        public SecurityProviderConfigurationSettings SecurityProviderSettings
        {
            get
            {
                if (_securityProviderSettings == null)
                {
                    _securityProviderSettings = new SecurityProviderConfigurationSettings
                    {
                        DefaultSecurityProviderId = new Guid("9554069b-795e-4bb1-bff3-9af0f47fc0ff")
                    };
                }
                return _securityProviderSettings;
            }
            set
            {
                _securityProviderSettings = value;
            }
        }

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

        APISettings _apiSettings;
        public APISettings APISettings
        {
            get
            {
                if (_apiSettings == null)
                {
                    _apiSettings = new APISettings
                    {
                        ExactExceptionMessage = false

                    };
                }
                return _apiSettings;
            }
            set
            {
                _apiSettings = value;
            }
        }

        public bool SendEmailNewUser { get; set; }

        public bool SendEmailOnResetPasswordByAdmin { get; set; }

        int _sessionExpirationInMinutes = 30;
        public int SessionExpirationInMinutes
        {
            get
            {
                return _sessionExpirationInMinutes;
            }
            set
            {
                _sessionExpirationInMinutes = value;
            }
        }
    }

}
