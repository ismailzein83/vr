using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class SellingRuleActionSettings : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WHS_Sales_Rules_SellingRuleActionSetting";
        public string Editor { get; set; }
    }
}