using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBPService" in both code and config file together.
    [ServiceContract(Namespace="http://businessprocess.vanrise.com/IBPService")]
    public interface IBPService
    {
        [OperationContract]
        CreateProcessOutput CreateNewProcess(string serializedInput);

        [OperationContract]
        TriggerProcessEventOutput TriggerProcessEvent(string serializedInput);

        [OperationContract]
        BPInstance GetInstance(long processInstanceId);
    }
}
