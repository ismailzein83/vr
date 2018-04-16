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
    [RoutePrefix(Constants.ROUTE_PREFIX + "SMSMessageType")]
    [JSONWithTypeAttribute]
    public class SMSMessageTypeController : BaseAPIController
    {
        SMSMessageTypeManager _manager = new SMSMessageTypeManager();

        [HttpGet]
        [Route("GetSMSMessageTypesInfo")]
        public IEnumerable<SMSMessageTypeInfo> GetSMSMessageTypesInfo(string filter = null)
        {
            SMSMessageTypeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<SMSMessageTypeInfoFilter>(filter) : null;
            return _manager.GetSMSMessageTypeIfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetSMSMessageType")]
        public SMSMessageTypeSettings GetSMSMessageType(Guid SMSMessageTypeId)
        {
            return _manager.GetSMSMessageTypeSettings(SMSMessageTypeId);
        }
    }
}