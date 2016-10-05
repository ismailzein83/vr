using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierProfileManager : IBusinessEntityManager
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

                 (input.Query.CountriesIds == null || input.Query.CountriesIds.Count == 0 || (prod.Settings.CountryId.HasValue && input.Query.CountriesIds.Contains(prod.Settings.CountryId.Value)))
                 &&
                 (input.Query.CarrierProfileIds == null || input.Query.CarrierProfileIds.Contains(prod.CarrierProfileId));


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCarrierProfiles.ToBigResult(input, filterExpression, CarrierProfileDetailMapper));
        }
        public CarrierProfile GetCarrierProfile(int carrierProfileId)
        {
            var carrierProfiles = GetCachedCarrierProfiles();
            return carrierProfiles.GetRecord(carrierProfileId);
        }
        public string GetCarrierProfileName(int carrierProfileId)
        {
            CarrierProfile carrierProfile = GetCarrierProfile(carrierProfileId);
            return carrierProfile != null ? carrierProfile.Name : null;
        }
        public IEnumerable<CarrierProfileInfo> GetCarrierProfilesInfo()
        {
            return GetCachedCarrierProfiles().MapRecords(CarrierProfileInfoMapper).OrderBy(x => x.Name);
        }
        public TOne.Entities.InsertOperationOutput<CarrierProfileDetail> AddCarrierProfile(CarrierProfile carrierProfile)
        {
            ValidateCarrierProfileToAdd(carrierProfile);

            TOne.Entities.InsertOperationOutput<CarrierProfileDetail> insertOperationOutput = new TOne.Entities.InsertOperationOutput<CarrierProfileDetail>();

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
        public TOne.Entities.UpdateOperationOutput<CarrierProfileDetail> UpdateCarrierProfile(CarrierProfileToEdit carrierProfile)
        {
            ValidateCarrierProfileToEdit(carrierProfile);

            ICarrierProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierProfileDataManager>();

            bool updateActionSucc = dataManager.Update(carrierProfile);
            TOne.Entities.UpdateOperationOutput<CarrierProfileDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CarrierProfileDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CarrierProfileDetailMapper(this.GetCarrierProfile(carrierProfile.CarrierProfileId));
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }
        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetCarrierProfileName(Convert.ToInt32(context.EntityId));
        }
        public IEnumerable<CarrierProfile> GetCarrierProfiles()
        {
            return GetCachedCarrierProfiles().Values;
        }
        #endregion

        #region Validation Methods

        void ValidateCarrierProfileToAdd(CarrierProfile carrierProfile)
        {
            ValidateCarrierProfile(carrierProfile.Name, carrierProfile.Settings);
        }

        void ValidateCarrierProfileToEdit(CarrierProfileToEdit carrierProfile)
        {
            ValidateCarrierProfile(carrierProfile.Name, carrierProfile.Settings);
        }

        void ValidateCarrierProfile(string cpName, CarrierProfileSettings cpSettings)
        {
            if (String.IsNullOrWhiteSpace(cpName))
                throw new MissingArgumentValidationException("CarrierProfile.Name");

            if (cpSettings == null)
                throw new MissingArgumentValidationException("CarrierProfile.Settings");

            if (String.IsNullOrWhiteSpace(cpSettings.Company))
                throw new MissingArgumentValidationException("CarrierProfile.Settings.Company");
        }

        #endregion

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

            CountryManager countryManager = new CountryManager();
            if (carrierProfile.Settings != null && carrierProfile.Settings.CountryId.HasValue)
                carrierProfileDetail.CountryName = countryManager.GetCountryName(carrierProfile.Settings.CountryId.Value);

            return carrierProfileDetail;
        }
        #endregion

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetCarrierProfile(context.EntityId);
        }
        
        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var allCarrierProfiles = GetCachedCarrierProfiles();
            if (allCarrierProfiles == null)
                return null;
            else
                return allCarrierProfiles.Values.Select(itm => itm as dynamic).ToList();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }


        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }


        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }
    }
}
