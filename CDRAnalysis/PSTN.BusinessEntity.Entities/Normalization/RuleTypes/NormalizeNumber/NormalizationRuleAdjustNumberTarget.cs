
namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleAdjustNumberTarget : NormalizationRuleTarget
    {        
        public override NormalizationRuleType RuleType
        {
            get { return NormalizationRuleType.AdjustNumber; }
        }
    }
}
