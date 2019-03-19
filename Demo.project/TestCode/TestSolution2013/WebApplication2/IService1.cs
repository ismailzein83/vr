using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WebApplication2
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(Namespace="http://vanrise.bp.services")]
    public interface IService1
    {
        [OperationContract]
        string DoWork();

        [OperationContract]
        TestMethodOutput TestMethod2(TestMethodInput input, TestMethodInput input2);
    }
}
