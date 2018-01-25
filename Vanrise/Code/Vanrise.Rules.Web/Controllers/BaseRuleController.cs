using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;

namespace Vanrise.Rules.Web.Controllers
{
    public abstract class BaseRuleController<T, Q> : BaseRuleController<T,T,Q>
        where T : BaseRule
        where Q : RuleManager<T>
    {
    }

    public abstract class BaseRuleController<T, Q, R> : Vanrise.Web.Base.BaseAPIController
        where T : BaseRule
        where Q : class
        where R : RuleManager<T, Q>
    {

        [HttpPost]
        [Route("AddRule")]
        public InsertOperationOutput<Q> AddRule(T rule)
        {
            R manager = Activator.CreateInstance<R>();
            return manager.AddRule(rule);
        }

        [HttpPost]
        [Route("UpdateRule")]
        public Vanrise.Entities.UpdateOperationOutput<Q> UpdateRule(T rule)
        {
            R manager = Activator.CreateInstance<R>();
            return manager.UpdateRule(rule);
        }

        [HttpPost]
        [Route("DeleteRule")]
        public Vanrise.Entities.DeleteOperationOutput<Q> DeleteRule(int ruleId)
        {
            R manager = Activator.CreateInstance<R>();
            return manager.DeleteRule(ruleId);
        }

        [HttpPost]
        [Route("SetRuleDeleted")]
        public Vanrise.Entities.DeleteOperationOutput<Q> SetRuleDeleted(List<int> ruleIds)
        {
            R manager = Activator.CreateInstance<R>();
            return manager.SetDeleted(ruleIds);
        }

        [HttpGet]
        [Route("GetRule")]
        public T GetRule(int ruleId)
        {
            R manager = Activator.CreateInstance<R>();
            return manager.GetRule(ruleId, true);
        }
    }
}