using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Rules;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Integration.Business;
using Vanrise.Integration.Entities;
namespace TOne.WhS.CDRProcessing.Business
{
    public class SwitchIdentificationRuleManager : Vanrise.Rules.RuleManager<SwitchIdentificationRule, SwitchIdentificationRuleDetail>
    {
        private readonly SwitchManager _switchManager;
        private readonly DataSourceManager _dataSourceManager;
        public SwitchIdentificationRuleManager()
        {
            _switchManager = new SwitchManager();
            _dataSourceManager = new DataSourceManager();
        }

        public SwitchIdentificationRule GetMatchRule(SwitchIdentificationRuleTarget target)
        {
            var ruleTree = GetRuleTree();
            if (ruleTree == null)
                return null;
            else
                return ruleTree.GetMatchRule(target) as SwitchIdentificationRule;
        }
        RuleTree GetRuleTree()
        {
            return GetCachedOrCreate(String.Format("GetRuleTree_SwitchIdentificationRules"),
                () =>
                {
                    return new Vanrise.Rules.RuleTree(base.GetAllRules().Values, GetRuleStructureBehaviors());
                });
        }
        IEnumerable<BaseRuleStructureBehavior> GetRuleStructureBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            ruleStructureBehaviors.Add(new SwitchRule.Rules.StructureRulesBehaviors.RuleBehaviorByDataSource());
            return ruleStructureBehaviors;
        }
        public Vanrise.Entities.IDataRetrievalResult<SwitchIdentificationRuleDetail> GetFilteredSwitchIdentificationRules(Vanrise.Entities.DataRetrievalInput<SwitchIdentificationRuleQuery> input)
        {
            Func<SwitchIdentificationRule, bool> filterExpression = (prod) =>
                (input.Query.Description == null || prod.Description.ToLower().Contains(input.Query.Description.ToLower()))
                && (input.Query.SwitchIds == null || input.Query.SwitchIds.Contains(prod.Settings.SwitchId))
                && (input.Query.EffectiveDate == null || (prod.BeginEffectiveTime <= input.Query.EffectiveDate && (prod.EndEffectiveTime == null || prod.EndEffectiveTime >= input.Query.EffectiveDate)))
                && (input.Query.DataSourceIds == null || input.Query.DataSourceIds.Any(x=>prod.Criteria.DataSources.Contains(x)));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, base.GetFilteredRules(filterExpression).ToBigResult(input, filterExpression, MapToDetails));
        }
        protected override SwitchIdentificationRuleDetail MapToDetails(SwitchIdentificationRule rule)
        {
           
            Switch switchEntity = _switchManager.GetSwitch(rule.Settings.SwitchId);
            List<string> dataSourcesNames=new List<string>();
            for(int i =0;i<rule.Criteria.DataSources.Count;i++)
            {
                 DataSourceDetail  dataSourceDetail=_dataSourceManager.GetDataSource(rule.Criteria.DataSources[i]);
                 if(dataSourceDetail!=null)
                     dataSourcesNames.Add(dataSourceDetail.Name);

            }
            return new SwitchIdentificationRuleDetail
            {
                Entity = rule,
                SwitchName= switchEntity != null ? switchEntity.Name :null,
                DataSourcesNames = GetDescription(dataSourcesNames),
            };
        }
        private string GetDescription(IEnumerable<string> list)
        {
            if (list != null)
                return string.Join(", ", list);
            else
                return null;
        } 
    }
}
