using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericRule")]
    public class GenericRuleController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("AddGenericRule")]
        public Vanrise.Entities.InsertOperationOutput<GenericRule> AddGenericRule(GenericRule rule)
        {
            var manager = GetManager(rule);
            return manager.AddGenericRule(rule);
        }

        IGenericRuleManager GetManager(GenericRule rule)
        {
            Type managerType = Type.GetType("Vanrise.GenericData.Transformation.MappingRuleManager,Vanrise.GenericData.Transformation");
            return Activator.CreateInstance(managerType) as IGenericRuleManager;
        }
    }
}