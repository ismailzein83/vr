using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ServiceType
    {
        public int ServiceTypeId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public ServiceTypeSettings Settings { get; set; }
    }

    public class ServiceTypeSettings
    {
        public string Description { get; set; }

        public string ChargingPolicyEditor { get; set; }

        public ServiceTypeChargingPolicySettings ChargingPolicySettings { get; set; }

        public string AccountServiceEditor { get; set; }

        public string ServiceVolumeEditor { get; set; }
    }



    public abstract class ServiceTypeChargingPolicySettings
    {
        public List<ServiceTypeChargingPolicyPartSettings> PartSettings { get; set; }
    }

    public class ServiceTypeChargingPolicyPartSettings
    {
        public ChargingPolicyPartType PartType { get; set; }

        public int PartConfigId { get; set; }
    }
}
