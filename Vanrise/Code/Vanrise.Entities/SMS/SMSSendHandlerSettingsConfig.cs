using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities.SMS
{
    public class SMSSendHandlerSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "SMSSendHandlerSettings";

        public string Editor { get; set; }
    }
}
