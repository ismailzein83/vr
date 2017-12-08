using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Runtime.Data;

namespace Vanrise.Runtime
{
    internal static class RuntimeManagerClient
    {
        static string s_runtimeManagerServiceURL;
        public static void CreateClient(Action<IRuntimeManagerWCFService> onClientReady)
        {
            if (s_runtimeManagerServiceURL == null)
                s_runtimeManagerServiceURL = RuntimeDataManagerFactory.GetDataManager<IRuntimeManagerDataManager>().GetRuntimeManagerServiceURL();
            if (s_runtimeManagerServiceURL == null)
                throw new NullReferenceException("s_runtimeManagerServiceURL");
            try
            {
                ServiceClientFactory.CreateTCPServiceClient<IRuntimeManagerWCFService>(s_runtimeManagerServiceURL,
                    (client) =>
                    {
                        onClientReady(client);
                    });
            }
            catch
            {
                s_runtimeManagerServiceURL = null;
                throw;
            }
        }
    }
}
