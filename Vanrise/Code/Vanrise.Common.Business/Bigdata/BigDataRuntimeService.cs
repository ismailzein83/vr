using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Runtime;

namespace Vanrise.Common.Business
{
    public class BigDataRuntimeService : Vanrise.Runtime.RuntimeService
    {
        ServiceHost _serviceHost;
        IBigDataServiceDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IBigDataServiceDataManager>();
        long _bigDataServiceId;

        int _wcfPortRangeStart;
        int _wcfPortRangeEnd;
        int _wcfServiceHostingRetries;

        TimeSpan _deleteTimedOutServicesInterval;
        DateTime _deleteTimedOutServicesLastTime;

        public BigDataRuntimeService()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["WCFPortRangeStart"], out _wcfPortRangeStart))
                _wcfPortRangeStart = 40000;
            if (!int.TryParse(ConfigurationManager.AppSettings["WCFPortRangeEnd"], out _wcfPortRangeEnd))
                _wcfPortRangeEnd = 50000;
            if (!int.TryParse(ConfigurationManager.AppSettings["WCFServiceHostingRetries"], out _wcfServiceHostingRetries))
                _wcfServiceHostingRetries = 100;
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["BigDataCache_DeleteTimedOutServicesInterval"], out _deleteTimedOutServicesInterval))
                _deleteTimedOutServicesInterval = TimeSpan.FromMinutes(2);
        }


        protected override void OnStarted(IRuntimeServiceStartContext context)
        {
            BigDataManager.Instance._isBigDataHost = true;
            var random = new Random();

            string serviceUrl = null;
            for (int i = 0; i < _wcfServiceHostingRetries; i++)
            {
                string portNumber = random.Next(_wcfPortRangeStart, _wcfPortRangeEnd).ToString();
                serviceUrl = String.Format("net.tcp://{0}:{1}/BigDataWCFService", Environment.MachineName, portNumber);

                try
                {
                    CreateServiceHost(serviceUrl);
                    LoggerFactory.GetLogger().WriteInformation("Service URL registered successfully '{0}'", serviceUrl);
                    break;
                }
                catch
                {
                    _serviceHost.Opening -= serviceHost_Opening;
                    _serviceHost.Opened -= serviceHost_Opened;
                    _serviceHost.Closing -= serviceHost_Closing;
                    _serviceHost.Closed -= serviceHost_Closed;
                    LoggerFactory.GetLogger().WriteWarning("Could not register '{0}'", serviceUrl);
                    if (i == (_wcfServiceHostingRetries - 1))//last iteration
                        throw;
                }
            }
            if (!_dataManager.Insert(serviceUrl, Vanrise.Runtime.RunningProcessManager.CurrentProcess.ProcessId, out _bigDataServiceId))
                throw new Exception("Could not insert BigDataService into database");
            base.OnStarted(context);
        }

        private void CreateServiceHost(string serviceUrl)
        {
            _serviceHost = new ServiceHost(typeof(BigDataWCFService));
            var endPoint = AddTCPEndPoint(_serviceHost, serviceUrl);
            _serviceHost.Opening += serviceHost_Opening;
            _serviceHost.Opened += serviceHost_Opened;
            _serviceHost.Closing += serviceHost_Closing;
            _serviceHost.Closed += serviceHost_Closed;

            _serviceHost.Open();
        }

        protected override void OnStopped(IRuntimeServiceStopContext context)
        {
            if(_serviceHost != null && _serviceHost.State == CommunicationState.Opened)
            {
                _serviceHost.Close();
            }
            base.OnStopped(context);
        }

        private ServiceEndpoint AddTCPEndPoint(ServiceHost serviceHost, string serviceUrl)
        {
            //LoggerFactory.GetLogger().WriteInformation("Adding NetTcp End Point to Business Process WCF Service (BPService)...");
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            binding.MaxReceivedMessageSize = int.MaxValue;
            var endPoint = serviceHost.AddServiceEndpoint(typeof(IBigDataWCFService), binding, serviceUrl);
            //LoggerFactory.GetLogger().WriteInformation("NetTcp End Point to Business Process WCF Service (BPService) added");
            return endPoint;
        }

        static void serviceHost_Opening(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("BigData WCF Service is opening..");
        }

        static void serviceHost_Opened(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("BigData WCF Service opened");
        }

        static void serviceHost_Closed(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("BigData WCF Service closed");
        }

        static void serviceHost_Closing(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("BigData WCF Service is closing..");
        }

        protected override void Execute()
        {
            if(BigDataManager.Instance._isCachedDataChanged)
            {
                _dataManager.Update(_bigDataServiceId, BigDataManager.Instance._totalRecordsCount, BigDataManager.Instance.CachedObjectIds);
                BigDataManager.Instance._isCachedDataChanged = false;
            }
            if ((DateTime.Now - _deleteTimedOutServicesLastTime) > _deleteTimedOutServicesInterval)
            {
                Vanrise.Runtime.RunningProcessManager runningProcessManager = new Runtime.RunningProcessManager();
                IEnumerable<int> runningProcessIds = runningProcessManager.GetCachedRunningProcesses().MapRecords(itm => itm.ProcessId);
                _dataManager.DeleteTimedOutServices(runningProcessIds);
                _deleteTimedOutServicesLastTime = DateTime.Now;
            }
        }
    }
}
