using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Data;
using Demo.Module.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class CarrierProfileManager
    {
      
        #region ctor/Local Variables
        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<CarrierProfileDetail> GetFilteredCarrierProfiles(Vanrise.Entities.DataRetrievalInput<CarrierProfileQuery> input)
        {
            var allCarrierProfiles = GetCachedCarrierProfiles();

            Func<CarrierProfile, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&

                  (input.Query.Company == null || prod.Settings.Company.ToLower().Contains(input.Query.Company.ToLower()))
                 &&

                 (input.Query.CountriesIds == null || input.Query.CountriesIds.Count == 0 || input.Query.CountriesIds.Contains(prod.Settings.CountryId))
                 &&
                 (input.Query.CarrierProfileIds == null || input.Query.CarrierProfileIds.Contains(prod.CarrierProfileId));


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCarrierProfiles.ToBigResult(input, filterExpression, CarrierProfileDetailMapper));
        }
        public CarrierProfile GetCarrierProfile(int carrierProfileId)
        {
            var carrierProfiles = GetCachedCarrierProfiles();
            return carrierProfiles.GetRecord(carrierProfileId);
        }
        public IEnumerable<CarrierProfileInfo> GetCarrierProfilesInfo()
        {
            var carrierProfiles = GetCachedCarrierProfiles();
            return carrierProfiles.MapRecords(CarrierProfileInfoMapper);
        }
        public Vanrise.Entities.InsertOperationOutput<CarrierProfileDetail> AddCarrierProfile(CarrierProfile carrierProfile)
        {
            Vanrise.Entities.InsertOperationOutput<CarrierProfileDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CarrierProfileDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int carrierProfileId = -1;

            ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
            bool insertActionSucc = dataManager.Insert(carrierProfile, out carrierProfileId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                carrierProfile.CarrierProfileId = carrierProfileId;
                insertOperationOutput.InsertedObject = CarrierProfileDetailMapper(carrierProfile);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<CarrierProfileDetail> UpdateCarrierProfile(CarrierProfile carrierProfile)
        {
            ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();

            bool updateActionSucc = dataManager.Update(carrierProfile);
            Vanrise.Entities.UpdateOperationOutput<CarrierProfileDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CarrierProfileDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CarrierProfileDetailMapper(carrierProfile);
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }
        #endregion

        #region Private Members
        private Dictionary<int, CarrierProfile> GetCachedCarrierProfiles()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCarrierProfiles",
               () =>
               {
                   ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();
                   IEnumerable<CarrierProfile> carrierProfiles = dataManager.GetCarrierProfiles();
                   return carrierProfiles.ToDictionary(cn => cn.CarrierProfileId, cn => cn);
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

        #endregion
     
        #region  Mappers
        private CarrierProfileInfo CarrierProfileInfoMapper(CarrierProfile carrierProfile)
        {
            return new CarrierProfileInfo()
            {
                CarrierProfileId = carrierProfile.CarrierProfileId,
                Name = carrierProfile.Name,
            };
        }
        private CarrierProfileDetail CarrierProfileDetailMapper(CarrierProfile carrierProfile)
        {
            CarrierProfileDetail carrierProfileDetail = new CarrierProfileDetail();

            carrierProfileDetail.Entity = carrierProfile;

            CountryManager manager = new CountryManager();
            if (carrierProfile.Settings != null)
            {
                carrierProfileDetail.CountryName = manager.GetCountry(carrierProfile.Settings.CountryId).Name;
            }

            return carrierProfileDetail;
        }
        #endregion
    }

}
