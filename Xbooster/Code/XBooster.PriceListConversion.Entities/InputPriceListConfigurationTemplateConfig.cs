using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace XBooster.PriceListConversion.Entities
{
    public class InputPriceListConfigurationTemplateConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "XBooster_PriceListConversion_InputPriceListConfiguration";
        public string Editor { get; set; }
    }
}
