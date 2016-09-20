using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace CDRComparison.Entities
{
    public class FileReaderConfigType : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "CDRComparison_FileReader";
        public string Editor { get; set; }
    }
}
