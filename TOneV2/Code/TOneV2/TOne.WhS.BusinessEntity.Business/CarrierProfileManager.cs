using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierProfileManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CarrierProfile> GetFilteredCarrierProfiles(Vanrise.Entities.DataRetrievalInput<CarrierProfileQuery> input)
        {
              var allCarrierProfiles = GetCachedCarrierProfiles();

            Func<CarrierProfile, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.CarrierProfileIds == null || input.Query.CarrierProfileIds.Contains(prod.CarrierProfileId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCarrierProfiles.ToBigResult(input, filterExpression));
        }


        public CarrierProfile GetCarrierProfile(int carrierProfileId)
        {
            List<CarrierProfile> carrierProfiles = GetCachedCarrierProfiles();
            return carrierProfiles.FindRecord(x => x.CarrierProfileId == carrierProfileId);
        }
        public IEnumerable<CarrierProfileInfo> GetCarrierProfilesInfo()
        {
            List<CarrierProfile> carrierProfiles = GetCachedCarrierProfiles();
            return carrierProfiles.MapRecords(CarrierProfileInfoMapper);
        }
        
        public TOne.Entities.InsertOperationOutput<CarrierProfile> AddCarrierProfile(CarrierProfile carrierProfile)
        {
            TOne.Entities.InsertOperationOutput<CarrierProfile> insertOperationOutput = new TOne.Entities.InsertOperationOutput<CarrierProfile>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int carrierProfileId = -1;

            ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
            bool insertActionSucc = dataManager.Insert(carrierProfile, out carrierProfileId);
            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                carrierProfile.CarrierProfileId = carrierProfileId;
                insertOperationOutput.InsertedObject = carrierProfile;
            }

            return insertOperationOutput;
        }

        public TOne.Entities.UpdateOperationOutput<CarrierProfile> UpdateCarrierProfile(CarrierProfile carrierProfile)
        {
            ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();

            bool updateActionSucc = dataManager.Update(carrierProfile);
            TOne.Entities.UpdateOperationOutput<CarrierProfile> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CarrierProfile>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = carrierProfile;
            }

            return updateOperationOutput;
        }


          #region Private Members

        List<CarrierProfile> GetCachedCarrierProfiles()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCarrierProfiles",
               () =>
               {
                   ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
                   return dataManager.GetCarrierProfiles();
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICarrierProfileDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCarrierProfilesUpdated(ref _updateHandle);
            }
        }

        private CarrierProfileInfo CarrierProfileInfoMapper(CarrierProfile carrierProfile)
        {
            return new CarrierProfileInfo()
            {
                CarrierProfileId = carrierProfile.CarrierProfileId,
                Name = carrierProfile.Name,
            };
        }

        #endregion
    }
    
}
