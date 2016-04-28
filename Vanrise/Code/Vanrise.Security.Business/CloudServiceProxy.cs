using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class CloudServiceProxy : ICloudService
    {
        CloudAuthServer _authServer;
        public CloudServiceProxy(CloudAuthServer authServer)
        {
            if (authServer == null)
                throw new ArgumentNullException("authServer");
            if (authServer.Settings == null)
                throw new ArgumentNullException("authServer.Settings");
            this._authServer = authServer;
        }
        private Q Post<T, Q>(string actionName, T request)
        {
            string actionPath = string.Format("/api/CloudPortal_BEInternal/CloudService/{0}", actionName);
            using (var client = new HttpClient())
            {
                // New code:
                client.BaseAddress = new Uri(this._authServer.Settings.InternalURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add(this._authServer.Settings.CloudServiceHTTPHeaderName, Vanrise.Common.Serializer.Serialize(this._authServer.ApplicationIdentification));
                var responseTask = client.PostAsJsonAsync(actionPath, request);
                responseTask.Wait();
                if (responseTask.Exception != null)
                    throw responseTask.Exception;
                if(responseTask.Result.IsSuccessStatusCode)
                {
                    var rsltTask = responseTask.Result.Content.ReadAsAsync<Q>();
                    rsltTask.Wait();
                    if (rsltTask.Exception != null)
                        throw rsltTask.Exception;
                    return rsltTask.Result;
                }
            }
            return default(Q);
        }

        public GetApplicationUsersOutput GetApplicationUsers(GetApplicationUsersInput input)
        {
            return Post<GetApplicationUsersInput, GetApplicationUsersOutput>("GetApplicationUsers", input);
        }

        public CheckApplicationUsersUpdatedOuput CheckApplicationUsersUpdated(CheckApplicationUsersUpdatedInput input)
        {
            return Post<CheckApplicationUsersUpdatedInput, CheckApplicationUsersUpdatedOuput>("CheckApplicationUsersUpdated", input);
        }

        public AddUserToApplicationOutput AddUserToApplication(AddUserToApplicationInput input)
        {
            return Post<AddUserToApplicationInput, AddUserToApplicationOutput>("AddUserToApplication", input);
        }
    }
}
