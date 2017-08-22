using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class WHSFinancialAccountDefinitionConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_BE_FinancialAccountDefinition";
        public string Editor { get; set; }
    }
}
