using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.DataParser.Entities.HexTLV
{
    public class CompositeFieldsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_DataParser_CompositeFieldsParser";
        public string Editor { get; set; }
    }
}
