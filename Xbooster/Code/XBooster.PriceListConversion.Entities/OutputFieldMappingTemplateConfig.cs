using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace XBooster.PriceListConversion.Entities
{
    public class OutputFieldMappingTemplateConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "XBooster_PriceListConversion_OutputFieldMapping";
        public string Editor { get; set; }
    }
}
