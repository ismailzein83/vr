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
using Vanrise.Runtime.Entities;

namespace Vanrise.Common.Business
{
    public class BigDataRuntimeService : RuntimeService
    {
        ServiceHost _serviceHost;
        IBigDataServiceDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IBigDataServiceDataManager>();
        long _bigDataServiceId;


        TimeSpan _deleteTimedOutServicesInterval;
        DateTime _deleteTimedOutServicesLastTime;
        string _serviceUrl;

        public override Guid ConfigId { get { return new Guid("6B58776E-B72C-4852-A5A8-B1631A8873F1"); } }

        public BigDataRuntimeService()
        {
            
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["BigDataCache_DeleteTimedOutServicesInterval"], out _deleteTimedOutServicesInterval))
                _deleteTimedOutServicesInterval = TimeSpan.FromMinutes(2);
        }

        public override void OnInitialized(IRuntimeServiceInitializeContext context)
        {
            BigDataManager.Instance._isBigDataHost = true;
            _serviceHost = ServiceHostManager.Current.CreateAndOpenTCPServiceHost(typeof(BigDataWCFService), typeof(IBigDataWCFService), OnServiceHostCreated, OnServiceHostRemoved, out _serviceUrl);            
            base.OnInitialized(context);
        }


        public override void OnStarted(IRuntimeServiceStartContext context)
        {            
            if (!_dataManager.Insert(_serviceUrl, Vanrise.Runtime.RunningProcessManager.CurrentProcess.ProcessId, out _bigDataServiceId))
                throw new Exception("Could not insert BigDataService into database");
            base.OnStarted(context);
        }

        private void OnServiceHostCreated(ServiceHost serviceHost)
        {
            serviceHost.Opening += serviceHost_Opening;
            serviceHost.Opened += serviceHost_Opened;
            serviceHost.Closing += serviceHost_Closing;
            serviceHost.Closed += serviceHost_Closed;
        }

        private void OnServiceHostRemoved(ServiceHost serviceHost)
        {
            serviceHost.Opening -= serviceHost_Opening;
            serviceHost.Opened -= serviceHost_Opened;
            serviceHost.Closing -= serviceHost_Closing;
            serviceHost.Closed -= serviceHost_Closed;
        }

        public override void OnStopped(IRuntimeServiceStopContext context)
        {
            if(_serviceHost != null && _serviceHost.State == CommunicationState.Opened)
            {
                _serviceHost.Close();
            }
            base.OnStopped(context);
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

        public override void Execute()
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
