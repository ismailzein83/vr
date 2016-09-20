using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace CDRComparison.Entities
{
    public class CDRSourceConfigType : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "CDRComparison_CDRSource";
        public string Editor { get; set; }
    }
}
