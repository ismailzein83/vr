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

        [HttpGet]
        [Route("GetTranslationRulesInfo")]
        public IEnumerable<TranslationRuleInfo> GetTranslationRulesInfo(string filter = null)
        {
            TranslationRuleFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<TranslationRuleFilter>(filter) : null;
            return _manager.GetTranslationRulesInfo(deserializedFilter);
        }  

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
            return _manager.GetTranslationRule(translationRuleId,true);
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

        [HttpGet]
        [Route("DeleteTranslationRule")]
        public Vanrise.Entities.DeleteOperationOutput<TranslationRuleDetail> DeleteTranslationRule(int translationRuleId)
        {
            return _manager.DeleteTranslationRule(translationRuleId);
        }

        [HttpGet]
        [Route("GetTranslationRuleHistoryDetailbyHistoryId")]
        public TranslationRule GetTranslationRuleHistoryDetailbyHistoryId(int translationRuleHistoryId)
        {
            return _manager.GetTranslationRuleHistoryDetailbyHistoryId(translationRuleHistoryId);
        }


        [HttpPost]
        [Route("GetFilteredCLIPatterns")]
        public object GetFilteredCLIPatterns(Vanrise.Entities.DataRetrievalInput<CLIPatternQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredCLIPatterns(input));
        }
    
    }
}