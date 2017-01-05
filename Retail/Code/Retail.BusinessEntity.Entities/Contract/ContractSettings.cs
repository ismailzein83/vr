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

        public List<ProductPackageItem> Packages { get; set; }

        public List<AccountRecurringChargeRuleSet> RecurringPricingRuleSets { get; set; }
        
        public ProductExtendedSettings ExtendedSettings { get; set; }
    }
}
