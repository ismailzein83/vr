using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.BEBridge.Entities
{
    public class SourceBeReadersConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_BEBridge_SourceBeReadersDefinition";
        public string Editor { get; set; }
    }
}
