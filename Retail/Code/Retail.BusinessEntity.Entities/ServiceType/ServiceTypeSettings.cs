using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ServiceTypeSettings
    {
        public string Description { get; set; }

        public int IdentificationRuleDefinitionId { get; set; }

        public ChargingPolicyDefinitionSettings ChargingPolicyDefinitionSettings { get; set; }

        public string AccountServiceEditor { get; set; }

        public string ServiceVolumeEditor { get; set; }
    }
}
