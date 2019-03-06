using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SMSServiceType")]
    public class SMSServiceTypeController : BaseAPIController
    {
        [HttpGet]
        [Route("GetSMSServicesTypesInfo")]
        public IEnumerable<SMSServiceTypeInfo> GetSMSServicesTypesInfo(string serializedFilter = null)
        {
            SMSServiceTypeManager manager = new SMSServiceTypeManager();
            SMSServiceTypeFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<SMSServiceTypeFilter>(serializedFilter) : null;
            return manager.GetSMSServicesTypeInfo(filter);
        }
    }
}