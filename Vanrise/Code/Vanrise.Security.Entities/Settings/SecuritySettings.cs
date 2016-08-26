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
        public MailMessageTemplateSettings mailMessageTemplateSettings { get; set; }
    }
}
