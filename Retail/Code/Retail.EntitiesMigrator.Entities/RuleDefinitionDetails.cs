using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.EntitiesMigrator.Entities
{
    public class RuleDefinitionDetails
    {
        public string Name { get; set; }
        public Guid RateDefinitionId { get; set; }
        public Guid TariffDefinitionId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public bool IsInternational { get; set; }
        public int ChargingPolicyId { get; set; }
    }
}
