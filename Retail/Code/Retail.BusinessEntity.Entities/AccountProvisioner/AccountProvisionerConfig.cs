using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountProvisionerConfig
    {
        public const string EXTENSION_TYPE = "Retail_BE_AccountProvisioner";

        public string DefinitionEditor { get; set; }

        public string RuntimeEditor { get; set; }
    }
}
