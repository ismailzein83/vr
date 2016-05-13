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
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(CloudAuthServer.CLOUDSERVICE_HTTPHEADERNAME, Vanrise.Common.Serializer.Serialize(this._authServer.ApplicationIdentification));
            return Vanrise.Common.VRWebAPIClient.Post<T, Q>(this._authServer.Settings.InternalURL, actionPath, request, headers);
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

        public UpdateUserToApplicationOutput UpdateUserToApplication(UpdateUserToApplicationInput input)
        {
            return Post<UpdateUserToApplicationInput, UpdateUserToApplicationOutput>("UpdateUserToApplication", input);
        }

        public GetApplicationTenantsOutput GetApplicationTenants(GetApplicationTenantsInput input)
        {
            return Post<GetApplicationTenantsInput, GetApplicationTenantsOutput>("GetApplicationTenants", input);
        }


        public CheckApplicationTenantsUpdatedOuput CheckApplicationTenantsUpdated(CheckApplicationTenantsUpdatedInput input)
        {
            return Post<CheckApplicationTenantsUpdatedInput, CheckApplicationTenantsUpdatedOuput>("CheckApplicationTenantsUpdated", input);
        }

        public AddTenantToApplicationOutput AddTenantToApplication(AddTenantToApplicationInput input)
        {
            return Post<AddTenantToApplicationInput, AddTenantToApplicationOutput>("AddTenantToApplication", input);
        }


        public UpdateTenantToApplicationOutput UpdateTenantToApplication(UpdateTenantToApplicationInput input)
        {
            return Post<UpdateTenantToApplicationInput, UpdateTenantToApplicationOutput>("UpdateTenantToApplication", input);
        }


        public List<CloudTenantOutput> GetCloudTenants(CloudTenantInput input)
        {
            return Post<CloudTenantInput, List<CloudTenantOutput>>("GetCloudTenants", input);
        }
    }
}