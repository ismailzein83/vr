using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.DataParser.Entities
{
    public class ParserTypeExtendedSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_DataParser_ParserTypeExtendedSettings";
        public string Editor { get; set; }
    }
}
