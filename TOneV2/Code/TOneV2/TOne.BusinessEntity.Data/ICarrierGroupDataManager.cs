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
        List<CarrierGroup> GetEntities();
        List<CarrierAccount> GetCarriersByGroup(string groupId);
        bool AddCarrierGroup(Entities.CarrierGroup carrierGroup, out int insertedId);
        bool UpdateCarrierGroup(Entities.CarrierGroup carrierGroup);
        CarrierGroup GetCarrierGroup(int carrierGroupId);
    }
}
