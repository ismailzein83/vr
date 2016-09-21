using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class SourceCountryReaderConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VRCommon_SourceCountryReader";
        public string Editor { get; set; }
    }
}
