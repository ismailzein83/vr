using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class PackageServiceDetail
    {
        public PackageItem Entity { get; set; }

        public string ServiceTypeName { get; set; }

        public IEnumerable<ChargingPolicyRuleDefinition> RuleDefinitions { get; set; }
    }
}
