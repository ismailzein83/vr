﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcAlertRuleSettings : VRAlertRuleExtendedSettings
    {
        public Guid OutputItemDefinitionId { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }

        public RecordFilterGroup DataAnalysisFilterGroup { get; set; }

        public List<VRAction> Actions { get; set; }

        public List<string> GroupingFieldNames { get; set; }
    }
}
