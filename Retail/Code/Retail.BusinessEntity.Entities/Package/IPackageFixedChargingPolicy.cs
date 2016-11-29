using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public interface IPackageFixedChargingPolicy
    {
        bool TryGetFixedChargingPolicyId(IPackageFixedChargingPolicyContext context);
    }

    public interface IPackageFixedChargingPolicyContext
    {
        int ChargingPolicyId { set; }
    }
}
