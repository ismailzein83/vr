using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{
    public class ActionProvisionerConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Integration_Send_ActionProvisioner";

        public string DefinitionEditor { get; set; }

        public string RuntimeEditor { get; set; }
    }
}
