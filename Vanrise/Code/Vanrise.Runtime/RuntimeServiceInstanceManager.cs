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
        static IRuntimeServiceInstanceDataManager s_dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeServiceInstanceDataManager>();
        internal RuntimeServiceInstance RegisterServiceInstance(Guid serviceInstanceId, int processId, string serviceTypeUniqueName, ServiceInstanceInfo info)
        {
            int serviceTypeId = GetServiceTypeId(serviceTypeUniqueName);
            RuntimeServiceInstance serviceInstance = new RuntimeServiceInstance
            {
                ServiceInstanceId = serviceInstanceId,
                ServiceTypeId = serviceTypeId,
                ProcessId = processId,
                InstanceInfo = info
            };

            var dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeServiceInstanceDataManager>();
            dataManager.AddService(serviceInstance);
            return serviceInstance;
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

        public List<RuntimeServiceInstance> GetServicesFromDB(string serviceTypeUniqueName)
        {
            int serviceTypeId = GetServiceTypeId(serviceTypeUniqueName);
            return s_dataManager.GetServicesByTypeId(serviceTypeId);
        }

        public Dictionary<Guid, RuntimeServiceInstance> GetServicesDictionary(string serviceTypeUniqueName)
        {
            int serviceTypeId = GetServiceTypeId(serviceTypeUniqueName);
            var runtimeServicesDictByTypeId = Vanrise.Caching.CacheManagerFactory.GetCacheManager<RunningProcessManager.CacheManager>().GetOrCreateObject(
                $"RuntimeServiceInstanceManager_GetServicesDictionary_{serviceTypeId}",
                  () =>
                  {
                      Dictionary<int, Dictionary<Guid, RuntimeServiceInstance>> runtimeServicesDictByTypeId_local = new Dictionary<int, Dictionary<Guid, RuntimeServiceInstance>>();
                      var allServices = GetAllServices();
                      foreach (var s in allServices)
                      {
                          runtimeServicesDictByTypeId_local.GetOrCreateItem(s.ServiceTypeId).Add(s.ServiceInstanceId, s);
                      }
                      return runtimeServicesDictByTypeId_local;
                  });
            return runtimeServicesDictByTypeId.GetRecord(serviceTypeId);
        }


        Dictionary<int, List<RuntimeServiceInstance>> GetServicesByType()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<RunningProcessManager.CacheManager>().GetOrCreateObject("RuntimeServiceInstanceManager_GetServicesByType",
                  () =>
                  {
                      Dictionary<int, List<RuntimeServiceInstance>> servicesByType = new Dictionary<int, List<RuntimeServiceInstance>>();
                      var allServices = GetAllServices();
                      foreach (var s in allServices)
                      {
                          servicesByType.GetOrCreateItem(s.ServiceTypeId).Add(s);
                      }
                      return servicesByType;
                  });
        }

        List<RuntimeServiceInstance> GetAllServices()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<RunningProcessManager.CacheManager>().GetOrCreateObject("RuntimeServiceInstanceManager_GetAllServices",
                () =>
                {                    
                    return s_dataManager.GetServices();
                });
        }

        private int GetServiceTypeId(string serviceTypeUniqueName)
        {
            int serviceTypeId = Common.BusinessManagerFactory.GetManager<Common.ITypeManager>().GetTypeId(serviceTypeUniqueName);
            return serviceTypeId;
        }
    }
}
