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

        public Dictionary<Guid, ServiceRecurringChargingDefinitionItem> RecurringChargingDefinitionItems { get; set; }

        public Dictionary<Guid, ServiceSetupChargingDefinitionItem> SetupChargingDefinitionItems { get; set; }
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

    public abstract class ServiceTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }
    }

    public class ServiceRecurringChargingDefinitionItem
    {
        public Guid ItemId { get; set; }

        public string Title { get; set; }

        public AccountServiceCondition ChargeCondition { get; set; }
    }

    public class ServiceSetupChargingDefinitionItem
    {
        public Guid ItemId { get; set; }

        public string Title { get; set; }

        public AccountServiceCondition ChargeCondition { get; set; }
    }
}
