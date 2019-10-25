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
    [RoutePrefix(Constants.ROUTE_PREFIX + "NodeType")]

    public class NodeTypeController: BaseAPIController
    {
        NodeTypeManager nodeTypeManager = new NodeTypeManager();

        [HttpGet]
        [Route("GetNodeTypeInfo")]
        public IEnumerable<NodeTypeInfo> GetNodeTypeInfo(string filter = null)
        {
            NodeTypeInfoFilter nodeTypeInfoFilter = !string.IsNullOrEmpty(filter) ? Serializer.Deserialize<NodeTypeInfoFilter>(filter) : null;
            return nodeTypeManager.GetNodeTypeInfo(nodeTypeInfoFilter);
        }
    }
}