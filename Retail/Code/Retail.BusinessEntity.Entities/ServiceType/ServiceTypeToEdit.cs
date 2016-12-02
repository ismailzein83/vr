using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ServiceTypeToEdit
    {
        public Guid ServiceTypeId { get; set; }
        public Guid InitialStatusId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid IdentificationRuleDefinitionId { get; set; }
        public ChargingPolicyDefinitionSettings ChargingPolicyDefinitionSettings { get; set; }
        public ServiceTypeExtendedSettings ExtendedSettings { get; set; }
    }
}
