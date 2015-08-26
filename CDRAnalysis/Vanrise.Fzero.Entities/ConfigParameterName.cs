using System;

namespace Vanrise.Fzero.Entities
{
    public enum ConfigParameterName
    {
        [ConfigParameterName(DefaultValue = "1")]
        OperatorType = CommonEnums.OperatorType.Mobile
    }

    public class ConfigParameterNameAttribute : Attribute
    {
        public string DefaultValue { get; set; }
    }
}
