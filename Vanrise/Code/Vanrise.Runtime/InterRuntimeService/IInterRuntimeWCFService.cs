using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime
{
    [ServiceContract(Namespace = "http://runtime.vanrise.com/IInterRuntimeWCFService")]
    public interface IInterRuntimeWCFService
    {
        [OperationContract]
        string ExecuteRequest(string request);
    }
}
