using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;
using Vanrise.Common;

namespace Vanrise.Runtime
{
    public class ServiceInstanceManager
    {
        public ServiceInstance RegisterServiceInstance(Guid serviceType, ServiceInstanceInfo info)
        {
            ServiceInstance serviceInstance = new ServiceInstance
            {
                ServiceInstanceId = Guid.NewGuid(),
                ServiceType = serviceType,
                ProcessId = RunningProcessManager.CurrentProcess.ProcessId,
                InstanceInfo = info
            };

            var dataManager = RuntimeDataManagerFactory.GetDataManager<IServiceInstanceDataManager>();
            dataManager.AddService(serviceInstance);
            return serviceInstance;
        }

        public ServiceInstance GetServiceInstance(Guid serviceType, Guid serviceInstanceId)
        {
            return GetServicesDictionary(serviceType).GetRecord(serviceInstanceId);
        }

        public List<ServiceInstance> GetServices(Guid serviceType)
        {
            return GetServicesDictionary(serviceType).Values.ToList();
        }

        public Dictionary<Guid,ServiceInstance> GetServicesDictionary(Guid serviceType)
        {
            string cacheName = String.Format("GetServicesDictionary_{0}", serviceType);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                serviceType,
                () =>
                {
                    var dataManager = RuntimeDataManagerFactory.GetDataManager<IServiceInstanceDataManager>();
                    return dataManager.GetServices(serviceType).ToDictionary(itm => itm.ServiceInstanceId, itm => itm);
                });
        }

        public void DeleteNonRunningServices(Guid serviceType)
        {
            var services = GetServices(serviceType);
            if (services != null)
            {
                HashSet<int> runningProcessIds = new HashSet<int>(new RunningProcessManager().GetCachedRunningProcesses().Select(itm => itm.ProcessId));
                var dataManager = RuntimeDataManagerFactory.GetDataManager<IServiceInstanceDataManager>();
                bool isAnyDeleted = false;
                foreach (var service in services)
                {
                    if (!runningProcessIds.Contains(service.ProcessId))
                    {
                        dataManager.Delete(service.ServiceInstanceId);
                        isAnyDeleted = true;
                    }
                }
                if (isAnyDeleted)
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(serviceType);
            }
        }

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager<Guid>
        {
            IServiceInstanceDataManager _dataManager = RuntimeDataManagerFactory.GetDataManager<IServiceInstanceDataManager>();
            ConcurrentDictionary<Guid, Object> _updateHandlesByServiceType = new ConcurrentDictionary<Guid, Object>();

            protected override bool ShouldSetCacheExpired(Guid parameter)
            {
                Object updateHandle;
                _updateHandlesByServiceType.TryGetValue(parameter, out updateHandle);
                bool isCacheExpired = _dataManager.AreServiceInstancesUpdated(parameter, ref updateHandle);
                _updateHandlesByServiceType.AddOrUpdate(parameter, updateHandle, (key, existingHandle) => updateHandle);
                return isCacheExpired;
            }
        }

        #endregion
    }
}
