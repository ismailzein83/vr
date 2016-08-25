using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.Common.Business
{
    public class DataGroupingDistributorRuntimeService : RuntimeService
    {
        internal static Guid s_dataGroupingDistributorServiceInstanceType = new Guid("1EE15131-82BF-46F8-9A54-39E47E797B74");

        protected override void OnStarted()
        {
            HostServiceIfNeeded();
            RegisterServiceInstance();
            base.OnStarted();
        }

        static ServiceHost s_serviceHost;
        static string s_serviceURL;
        static Object s_hostServiceLockObject = new object();
        private void HostServiceIfNeeded()
        {
            lock (s_hostServiceLockObject)
            {
                if (s_serviceHost == null)
                {
                    s_serviceHost = ServiceHostManager.Current.CreateAndOpenTCPServiceHost(typeof(DataGroupingDistributorWCFService), typeof(IDataGroupingDistributorWCFService), OnServiceHostCreated, OnServiceHostRemoved, out s_serviceURL);
                }
            }
        }

        #region WCF Events
        void OnServiceHostCreated(ServiceHost serviceHost)
        {
            serviceHost.Opening += serviceHost_Opening;
            serviceHost.Opened += serviceHost_Opened;
            serviceHost.Closing += serviceHost_Closing;
            serviceHost.Closed += serviceHost_Closed;
        }

        void OnServiceHostRemoved(ServiceHost serviceHost)
        {
            serviceHost.Opening -= serviceHost_Opening;
            serviceHost.Opened -= serviceHost_Opened;
            serviceHost.Closing -= serviceHost_Closing;
            serviceHost.Closed -= serviceHost_Closed;
        }

        void serviceHost_Opening(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("DataGroupingDistributorWCFService is opening..");
        }

        void serviceHost_Opened(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("DataGroupingDistributorWCFService opened");
        }

        void serviceHost_Closed(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("DataGroupingDistributorWCFService closed");
        }

        void serviceHost_Closing(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("DataGroupingDistributorWCFService is closing..");
        }

        #endregion

        ServiceInstance _serviceInstance;
        private void RegisterServiceInstance()
        {
            _serviceInstance = new ServiceInstanceManager().RegisterServiceInstance(s_dataGroupingDistributorServiceInstanceType, new DataGroupingDistributorServiceInstanceInfo
            {
                TCPServiceURL = s_serviceURL
            });
        }

        protected override void Execute()
        {
            TransactionLocker.Instance.TryLock("DataGroupingDistributorRuntimeService_Execute", () =>
                {
                    var serviceInstanceManager = new ServiceInstanceManager();
                    serviceInstanceManager.DeleteNonRunningServices(s_dataGroupingDistributorServiceInstanceType);
                    serviceInstanceManager.DeleteNonRunningServices(DataGroupingExecutorRuntimeService.s_dataGroupingExecutorServiceInstanceType);
                });
        }
    }

    internal class DataGroupingDistributorServiceInstanceInfo : Vanrise.Runtime.Entities.ServiceInstanceInfo
    {
        public string TCPServiceURL { get; set; }
    }
}
