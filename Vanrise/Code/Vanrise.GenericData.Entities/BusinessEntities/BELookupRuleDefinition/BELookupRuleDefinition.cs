using System;
using System.Collections.Generic;
using Vanrise.Rules;

namespace Vanrise.GenericData.Entities
{
    public class BELookupRuleDefinition
    {
        public Guid BELookupRuleDefinitionId { get; set; }

        public string Name { get; set; }

        public Guid BusinessEntityDefinitionId { get; set; }

        public List<BELookupRuleDefinitionCriteriaField> CriteriaFields { get; set; }
    }

    public class BELookupRuleDefinitionCriteriaField
    {
        public string Title { get; set; }

        public string FieldPath { get; set; }

        public MappingRuleStructureBehaviorType RuleStructureBehaviorType { get; set; }
    }

    public class BELookupRule : Vanrise.Rules.IVRRule
    {
        public BELookupRuleDefinition RuleDefinition { get; set; }

        public dynamic BusinessEntityObject { get; set; }
        

        public bool IsAnyCriteriaExcluded(object target)
        {
            return false;
        }

        public DateTime BeginEffectiveTime
        {
            get
            {
                return DateTime.MinValue;
            }
            set
            {

            }
        }

        public DateTime? EndEffectiveTime
        {
            get
            {
                return null;
            }
            set
            {

            }
        }

        public DateTime? LastRefreshedTime
        {
            get;
            set;
        }

        public TimeSpan RefreshTimeSpan
        {
            get { return TimeSpan.MaxValue; }
        }

        public void RefreshRuleState(IRefreshRuleStateContext context)
        {

        }

        public long GetPriorityIfSameCriteria(IRuleGetPriorityContext context)
        {
            return 0;
        }
    }
}
