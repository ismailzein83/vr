using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.Analytic.BP.Arguments
{
    public class DAProfCalcProcessInput : BaseProcessInputArgument
    {
        public Guid DAProfCalcDefinitionId { get; set; }

        public List<Guid> InRecordStorageIds { get; set; }
        
        public List<Guid> OutRecordStorageIds { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public ChunkTime ChunkTime { get; set; }

        public override string GetTitle()
        {
            return "Data Analysis Profiling And Calculation Process";
        }
    }
}