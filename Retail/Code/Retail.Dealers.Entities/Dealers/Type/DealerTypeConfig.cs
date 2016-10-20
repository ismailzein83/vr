using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.Dealers.Entities
{
    public class DealerTypeConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Retail_Dealers_DealerTypeConfig";
        public string Editor { get; set; }
    }
}
