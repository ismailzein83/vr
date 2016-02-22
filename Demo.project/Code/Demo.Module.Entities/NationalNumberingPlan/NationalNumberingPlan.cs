using System;
using System.Collections.Generic;

namespace Demo.Module.Entities
{
    public class NationalNumberingPlan
    {
        public int NationalNumberingPlanId { get; set; }

        public int OperatorId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public NationalNumberingPlanSettings Settings { get; set;  }
    }
    public class NationalNumberingPlanSettings
    {
        public List<string> Range { get; set; }

    }
}
