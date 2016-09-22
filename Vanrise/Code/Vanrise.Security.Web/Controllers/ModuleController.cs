using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Security.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Module")]
    public class ModuleController : Vanrise.Web.Base.BaseAPIController
    {
        private ModuleManager _manager;
        public ModuleController()
        {
            this._manager = new ModuleManager();
        }

        [HttpGet]
        [Route("GetModule")]
        public Module GetModule(Guid moduleId)
        {
            return _manager.GetModule(moduleId);
        }
        [HttpGet]
        [Route("GetModules")]
        public List<Module> GetModules()
        {
            return _manager.GetModules();
        }
        [HttpPost]
        [Route("UpdateModule")]
        public Vanrise.Entities.UpdateOperationOutput<ModuleDetail> UpdateModule(Module moduleObject)
        {
            return _manager.UpdateModule(moduleObject);
        }

        [HttpPost]
        [Route("AddModule")]
        public Vanrise.Entities.InsertOperationOutput<ModuleDetail> AddModule(Module moduleObject)
        {
            return _manager.AddModule(moduleObject);
        }
    }
}