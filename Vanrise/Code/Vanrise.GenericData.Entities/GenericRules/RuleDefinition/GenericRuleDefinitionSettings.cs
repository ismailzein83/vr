using System;

namespace Vanrise.GenericData.Entities
{
    public abstract class GenericRuleDefinitionSettings
    {
        public abstract Guid ConfigId { get; }

        public string GridSettingTitle { get; set; }
    }
}
