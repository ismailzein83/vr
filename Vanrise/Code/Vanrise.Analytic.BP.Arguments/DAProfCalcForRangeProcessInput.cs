using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.Analytic.BP.Arguments
{
    public class DAProfCalcForRangeProcessInput : BaseProcessInputArgument
    {
        public List<Guid> RecordStorageIds { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public Guid DAProfCalcDefinitionId { get; set; }

        public List<DAProfCalcExecInputDetail> DAProfCalcExecInputDetails { get; set; }

        public string DataAnalysisUniqueName { get; set; }

        public IDAProfCalcOutputRecordProcessor OutputRecordProcessor { get; set; }

        public bool UseRemoteDataGrouper { get; set; }

        public override string GetTitle()
        {
            return string.Format("From {0} To {1}", FromTime.ToString("yyyy-MM-dd HH:mm:ss"), ToTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}