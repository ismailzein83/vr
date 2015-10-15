﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;

namespace Vanrise.Rules.Web.Controllers
{
    public abstract class BaseRuleController<T,Q> : Vanrise.Web.Base.BaseAPIController where T : BaseRule where Q : RuleManager<T>
    {

        [HttpPost]
        [Route("AddRule")]
        public InsertOperationOutput<T> AddRule(T rule)
        {
            Q manager = Activator.CreateInstance<Q>();
            return manager.AddRule(rule);
        }

        [HttpPost]
        [Route("UpdateRule")]
        public Vanrise.Entities.UpdateOperationOutput<T> UpdateRule(T rule)
        {
            Q manager = Activator.CreateInstance<Q>();
            return manager.UpdateRule(rule);
        }

        [HttpPost]
        [Route("DeleteRule")]
        public Vanrise.Entities.DeleteOperationOutput<T> DeleteRule(int ruleId)
        {
            Q manager = Activator.CreateInstance<Q>();
            return manager.DeleteRule(ruleId);
        }

        [HttpGet]
        [Route("GetRule")]
        public T GetRule(int ruleId)
        {
            Q manager = Activator.CreateInstance<Q>();
            return manager.GetRule(ruleId);
        }
    }
}