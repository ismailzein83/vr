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
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "NodePart")]

    public class NodePartController : BaseAPIController
    {
        NodePartManager nodePartManager = new NodePartManager();

        [HttpGet]
        [Route("GetNodePartTree")]
        public NodePartTreeNode GetNodePartTree(long nodeId)
        {
            return nodePartManager.GetNodePartTree(nodeId);
        }
    }
}