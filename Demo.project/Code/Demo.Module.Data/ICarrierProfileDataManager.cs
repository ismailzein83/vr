using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace Demo.Module.Data
{
    public interface ICarrierProfileDataManager:IDataManager
    {
        List<CarrierProfile> GetCarrierProfiles();
        bool Insert(CarrierProfile carrierProfile, out int carrierProfileId);
        bool Update(CarrierProfile carrierProfile);
        bool AreCarrierProfilesUpdated(ref object updateHandle);
    }
}
