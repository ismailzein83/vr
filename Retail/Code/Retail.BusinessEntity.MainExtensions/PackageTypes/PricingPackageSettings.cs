using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{
    public class PricingPackageSettings : PackageExtendedSettings, IPackageAccountCharging, IPackageServiceUsageChargingPolicy, IPackageServiceRecurringCharging
    {
        public override Guid ConfigId
        {
            get { return new Guid("B78610BA-4CA2-4E60-8143-73CEF6E99D14"); }
        }

        public int? AccountRecurringChargingId { get; set; }

        public Dictionary<Guid, ServiceTypeUsageChargingSettings> ServiceTypeUsageChargings { get; set; }

        public Dictionary<Guid, ServiceTypeRecurringChargingSettings> ServiceTypeRecurringChargings { get; set; }

        bool IPackageAccountCharging.TryGetAccountRecurringChargingId(IPackageAccountChargingContext context)
        {
            if (this.AccountRecurringChargingId.HasValue)
            {
                context.RecurringChargingId = this.AccountRecurringChargingId.Value;
                return true;
            }
            else
                return false;
        }

        bool IPackageServiceUsageChargingPolicy.TryGetServiceUsageChargingPolicyId(IPackageServiceUsageChargingPolicyContext context)
        {
            ServiceTypeUsageChargingSettings serviceUsageChargingSettings;
            if (this.ServiceTypeUsageChargings != null && this.ServiceTypeUsageChargings.TryGetValue(context.ServiceTypeId, out serviceUsageChargingSettings))
            {
                context.ChargingPolicyId = serviceUsageChargingSettings.UsageChargingId;
                return true;
            }
            else
                return false;
        }

        bool IPackageServiceRecurringCharging.TryGetServiceRecurringChargingId(IPackageServiceRecurringChargingContext context)
        {
            ServiceTypeRecurringChargingSettings serviceRecurringChargingSettings;
            if (this.ServiceTypeRecurringChargings != null && this.ServiceTypeRecurringChargings.TryGetValue(context.ServiceTypeId, out serviceRecurringChargingSettings))
            {
                context.RecurringChargingId = serviceRecurringChargingSettings.RecurringChargingId;
                return true;
            }
            else
                return false;
        }
    }

    public class ServiceTypeUsageChargingSettings
    {
        public int UsageChargingId { get; set; }
    }


    public class ServiceTypeRecurringChargingSettings
    {
        public int RecurringChargingId { get; set; }
    }
}
