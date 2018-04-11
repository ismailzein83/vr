using System;

namespace Vanrise.Rules
{
	public abstract class BaseRule : IVRRule
	{
		public int RuleId { get; set; }

		public string Description { get; set; }

		public DateTime BeginEffectiveTime { get; set; }

		public DateTime? EndEffectiveTime { get; set; }

		public DateTime? LastRefreshedTime { get; set; }

		public virtual TimeSpan RefreshTimeSpan { get { return new TimeSpan(1, 0, 0); } }

		public virtual bool HasAdditionalInformation { get { return false; } }

		public bool IsDeleted { get; set; }

		public int? CreatedBy { get; set; }

		public int? LastModifiedBy { get; set; }

		public virtual void RefreshRuleState(IRefreshRuleStateContext context)
		{
		}

		public virtual bool IsAnyCriteriaExcluded(object target)
		{
			return false;
		}

		public virtual void UpdateAdditionalInformation(BaseRule existingRule, ref AdditionalInformation additionalInformation)
		{
		}
	}

	public interface IVRRule
	{
		bool IsAnyCriteriaExcluded(object target);

		DateTime BeginEffectiveTime { get; set; }

		DateTime? EndEffectiveTime { get; set; }

		DateTime? LastRefreshedTime { get; set; }

		TimeSpan RefreshTimeSpan { get; }

		void RefreshRuleState(IRefreshRuleStateContext context);
	}
}