using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class SellingRuleSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_Sales_SellingRuleSettingsType";
        public string Editor { get; set; }
    }
}
