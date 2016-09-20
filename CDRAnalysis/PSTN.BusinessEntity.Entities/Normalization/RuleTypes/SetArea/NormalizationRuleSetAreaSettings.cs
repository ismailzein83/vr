
using System;
namespace PSTN.BusinessEntity.Entities
{
    public abstract class NormalizationRuleSetAreaSettings : NormalizationRuleSettings
    {
        public abstract Guid ConfigId { get;}

        public abstract void Execute(INormalizationRuleSetAreaContext context, NormalizationRuleSetAreaTarget target);
    }
}
