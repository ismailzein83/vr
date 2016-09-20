
using System;
namespace PSTN.BusinessEntity.Entities
{
    public abstract class NormalizationRuleAdjustNumberActionSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetDescription();

        public abstract void Execute(INormalizationRuleAdjustNumberActionContext context, NormalizationRuleAdjustNumberTarget target);
    }
}
