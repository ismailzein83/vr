﻿
namespace PSTN.BusinessEntity.Entities
{
    public abstract class NormalizationRuleSetAreaSettings : NormalizationRuleSettings
    {
        public int ConfigId { get; set; }

        public abstract void Execute(INormalizationRuleSetAreaContext context, NormalizationRuleSetAreaTarget target);
    }
}
