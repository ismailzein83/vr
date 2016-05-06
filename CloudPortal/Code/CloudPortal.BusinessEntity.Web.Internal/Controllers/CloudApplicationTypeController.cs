using CloudPortal.BusinessEntity.Business;
using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace CloudPortal.BusinessEntity.Web.Internal.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CloudApplicationType")]
    public class CloudApplicationTypeController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredCloudApplicationTypes")]
        public object GetFilteredCloudApplicationTypes(Vanrise.Entities.DataRetrievalInput<CloudApplicationTypeQuery> input)
        {
            CloudApplicationTypeManager manager = new CloudApplicationTypeManager();
            return GetWebResponse(input, manager.GetFilteredCloudApplicationTypes(input));
        }

        [HttpGet]
        [Route("GetCloudApplicationTypesInfo")]
        public IEnumerable<CloudApplicationType> GetCloudApplicationTypesInfo(string serializedFilter)
        {
            CloudApplicationTypeFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<CloudApplicationTypeFilter>(serializedFilter) : null;
            CloudApplicationTypeManager manager = new CloudApplicationTypeManager();
            return manager.GetCloudApplicationTypesInfo(filter);
        }

        [HttpPost]
        [Route("AddCloudApplicationType")]
        public Vanrise.Entities.InsertOperationOutput<CloudApplicationType> AddCloudApplicationType(CloudApplicationType cloudApplicationType)
        {
            CloudApplicationTypeManager manager = new CloudApplicationTypeManager();
            return manager.AddCloudApplicationType(cloudApplicationType);
        }

        [HttpPost]
        [Route("UpdateCloudApplicationType")]
        public Vanrise.Entities.UpdateOperationOutput<CloudApplicationType> UpdateCloudApplicationType(CloudApplicationType cloudApplicationType)
        {
            CloudApplicationTypeManager manager = new CloudApplicationTypeManager();
            return manager.UpdateCloudApplicationType(cloudApplicationType);
        }


        [HttpGet]
        [Route("GetCloudApplicationType")]
        public CloudApplicationType GetCloudApplicationType(int id)
        {
            CloudApplicationTypeManager manager = new CloudApplicationTypeManager();
            return manager.GetCloudApplicationType(id);
        }
    }
}