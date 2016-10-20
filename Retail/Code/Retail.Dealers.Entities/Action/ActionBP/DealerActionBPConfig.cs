using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.Dealers.Entities
{
    public class DealerActionBPConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Retail_Dealers_DealerActionBP";

        public string DefinitionEditor { get; set; }

        public string RuntimeEditor { get; set; }
    }
}
