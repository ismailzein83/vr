﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRAlertRuleDetail
    {
        public VRAlertRule Entity { get; set; }
        
        public string RuleTypeName { get; set; }

        public bool AllowEdit { get; set; }

        public string CreatedBy { get; set; }
    }
}
 