using System;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class SellingRuleThresholdSettings : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WHS_Sales_Rules_SellingRuleThresholdSetting";
        public string Editor { get; set; }
    }
}
