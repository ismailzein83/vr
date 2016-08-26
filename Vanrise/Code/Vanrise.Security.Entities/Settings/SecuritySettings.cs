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
    }
}
