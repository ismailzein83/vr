using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class PackageDefinitionConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Retail_BE_PackageDefinitionConfig";

        public string DefinitionEditor { get; set; }
    }
}
