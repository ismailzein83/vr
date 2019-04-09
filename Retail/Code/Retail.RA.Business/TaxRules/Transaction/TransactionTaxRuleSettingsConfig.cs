using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.RA.Business
{
    public class TransactionTaxRuleSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "RA_Rules_TransactionTaxRuleSettings";
        public string Editor { get; set; }
    }
}
