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
    public class NationalNumberingPlanManager
    {
      
        #region ctor/Local Variables
        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<NationalNumberingPlanDetail> GetFilteredNationalNumberingPlans(Vanrise.Entities.DataRetrievalInput<NationalNumberingPlanQuery> input)
        {
            var allNationalNumberingPlans = GetCachedNationalNumberingPlans();

            Func<NationalNumberingPlan, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&

                  (input.Query.Company == null || prod.Settings.Company.ToLower().Contains(input.Query.Company.ToLower()))
                 &&

                 (input.Query.CountriesIds == null || input.Query.CountriesIds.Count == 0 || input.Query.CountriesIds.Contains(prod.Settings.CountryId))
                 &&
                 (input.Query.NationalNumberingPlanIds == null || input.Query.NationalNumberingPlanIds.Contains(prod.NationalNumberingPlanId));


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allNationalNumberingPlans.ToBigResult(input, filterExpression, NationalNumberingPlanDetailMapper));
        }
        public NationalNumberingPlan GetNationalNumberingPlan(int operatorProfileId)
        {
            var operatorProfiles = GetCachedNationalNumberingPlans();
            return operatorProfiles.GetRecord(operatorProfileId);
        }
        public IEnumerable<NationalNumberingPlanInfo> GetNationalNumberingPlansInfo()
        {
            var operatorProfiles = GetCachedNationalNumberingPlans();
            return operatorProfiles.MapRecords(NationalNumberingPlanInfoMapper);
        }
        public Vanrise.Entities.InsertOperationOutput<NationalNumberingPlanDetail> AddNationalNumberingPlan(NationalNumberingPlan operatorProfile)
        {
            Vanrise.Entities.InsertOperationOutput<NationalNumberingPlanDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<NationalNumberingPlanDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int operatorProfileId = -1;

            INationalNumberingPlanDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<INationalNumberingPlanDataManager>();
            bool insertActionSucc = dataManager.Insert(operatorProfile, out operatorProfileId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                operatorProfile.NationalNumberingPlanId = operatorProfileId;
                insertOperationOutput.InsertedObject = NationalNumberingPlanDetailMapper(operatorProfile);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<NationalNumberingPlanDetail> UpdateNationalNumberingPlan(NationalNumberingPlan operatorProfile)
        {
            INationalNumberingPlanDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<INationalNumberingPlanDataManager>();

            bool updateActionSucc = dataManager.Update(operatorProfile);
            Vanrise.Entities.UpdateOperationOutput<NationalNumberingPlanDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<NationalNumberingPlanDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = NationalNumberingPlanDetailMapper(operatorProfile);
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }
        #endregion

        #region Private Members
        private Dictionary<int, NationalNumberingPlan> GetCachedNationalNumberingPlans()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetNationalNumberingPlans",
               () =>
               {
                   INationalNumberingPlanDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<INationalNumberingPlanDataManager>();
                   IEnumerable<NationalNumberingPlan> operatorProfiles = dataManager.GetNationalNumberingPlans();
                   return operatorProfiles.ToDictionary(cn => cn.NationalNumberingPlanId, cn => cn);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            INationalNumberingPlanDataManager _dataManager = DemoModuleDataManagerFactory.GetDataManager<INationalNumberingPlanDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreNationalNumberingPlansUpdated(ref _updateHandle);
            }
        }

        #endregion
     
        #region  Mappers
        private NationalNumberingPlanInfo NationalNumberingPlanInfoMapper(NationalNumberingPlan operatorProfile)
        {
            return new NationalNumberingPlanInfo()
            {
                NationalNumberingPlanId = operatorProfile.NationalNumberingPlanId,
                Name = operatorProfile.Name,
            };
        }
        private NationalNumberingPlanDetail NationalNumberingPlanDetailMapper(NationalNumberingPlan operatorProfile)
        {
            NationalNumberingPlanDetail operatorProfileDetail = new NationalNumberingPlanDetail();

            operatorProfileDetail.Entity = operatorProfile;

            CountryManager manager = new CountryManager();
            if (operatorProfile.Settings != null)
            {
                operatorProfileDetail.CountryName = manager.GetCountry(operatorProfile.Settings.CountryId).Name;
            }

            return operatorProfileDetail;
        }
        #endregion
    }

}
