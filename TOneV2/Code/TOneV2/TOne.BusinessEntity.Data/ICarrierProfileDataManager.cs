using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ICarrierProfileDataManager : IDataManager
    {
        BigResult<CarrierProfile> GetFilteredCarrierProfiles(Vanrise.Entities.DataRetrievalInput<CarrierProfileQuery> input);
        CarrierProfile GetCarrierProfile(int profileId);
        bool UpdateCarrierProfile(CarrierProfile carrierProfile);
        bool AddCarrierProfile(CarrierProfile carrierProfile, out int insertedId);
    }
}
