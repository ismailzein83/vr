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
    public class DataGroupingExecutorRuntimeService : RuntimeService
    {
        internal static Guid s_dataGroupingExecutorServiceInstanceType = new Guid("C6D320B9-39F1-4E32-AFB3-81B15A81EFC5");

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
                    s_serviceHost = ServiceHostManager.Current.CreateAndOpenTCPServiceHost(typeof(DataGroupingExecutorWCFService), typeof(IDataGroupingExecutorWCFService), OnServiceHostCreated, OnServiceHostRemoved, out s_serviceURL);
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
            LoggerFactory.GetLogger().WriteInformation("DataGroupingExecutorRuntimeService is opening..");
        }

        void serviceHost_Opened(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("DataGroupingExecutorRuntimeService opened");
        }

        void serviceHost_Closed(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("DataGroupingExecutorRuntimeService closed");
        }

        void serviceHost_Closing(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("DataGroupingExecutorRuntimeService is closing..");
        }

        #endregion

        ServiceInstance _serviceInstance;
        private void RegisterServiceInstance()
        {
            _serviceInstance = new ServiceInstanceManager().RegisterServiceInstance(s_dataGroupingExecutorServiceInstanceType, new DataGroupingExecutorServiceInstanceInfo
            {
                TCPServiceURL = s_serviceURL
            });
        }

        protected override void Execute()
        {
            DataGroupingExecutor.Current.ProcessItems();
        }
    }

    internal class DataGroupingExecutorServiceInstanceInfo : Vanrise.Runtime.Entities.ServiceInstanceInfo
    {
        public string TCPServiceURL { get; set; }
    }
}
