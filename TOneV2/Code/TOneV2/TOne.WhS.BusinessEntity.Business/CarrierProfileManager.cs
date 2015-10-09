using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierProfileManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CarrierProfile> GetFilteredCarrierProfiles(Vanrise.Entities.DataRetrievalInput<CarrierProfileQuery> input)
        {
            ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredCarrierProfiles(input));
        }
        public CarrierProfile GetCarrierProfile(int carrierProfileId)
        {
            ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
            return dataManager.GetCarrierProfile(carrierProfileId);
        }
        public List<CarrierProfileInfo> GetAllCarrierProfiles()
        {
            ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
            return dataManager.GetAllCarrierProfiles();
        }
    }
}
