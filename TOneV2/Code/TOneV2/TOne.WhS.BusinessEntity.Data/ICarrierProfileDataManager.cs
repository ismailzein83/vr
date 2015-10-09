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
        Vanrise.Entities.BigResult<CarrierProfile> GetFilteredCarrierProfiles(Vanrise.Entities.DataRetrievalInput<CarrierProfileQuery> input);
        CarrierProfile GetCarrierProfile(int carrierProfileId);
        List<CarrierProfileInfo> GetAllCarrierProfiles();

    }
}
