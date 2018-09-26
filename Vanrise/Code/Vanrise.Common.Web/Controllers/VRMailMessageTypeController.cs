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
            return GetWebResponse(input, _manager.GetFilteredMailMessageTypes(input), "Mail Message Types");
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
            return _manager.AddMailMessageType(vrMailMessageTypeItem);
        }

        [HttpPost]
        [Route("UpdateMailMessageType")]
        public Vanrise.Entities.UpdateOperationOutput<VRMailMessageTypeDetail> UpdateMailMessageType(VRMailMessageType vrMailMessageTypeItem)
        {
            return _manager.UpdateMailMessageType(vrMailMessageTypeItem);
        }

        [HttpGet]
        [Route("GetMailMessageTypesInfo")]
        public IEnumerable<VRMailMessageTypeInfo> GetMailMessageTypesInfo(string filter = null)
        {
            VRMailMessageTypeFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRMailMessageTypeFilter>(filter) : null;
            return _manager.GetMailMessageTypesInfo(deserializedFilter);
        }
    }
}