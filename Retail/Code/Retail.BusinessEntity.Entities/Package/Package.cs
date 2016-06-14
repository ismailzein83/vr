using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class Package
    {
        public int PackageId { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public PackageSettings Settings { get; set; }
    }    
    public class PackageSettings
    {
        public List<ServicePackageItem> Services { get; set; }

    }

    public abstract class ServicePackageItem
    {
        public int ConfigId { get; set; }
        public int ServiceTypeId { get; set; }
      
    }

    public class ServicePackageChargingPolicyItem : ServicePackageItem
    {
        public ChargingPolicySettings ChargingPolicySettings { get; set; }
    }

    public class ServicePackageVolumeItem : ServicePackageItem
    {
        public VolumeSettings VolumeSettings { get; set; }
    }

    public class PackageService
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; }

        public PackageServiceSettings Settings { get; set; }
    }

    public abstract class PackageServiceSettings
    {
        public int ConfigId { get; set; }
    }
}
