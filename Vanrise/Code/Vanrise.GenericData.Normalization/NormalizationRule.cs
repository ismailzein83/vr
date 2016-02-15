﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Normalization
{
    public class NormalizationRule : GenericRule
    {
        public Vanrise.Rules.Normalization.NormalizeNumberSettings Settings { get; set; }

        public override string GetSettingsDescription(GenericRuleDefinitionSettings settingsDefinition)
        {
            if (Settings != null && Settings.Actions != null)
            {
                List<string> actionDescriptions = new List<string>();
                foreach (var action in Settings.Actions)
                    actionDescriptions.Add(action.GetDescription());
                return String.Join("; ", actionDescriptions);
            }
            return null;
        }
    }
}
