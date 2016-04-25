﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Vanrise.Common;
using Vanrise.Common.Data;
using Vanrise.Entities;

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

        internal BigDataManager()
        {
            if (!long.TryParse(ConfigurationManager.AppSettings["BigDataCache_CleanRecordCountThreshold"], out _cleanCacheRecordCountThreshold))
                _cleanCacheRecordCountThreshold = 10000000;
            if (!long.TryParse(ConfigurationManager.AppSettings["BigDataCache_CleanStopOnRecordCount"], out _cleanCacheStopOnRecordCount))
                _cleanCacheStopOnRecordCount = 1000000;
            if (!int.TryParse(ConfigurationManager.AppSettings["BigDataCache_CleanSizePriorityFactor"], out _cleanCacheSizePriorityFactor))
                _cleanCacheSizePriorityFactor = 10;
            if (!int.TryParse(ConfigurationManager.AppSettings["BigDataCache_CleanAgePriorityFactor"], out _cleanCacheAgePriorityFactor))
                _cleanCacheAgePriorityFactor = 1;

            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["BigDataCache_PingBigDataServiceTimeOutInterval"], out _pingBigDataServiceTimeOutInterval))
                _pingBigDataServiceTimeOutInterval = TimeSpan.FromMilliseconds(500);
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["BigDataCache_PingBigDataServiceCheckInterval"], out _pingBigDataServiceCheckInterval))
                _pingBigDataServiceCheckInterval = TimeSpan.FromMinutes(2);
        }

        #endregion

        #region Local Variables

        internal bool _isBigDataHost;
        long _cleanCacheRecordCountThreshold;
        long _cleanCacheStopOnRecordCount;
        int _cleanCacheSizePriorityFactor;
        int _cleanCacheAgePriorityFactor;

        TimeSpan _pingBigDataServiceTimeOutInterval;
        TimeSpan _pingBigDataServiceCheckInterval;

        private ConcurrentDictionary<Guid, CachedBigData> _cachedData = new ConcurrentDictionary<Guid, CachedBigData>();
        internal IEnumerable<Guid> CachedObjectIds
        {
            get
            {
                return _cachedData.Keys;
            }
        }
        internal long _totalRecordsCount;

        internal bool _isCachedDataChanged;
        ConcurrentDictionary<string, DateTime> _unreachableServiceURLs = new ConcurrentDictionary<string, DateTime>();

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<R> RetrieveData<T, Q, R>(Vanrise.Entities.DataRetrievalInput<T> input, BigDataRequestHandler<T, Q, R> requestHandler)
        {
            if (input.FromRow.HasValue || input.ToRow.HasValue)
            {
                Guid cacheObjectId;
                CachedBigData cachedBigData;
                if (Guid.TryParse(input.ResultKey, out cacheObjectId) && _cachedData.TryGetValue(cacheObjectId, out cachedBigData))
                    return BigDataToDataResult(input, cachedBigData, requestHandler);
                else if (IsCurrentABigDataHost())
                {
                    cachedBigData = RetrieveAllData(input, requestHandler);
                    return BigDataToDataResult(input, cachedBigData, requestHandler);
                }
                else
                {
                    Vanrise.Entities.IDataRetrievalResult<R> rslt;
                    if (!TryCallBigDataService(cacheObjectId, input, requestHandler, out rslt))
                    {
                        cachedBigData = RetrieveAllData(input, requestHandler);
                        rslt = BigDataToDataResult(input, cachedBigData, requestHandler);
                    }
                    return rslt;
                }
            }
            else
            {
                return requestHandler.AllRecordsToDataResult(input, requestHandler.RetrieveAllData(input));
            }
        }

        #endregion

        #region Private Methods

        private bool IsCurrentABigDataHost()
        {
            return _isBigDataHost;
        }

        private bool TryCallBigDataService<T, Q, R>(Guid cacheObjectId, Entities.DataRetrievalInput<T> input, BigDataRequestHandler<T, Q, R> requestHandler, out Entities.IDataRetrievalResult<R> rslt)
        {
            string serializedResult = null;
            foreach (var bigDataService in GetBigDataServicesByPriority(cacheObjectId))
            {
                DateTime unreachableCheckTime;
                if(_unreachableServiceURLs.TryGetValue(bigDataService.URL, out unreachableCheckTime))
                {
                    if ((DateTime.Now - unreachableCheckTime) < _pingBigDataServiceCheckInterval)
                        continue;
                }
                bool isServiceCallSucceeded = TryCreateServiceClient(bigDataService,
                    (client) =>
                    {
                        BigDataRequest<T, Q, R> request = new BigDataRequest<T, Q, R>
                        {
                            Input = input,
                            RequestHandler = requestHandler
                        };
                        serializedResult = client.RetrieveData(Vanrise.Common.Serializer.Serialize(request));
                    });
                if (isServiceCallSucceeded)
                {
                    rslt = Vanrise.Common.Serializer.Deserialize(serializedResult) as IDataRetrievalResult<R>;
                    return true;
                }
                else
                    _unreachableServiceURLs.AddOrUpdate(bigDataService.URL, DateTime.Now, (k, v) => DateTime.Now);
            }
            rslt = null;
            return false;
        }

        private CachedBigData RetrieveAllData<T, Q, R>(Vanrise.Entities.DataRetrievalInput<T> input, BigDataRequestHandler<T, Q, R> requestHandler)
        {
            CleanCacheIfNeeded();
            var dataList = requestHandler.RetrieveAllData(input).ToList();
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
            _isCachedDataChanged = true;
            return cachedBigData;
        }

        private Entities.IDataRetrievalResult<R> BigDataToDataResult<T, Q, R>(Vanrise.Entities.DataRetrievalInput<T> input, CachedBigData cachedBigData, BigDataRequestHandler<T, Q, R> requestHandler)
        {
            cachedBigData.LastAccessedTime = DateTime.Now;
            var allRecords = cachedBigData.Data as IEnumerable<Q>;
            var rslt = requestHandler.AllRecordsToDataResult(input, allRecords);
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
            _isCachedDataChanged = true;
            GC.Collect();
        }

        public IEnumerable<BigDataService> GetBigDataServicesByPriority(Guid cacheObjectId)
        {
            IEnumerable<BigDataService> allServices = Vanrise.Caching.CacheManagerFactory.GetCacheManager<BigDataServiceCacheManager>().GetOrCreateObject("GetAllBigDataServices",
                () =>
                {
                    IBigDataServiceDataManager dataManager = CommonDataManagerFactory.GetDataManager<IBigDataServiceDataManager>();
                    return dataManager.GetAll();
                });
            return allServices.OrderBy(itm => itm.CachedObjectIds != null && itm.CachedObjectIds.Contains(cacheObjectId) ? 0 : 1).ThenBy(itm => itm.TotalCachedRecordsCount);
        }

        private bool TryCreateServiceClient(BigDataService bigDataService, Action<IBigDataWCFService> onClientReady)
        {
            Binding binding;
            string serviceURL;

            binding = new NetTcpBinding(SecurityMode.None)
            {
                MaxBufferPoolSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue
            };
            serviceURL = String.Format(bigDataService.URL);

            binding.OpenTimeout = TimeSpan.FromMinutes(5);
            binding.CloseTimeout = TimeSpan.FromMinutes(5);
            binding.SendTimeout = TimeSpan.FromMinutes(5);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(5);

            ChannelFactory<IBigDataWCFService> channelFactory = new ChannelFactory<IBigDataWCFService>(binding, new EndpointAddress(serviceURL));
            IChannel channel = channelFactory.CreateChannel() as IChannel;
            try
            {
                channel.Open(_pingBigDataServiceTimeOutInterval);
            }
            catch
            {
                LoggerFactory.GetLogger().WriteWarning("cannot connect to BigDataService '{0}'", serviceURL);
                return false;
            }
            IBigDataWCFService client = channel as IBigDataWCFService; 
            try
            {
                onClientReady(client);
            }
            finally
            {
                (client as IDisposable).Dispose();
            }
            return true;
        }

        #endregion

        #region Private Classes

        private class CachedBigData
        {
            public Guid CacheObjectId { get; set; }

            public Object Data { get; set; }

            public long RecordsCount { get; set; }

            public DateTime LastAccessedTime { get; set; }
        }

        private class BigDataRequest<T,Q,R> : IBigDataRequest
        {
            public Entities.DataRetrievalInput<T> Input { get; set; }

            public BigDataRequestHandler<T, Q, R> RequestHandler { get; set; }

            public string RetrieveData()
            {
                var dataResult = BigDataManager.Instance.RetrieveData(this.Input, this.RequestHandler);
                return Vanrise.Common.Serializer.Serialize(dataResult);
            }
        }

        #endregion
    }
}
