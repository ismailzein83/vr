using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SellingNumberPlanManager : IBusinessEntityManager , ISellingNumberPlanManager
    {
        #region Public Methods

        public IEnumerable<SellingNumberPlanInfo> GetSellingNumberPlans()
        {
            return GetCachedSellingNumberPlans().Values.MapRecords(SellingNumberPlanInfoMapper).OrderBy(x => x.Name);
        }

        public SellingNumberPlan GetSellingNumberPlan(int numberPlanId)
        {
            return GetCachedSellingNumberPlans().GetRecord(numberPlanId);
        }

        public IDataRetrievalResult<SellingNumberPlanDetail> GetFilteredSellingNumberPlans(DataRetrievalInput<SellingNumberPlanQuery> input)
        {
            var allSellingNumberPlans = GetCachedSellingNumberPlans();
            Func<SellingNumberPlan, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSellingNumberPlans.ToBigResult(input, filterExpression, SellingNumberPlanDetailMapper));

        }

        public TOne.Entities.InsertOperationOutput<SellingNumberPlanDetail> AddSellingNumberPlan(SellingNumberPlan sellingNumberPlan)
        {
            ValidateSellingNumberPlanToAdd(sellingNumberPlan);

            TOne.Entities.InsertOperationOutput<SellingNumberPlanDetail> insertOperationOutput = new TOne.Entities.InsertOperationOutput<SellingNumberPlanDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int sellingNumberPlanId = -1;

            ISellingNumberPlanDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();
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

        public TOne.Entities.UpdateOperationOutput<SellingNumberPlanDetail> UpdateSellingNumberPlan(SellingNumberPlanToEdit sellingNumberPlan)
        {
            ValidateSellingNumberPlanToEdit(sellingNumberPlan);

            ISellingNumberPlanDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();

            bool updateActionSucc = dataManager.Update(sellingNumberPlan);
            TOne.Entities.UpdateOperationOutput<SellingNumberPlanDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<SellingNumberPlanDetail>();

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

        public string GetDescription(IEnumerable<int> sellingNumberPlanIds)
        {
            List<string> sellingNumberPlanNames = new List<string>();
            foreach (int sellingNumberPlanId in sellingNumberPlanIds)
            {
                SellingNumberPlan sellingNumberPlan = GetSellingNumberPlan(sellingNumberPlanId);
                sellingNumberPlanNames.Add(sellingNumberPlan.Name);
            }

            if (sellingNumberPlanNames != null)
                return string.Join(", ", sellingNumberPlanNames.Select(x => x));

            return string.Empty;
        }
       
        public SellingNumberPlan GetMasterSellingNumberPlan()
        {
            var allNumberPlans = GetCachedSellingNumberPlans();
            if (allNumberPlans != null)
                return allNumberPlans.Values.FirstOrDefault();
            else
                return null;
        }

        #endregion

        #region ISellingNumberPlanManager Memebers
       
        public string GetSellingNumberPlanName(int sellingNumberPlanId)
        {
            SellingNumberPlan sellingNumberPlan = GetSellingNumberPlan(sellingNumberPlanId);

            if (sellingNumberPlan != null)
                return sellingNumberPlan.Name;

            return null;
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

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISellingNumberPlanDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSellingNumberPlansUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, SellingNumberPlan> GetCachedSellingNumberPlans()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSellingNumberPlans",
               () =>
               {
                   ISellingNumberPlanDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();
                   return dataManager.GetSellingNumberPlans().ToDictionary(x => x.SellingNumberPlanId, x => x);
               });

        }

        public SellingNumberPlanDetail SellingNumberPlanDetailMapper(SellingNumberPlan sellingNumberPlan)
        {
            SellingNumberPlanDetail sellingNumberPlanDetail = new SellingNumberPlanDetail()
            {
                Entity = sellingNumberPlan
            };
            return sellingNumberPlanDetail;
        }

        private SellingNumberPlanInfo SellingNumberPlanInfoMapper(SellingNumberPlan sellingNumberPlan)
        {
            return new SellingNumberPlanInfo
            {
                Name = sellingNumberPlan.Name,
                SellingNumberPlanId = sellingNumberPlan.SellingNumberPlanId
            };
        }

        #endregion

        #region IBusinessEntityManager

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var allSellingNumberPlans = GetCachedSellingNumberPlans();
            if (allSellingNumberPlans == null)
                return null;
            else
                return allSellingNumberPlans.Values.Select(itm => itm as dynamic).ToList();
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetSellingNumberPlan(context.EntityId);
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetSellingNumberPlanName(Convert.ToInt32(context.EntityId));
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var sellingNumberPlan = context.Entity as SellingNumberPlan;
            return sellingNumberPlan.SellingNumberPlanId;
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

        #endregion
    }
}
