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
    [RoutePrefix(Constants.ROUTE_PREFIX + "NodePartType")]

    public class NodePartTypeController : BaseAPIController
    {
        NodePartTypeManager nodePartTypeManager = new NodePartTypeManager();

        [HttpGet]
        [Route("GetNodePartTypeInfo")]
        public IEnumerable<NodePartTypeInfo> GetNodePartTypeInfo(string filter = null)
        {
            NodePartTypeInfoFilter nodePartTypeInfoFilter = !string.IsNullOrEmpty(filter) ? Serializer.Deserialize<NodePartTypeInfoFilter>(filter) : null;
            return nodePartTypeManager.GetNodePartTypeInfo(nodePartTypeInfoFilter);
        }
    }
}