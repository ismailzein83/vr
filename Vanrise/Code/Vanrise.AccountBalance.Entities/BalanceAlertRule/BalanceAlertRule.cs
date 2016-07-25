using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class BalanceAlertRule : GenericRule
    {
        public BalanceAlertSettings Settings { get; set; }

        public override string GetSettingsDescription(IGenericRuleSettingsDescriptionContext context)
        {
            if (Settings != null && Settings.ThresholdActions != null)
            {
                List<string> actionDescriptions = new List<string>();
                foreach (var action in Settings.ThresholdActions)
                    actionDescriptions.Add(action.Threshold.ConfigId.ToString());
                return String.Join("; ", actionDescriptions);
            }
            return null;
        }

        public override bool AreSettingsMatched(object ruleDefinitionSettings, object settingsFilterValue)
        {
            return true;
        }
    }
}
