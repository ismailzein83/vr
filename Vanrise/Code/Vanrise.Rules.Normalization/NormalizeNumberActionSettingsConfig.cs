using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Rules.Normalization
{
    public class NormalizeNumberActionSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Rules_NormalizeNumberAction";
        public string Editor { get; set; }
    }
}
