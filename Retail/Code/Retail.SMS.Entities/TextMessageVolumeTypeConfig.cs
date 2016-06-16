using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.SMS.Entities
{
    public class TextMessageVolumeTypeConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Retail_SMS_TextMessageVolume";
        public string Editor { get; set; }
    }
}
