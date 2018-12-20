using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class SMSTaxRule : Vanrise.GenericData.Entities.GenericRule
    {
        public SMSTaxRuleSettings Settings { get; set; }

        public override bool AreSettingsMatched(object ruleDefinitionSettings, object settingsFilterValue)
        {
            return true;
        }

        public override string GetSettingsDescription(IGenericRuleSettingsDescriptionContext context)
        {
            return Settings.GetDescription(context);
        }

        public override Dictionary<string, object> GetSettingsValuesByName()
        {
            return Settings.GetSettingsValuesByName();
        }
    }
}