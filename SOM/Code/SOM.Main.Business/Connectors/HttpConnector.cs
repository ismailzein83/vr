using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Common;
using Newtonsoft.Json;

namespace SOM.Main.Business
{
    public abstract class HttpConnector
    {
        //TODO: expose Http method parameter to be used by BSCS as XML and JSON for Activara
        //TODO: replace JSONCOnvert with Vanrise Serializer
        //TODO: change the way we are building the request from BPM Extended to SOM

        protected abstract Guid GetConnectionId();

        private VRHttpConnection _vrHttpConnection;

        static JsonSerializerSettings s_Settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All
        };

        public HttpConnector()
        {
            if (this.GetConnectionId() == Guid.Empty)
                throw new ArgumentException("ConnectionId is empty");

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection(this.GetConnectionId());
            this._vrHttpConnection = vrConnection.Settings.CastWithValidate<VRHttpConnection>("vrConnection.Settings", this.GetConnectionId());
        }

        public T Get<T>(string actionPath, Dictionary<string, string> parameters)
        {
            T response = default(T);

            Action<VRHttpResponse> onResponseReceived = x => {
                response = JsonConvert.DeserializeObject<T>(x.Response, s_Settings);
            };

            this._vrHttpConnection.TrySendRequest(actionPath, VRHttpMethod.Get, 
                VRHttpMessageFormat.ApplicationJSON, parameters, null, null, onResponseReceived, true, null);

            return response;
        }

        public Q POST<T, Q>(string actionPath, T data)
        {
            Q response = default(Q);

            Action<VRHttpResponse> onResponseReceived = x => {
                response = JsonConvert.DeserializeObject<Q>(x.Response, s_Settings);
            };

            string body = JsonConvert.SerializeObject(data, Formatting.None, s_Settings);

            this._vrHttpConnection.TrySendRequest(actionPath, VRHttpMethod.Post,
                VRHttpMessageFormat.ApplicationJSON, null, null, body, onResponseReceived, true, null);

            return response;
        }
    }
}
