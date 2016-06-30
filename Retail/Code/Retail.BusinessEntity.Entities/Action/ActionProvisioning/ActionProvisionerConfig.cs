using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class ActionProvisionerConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Retail_BE_ActionProvisioner";
        
        public string DefinitionEditor { get; set; }

        public string RuntimeEditor { get; set; }
    }
}
