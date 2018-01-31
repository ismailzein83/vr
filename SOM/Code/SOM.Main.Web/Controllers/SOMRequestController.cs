using SOM.Main.Business;
using SOM.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace SOM.Main.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SOMRequest")]
    public class SOMRequestController : BaseAPIController
    {
        static SOMRequestManager s_manager = new SOMRequestManager();

        [HttpPost]
        [Route("CreateSOMRequest")]
        public CreateSOMRequestOutput CreateSOMRequest(CreateSOMRequestInput input)
        {
            return s_manager.CreateSOMRequest(input);
        }

        [HttpPost]
        [Route("CreateLineSubscriptionRequest")]
        public CreateSOMRequestOutput CreateLineSubscriptionRequest(SOM.Main.BP.Arguments.CreateLineSubscriptionInput input)
        {
            CreateSOMRequestInput createSOMRequestInput = new CreateSOMRequestInput
            {
                EntityId = input.EntityId,
                RequestTitle = input.RequestTitle,
                SOMRequestId = input.SOMRequestId,
                Settings = new SOMRequestSettings
                {
                    ExtendedSettings = input.RequestDetails
                }
            };
            return s_manager.CreateSOMRequest(createSOMRequestInput);
        }

        [HttpPost]
        [Route("GetFilteredSOMRequests")]
        public object GetFilteredSOMRequests(Vanrise.Entities.DataRetrievalInput<SOMRequestQuery> input)
        {
            return GetWebResponse(input, s_manager.GetFilteredSOMRequests(input)); 
        }

        [HttpGet]
        [Route("GetRecentSOMRequestHeaders")]
        public List<SOMRequestHeader> GetRecentSOMRequestHeaders(string entityId, int nbOfRecords, long? lessThanSequenceNb)
        {
            return s_manager.GetRecentSOMRequestHeaders(entityId, nbOfRecords, lessThanSequenceNb);
        }

        [HttpGet]
        [Route("GetSOMRequestLogs")]
        public List<SOMRequestLog> GetSOMRequestLogs(Guid somRequestId, int nbOfRecords, long? lessThanId)
        {
            return s_manager.GetSOMRequestLogs(somRequestId, nbOfRecords, lessThanId);
        }
    }
}