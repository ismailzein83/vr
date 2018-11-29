using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{
    public class FileMissingCheckerSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Integration_FileMissingCheckerSettingsConfig";
        public string Editor { get; set; }
    }
}
