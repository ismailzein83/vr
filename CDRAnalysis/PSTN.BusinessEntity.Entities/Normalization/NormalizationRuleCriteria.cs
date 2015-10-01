﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public enum NormalizationPhoneNumberType { Any = 0, CGPN = 1, CDPN = 2 }

    public class NormalizationRuleCriteria
    {
        public List<int> SwitchIds { get; set; }

        public List<int> TrunkIds { get; set; }

        public NormalizationPhoneNumberType PhoneNumberType { get; set; }

        public int Length { get; set; }

        public string PhoneNumberPrefix { get; set; }
    }
}
