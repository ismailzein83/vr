using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    public class InterRuntimeServiceManager
    {
        public T SendRequest<T>(int processId, InterRuntimeServiceRequest<T> request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            string serializedResponse = null;
            string serviceURL = new RunningProcessManager().GetProcessTCPServiceURL(processId);
            ServiceClientFactory.CreateTCPServiceClient<IInterRuntimeWCFService>(serviceURL,
                              (client) =>
                              {
                                  var serializedRequest = Common.Serializer.Serialize(request);
                                  serializedResponse = client.ExecuteRequest(serializedRequest);
                              });
            if (serializedResponse != null)
                return Common.Serializer.Deserialize<T>(serializedResponse);
            else
                return default(T);
        }
    }
}
