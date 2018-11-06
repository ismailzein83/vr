using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Demo.Module.Entities
{
    public class SettingConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "DP_Demo_Module_BranchSetting";
        public string Editor { get; set; }
    }
}
