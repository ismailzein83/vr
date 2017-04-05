using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.DataParser.Entities
{
    public class HexTLVTagValueParserConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_DataParser_HexTLVTagValueParser";
        public string Editor { get; set; }
    }
}
