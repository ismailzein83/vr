using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.Common.Business
{
    public class DataGroupingExecutorRuntimeService : RuntimeService
    {
        internal const string SERVICE_TYPE_UNIQUE_NAME = "VR_Common_DataGroupingExecutorRuntimeService";

        public override string ServiceTypeUniqueName
        {
            get
            {
                return SERVICE_TYPE_UNIQUE_NAME;
            }
        }

        public override void OnInitialized(IRuntimeServiceInitializeContext context)
        {
            HostServiceIfNeeded();
            context.ServiceInstanceInfo = new DataGroupingExecutorServiceInstanceInfo
            {
                TCPServiceURL = s_serviceURL
            };
            base.OnInitialized(context);
        }

        static bool s_isServiceRegistered;
        static string s_serviceURL;
        static Object s_hostServiceLockObject = new object();
        private void HostServiceIfNeeded()
        {
            lock (s_hostServiceLockObject)
            {
                if (!s_isServiceRegistered)
                {
                    VRInterAppCommunication.RegisterService(typeof(DataGroupingExecutorWCFService), typeof(IDataGroupingExecutorWCFService), out s_serviceURL);
                    s_isServiceRegistered = true;
                }
            }
        }
        
        public override void Execute()
        {
            DataGroupingExecutor.Current.ProcessItems();
        }

        public override Guid ConfigId { get { return new Guid("7B035997-0941-48F1-B37D-F14A039DDB3E"); } }
    }

    internal class DataGroupingExecutorServiceInstanceInfo : Vanrise.Runtime.Entities.ServiceInstanceInfo
    {
        public string TCPServiceURL { get; set; }
    }
}
