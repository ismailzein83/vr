using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class SMSRateValueRule : Vanrise.GenericData.Entities.GenericRule
    {
        public SMSRateValueSettings Settings { get; set; }

        public override string GetSettingsDescription(IGenericRuleSettingsDescriptionContext context)
        {
            return Settings.GetDescription(context);
        }

        public override bool AreSettingsMatched(object ruleDefinitionSettings, object settingsFilterValue)
        {
            return true;
        }

        public override Dictionary<string, object> GetSettingsValuesByName()
        {
            return Settings.GetSettingsValuesByName();
        }
    }
}
