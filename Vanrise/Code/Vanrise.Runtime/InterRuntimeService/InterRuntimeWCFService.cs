using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    public class InterRuntimeWCFService : IInterRuntimeWCFService
    {
        public string ExecuteRequest(string request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            object deserializedRequest = Common.Serializer.Deserialize(request);
            if (deserializedRequest == null)
                throw new NullReferenceException("deserializedRequest");
            InterRuntimeServiceRequest interRuntimeRequest = deserializedRequest as InterRuntimeServiceRequest;
            if (interRuntimeRequest == null)
                throw new Exception(String.Format("interRuntimeRequest is not of type InterRuntimeServiceRequest. it is of type {0}", deserializedRequest.GetType()));
            var response = interRuntimeRequest.CallExecute();
            return response != null ? Common.Serializer.Serialize(response) : null;
        }
    }
}
