﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierProfileManager
    {

        public Vanrise.Entities.IDataRetrievalResult<CarrierProfileDetail> GetFilteredCarrierProfiles(Vanrise.Entities.DataRetrievalInput<CarrierProfileQuery> input)
        {
            var allCarrierProfiles = GetCachedCarrierProfiles();

            Func<CarrierProfile, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&

                  (input.Query.Company == null || prod.Settings.Company.ToLower().Contains(input.Query.Company.ToLower()))
                 &&

                  (input.Query.BillingEmail == null || prod.Settings.BillingEmail.ToLower().Contains(input.Query.BillingEmail.ToLower()))
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

        public Dictionary<int, CarrierProfile> GetCachedCarrierProfiles()
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
            CountryManager manager = new CountryManager();

            return new CarrierProfileDetail()
            {
                Entity = carrierProfile,
                CountryName = (carrierProfile.Settings != null ? string.Empty : manager.GetCountry(carrierProfile.Settings.CountryId).Name)
            };
        }

        #endregion
    }

}
