﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleSetAreaSettings : NormalizationRuleSettings
    {
        public NormalizationRuleActionSettings Action { get; set; }
    }
}
