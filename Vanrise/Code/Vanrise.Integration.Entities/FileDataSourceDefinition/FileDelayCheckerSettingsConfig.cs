using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{
    public class FileDelayCheckerSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Integration_FileDelayCheckerSettingsConfig";
        public string Editor { get; set; }
    }
}
