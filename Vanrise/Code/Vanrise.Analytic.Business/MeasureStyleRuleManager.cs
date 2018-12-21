using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Business
{
    public class MeasureStyleRuleManager
    {
        RecordFilterManager _recordFilterManager = new RecordFilterManager();

        public MeasureStyleRuleEditorRuntime GetMeasureStyleRuleEditorRuntime(List<MeasureStyleRule> measureStyleRules, Guid analyticTableId)
        {
            MeasureStyleRuleEditorRuntime measureStyleRuleEditorRuntime = new MeasureStyleRuleEditorRuntime();
            AnalyticItemConfigManager analyticItemConfigManager = new AnalyticItemConfigManager();

            Dictionary<string, AnalyticMeasure> analyticMeasuresByMeasureName = analyticItemConfigManager.GetMeasures(analyticTableId);
            if (analyticMeasuresByMeasureName == null)
                throw new Exception("analyticMeasuresByMeasureName");

            if (measureStyleRules != null && measureStyleRules.Count > 0)
            {
                measureStyleRuleEditorRuntime.MeasureStyleRulesRuntime = new List<MeasureStyleRuleRuntime>();
                
                foreach (var measureRule in measureStyleRules)
                {
                    measureStyleRuleEditorRuntime.MeasureStyleRulesRuntime.Add(GetMeasureStyleRuleDetail(measureRule, analyticMeasuresByMeasureName));
                }
            }
           
            return measureStyleRuleEditorRuntime;
        }
        public MeasureStyleRuleRuntime GetMeasureStyleRuleDetail(MeasureStyleRule measureStyleRule, Guid analyticTableId)
        {
            AnalyticItemConfigManager analyticItemConfigManager = new AnalyticItemConfigManager();
            Dictionary<string, AnalyticMeasure> analyticMeasuresByMeasureName = analyticItemConfigManager.GetMeasures(analyticTableId);
            return GetMeasureStyleRuleDetail(measureStyleRule, analyticMeasuresByMeasureName);
        }
        public MeasureStyleRuleRuntime GetMeasureStyleRuleDetail(MeasureStyleRule measureStyleRule, Dictionary<string, AnalyticMeasure> analyticMeasuresByMeasureName)
        {
            StatusDefinitionManager statusDefinitionManager = new StatusDefinitionManager();
         
            var recordFilterFieldInfosByFieldName = analyticMeasuresByMeasureName.ToDictionary(key => key.Key, value => new RecordFilterFieldInfo
            {
                Name = value.Key,
                Title = value.Value.Title,
                Type = value.Value.Config.FieldType
            });
            List<StyleRuleDetail> rules = new List<StyleRuleDetail>();

            foreach (var rule in measureStyleRule.Rules)
            {
                var statusDefinition = statusDefinitionManager.GetStatusDefinition(rule.StatusDefinitionId);
                statusDefinition.ThrowIfNull("statusDefinition");
                rules.Add(new StyleRuleDetail()
                {
                    RecordFilterDescription = _recordFilterManager.BuildRecordFilterGroupExpression(new RecordFilterGroup
                    {
                        Filters = new List<RecordFilter> {
                           rule.RecordFilter
                        },
                        LogicalOperator = RecordQueryLogicalOperator.And
                    }, recordFilterFieldInfosByFieldName),
                    RecordFilter = rule.RecordFilter,
                    StatusDefinitionId = rule.StatusDefinitionId,
                    StyleValueDescription = statusDefinition.Name
                });
            }
            AnalyticMeasure analyticMeasure = analyticMeasuresByMeasureName.GetRecord(measureStyleRule.MeasureName);
            analyticMeasure.ThrowIfNull("AnalyticMeasure");

            var recommendedStyleRuleDescription = _recordFilterManager.BuildRecordFilterGroupExpression(new RecordFilterGroup
            {
                Filters = measureStyleRule.RecommendedStyleRule,
                LogicalOperator = RecordQueryLogicalOperator.Or
            }, recordFilterFieldInfosByFieldName);
            recommendedStyleRuleDescription.ThrowIfNull("recommendedStyleRuleDescription");

            var measureStyleRuleDetail = new MeasureStyleRuleDetail()
            {
                MeasureName = measureStyleRule.MeasureName,
                MeasureTitle = analyticMeasure.Title,
                Rules = rules,
                RecommendedRecordFilterDescription = recommendedStyleRuleDescription
            };

            return new MeasureStyleRuleRuntime
            {
                MeasureStyleRule = measureStyleRule,
                MeasureStyleRuleDetail = measureStyleRuleDetail
            };

        }
    }
}
