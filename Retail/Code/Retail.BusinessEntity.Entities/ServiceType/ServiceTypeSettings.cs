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

        public Guid IdentificationRuleDefinitionId { get; set; }

        public ChargingPolicyDefinitionSettings ChargingPolicyDefinitionSettings { get; set; }

        public VolumeDefinitionSettings VolumeDefinitionSettings { get; set; }

        public string AccountServiceEditor { get; set; }

        public string ServiceVolumeEditor { get; set; }

        public List<ServiceTypeStatusSettings> SupportedStatuses { get; set; }

        public List<ServiceTypeActionSettings> SupportedActions { get; set; }

        public Guid InitialStatusId { get; set; }

        public Guid? InitiationActionDefinitionId { get; set; }

        public ServiceTypeExtendedSettings ExtendedSettings { get; set; }
    }

    public class ServiceTypeStatusSettings
    {
        public Guid StatusDefinitionId { get; set; }

        public bool IsChargeable { get; set; }
    }

    public class ServiceTypeActionSettings
    {
        public Guid ActionDefinitionId { get; set; }
    }
}
