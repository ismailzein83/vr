using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{
    public class PricingPackageSettings : PackageExtendedSettings, IPackageFixedChargingPolicy, IPackageUsageChargingPolicy
    {
        public override Guid ConfigId
        {
            get { return new Guid("B78610BA-4CA2-4E60-8143-73CEF6E99D14"); }
        }

        public int? FixedChargingPolicyId { get; set; }

        public Dictionary<Guid, ServiceTypeUsageChargingPolicySettings> ServiceTypeUsageChargingPolicies { get; set; }

        bool IPackageFixedChargingPolicy.TryGetFixedChargingPolicyId(IPackageFixedChargingPolicyContext context)
        {
            if (this.FixedChargingPolicyId.HasValue)
            {
                context.ChargingPolicyId = this.FixedChargingPolicyId.Value;
                return true;
            }
            else
                return false;
        }

        bool IPackageUsageChargingPolicy.TryGetServiceUsageChargingPolicyId(IPackageServiceUsageChargingPolicyContext context)
        {
            ServiceTypeUsageChargingPolicySettings serviceUsageChargingPolicySettings;
            if (this.ServiceTypeUsageChargingPolicies != null && this.ServiceTypeUsageChargingPolicies.TryGetValue(context.ServiceTypeId, out serviceUsageChargingPolicySettings))
            {
                context.ChargingPolicyId = serviceUsageChargingPolicySettings.UsageChargingPolicyId;
                return true;
            }
            else
                return false;
        }
    }

    public class ServiceTypeUsageChargingPolicySettings
    {
        public int UsageChargingPolicyId { get; set; }
    }
}
