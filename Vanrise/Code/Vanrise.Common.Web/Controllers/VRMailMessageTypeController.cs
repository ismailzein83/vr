using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers 
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRMailMessageType")]
    [JSONWithTypeAttribute]
    public class VRMailMessageTypeController : BaseAPIController
    {

        VRMailMessageTypeManager _manager = new VRMailMessageTypeManager();

        [HttpPost]
        [Route("GetFilteredMailMessageTypes")]
        public object GetFilteredMailMessageTypes(Vanrise.Entities.DataRetrievalInput<VRMailMessageTypeQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRMailMessageTypes(input));
        }

        [HttpGet]
        [Route("GetMailMessageType")]
        public VRMailMessageType GetMailMessageType(Guid VRMailMessageTypeId)
        {
            return _manager.GetMailMessageType(VRMailMessageTypeId);
        }

        [HttpPost]
        [Route("AddMailMessageType")]
        public Vanrise.Entities.InsertOperationOutput<VRMailMessageTypeDetail> AddMailMessageType(VRMailMessageType vrMailMessageTypeItem)
        {
            return _manager.AddVRMailMessageType(vrMailMessageTypeItem);
        }

        [HttpPost]
        [Route("UpdateMailMessageType")]
        public Vanrise.Entities.UpdateOperationOutput<VRMailMessageTypeDetail> UpdateMailMessageType(VRMailMessageType vrMailMessageTypeItem)
        {
            return _manager.UpdateVRMailMessageType(vrMailMessageTypeItem);
        }
    }
}