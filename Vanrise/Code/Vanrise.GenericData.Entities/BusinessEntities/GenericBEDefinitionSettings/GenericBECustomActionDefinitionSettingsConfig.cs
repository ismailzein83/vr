using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class GenericBECustomActionDefinitionSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_GenericData_GenericBECustomActionDefinitionSettings";
        public string Editor { get; set; }
    }
}
