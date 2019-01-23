using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.Web.Base;
using System.Web.Http;
using Vanrise.Entities;
using TOne.WhS.Jazz.Web;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using TOne.WhS.Jazz.Business;
using TOne.WhS.Jazz.Entities;
using Vanrise.Common;
namespace TOne.WhS.Jazz.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSJazzCustomerTypeCode")]
    public class WhSJazzCustomerTypeCodeController : BaseAPIController
    {
        WhSJazzCustomerTypeCodeManager _manager = new WhSJazzCustomerTypeCodeManager();

        [HttpGet]
        [Route("GetCustomerTypeCodesInfo")]
        public IEnumerable<WhSJazzCustomerTypeCodeDetail> GetCustomerTypeCodesInfo(string filter=null)
        {
            WhSJazzCustomerTypeCodeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<WhSJazzCustomerTypeCodeInfoFilter>(filter) : null;
            return _manager.GetCustomerTypeCodesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllCustomerTypeCodes")]
        public IEnumerable<WhSJazzCustomerTypeCode> GetAllCustomerTypeCodes()
        {
            return _manager.GetAllCustomerTypeCodes();
        }
    }
}