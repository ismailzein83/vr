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
                foreach (var thresholdAction in Settings.ThresholdActions)
                {
                    foreach(var action in thresholdAction.Actions)
                    {
                        actionDescriptions.Add(action.ActionName);
                    }
                }
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
