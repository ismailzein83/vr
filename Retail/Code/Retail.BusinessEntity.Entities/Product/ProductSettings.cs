using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ProductSettings
    {
        public Guid ProductDefinitionId { get; set; }

        public int PricingCurrencyId { get; set; }

        public Dictionary<int, ProductPackageItem> Packages { get; set; }

        public List<AccountRecurringChargeRuleSet> RecurringChargeRuleSets { get; set; }
        
        public ProductExtendedSettings ExtendedSettings { get; set; }
    }
}
