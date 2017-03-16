using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ChargingPolicyDetail
    {
        public ChargingPolicy Entity { get; set; }

        public string ServiceTypeName { get; set; }

        public Guid AccountBEDefinitionId { get; set; }
        public IEnumerable<ChargingPolicyRuleDefinition> RuleDefinitions { get; set; }
    }
}
