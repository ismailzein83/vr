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
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSJazzMarketCode")]
    public class WhSJazzMarketCodeController : BaseAPIController
    {
        WhsJazzMarketCodeManager _manager = new WhsJazzMarketCodeManager();

        [HttpGet]
        [Route("GetMarketCodesInfo")]
        public IEnumerable<WhSJazzMarketCodeDetail> GetMarketCodesInfo(string filter=null)
        {
            WhSJazzMarketCodeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<WhSJazzMarketCodeInfoFilter>(filter) : null;
            return _manager.GetMarketCodesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllMarketCodes")]
        public IEnumerable<WhSJazzMarketCode> GetAllMarketCodes()
        {
            return _manager.GetAllMarketCodes();
        }
    }
}