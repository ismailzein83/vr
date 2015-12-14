using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business.SwitchRule.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByDataSource : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {


        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<int> keys)
        {
            IRuleSwitchDataSourceCriteria ruleSwitchDataSourceCriteria = rule as IRuleSwitchDataSourceCriteria;
            keys = ruleSwitchDataSourceCriteria.DataSources;
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            IRuleSwitchDataSourceTarget ruleSwitchDataSourceTarget = target as IRuleSwitchDataSourceTarget;
            if (ruleSwitchDataSourceTarget.DataSource.HasValue)
            {
                key = ruleSwitchDataSourceTarget.DataSource.Value;
                return true;
            }
            else
            {
                key = 0;
                return false;
            }
        }
    }
}
