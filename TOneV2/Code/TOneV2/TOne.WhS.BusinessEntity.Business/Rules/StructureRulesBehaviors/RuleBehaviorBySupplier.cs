using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors
{
    public class RuleBehaviorBySupplier : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.IVRRule rule, out IEnumerable<int> keys)
        {            
            IRuleSupplierCriteria ruleSupplierCriteria = rule as IRuleSupplierCriteria;
            keys = ruleSupplierCriteria.SupplierIds;
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            IRuleSupplierTarget ruleSupplierTarget = target as IRuleSupplierTarget;
            if(ruleSupplierTarget.SupplierId.HasValue)
            {
                key = ruleSupplierTarget.SupplierId.Value;
                return true;
            }
            else
            {
                key = 0;
                return false;
            }
        }

        public override Vanrise.Rules.BaseRuleStructureBehavior CreateNewBehaviorObject()
        {
            return new RuleBehaviorBySupplier();
        }
    }
}
