using System;

namespace Vanrise.Rules
{
    public abstract class BaseRule
    {
        public int RuleId { get; set; }

        public virtual bool IsAnyCriteriaExcluded(object target)
        {
            return false;
        }

        public string Description { get; set; }

        public DateTime BeginEffectiveTime { get; set; }

        public DateTime? EndEffectiveTime { get; set; }

        public DateTime? LastRefreshedTime { get; set; }

        public virtual TimeSpan RefreshTimeSpan { get { return new TimeSpan(1, 0, 0); } }

        public virtual void RefreshRuleState(IRefreshRuleStateContext context) { }
    }
}