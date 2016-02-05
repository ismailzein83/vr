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
   
        #region ctor/Local Variables
        #endregion

        #region Public Methods
        public IDataRetrievalResult<TOne.WhS.BusinessEntity.Entities.RateTypeDetail> GetFilteredRateTypes(DataRetrievalInput<TOne.WhS.BusinessEntity.Entities.RateTypeQuery> input)
        {
            var allRateTypes = GetCachedRateTypes();
            Func<TOne.WhS.BusinessEntity.Entities.RateType, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allRateTypes.ToBigResult(input, filterExpression, RateTypeDetailMapper));

        }
       
        public IEnumerable<TOne.WhS.BusinessEntity.Entities.RateTypeInfo> GetAllRateTypes()
        {
            var allRateTypes = GetCachedRateTypes();
            if (allRateTypes == null)
                return null;
            return allRateTypes.Values.MapRecords(RateTypeInfoMapper);
        }
        public TOne.WhS.BusinessEntity.Entities.RateType GetRateType(int rateTypeId)
        {
            var allRateTypes = GetCachedRateTypes();
            return allRateTypes.GetRecord(rateTypeId);
        }
        public TOne.Entities.InsertOperationOutput<TOne.WhS.BusinessEntity.Entities.RateType> AddRateType(TOne.WhS.BusinessEntity.Entities.RateType rateType)
        {
            TOne.Entities.InsertOperationOutput<TOne.WhS.BusinessEntity.Entities.RateType> insertOperationOutput = new TOne.Entities.InsertOperationOutput<TOne.WhS.BusinessEntity.Entities.RateType>();

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
        public TOne.Entities.UpdateOperationOutput<TOne.WhS.BusinessEntity.Entities.RateType> UpdateRateType(TOne.WhS.BusinessEntity.Entities.RateType rateType)
        {
            IRateTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IRateTypeDataManager>();

            bool updateActionSucc = dataManager.Update(rateType);
            TOne.Entities.UpdateOperationOutput<TOne.WhS.BusinessEntity.Entities.RateType> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<TOne.WhS.BusinessEntity.Entities.RateType>();

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
    
        #endregion

        #region Private Methods
        private class CacheManager : BaseCacheManager
        {
            IRateTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IRateTypeDataManager>();

            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreRateTypesUpdated(ref _updateHandle);
            }
        }
        private Dictionary<int, TOne.WhS.BusinessEntity.Entities.RateType> GetCachedRateTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRateTypes",
               () =>
               {
                   IRateTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IRateTypeDataManager>();
                   IEnumerable<TOne.WhS.BusinessEntity.Entities.RateType> rateTypes = dataManager.GetRateTypes();
                   return rateTypes.ToDictionary(x => x.RateTypeId, x => x);
               });
        }
        #endregion

        #region  Mappers
        private TOne.WhS.BusinessEntity.Entities.RateTypeInfo RateTypeInfoMapper(TOne.WhS.BusinessEntity.Entities.RateType rateType)
        {
            return new TOne.WhS.BusinessEntity.Entities.RateTypeInfo
            {
                Name = rateType.Name,
                RateTypeId = rateType.RateTypeId
            };
        }
        private TOne.WhS.BusinessEntity.Entities.RateTypeDetail RateTypeDetailMapper(TOne.WhS.BusinessEntity.Entities.RateType rateType)
        {
            return new TOne.WhS.BusinessEntity.Entities.RateTypeDetail
            {
                Entity = rateType,
            };
        }
        #endregion
        
    }
}
