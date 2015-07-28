using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.BusinessEntity.Business
{
    public class CarrierProfileManager
    {
        ICarrierProfileDataManager _dataManager;
        public CarrierProfileManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
        }
        public Vanrise.Entities.IDataRetrievalResult<CarrierProfile> GetFilteredCarrierProfiles(Vanrise.Entities.DataRetrievalInput<CarrierProfileQuery> input)
        {
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _dataManager.GetFilteredCarrierProfiles(input));
        }
        public CarrierProfile GetCarrierProfile(int profileId)
        {
            return _dataManager.GetCarrierProfile(profileId);
        }
        public TOne.Entities.UpdateOperationOutput<CarrierProfile> UpdateCarrierProfile(CarrierProfile carrierProfile)
        {
            bool updateActionSucc = _dataManager.UpdateCarrierProfile(carrierProfile);
            TOne.Entities.UpdateOperationOutput<CarrierProfile> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CarrierProfile>();

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = carrierProfile;
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            return updateOperationOutput;
        }
    }
}
