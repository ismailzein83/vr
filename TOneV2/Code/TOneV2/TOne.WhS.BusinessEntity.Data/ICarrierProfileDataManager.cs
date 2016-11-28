    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ICarrierProfileDataManager:IDataManager
    {
        List<CarrierProfile> GetCarrierProfiles();
        bool Insert(CarrierProfile carrierProfile, out int carrierProfileId);
        bool Update(CarrierProfileToEdit carrierProfile);
        bool AreCarrierProfilesUpdated(ref object updateHandle);
        bool UpdateExtendedSettings(int carrierProfileId, Dictionary<string, Object> extendedSettings);
    }
}
