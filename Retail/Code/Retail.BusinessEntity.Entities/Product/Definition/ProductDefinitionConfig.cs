using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class ProductDefinitionConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Retail_BE_ProductDefinition";

        public string DefinitionEditor { get; set; }
    }

    public class ProductDefinitionInfo
    {
        public Guid ProductDefinitionId { get; set; }
        public string Name { get; set; }
        public string RuntimeEditor { get; set; }
    }
}
