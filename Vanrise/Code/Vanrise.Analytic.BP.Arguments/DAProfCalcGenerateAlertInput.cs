using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.Analytic.BP.Arguments
{
    public class DAProfCalcGenerateAlertInput : BaseProcessInputArgument
    {
        public Guid AlertRuleTypeId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public DAProfCalcChunkTimeEnum ChunkTime { get; set; }

        public override string GetTitle()
        {
            return String.Format("Data Analysis Profiling And Calculation Generate Alert Process from {0} to {1}", FromTime, ToTime);
        }
    }
}