using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.NumberingPlan.Data;
using Vanrise.NumberingPlan.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class SellingNumberPlanManager :  ISellingNumberPlanManager
    {
        #region Public Methods

        public SellingNumberPlan GetSellingNumberPlan(int numberPlanId)
        {
            return GetCachedSellingNumberPlans().GetRecord(numberPlanId);
        }

        public IEnumerable<SellingNumberPlanInfo> GetSellingNumberPlans()
        {
            return GetCachedSellingNumberPlans().Values.MapRecords(SellingNumberPlanInfoMapper).OrderBy(x => x.Name);
        }
       
        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public string GetSellingNumberPlanName(int sellingNumberPlanId)
        {
            SellingNumberPlan sellingNumberPlan = GetSellingNumberPlan(sellingNumberPlanId);

            if (sellingNumberPlan != null)
                return sellingNumberPlan.Name;

            return null;
        }

        public SellingNumberPlan GetMasterSellingNumberPlan()
        {
            var allNumberPlans = GetCachedSellingNumberPlans();
            if (allNumberPlans != null)
                return allNumberPlans.Values.FirstOrDefault();
            else
                return null;
        }
        public IDataRetrievalResult<SellingNumberPlanDetail> GetFilteredSellingNumberPlans(DataRetrievalInput<SellingNumberPlanQuery> input)
        {
            var allSellingNumberPlans = GetCachedSellingNumberPlans();
            Func<SellingNumberPlan, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSellingNumberPlans.ToBigResult(input, filterExpression, SellingNumberPlanDetailMapper));

        }

        public InsertOperationOutput<SellingNumberPlanDetail> AddSellingNumberPlan(SellingNumberPlan sellingNumberPlan)
        {
            ValidateSellingNumberPlanToAdd(sellingNumberPlan);

            InsertOperationOutput<SellingNumberPlanDetail> insertOperationOutput = new InsertOperationOutput<SellingNumberPlanDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int sellingNumberPlanId = -1;

            ISellingNumberPlanDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();
            bool insertActionSucc = dataManager.Insert(sellingNumberPlan, out sellingNumberPlanId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                sellingNumberPlan.SellingNumberPlanId = sellingNumberPlanId;
                insertOperationOutput.InsertedObject = SellingNumberPlanDetailMapper(sellingNumberPlan);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<SellingNumberPlanDetail> UpdateSellingNumberPlan(SellingNumberPlanToEdit sellingNumberPlan)
        {
            ValidateSellingNumberPlanToEdit(sellingNumberPlan);

            ISellingNumberPlanDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();

            bool updateActionSucc = dataManager.Update(sellingNumberPlan);
            UpdateOperationOutput<SellingNumberPlanDetail> updateOperationOutput = new UpdateOperationOutput<SellingNumberPlanDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SellingNumberPlanDetailMapper(this.GetSellingNumberPlan(sellingNumberPlan.SellingNumberPlanId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISellingNumberPlanDataManager _dataManager = CodePrepDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSellingNumberPlansUpdated(ref _updateHandle);
            }
        }

        #endregion


        #region Mappers

        private SellingNumberPlanInfo SellingNumberPlanInfoMapper(SellingNumberPlan sellingNumberPlan)
        {
            return new SellingNumberPlanInfo
            {
                Name = sellingNumberPlan.Name,
                SellingNumberPlanId = sellingNumberPlan.SellingNumberPlanId
            };
        }
        public SellingNumberPlanDetail SellingNumberPlanDetailMapper(SellingNumberPlan sellingNumberPlan)
        {
            SellingNumberPlanDetail sellingNumberPlanDetail = new SellingNumberPlanDetail()
            {
                Entity = sellingNumberPlan
            };
            return sellingNumberPlanDetail;
        }
        #endregion

        #region Private Methods

        Dictionary<int, SellingNumberPlan> GetCachedSellingNumberPlans()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSellingNumberPlans",
               () =>
               {
                   ISellingNumberPlanDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();
                   return dataManager.GetSellingNumberPlans().ToDictionary(x => x.SellingNumberPlanId, x => x);
               });

        }

        #endregion

        #region Validation Methods

        void ValidateSellingNumberPlanToAdd(SellingNumberPlan sellingNumberPlan)
        {
            ValidateSellingNumberPlan(sellingNumberPlan.Name);
        }

        void ValidateSellingNumberPlanToEdit(SellingNumberPlanToEdit sellingNumberPlan)
        {
            ValidateSellingNumberPlan(sellingNumberPlan.Name);
        }

        void ValidateSellingNumberPlan(string snpName)
        {
            if (String.IsNullOrWhiteSpace(snpName))
                throw new MissingArgumentValidationException("SellingNumberPlan.Name");
        }

        #endregion
       
    }
}
