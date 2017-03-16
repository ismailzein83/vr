﻿using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.Analytic.BP.Arguments
{
    public class DAProfCalcStoreOutputInput : BaseProcessInputArgument
    {
        public Guid DAProfCalcDefinitionId { get; set; }

        public List<Guid> InRecordStorageIds { get; set; }

        public List<Guid> OutRecordStorageIds { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public DAProfCalcChunkTimeEnum ChunkTime { get; set; }

        public override string GetTitle()
        {
            return String.Format("Data Analysis Profiling And Calculation Store Output Process from {0} to {1}", FromTime.ToString("yyyy-MM-dd HH:mm:ss"), ToTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}