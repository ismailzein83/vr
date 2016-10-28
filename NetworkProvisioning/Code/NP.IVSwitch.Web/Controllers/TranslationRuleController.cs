using NP.IVSwitch.Business;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;


namespace NP.IVSwitch.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "TranslationRule")]  
    [JSONWithTypeAttribute]
    public class TranslationRuleController : BaseAPIController
    {
        TranslationRuleManager _manager = new TranslationRuleManager();

        [HttpPost]
        [Route("GetFilteredTranslationRules")]
        public object GetFilteredTranslationRules(Vanrise.Entities.DataRetrievalInput<TranslationRuleQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredTranslationRules(input));
        }

        [HttpGet]
        [Route("GetTranslationRule")]
        public TranslationRule GetTranslationRule(int translationRuleId)
        {
            return _manager.GetTranslationRule(translationRuleId);
        }

        [HttpPost]
        [Route("AddTranslationRule")]
        public Vanrise.Entities.InsertOperationOutput<TranslationRuleDetail> AddTranslationRule(TranslationRule translationRuleItem)
        {
            return _manager.AddTranslationRule(translationRuleItem);
        }

        [HttpPost]
        [Route("UpdateTranslationRule")]
        public Vanrise.Entities.UpdateOperationOutput<TranslationRuleDetail> UpdateTranslationRule(TranslationRule translationRuleItem)
        {
            return _manager.UpdateTranslationRule(translationRuleItem);
        }

    
    }
}