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


        //[HttpGet]
        //[Route("GetNodeConfiguration")]
        //public RuntimeNode GetNode(Guid nodeId)
        //{
        //    return manager.GetNode(nodeId);
        //}
        [HttpGet]
        [Route("GetAllNodes")]
        public Dictionary<Guid, RuntimeNode> GetAllNodes()
        {
            return manager.GetAllNodes();
        }

        //[HttpPost]
        //[Route("AddRuntimeNodeConfiguration")]
        //public Vanrise.Entities.InsertOperationOutput<RuntimeNode> AddRuntimeNodeConfiguration(RuntimeNode node)
        //{
        //    return manager.AddRuntimeNode(node);
        //}

        //[HttpPost]
        //[Route("UpdateRuntimeNode")]
        //public Vanrise.Entities.UpdateOperationOutput<RuntimeNode> UpdateRuntimeNodeConfiguration(RuntimeNode node)
        //{
        //    return manager.UpdateRuntimeNode(node);
        //}
    }
}