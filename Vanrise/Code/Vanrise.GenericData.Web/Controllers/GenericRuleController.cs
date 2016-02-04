using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Web.Controllers
{
    public class GenericRuleController : Vanrise.Web.Base.BaseAPIController
    {
        public Vanrise.Entities.InsertOperationOutput<GenericRule> AddRule(GenericRule rule)
        {
            GenericRuleManager<GenericRule> manager = new GenericRuleManager<GenericRule>();
            return manager.AddRule(rule);
        }
    }
}