using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public interface IPackageServiceUsageChargingPolicy
    {
        bool TryGetServiceUsageChargingPolicyId(IPackageServiceUsageChargingPolicyContext context);
    }

    public interface IPackageServiceUsageChargingPolicyContext
    {
        Guid ServiceTypeId { get; }
        int ChargingPolicyId { set; }
    }
}
