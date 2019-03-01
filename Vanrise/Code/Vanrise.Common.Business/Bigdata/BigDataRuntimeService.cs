using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.Common.Business
{
    public class BigDataRuntimeService : RuntimeService
    {
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
            VRInterAppCommunication.RegisterService(typeof(BigDataWCFService), typeof(IBigDataWCFService), out _serviceUrl);            
            base.OnInitialized(context);
        }


        public override void OnStarted(IRuntimeServiceStartContext context)
        {            
            if (!_dataManager.Insert(_serviceUrl, Vanrise.Runtime.RunningProcessManager.CurrentProcess.ProcessId, out _bigDataServiceId))
                throw new Exception("Could not insert BigDataService into database");
            base.OnStarted(context);
        }
        
        public override void OnStopped(IRuntimeServiceStopContext context)
        {            
            base.OnStopped(context);
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
                IEnumerable<int> runningProcessIds = runningProcessManager.GetRunningProcesses().MapRecords(itm => itm.ProcessId);
                _dataManager.DeleteTimedOutServices(runningProcessIds);
                _deleteTimedOutServicesLastTime = DateTime.Now;
            }
        }
    }
}
