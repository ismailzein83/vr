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
        static List<RuntimeServiceInstance> s_runtimeServices;
        static Dictionary<int, List<RuntimeServiceInstance>> s_runtimeServicesByTypeId;
        static Dictionary<int, Dictionary<Guid, RuntimeServiceInstance>> s_runtimeServicesDictByTypeId;
        static Object s_lockObj = new object();

        static bool s_areServicesChanged = true;
        internal static void SetServicesChanged()
        {
            lock (s_lockObj)
            {
                s_areServicesChanged = true;
            }
        }

        private void RefreshServicesIfNeeded()
        {
            if (s_areServicesChanged)
            {
                lock (s_lockObj)
                {
                    if (s_areServicesChanged)
                    {
                        IRuntimeServiceInstanceDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeServiceInstanceDataManager>();
                        var runtimeServices = dataManager.GetServices();
                        Dictionary<int, List<RuntimeServiceInstance>> runtimeServicesByTypeId = new Dictionary<int, List<RuntimeServiceInstance>>();
                        Dictionary<int, Dictionary<Guid, RuntimeServiceInstance>> runtimeServicesDictByTypeId = new Dictionary<int, Dictionary<Guid, RuntimeServiceInstance>>();
                        foreach (var s in runtimeServices)
                        {
                            runtimeServicesByTypeId.GetOrCreateItem(s.ServiceTypeId).Add(s);
                            runtimeServicesDictByTypeId.GetOrCreateItem(s.ServiceTypeId).Add(s.ServiceInstanceId, s);
                        }

                        s_runtimeServices = runtimeServices;
                        s_runtimeServicesByTypeId = runtimeServicesByTypeId;
                        s_runtimeServicesDictByTypeId = runtimeServicesDictByTypeId;
                        s_areServicesChanged = false;
                    }
                }
            }
        }

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
            RefreshServicesIfNeeded();
            int serviceTypeId = GetServiceTypeId(serviceTypeUniqueName);
            return s_runtimeServicesDictByTypeId.GetRecord(serviceTypeId);
        }


        Dictionary<int, List<RuntimeServiceInstance>> GetServicesByType()
        {
            RefreshServicesIfNeeded();
            return s_runtimeServicesByTypeId;
        }

        List<RuntimeServiceInstance> GetAllServices()
        {
            RefreshServicesIfNeeded();
            return s_runtimeServices;
        }
    }
}
