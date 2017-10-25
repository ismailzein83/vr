using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class CompanyDefinitionConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Common_CompanyDefinition";
        public string Editor { get; set; }
    }
}
