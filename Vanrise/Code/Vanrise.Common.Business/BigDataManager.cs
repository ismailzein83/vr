using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Common.Business
{
    public class BigDataManager
    {
        #region Singleton Instance

        static BigDataManager _instance = new BigDataManager();

        public static BigDataManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private BigDataManager()
        {
            if (!long.TryParse(ConfigurationManager.AppSettings["BigDataCache_CleanRecordCountThreshold"], out _cleanCacheRecordCountThreshold))
                _cleanCacheRecordCountThreshold = 10000000;
            if (!long.TryParse(ConfigurationManager.AppSettings["BigDataCache_CleanStopOnRecordCount"], out _cleanCacheStopOnRecordCount))
                _cleanCacheStopOnRecordCount = 1000000;
            if (!int.TryParse(ConfigurationManager.AppSettings["BigDataCache_CleanSizePriorityFactor"], out _cleanCacheSizePriorityFactor))
                _cleanCacheSizePriorityFactor = 10;
            if (!int.TryParse(ConfigurationManager.AppSettings["BigDataCache_CleanAgePriorityFactor"], out _cleanCacheAgePriorityFactor))
                _cleanCacheAgePriorityFactor = 1;
        }

        #endregion
       
        long _cleanCacheRecordCountThreshold;
        long _cleanCacheStopOnRecordCount;
        int _cleanCacheSizePriorityFactor;
        int _cleanCacheAgePriorityFactor;

        private ConcurrentDictionary<Guid, CachedBigData> _cachedData = new ConcurrentDictionary<Guid, CachedBigData>();

        long _totalRecordsCount;

        public Vanrise.Entities.IDataRetrievalResult<R> RetrieveData<T, Q, R>(Vanrise.Entities.DataRetrievalInput<T> input,
            Func<IEnumerable<Q>> retrieveDataFunc, Func<Q, R> itemDetailMapper)
        {
            Guid cacheObjectId;            
            CachedBigData cachedBigData;
            if (Guid.TryParse(input.ResultKey, out cacheObjectId) && _cachedData.TryGetValue(cacheObjectId, out cachedBigData))
                return BigDataToDataResult(input, itemDetailMapper, cachedBigData);
            else if(ShouldRetrieveData())
            {
                cachedBigData = RetrieveData(input, retrieveDataFunc);
                return BigDataToDataResult(input, itemDetailMapper, cachedBigData);
            }
            else
            {
                return CallBigDataService(input, itemDetailMapper);
            }
        }

        public Vanrise.Entities.IDataRetrievalResult<R> RetrieveData<T, Q, R>(Vanrise.Entities.DataRetrievalInput<T> input, IBigDataRequestHandler<T, Q, R> requestHandler)
        {
            Guid cacheObjectId;
            CachedBigData cachedBigData;
            if (Guid.TryParse(input.ResultKey, out cacheObjectId) && _cachedData.TryGetValue(cacheObjectId, out cachedBigData))
                return BigDataToDataResult<T, Q, R>(input, (entity) => requestHandler.EntityDetailMapper(entity), cachedBigData);
            else if (ShouldRetrieveData())
            {
                cachedBigData = RetrieveData(input, () => requestHandler.RetrieveAllData());
                return BigDataToDataResult<T, Q, R>(input, (entity) => requestHandler.EntityDetailMapper(entity), cachedBigData);
            }
            else
            {
                return CallBigDataService<T, Q, R>(input, (entity) => requestHandler.EntityDetailMapper(entity));
            }
        }

        #region Private Methods

        private bool ShouldRetrieveData()
        {
            return true;
        }

        private Entities.IDataRetrievalResult<R> CallBigDataService<T, Q, R>(Entities.DataRetrievalInput<T> input, Func<Q, R> itemDetailMapper)
        {
            throw new NotImplementedException();
        }



        #endregion

        private CachedBigData RetrieveData<T, Q>(Vanrise.Entities.DataRetrievalInput<T> input, Func<IEnumerable<Q>> retrieveDataFunc)
        {
            CleanCacheIfNeeded();
            var dataList = retrieveDataFunc().ToList();
            var recordsCount = dataList != null ? dataList.Count : 0;
            lock (this)
            {
                _totalRecordsCount += recordsCount;
            }
            var cachedBigData = new CachedBigData
            {
                CacheObjectId = Guid.NewGuid(),
                Data = dataList,
                RecordsCount = recordsCount
            };
            _cachedData.TryAdd(cachedBigData.CacheObjectId, cachedBigData);
            return cachedBigData;
        }

        private Entities.IDataRetrievalResult<R> BigDataToDataResult<T, Q, R>(Vanrise.Entities.DataRetrievalInput<T> input, Func<Q, R> itemDetailMapper, CachedBigData cachedBigData)
        {
            cachedBigData.LastAccessedTime = DateTime.Now;
            var allData = cachedBigData.Data as IEnumerable<Q>;
            var rslt = Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allData.ToBigResult(input, null, itemDetailMapper));
            Vanrise.Entities.BigResult<R> bigResult = rslt as Vanrise.Entities.BigResult<R>;
            if (bigResult != null)
                bigResult.ResultKey = cachedBigData.CacheObjectId.ToString();
            return rslt;
        }

        private void CleanCacheIfNeeded()
        {
            if (_totalRecordsCount < _cleanCacheRecordCountThreshold)
                return;
            Double maxRecordsCount = _cachedData.Values.Max(itm => itm.RecordsCount);
            Double maxAge = _cachedData.Values.Max(itm => (DateTime.Now - itm.LastAccessedTime).TotalSeconds);
            //records count is big, it has more priority to keep in the cache
            //age is big, it has less priority to keep in the cache
            var orderedCachedObjects =
                       _cachedData.Values.OrderByDescending(itm => (itm.RecordsCount * _cleanCacheSizePriorityFactor / maxRecordsCount) - ((DateTime.Now - itm.LastAccessedTime).TotalSeconds * _cleanCacheAgePriorityFactor / maxAge)).ToList();
            foreach(var cacheObject in orderedCachedObjects)
            {
                CachedBigData dummy;
                _cachedData.TryRemove(cacheObject.CacheObjectId, out dummy);
                lock(this)
                {
                    _totalRecordsCount -= cacheObject.RecordsCount;
                }
                if(_totalRecordsCount <= _cleanCacheStopOnRecordCount)
                    break;
            }
            GC.Collect();
        }
     

        #region Private Classes

        private class CachedBigData
        {
            public Guid CacheObjectId { get; set; }

            public Object Data { get; set; }

            public long RecordsCount { get; set; }

            public DateTime LastAccessedTime { get; set; }
        }

        #endregion
    }

    public interface IBigDataRequestHandler<T, Q, R>
    {
        Vanrise.Entities.IDataRetrievalResult<R> RetrieveData(Vanrise.Entities.DataRetrievalInput<T> input);

        R EntityDetailMapper(Q entity);

        IEnumerable<Q> RetrieveAllData();
    }
}
