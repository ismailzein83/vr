using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;
using TOne.WhS.BusinessEntity.Data;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RateTypeManager
    {
        IDataRetrievalResult<RateType> GetFilteredRateType(DataRetrievalInput<RateTypeQuery> input)
        {
            var allRateTypes = GetCachedRateTypes();
            Func<RateType, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allRateTypes.ToBigResult(input, filterExpression));

        }

        public Dictionary<int, RateType> GetCachedRateTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRateTypes",
               () =>
               {
                   IRateTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IRateTypeDataManager>();
                   IEnumerable<RateType> rateTypes = dataManager.GetRateTypes();
                   return rateTypes.ToDictionary(x => x.RateTypeId, x => x);
               });
        }

        private class CacheManager : BaseCacheManager
        {
            IRateTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IRateTypeDataManager>();

            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreRateTypesUpdated(ref _updateHandle);
            }
        }


        public IEnumerable<RateType> GetAllRateTypes()
        {
            var allRateTypes = GetCachedRateTypes();
            if (allRateTypes == null)
                return null;
            return allRateTypes.Values;
        }

        public RateType GetRateType(int rateTypeId)
        {
            var allRateTypes = GetCachedRateTypes();
            return allRateTypes.GetRecord(rateTypeId);
        }

        InsertOperationOutput<RateType> Insert(RateType rateType)
        {
            InsertOperationOutput<RateType> insertOperationOutput = new InsertOperationOutput<RateType>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            int rateTypeId = -1;
            IRateTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IRateTypeDataManager>();
            bool insertActionSucc = dataManager.Insert(rateType, out rateTypeId);
            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                rateType.RateTypeId = rateTypeId;
                insertOperationOutput.InsertedObject = rateType;
            }

            return insertOperationOutput;
        }

        UpdateOperationOutput<RateType> Update(RateType rateType)
        {
            UpdateOperationOutput<RateType> updateOperationOutput = new UpdateOperationOutput<RateType>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            IRateTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IRateTypeDataManager>();
            bool updateActionSucc = dataManager.Update(rateType);
            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = rateType;
            }

            return updateOperationOutput;

        }





    }
}
