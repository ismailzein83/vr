﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleAdjustNumberSettings : NormalizationRuleSettings
    {
        public int ConfigId { get; set; }

        public List<NormalizationRuleAdjustNumberActionSettings> Actions { get; set; }
    }
}
