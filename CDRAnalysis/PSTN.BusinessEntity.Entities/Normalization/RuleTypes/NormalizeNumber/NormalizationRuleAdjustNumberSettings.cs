﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleAdjustNumberSettings : NormalizationRuleSettings
    {
        public List<NormalizationRuleAdjustNumberActionSettings> Actions { get; set; }

        public override string GetDescription()
        {
            if (this.Actions == null) return null;

            List<string> descriptionList = new List<string>();

            foreach (NormalizationRuleAdjustNumberActionSettings action in this.Actions)
            {
                descriptionList.Add(action.GetDescription());
            }

            return string.Join<string>(" | ", descriptionList);
        }
    }
}
