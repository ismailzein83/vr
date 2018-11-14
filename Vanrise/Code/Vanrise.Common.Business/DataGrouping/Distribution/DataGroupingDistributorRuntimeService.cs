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
        internal const string SERVICE_TYPE_UNIQUE_NAME = "VR_Common_DataGroupingDistributorRuntimeService";

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
            context.ServiceInstanceInfo = new DataGroupingDistributorServiceInstanceInfo
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
                    VRInterAppCommunication.RegisterService(typeof(DataGroupingDistributorWCFService), typeof(IDataGroupingDistributorWCFService), out s_serviceURL);
                    s_isServiceRegistered = true;
                }
            }
        }
        
        public override void Execute()
        {            
        }

        public override Guid ConfigId { get { return new Guid("49817A1B-4D9D-4F62-B5A5-F3D5AFDA0487"); } }
    }

    internal class DataGroupingDistributorServiceInstanceInfo : Vanrise.Runtime.Entities.ServiceInstanceInfo
    {
        public string TCPServiceURL { get; set; }
    }
}
