using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Security.Entities
{
    public class GroupSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Sec_GroupSettings";
        public string Editor { get; set; }
    }
}
