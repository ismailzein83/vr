using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Runtime.Entities;
using Vanrise.Web.Base;
using Vanrise.Runtime;
using Vanrise.Runtime.Business;
using Vanrise.Entities;

namespace Vanrise.Runtime.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RuntimeNode")]
    [JSONWithTypeAttribute]
    public class VRRuntime_RuntimeNodeController : BaseAPIController
    {
        RuntimeNodeManager manager = new RuntimeNodeManager();


        [HttpGet]
        [Route("GetNode")]
        public RuntimeNode GetNode(Guid nodeId)
        {
            return manager.GetNode(nodeId);
        }

        [HttpGet]
        [Route("GetAllNodes")]
        public List<RuntimeNode> GetAllNodesStates()
        {
            return manager.GetAllNodes();
        }

        [HttpPost]
        [Route("AddRuntimeNode")]
        public Vanrise.Entities.InsertOperationOutput<RuntimeNodeDetails> AddRuntimeNode(RuntimeNode node)
        {
            return manager.AddRuntimeNode(node);
        }

        [HttpPost]
        [Route("UpdateRuntimeNode")]
        public Vanrise.Entities.UpdateOperationOutput<RuntimeNodeDetails> UpdateRuntimeNode(RuntimeNode node)
        {
            return manager.UpdateRuntimeNode(node);
        }
    }
}