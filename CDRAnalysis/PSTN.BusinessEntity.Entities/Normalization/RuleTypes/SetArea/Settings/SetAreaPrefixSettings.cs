﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities.Normalization.RuleTypes.SetArea.Settings
{
    public class SetAreaPrefixSettings : NormalizationRuleSetAreaSettings
    {
        public int PrefixLength { get; set; }

        public override List<string> GetDescriptions()
        {
            List<string> descriptions = new List<string>();

            descriptions.Add("Set Area Prefix: Prefix Length = " + this.PrefixLength);

            return descriptions;
        }
    }
}
