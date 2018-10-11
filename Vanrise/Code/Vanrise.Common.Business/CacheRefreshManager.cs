using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class CacheRefreshManager : ICacheRefreshManager
    {
        public IDataRetrievalResult<CacheRefreshHandleDetail> GetFilteredCacheRefreshHandles(DataRetrievalInput<CacheRefreshHandleQuery> input)
        {
            var allCacheHandles = this.GetAllCacheHandles();
            Func<CacheRefreshHandle, bool> filterExpression = (x) => (input.Query.TypeName == null || x.TypeName.ToLower().Contains(input.Query.TypeName.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCacheHandles.ToBigResult(input, filterExpression, CacheRefreshHandleDetailMapper));
        }

        public Vanrise.Entities.UpdateOperationOutput<CacheRefreshHandleDetail> SetCacheExpired(string cacheTypeName)
        {
            TriggerCacheExpiration(cacheTypeName);
            Vanrise.Entities.UpdateOperationOutput<CacheRefreshHandleDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CacheRefreshHandleDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            var record = s_dataManager.GetByCacheTypeName(cacheTypeName);
            updateOperationOutput.UpdatedObject = CacheRefreshHandleDetailMapper(record);            
            return updateOperationOutput;
        }

        static ICacheRefreshDataManager s_dataManager = CommonDataManagerFactory.GetDataManager<ICacheRefreshDataManager>();
        public bool ShouldRefreshCacheManager(string cacheTypeName, ref object updateHandle)
        {
            var lastUpdateHandle = GetCacheUpdateHandle(cacheTypeName);
            if (lastUpdateHandle == null)
                return false;
            bool shouldRefreshCacheManager = !s_dataManager.AreUpdateHandlesEqual(ref updateHandle, lastUpdateHandle.LastUpdateInfo);
            updateHandle = lastUpdateHandle.LastUpdateInfo;
            return shouldRefreshCacheManager;
        }

        public void TriggerCacheExpiration(string cacheTypeName)
        {
            s_dataManager.UpdateCacheTypeHandle(cacheTypeName);
        }

        private CacheRefreshHandle GetCacheUpdateHandle(string cacheTypeName)
        {
            return GetAllCacheHandles().GetRecord(cacheTypeName);
        }

        static Dictionary<string, CacheRefreshHandle> s_handles = new Dictionary<string, Entities.CacheRefreshHandle>();
        static DateTime s_lastRetrievedTime;
        static Object s_lockObj = new object();


        Dictionary<string, CacheRefreshHandle> GetAllCacheHandles()
        {
            if((VRClock.Now - s_lastRetrievedTime) >= TimeSpan.FromSeconds(2))
            {
                lock(s_lockObj)
                {
                    if ((VRClock.Now - s_lastRetrievedTime) >= TimeSpan.FromSeconds(2))
                    {
                        s_handles = s_dataManager.GetAll().ToDictionary(itm => itm.TypeName, itm => itm);
                    }
                    s_lastRetrievedTime = VRClock.Now;
                }
            }
            return s_handles;
        }

        #region Mappers

        public CacheRefreshHandleDetail CacheRefreshHandleDetailMapper(CacheRefreshHandle cacheRefreshHandle)
        {
            CacheRefreshHandleDetail cacheRefreshHandleDetail = new CacheRefreshHandleDetail()
            {
                TypeName = cacheRefreshHandle.TypeName,
                LastUpdateTime = cacheRefreshHandle.LastUpdateTime
            };
            return cacheRefreshHandleDetail;
        }

        #endregion
    }
}
