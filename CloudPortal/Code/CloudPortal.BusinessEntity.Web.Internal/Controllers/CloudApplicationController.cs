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
    [RoutePrefix(Constants.ROUTE_PREFIX + "CloudApplication")]
    public class CloudApplicationController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredCloudApplications")]
        public object GetFilteredCloudApplications(Vanrise.Entities.DataRetrievalInput<CloudApplicationQuery> input)
        {
            CloudApplicationManager manager = new CloudApplicationManager();
            return GetWebResponse(input, manager.GetFilteredCloudApplications(input));
        }

        [HttpPost]
        [Route("AddCloudApplication")]
        public Vanrise.Entities.InsertOperationOutput<CloudApplication> AddCloudApplication(CloudApplicationToAdd cloudApplication)
        {
            CloudApplicationManager manager = new CloudApplicationManager();
            return manager.AddCloudApplication(cloudApplication);
        }

        [HttpPost]
        [Route("UpdateCloudApplication")]
        public Vanrise.Entities.UpdateOperationOutput<CloudApplication> UpdateCloudApplication(CloudApplicationToUpdate cloudApplication)
        {
            CloudApplicationManager manager = new CloudApplicationManager();
            return manager.UpdateCloudApplication(cloudApplication);
        }


        [HttpGet]
        [Route("GetCloudApplication")]
        public CloudApplication GetCloudApplication(int id)
        {
            CloudApplicationManager manager = new CloudApplicationManager();
            return manager.GetCloudApplication(id);
        }
    }
}