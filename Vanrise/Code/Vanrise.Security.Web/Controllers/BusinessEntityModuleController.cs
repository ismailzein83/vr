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
    [RoutePrefix(Constants.ROUTE_PREFIX + "BusinessEntityModule")]
    public class BusinessEntityModuleController:BaseAPIController
    {
        BusinessEntityModuleManager _manager = new BusinessEntityModuleManager();

        [HttpGet]
        [Route("GetBusinessEntityModuleById")]
        public BusinessEntityModule GetBusinessEntityModuleById(Guid moduleId)
        {
            return _manager.GetBusinessEntityModuleById(moduleId);
        }
        [HttpPost]
        [Route("UpdateBusinessEntityModule")]
        public Vanrise.Entities.UpdateOperationOutput<BusinessEntityModule> UpdateBusinessEntityModule(BusinessEntityModule moduleObject)
        {
            return _manager.UpdateBusinessEntityModule(moduleObject);
        }

        [HttpPost]
        [Route("AddBusinessEntityModule")]
        public Vanrise.Entities.InsertOperationOutput<BusinessEntityModule> AddBusinessEntityModule(BusinessEntityModule moduleObject)
        {
            return _manager.AddBusinessEntityModule(moduleObject);
        }
    }
}