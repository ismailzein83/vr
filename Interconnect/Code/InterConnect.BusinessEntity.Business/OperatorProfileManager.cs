using InterConnect.BusinessEntity.Data;
using InterConnect.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
namespace InterConnect.BusinessEntity.Business
{
    public class OperatorProfileManager
    {
        public List<OperatorProfileExtendedSettingType> GetExtendedSettingTypes()
        {
            return new List<OperatorProfileExtendedSettingType> 
            {
                new OperatorProfileExtendedSettingType
                {
                    RecordTypeId = 0
                }
            };
        }

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<OperatorProfileDetail> GetFilteredOperatorProfiles(Vanrise.Entities.DataRetrievalInput<OperatorProfileQuery> input)
        {
            var allOperatorProfiles = GetCachedOperatorProfiles();

            Func<OperatorProfile, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower())) &&

                  (input.Query.Company == null || prod.Settings.Company.ToLower().Contains(input.Query.Company.ToLower()))
                 &&

                 (input.Query.CountriesIds == null || input.Query.CountriesIds.Count == 0 || input.Query.CountriesIds.Contains(prod.Settings.CountryId));


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allOperatorProfiles.ToBigResult(input, filterExpression, OperatorProfileDetailMapper));
        }
        public OperatorProfile GetOperatorProfile(int operatorProfileId)
        {
            var operatorProfiles = GetCachedOperatorProfiles();
            return operatorProfiles.GetRecord(operatorProfileId);
        }
        public string GetOperatorProfileName(int operatorProfileId)
        {
            OperatorProfile operatorProfile = GetOperatorProfile(operatorProfileId);
            return operatorProfile != null ? operatorProfile.Name : null;
        }
        public IEnumerable<OperatorProfileInfo> GetOperatorProfilsInfo()
        {
            var operatorProfiles = GetCachedOperatorProfiles();
            return operatorProfiles.MapRecords(OperatorProfileInfoMapper);
        }
        public InsertOperationOutput<OperatorProfileDetail> AddOperatorProfile(OperatorProfile operatorProfile)
        {
            InsertOperationOutput<OperatorProfileDetail> insertOperationOutput = new InsertOperationOutput<OperatorProfileDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int operatorProfileId = -1;

            IOperatorProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<IOperatorProfileDataManager>();
            bool insertActionSucc = dataManager.Insert(operatorProfile, out operatorProfileId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                operatorProfile.OperatorProfileId = operatorProfileId;
                insertOperationOutput.InsertedObject = OperatorProfileDetailMapper(operatorProfile);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }
        public UpdateOperationOutput<OperatorProfileDetail> UpdateOperatorProfile(OperatorProfile operatorProfile)
        {
            IOperatorProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<IOperatorProfileDataManager>();

            bool updateActionSucc = dataManager.Update(operatorProfile);
            UpdateOperationOutput<OperatorProfileDetail> updateOperationOutput = new UpdateOperationOutput<OperatorProfileDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = OperatorProfileDetailMapper(operatorProfile);
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }
        #endregion

        #region Private Members
        private Dictionary<int, OperatorProfile> GetCachedOperatorProfiles()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetOperatorProfiles",
               () =>
               {
                   IOperatorProfileDataManager dataManager = BEDataManagerFactory.GetDataManager<IOperatorProfileDataManager>();
                   IEnumerable<OperatorProfile> carrierProfiles = dataManager.GetOperatorProfiles();
                   return carrierProfiles.ToDictionary(cn => cn.OperatorProfileId, cn => cn);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IOperatorProfileDataManager _dataManager = BEDataManagerFactory.GetDataManager<IOperatorProfileDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreOperatorProfilesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region  Mappers
        private OperatorProfileInfo OperatorProfileInfoMapper(OperatorProfile operatorProfile)
        {
            return new OperatorProfileInfo()
            {
                OperatorProfileId = operatorProfile.OperatorProfileId,
                Name = operatorProfile.Name,
            };
        }
        private OperatorProfileDetail OperatorProfileDetailMapper(OperatorProfile operatorProfile)
        {
            OperatorProfileDetail operatorProfileDetail = new OperatorProfileDetail();
            operatorProfileDetail.Entity = operatorProfile;

            return operatorProfileDetail;
        }
        #endregion
    }
}
