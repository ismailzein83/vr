using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RDBDataType")]
    [JSONWithTypeAttribute]
    public class RDBDataTypeController : BaseAPIController
    {
        [HttpGet]
        [Route("GetRDBDataTypeInfo")]
        public IEnumerable<RDBDataTypeInfo> GetRDBDataTypeInfo()
        {
            RDBDataTypeManager manager = new RDBDataTypeManager();
            return manager.GetRDBDataTypeInfo();
        }
    }
}