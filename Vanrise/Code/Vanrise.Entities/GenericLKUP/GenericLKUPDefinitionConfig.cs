using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class GenericLKUPDefinitionConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VRCommon_GenericLKUPDefinition";

        public string DefinitionEditor { get; set; }
    }
}
