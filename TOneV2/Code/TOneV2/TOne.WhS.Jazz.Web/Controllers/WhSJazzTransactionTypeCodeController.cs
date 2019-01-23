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
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSJazzTransactionTypeCode")]
    public class WhSJazzTransactionTypeCodeController : BaseAPIController
    {
        WhSJazzTransactionTypeCodeManager _manager = new WhSJazzTransactionTypeCodeManager();

        [HttpGet]
        [Route("GetTransactionTypeCodesInfo")]
        public IEnumerable<WhSJazzTransactionTypeCodeDetail> GetTransactionTypeCodesInfo(string filter=null)
        {
            WhSJazzTransactionTypeCodeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<WhSJazzTransactionTypeCodeInfoFilter>(filter) : null;
            return _manager.GetTransactionTypeCodesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllTransactionTypeCodes")]
        public IEnumerable<WhSJazzTransactionTypeCode> GetAllTransactionTypeCodes()
        {
            return _manager.GetAllTransactionTypeCodes();
        }
    }
}