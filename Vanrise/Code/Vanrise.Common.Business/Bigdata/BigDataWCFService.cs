using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Business
{
    public interface IBigDataWCFService
    {
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
