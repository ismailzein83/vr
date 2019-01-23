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
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSJazzProductServiceCode")]
    public class WhSJazzProductServiceCodeController : BaseAPIController
    {
        WhSJazzProductServiceCodeManager _manager = new WhSJazzProductServiceCodeManager();

        [HttpGet]
        [Route("GetProductServiceCodesInfo")]
        public IEnumerable<WhSJazzProductServiceCodeDetail> GetProductServiceCodesInfo(string filter=null)
        {
            WhSJazzProductServiceCodeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<WhSJazzProductServiceCodeInfoFilter>(filter) : null;
            return _manager.GetProductServiceCodesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllProductServiceCodes")]
        public IEnumerable<WhSJazzProductServiceCode> GetAllProductServiceCodes()
        {
            return _manager.GetAllProductServiceCodes();
        }
    }
}