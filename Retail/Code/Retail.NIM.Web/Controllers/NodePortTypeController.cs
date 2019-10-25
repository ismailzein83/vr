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
    [RoutePrefix(Constants.ROUTE_PREFIX + "NodePortType")]

    public class NodePortTypeController : BaseAPIController
    {
        NodePortTypeManager nodePortTypeManager = new NodePortTypeManager();

        [HttpGet]
        [Route("GetNodePortTypeInfo")]
        public IEnumerable<NodePortTypeInfo> GetNodePortTypeInfo(string filter = null)
        {
            NodePortTypeInfoFilter nodePortTypeInfoFilter = !string.IsNullOrEmpty(filter) ? Serializer.Deserialize<NodePortTypeInfoFilter>(filter) : null;
            return nodePortTypeManager.GetNodePortTypeInfo(nodePortTypeInfoFilter);
        }
    }
}