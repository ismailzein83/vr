using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class QualityConfigurationDefinitionExtendedSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_Routing_QualityConfigurationDefinitionExtendedSettings";

        public string Editor { get; set; }
    }
}