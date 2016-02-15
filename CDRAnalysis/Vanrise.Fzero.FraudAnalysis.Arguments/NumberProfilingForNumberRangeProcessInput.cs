using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class NumberProfilingForNumberRangeProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public NumberProfileParameters Parameters { get; set; }

        public List<string> NumberPrefixes { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int PeriodId { get; set; }

        public bool IncludeWhiteList { get; set; }

        public override string GetTitle()
        {
            return String.Format("Number profiling For Number Prefixes '{0}', Time Range ({1:dd-MMM-yy HH:mm} to {2:dd-MMM-yy HH:mm})", string.Join(",", this.NumberPrefixes), this.FromDate, this.ToDate);
        }
    }
}
