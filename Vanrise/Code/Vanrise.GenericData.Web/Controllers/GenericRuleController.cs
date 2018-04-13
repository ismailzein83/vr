using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericRule")]
    public class GenericRuleController : Vanrise.Web.Base.BaseAPIController
    {
        GenericRuleDefinitionManager _manager = new GenericRuleDefinitionManager();

        [HttpPost]
        [Route("GetFilteredGenericRules")]
        public object GetFilteredGenericRules(Vanrise.Entities.DataRetrievalInput<GenericRuleQuery> input)
        {
            if (!_manager.DoesUserHaveViewAccess(input.Query.RuleDefinitionId))
                return GetUnauthorizedResponse();

            var manager = _manager.GetManager(input.Query.RuleDefinitionId);
            return GetWebResponse(input, manager.GetFilteredRules(input));
        }

        [HttpGet]
        [Route("GetGenericRule")]
        public GenericRule GetGenericRule(Guid ruleDefinitionId, int ruleId)
        {
            var manager = _manager.GetManager(ruleDefinitionId);
            return manager.GetGenericRule(ruleId, true);
        }

        [HttpPost]
        [Route("AddGenericRule")]
        public object AddGenericRule(GenericRule rule)
        {
            if (!DoesUserHaveAddAccess(rule.DefinitionId))
             return   GetUnauthorizedResponse();

            var manager = _manager.GetManager(rule.DefinitionId);
            return manager.AddGenericRule(rule);
        }

        [HttpGet]
        [Route("DoesUserHaveAddAccess")]
        public bool DoesUserHaveAddAccess(Guid ruleDefinitionId)
        {
            return _manager.DoesUserHaveAddAccess(ruleDefinitionId);
        }

        [HttpPost]
        [Route("UpdateGenericRule")]
        public object UpdateGenericRule(GenericRule rule)
        {
            if (!DoesUserHaveEditAccess(rule.DefinitionId))
                return GetUnauthorizedResponse();

            var manager = _manager.GetManager(rule.DefinitionId);
            return manager.UpdateGenericRule(rule);
           
        }

        [HttpGet]
        [Route("DoesUserHaveEditAccess")]
        public bool DoesUserHaveEditAccess(Guid ruleDefinitionId)
        {
            return _manager.DoesUserHaveEditAccess(ruleDefinitionId);
        }
        
        [HttpGet]
        [Route("DoesRuleSupportUpload")]
        public bool DoesRuleSupportUpload(Guid ruleDefinitionId)
        {
            return _manager.DoesRuleSupportUpload(ruleDefinitionId);
        }

        [HttpPost]
        [Route("DeleteGenericRule")]
        public Vanrise.Entities.DeleteOperationOutput<GenericRuleDetail> DeleteGenericRule(GenericRule rule)
        {
            var manager = _manager.GetManager(rule.DefinitionId);
            return manager.DeleteGenericRule(rule.RuleId);
        }

        [HttpPost]
        [Route("DownloadGenericRulesTemplate")]
        public object DownloadGenericRulesTemplate(DownloadUploadTemplateInput input)
        {
            Guid ruleDefinitionId = input.RuleDefinitionId;
            List<string> criteriaFieldsToHide = input.CriteriaFieldsToHide;
            GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
            byte[] bytes = ruleDefinitionManager.DownloadGenericRulesTemplate(ruleDefinitionId, criteriaFieldsToHide);
            MemoryStream memStreamRate = new System.IO.MemoryStream();
            memStreamRate.Write(bytes, 0, bytes.Length);
            memStreamRate.Seek(0, System.IO.SeekOrigin.Begin);
            return GetExcelResponse(memStreamRate, "UploadGenericRulesTemplate.xls");
        }

        [HttpPost]
        [Route("UploadGenericRules")]
        public UploadGenericRulesOutput UploadGenericRules(UploadGenericRulesInput uploadInput)
        {
            return _manager.UploadGenericRules(uploadInput);
        }

        [HttpGet]
        [Route("DownloadUploadGenericRulesOutput")]
        public object DownloadUploadGenericRulesOutput(long fileId)
        {
            GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
            byte[] bytes = ruleDefinitionManager.DownloadUploadGenericRulesOutput(fileId);
            MemoryStream memStreamRate = new System.IO.MemoryStream();
            memStreamRate.Write(bytes, 0, bytes.Length);
            memStreamRate.Seek(0, System.IO.SeekOrigin.Begin);
            return GetExcelResponse(memStreamRate, "GenericRulesUploadOutput.xls");
        }
    }
}