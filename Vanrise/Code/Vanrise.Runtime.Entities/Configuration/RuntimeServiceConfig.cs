using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Runtime.Entities
{
    public class RuntimeServiceConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Runtime_RuntimeNodeConfiguration_RuntimeService";
        public string Editor { get; set; }
    }
}