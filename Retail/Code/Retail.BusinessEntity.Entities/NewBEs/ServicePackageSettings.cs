using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ServicePackageSettings
    {
        public List<ServicePackageItem> Items { get; set; }
    }

    public abstract class ServicePackageItem
    {
        public string Name { get; set; }

        public int ServiceTypeId { get; set; }
    }

    public class ServicePackageChargingPolicyItem : ServicePackageItem
    {
        public ChargingPolicySettings ChargingPolicySettings { get; set; }        
    }

    public class ServicePackageVolumeItem : ServicePackageItem
    {
        public ServiceVolumeSettings VolumeSettings { get; set; }
    }
}
