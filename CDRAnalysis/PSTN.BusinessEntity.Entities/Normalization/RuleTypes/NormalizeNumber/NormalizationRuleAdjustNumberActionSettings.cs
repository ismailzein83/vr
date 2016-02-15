﻿
namespace PSTN.BusinessEntity.Entities
{
    public abstract class NormalizationRuleAdjustNumberActionSettings
    {
        public int ConfigId { get; set; }

        public abstract string GetDescription();

        public abstract void Execute(INormalizationRuleAdjustNumberActionContext context, NormalizationRuleAdjustNumberTarget target);
    }
}
