using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace CloudPortal.BusinessEntity.Business
{
    public class CloudApplicationServiceProxy : ICloudApplicationService
    {
        CloudApplication _application;

        public CloudApplicationServiceProxy(CloudApplication application)
        {
            if (application == null)
                throw new ArgumentNullException("application");
            if (application.Settings == null)
                throw new ArgumentNullException("application.Settings");
            this._application = application;
        }

        private Q Post<T, Q>(string actionName, T request)
        {
            string actionPath = string.Format("/api/Security_Internal/CloudApplicationService/{0}", actionName);
            return Vanrise.Common.VRWebAPIClient.Post<T, Q>(this._application.Settings.InternalURL, actionPath, request);
        }

        public ConfigureAuthServerOutput ConfigureAuthServer(ConfigureAuthServerInput input)
        {
            return Post<ConfigureAuthServerInput, ConfigureAuthServerOutput>("ConfigureAuthServer", input);
        }

        public UpdateAuthServerOutput UpdateAuthServer(UpdateAuthServerInput input)
        {
            return Post<UpdateAuthServerInput, UpdateAuthServerOutput>("UpdateAuthServer", input);
        }

        public AssignUserFullControlOutput AssignUserFullControl(AssignUserFullControlInput input)
        {
            return Post<AssignUserFullControlInput, AssignUserFullControlOutput>("AssignUserFullControl", input);
        }
    }
}
