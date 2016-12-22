using System;
using System.Collections.Generic;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcOutputRecord
    {
        public DAProfCalcExecInput DAProfCalcExecInput { get; set; }

        public Dictionary<string, dynamic> Records { get; set; }

        public string GroupingKey { get; set; }
    }
}
