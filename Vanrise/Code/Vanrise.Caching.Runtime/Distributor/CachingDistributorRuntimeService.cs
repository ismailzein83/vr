using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.Caching.Runtime
{
    public class CachingDistributorRuntimeService : RuntimeService
    {
        internal const string SERVICE_TYPE_UNIQUE_NAME = "VR_Caching_CachingDistributorRuntimeService";

        public override string ServiceTypeUniqueName
        {
            get
            {
                return SERVICE_TYPE_UNIQUE_NAME;
            }
        }

        RunningProcessManager _runningProcessManager = new RunningProcessManager();

        static Object s_lockExecutionObj = new object();

        public override void Execute()
        {
            lock (s_lockExecutionObj)//this locking is required to avoid simultanuous execution of multiple instances of the CachingDistributorRuntimeService
            {
                if (_runningProcessManager.TryLockRuntimeService(SERVICE_TYPE_UNIQUE_NAME))
                {
                    if (CachingDistributor.s_current == null)
                    {
                        var cachingDistributor = new CachingDistributor();
                        cachingDistributor.Initialize();
                        CachingDistributor.s_current = cachingDistributor;
                    }
                    else
                    {
                        CachingDistributor.s_current.SyncServiceRuntimeProcesses();
                    }
                }
                else
                {
                    CachingDistributor.s_current = null;
                }
            }
        }

        public override Guid ConfigId
        {
            get { return new Guid("36B8C9C3-32D6-4232-859E-DBCC7E7E2B66"); }
        }
    }
}
