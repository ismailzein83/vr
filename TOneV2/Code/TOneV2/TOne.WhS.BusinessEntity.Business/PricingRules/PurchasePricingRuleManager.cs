using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class PurchasePricingRuleManager : BasePricingRuleManager<PurchasePricingRule>
    {
        protected override IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> behaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            behaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorBySupplier());
            behaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorBySupplierZone());

            return behaviors;
        }
    }
}
