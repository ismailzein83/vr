using Retail.NIM.Business;
using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common;
using Vanrise.Web.Base;

namespace Retail.NIM.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ConnectionType")]

    public class ConnectionTypeController : BaseAPIController
    {
        ConnectionTypeManager connectionTypeManager = new ConnectionTypeManager();

        [HttpGet]
        [Route("GetConnectionTypeInfo")]
        public IEnumerable<ConnectionTypeInfo> GetConnectionTypeInfo(string filter = null)
        {
            ConnectionTypeInfoFilter connectionTypeInfoFilter = !string.IsNullOrEmpty(filter) ? Serializer.Deserialize<ConnectionTypeInfoFilter>(filter) : null;
            return connectionTypeManager.GetConnectionTypeInfo(connectionTypeInfoFilter);
        }
    }
}