using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Analytic.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "MeasureStyleRuleController")]
    public class MeasureStyleRuleController : BaseAPIController
    {
        [HttpPost]
        [Route("GetMeasureStyleRuleEditorRuntime")]
        public MeasureStyleRuleEditorRuntime GetMeasureStyleRuleEditorRuntime(MeasureStuleRuleEditorRuntimeInput measureStuleRuleEditorRuntimeInput)
        {
            MeasureStyleRuleManager measureStyleRuleManager = new MeasureStyleRuleManager();
            return measureStyleRuleManager.GetMeasureStyleRuleEditorRuntime(measureStuleRuleEditorRuntimeInput.MeasureStyleRules, measureStuleRuleEditorRuntimeInput.AnalyticTableId);
        }
        [HttpPost]
        [Route("GetMeasureStyleRuleDetail")]
        public MeasureStyleRuleRuntime GetMeasureStyleRuleDetail(MeasureStyleRuleInput measureStyleRuleInput)
        {
            MeasureStyleRuleManager measureStyleRuleManager = new MeasureStyleRuleManager();
            return measureStyleRuleManager.GetMeasureStyleRuleDetail(measureStyleRuleInput.MeasureStyleRule, measureStyleRuleInput.AnalyticTableId);
        }
        public class MeasureStuleRuleEditorRuntimeInput
        {
            public Guid AnalyticTableId { get; set; }
            public List<MeasureStyleRule> MeasureStyleRules { get; set; }
        }
        public class MeasureStyleRuleInput

        {
            public Guid AnalyticTableId { get; set; }
            public MeasureStyleRule MeasureStyleRule { get; set; }
        }
    }


}