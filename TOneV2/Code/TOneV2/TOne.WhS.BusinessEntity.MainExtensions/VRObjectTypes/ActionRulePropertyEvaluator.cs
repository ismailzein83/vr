using System;
using Vanrise.Entities;
using Vanrise.Notification.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{

    public enum ActionRuleField { ActionRuleId = 0, ActionRuleName = 1 };

    public class ActionRulePropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("70724B2E-DD4A-4F2B-8807-D139FCF28CDF"); }
        }

        public ActionRuleField ActionRuleField { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            VRAlertRule alertRule = context.Object as VRAlertRule;

            if (alertRule == null)
                return null;

            switch (this.ActionRuleField)
            {
                case MainExtensions.ActionRuleField.ActionRuleId:
                    return alertRule.VRAlertRuleId;
                case MainExtensions.ActionRuleField.ActionRuleName:
                    return alertRule.Name;
            }

            return null;
        }
    }
}
