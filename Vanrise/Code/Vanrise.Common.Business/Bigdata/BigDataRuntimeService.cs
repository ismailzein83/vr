using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;

namespace Vanrise.Common.Business
{
    public class BigDataRuntimeService : Vanrise.Runtime.RuntimeService
    {
        ServiceHost _serviceHost;
        IBigDataServiceDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IBigDataServiceDataManager>();
        long _bigDataServiceId;

        int _wcfPortRangeStart;
        int _wcfPortRangeEnd;

        protected override void OnStarted()
        {
            if(!int.TryParse(ConfigurationManager.AppSettings["WCFPortRangeStart"], out _wcfPortRangeStart))
                _wcfPortRangeStart = 40000;
            if (!int.TryParse(ConfigurationManager.AppSettings["WCFPortRangeEnd"], out _wcfPortRangeEnd))
                _wcfPortRangeEnd = 50000;
            BigDataManager.Instance._isBigDataHost = true;
            _serviceHost = new ServiceHost(typeof(BigDataWCFService));
            string machineName = Environment.MachineName;
            var random = new Random();
            string portNumber = random.Next(_wcfPortRangeStart, _wcfPortRangeEnd).ToString();
            string serviceUrl = String.Format("net.tcp://{0}:{1}/BigDataWCFService", machineName, portNumber);
            var endPoint = AddTCPEndPoint(_serviceHost, serviceUrl);
            _serviceHost.Opening += serviceHost_Opening;
            _serviceHost.Opened += new EventHandler(serviceHost_Opened);
            _serviceHost.Closing += serviceHost_Closing;
            _serviceHost.Closed += serviceHost_Closed;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    portNumber = random.Next(_wcfPortRangeStart, _wcfPortRangeEnd).ToString();
                    serviceUrl = String.Format("net.tcp://{0}:{1}/BigDataWCFService", machineName, portNumber);
                    endPoint.Address = new EndpointAddress(serviceUrl);
                    _serviceHost.Open();
                    break;
                }
                catch
                {
                    LoggerFactory.GetLogger().WriteWarning("Could not register '{0}'", serviceUrl);
                    throw ;
                }
            }
            if (!_dataManager.Insert(serviceUrl, Vanrise.Runtime.RunningProcessManager.CurrentProcess.ProcessId, out _bigDataServiceId))
                throw new Exception("Could not insert BigDataService into database");
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            if(_serviceHost != null && _serviceHost.State == CommunicationState.Opened)
            {
                _serviceHost.Close();
            }
            base.OnStopped();
        }

        private ServiceEndpoint AddTCPEndPoint(ServiceHost serviceHost, string serviceUrl)
        {
            LoggerFactory.GetLogger().WriteInformation("Adding NetTcp End Point to Business Process WCF Service (BPService)...");
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            binding.MaxReceivedMessageSize = int.MaxValue;
            var endPoint = serviceHost.AddServiceEndpoint(typeof(IBigDataWCFService), binding, serviceUrl);
            LoggerFactory.GetLogger().WriteInformation("NetTcp End Point to Business Process WCF Service (BPService) added");
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
            Vanrise.Runtime.RunningProcessManager runningProcessManager = new Runtime.RunningProcessManager();
            IEnumerable<int> runningProcessIds = runningProcessManager.GetCachedRunningProcesses().MapRecords(itm => itm.ProcessId);
            _dataManager.DeleteTimedOutServices(runningProcessIds);
        }
    }
}
