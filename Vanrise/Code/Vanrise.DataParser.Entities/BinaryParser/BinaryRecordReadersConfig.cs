using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.DataParser.Entities
{
    public class BinaryRecordReadersConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_DataParser_BinaryRecordReaders";
        public string Editor { get; set; }
    }
}
