﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleSetAreaTarget : Vanrise.Rules.BaseRuleTargetIdentifier
    {
        public string PhoneNumber { get; set; }

        public string AreaCode { get; set; }
    }
}
