using System;
using Retail.Voice.Entities;
using Vanrise.GenericData.Transformation;
using Vanrise.GenericData.Entities;

namespace Retail.Voice.MainExtensions
{
    public class RuleInternationalIdentification : InternationalIdentification
    {
        public override Guid ConfigId { get { return new Guid("D65AC3F8-3E92-4B48-AE0B-1F25C588916D"); } }

        public Guid RuleDefinitionId { get; set; }

        public override void Execute(IInternationalIdentificationContext context)
        {
            if (!string.IsNullOrEmpty(context.OtherPartyNumber))
            {
                MappingRuleManager mappingRuleManager = new MappingRuleManager();
                GenericRuleTarget target = new GenericRuleTarget()
                {
                    Objects = new System.Collections.Generic.Dictionary<string, dynamic>(),
                    TargetFieldValues = new System.Collections.Generic.Dictionary<string, object>()
                };

                target.Objects.Add("RawCDR", context.RawCDR);
                target.TargetFieldValues.Add("NumberPrefix", context.OtherPartyNumber);
                target.TargetFieldValues.Add("NumberLength", context.OtherPartyNumber.Length);

                var matchRule = mappingRuleManager.GetMatchRule(RuleDefinitionId, target);
                if (matchRule != null)
                    context.IsInternational = (Boolean)Convert.ChangeType(matchRule.Settings.Value, typeof(Boolean));
            }
        }
    }
}
