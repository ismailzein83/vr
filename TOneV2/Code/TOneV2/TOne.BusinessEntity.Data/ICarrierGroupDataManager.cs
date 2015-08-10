using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ICarrierGroupDataManager : IDataManager
    {
        CarrierGroup GetCarrierGroup(int carrierGroupId);
        List<CarrierAccount> GetCarrierGroupMembers(IEnumerable<int> carrierGroupIds);
        bool AddCarrierGroup(Entities.CarrierGroup carrierGroup, string[] CarrierAccountIds, out int insertedId);
        bool UpdateCarrierGroup(Entities.CarrierGroup carrierGroup, string[] CarrierAccountIds);
    }
}
