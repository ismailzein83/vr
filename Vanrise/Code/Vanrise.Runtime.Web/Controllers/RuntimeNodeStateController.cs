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
    [RoutePrefix(Constants.ROUTE_PREFIX + "RuntimeNodeState")]
    [JSONWithTypeAttribute]
    public class VRRuntime_RuntimeNodeStateController : BaseAPIController
    {

        RuntimeNodeStateManager manager = new RuntimeNodeStateManager();

        [HttpGet]
        [Route("GetNodeState")]
        public RuntimeNodeState GetNodeState(Guid nodeId)
        {
            return manager.GetNodeState(nodeId);
        }

        [HttpGet]
        [Route("GetAllNodesStates")]
        public List<RuntimeNodeState> GetAllNodesStates()
        {
            return manager.GetAllNodes();
        }

        //[HttpPost]
        //[Route("AddRuntimeNode")]
        //public Vanrise.Entities.InsertOperationOutput<RuntimeNodeDetails> AddRuntimeNode(RuntimeNode node)
        //{
        //    return manager.AddRuntimeNode(node);
        //}

        //[HttpPost]
        //[Route("UpdateRuntimeNode")]
        //public Vanrise.Entities.UpdateOperationOutput<RuntimeNodeDetails> UpdateRuntimeNode(RuntimeNode node)
        //{
        //    return manager.UpdateRuntimeNode(node);
        //}
    }
}
