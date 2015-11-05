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
        public IDataRetrievalResult<RateType> GetFilteredRateTypes(DataRetrievalInput<RateTypeQuery> input)
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

        public TOne.Entities.InsertOperationOutput<RateType> AddRateType(RateType rateType)
        {
            TOne.Entities.InsertOperationOutput<RateType> insertOperationOutput = new TOne.Entities.InsertOperationOutput<RateType>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int rateTypeId = -1;

            IRateTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IRateTypeDataManager>();
            bool insertActionSucc = dataManager.Insert(rateType, out rateTypeId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                rateType.RateTypeId = rateTypeId;
                insertOperationOutput.InsertedObject = rateType;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public TOne.Entities.UpdateOperationOutput<RateType> UpdateRateType(RateType rateType)
        {
            IRateTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IRateTypeDataManager>();

            bool updateActionSucc = dataManager.Update(rateType);
            TOne.Entities.UpdateOperationOutput<RateType> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<RateType>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = rateType;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }





    }
}
