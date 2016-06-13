using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IChargingPolicyDataManager : IDataManager
    {
        IEnumerable<ChargingPolicy> GetChargingPolicies();

        bool Insert(ChargingPolicy chargingPolicy, out int insertedId);

        bool Update(ChargingPolicyToEdit chargingPolicy);

        bool AreChargingPoliciesUpdated(ref object updateHandle);
    }
}
