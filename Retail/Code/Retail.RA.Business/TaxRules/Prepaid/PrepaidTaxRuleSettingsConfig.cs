using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.RA.Business
{
    public class PrepaidTaxRuleSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "RA_Rules_PrepaidTaxRuleSettings";
        public string Editor { get; set; }
    }
}
