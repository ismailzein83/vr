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
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSJazzTaxCode")]
    public class WhSJazzTaxCodeController : BaseAPIController
    {
        WhSJazzTaxCodeManager _manager = new WhSJazzTaxCodeManager();

        [HttpGet]
        [Route("GetTaxCodesInfo")]
        public IEnumerable<WhSJazzTaxCodeDetail> GetTaxCodesInfo(string filter=null)
        {
            WhSJazzTaxCodeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<WhSJazzTaxCodeInfoFilter>(filter) : null;
            return _manager.GetTaxCodesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllTaxCodes")]
        public IEnumerable<WhSJazzTaxCode> GetAllTaxCodes()
        {
            return _manager.GetAllTaxCodes();
        }
    }
}