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
    public class RuntimeServiceInstanceManager
    {
        internal RuntimeServiceInstance RegisterServiceInstance(string serviceTypeUniqueName, ServiceInstanceInfo info)
        {
            int serviceTypeId = GetServiceTypeId(serviceTypeUniqueName);
            RuntimeServiceInstance serviceInstance = new RuntimeServiceInstance
            {
                ServiceInstanceId = Guid.NewGuid(),
                ServiceTypeId = serviceTypeId,
                ProcessId = RunningProcessManager.CurrentProcess.ProcessId,
                InstanceInfo = info
            };

            var dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeServiceInstanceDataManager>();
            dataManager.AddService(serviceInstance);
            return serviceInstance;
        }

        private int GetServiceTypeId(string serviceTypeUniqueName)
        {
            int serviceTypeId = Common.BusinessManagerFactory.GetManager<Common.ITypeManager>().GetTypeId(serviceTypeUniqueName);
            return serviceTypeId;
        }

        public RuntimeServiceInstance GetServiceInstance(string serviceTypeUniqueName, Guid serviceInstanceId)
        {
            return GetServicesDictionary(serviceTypeUniqueName).GetRecord(serviceInstanceId);
        }

        public List<RuntimeServiceInstance> GetServices(string serviceTypeUniqueName)
        {
            int serviceTypeId = GetServiceTypeId(serviceTypeUniqueName);
            return GetServicesByType().GetRecord(serviceTypeId);
        }

        public Dictionary<Guid, RuntimeServiceInstance> GetServicesDictionary(string serviceTypeUniqueName)
        {
            string cacheName = String.Format("GetServicesDictionary_{0}", serviceTypeUniqueName);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                   () =>
                   {
                       int serviceTypeId = GetServiceTypeId(serviceTypeUniqueName);
                       return GetServicesByType().GetRecord(serviceTypeId).ToDictionary(itm => itm.ServiceInstanceId, itm => itm);
                   });
            
        }


        Dictionary<int, List<RuntimeServiceInstance>> GetServicesByType()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetServicesByType",
                () =>
                {
                    Dictionary<int, List<RuntimeServiceInstance>> rslt = new Dictionary<int, List<RuntimeServiceInstance>>();

                    var allServices = GetAllServices();
                    if(allServices != null)
                    {
                        foreach(var service in allServices)
                        {
                            rslt.GetOrCreateItem(service.ServiceTypeId).Add(service);
                        }
                    }
                    return rslt;
                });
        }

        List<RuntimeServiceInstance> GetAllServices()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllServices",
                   () =>
                   {
                       var dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeServiceInstanceDataManager>();
                       return dataManager.GetServices();
                   });
        }
        internal void DeleteNonRunningServices()
        {
            var services = GetAllServices();
            if (services != null)
            {
                HashSet<int> runningProcessIds = new HashSet<int>(new RunningProcessManager().GetRunningProcessesFromDB().Select(itm => itm.ProcessId));
                var dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeServiceInstanceDataManager>();
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
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
        }

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRuntimeServiceInstanceDataManager _dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeServiceInstanceDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreServiceInstancesUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
