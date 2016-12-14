using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Business
{
    [ServiceContract(Namespace = "http://common.vanrise.com/IBigDataWCFService")]
    internal interface IBigDataWCFService
    {
        [OperationContract]
        string RetrieveData(string serializedInput);
    }

    internal class BigDataWCFService : IBigDataWCFService
    {
        public string RetrieveData(string serializedRequest)
        {
            IBigDataRequest request = Vanrise.Common.Serializer.Deserialize(serializedRequest) as IBigDataRequest;
            Vanrise.Security.Entities.ContextFactory.GetContext().SetContextUserId(request.UserId);
            return request.RetrieveData();
        }
    }
}
