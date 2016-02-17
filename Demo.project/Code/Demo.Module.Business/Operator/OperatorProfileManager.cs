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
    public class OperatorProfileManager
    {

        #region ctor/Local Variables
        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<OperatorProfileDetail> GetFilteredOperatorProfiles(Vanrise.Entities.DataRetrievalInput<OperatorProfileQuery> input)
        {
            var allOperatorProfiles = GetCachedOperatorProfiles();

            Func<OperatorProfile, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&

                 (input.Query.Company == null || prod.Settings.Company.ToLower().Contains(input.Query.Company.ToLower()))
                 &&

                 (input.Query.BillingEmail == null || prod.Settings.Contacts.Where(x => x.Type == OperatorContactType.BillingEmail).Select(x => x.Description.ToLower()).Contains(input.Query.BillingEmail.ToLower()))
                 &&

                 (input.Query.CountriesIds == null || input.Query.CountriesIds.Count == 0 || input.Query.CountriesIds.Contains(prod.Settings.CountryId))
                 &&
                 (input.Query.OperatorProfileIds == null || input.Query.OperatorProfileIds.Contains(prod.OperatorProfileId));


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allOperatorProfiles.ToBigResult(input, filterExpression, OperatorProfileDetailMapper));
        }
        public OperatorProfile GetOperatorProfile(int operatorProfileId)
        {
            var operatorProfiles = GetCachedOperatorProfiles();
            return operatorProfiles.GetRecord(operatorProfileId);
        }
        public IEnumerable<OperatorProfileInfo> GetOperatorProfilesInfo()
        {
            var operatorProfiles = GetCachedOperatorProfiles();
            return operatorProfiles.MapRecords(OperatorProfileInfoMapper);
        }
        public Vanrise.Entities.InsertOperationOutput<OperatorProfileDetail> AddOperatorProfile(OperatorProfile operatorProfile)
        {
            Vanrise.Entities.InsertOperationOutput<OperatorProfileDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<OperatorProfileDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int operatorProfileId = -1;

            IOperatorProfileDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorProfileDataManager>();
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
        public Vanrise.Entities.UpdateOperationOutput<OperatorProfileDetail> UpdateOperatorProfile(OperatorProfile operatorProfile)
        {
            IOperatorProfileDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorProfileDataManager>();

            bool updateActionSucc = dataManager.Update(operatorProfile);
            Vanrise.Entities.UpdateOperationOutput<OperatorProfileDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<OperatorProfileDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
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
                   IOperatorProfileDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorProfileDataManager>();
                   IEnumerable<OperatorProfile> operatorProfiles = dataManager.GetOperatorProfiles();
                   return operatorProfiles.ToDictionary(cn => cn.OperatorProfileId, cn => cn);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IOperatorProfileDataManager _dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorProfileDataManager>();
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

            CountryManager manager = new CountryManager();
            Country country;
            if (operatorProfile.Settings != null)
            {
                country = manager.GetCountry(operatorProfile.Settings.CountryId);
                if (country != null)
                    operatorProfileDetail.CountryName = country.Name;
            }

            return operatorProfileDetail;
        }
        #endregion
    }

}
