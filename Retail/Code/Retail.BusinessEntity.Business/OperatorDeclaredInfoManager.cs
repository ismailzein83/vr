using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class OperatorDeclaredInfoManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<OperatorDeclaredInfoDetail> GetFilteredOperatorDeclaredInfos(Vanrise.Entities.DataRetrievalInput<OperatorDeclaredInfoQuery> input)
        {
            Dictionary<int, OperatorDeclaredInfo> cachedOperatorDeclaredInfos = this.GetCachedOperatorDeclaredInfos();

            Func<OperatorDeclaredInfo, bool> filterExpression = null;
            //(OperatorDeclaredInfo) =>
            //(input.Query.Name == null || OperatorDeclaredInfo.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedOperatorDeclaredInfos.ToBigResult(input, filterExpression, OperatorDeclaredInfoDetailMapper));
        }

        public OperatorDeclaredInfo GetOperatorDeclaredInfo(int OperatorDeclaredInfoId)
        {
            return this.GetCachedOperatorDeclaredInfos().GetRecord(OperatorDeclaredInfoId);
        }

       
     
        public Vanrise.Entities.InsertOperationOutput<OperatorDeclaredInfoDetail> AddOperatorDeclaredInfo(OperatorDeclaredInfo OperatorDeclaredInfo)
        {

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<OperatorDeclaredInfoDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IOperatorDeclaredInfoDataManager dataManager = BEDataManagerFactory.GetDataManager<IOperatorDeclaredInfoDataManager>();
            int OperatorDeclaredInfoId = -1;

            if (dataManager.Insert(OperatorDeclaredInfo, out OperatorDeclaredInfoId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                OperatorDeclaredInfo.OperatorDeclaredInfoId = OperatorDeclaredInfoId;
                insertOperationOutput.InsertedObject = OperatorDeclaredInfoDetailMapper(OperatorDeclaredInfo);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<OperatorDeclaredInfoDetail> UpdateOperatorDeclaredInfo(OperatorDeclaredInfo OperatorDeclaredInfo)
        {

            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<OperatorDeclaredInfoDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IOperatorDeclaredInfoDataManager dataManager = BEDataManagerFactory.GetDataManager<IOperatorDeclaredInfoDataManager>();

            if (dataManager.Update(OperatorDeclaredInfo))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = OperatorDeclaredInfoDetailMapper(OperatorDeclaredInfo);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Validation Methods

      
        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IOperatorDeclaredInfoDataManager _dataManager = BEDataManagerFactory.GetDataManager<IOperatorDeclaredInfoDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreOperatorDeclaredInfosUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, OperatorDeclaredInfo> GetCachedOperatorDeclaredInfos()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetOperatorDeclaredInfos", () =>
            {
                IOperatorDeclaredInfoDataManager dataManager = BEDataManagerFactory.GetDataManager<IOperatorDeclaredInfoDataManager>();
                IEnumerable<OperatorDeclaredInfo> OperatorDeclaredInfos = dataManager.GetOperatorDeclaredInfos();
                return OperatorDeclaredInfos.ToDictionary(op => op.OperatorDeclaredInfoId, op => op);
            });
        }

        #endregion

        #region Mappers

        private OperatorDeclaredInfoDetail OperatorDeclaredInfoDetailMapper(OperatorDeclaredInfo OperatorDeclaredInfo)
        {
            return new OperatorDeclaredInfoDetail()
            {
                Entity = OperatorDeclaredInfo
            };
        }
       
        #endregion
    }
}
