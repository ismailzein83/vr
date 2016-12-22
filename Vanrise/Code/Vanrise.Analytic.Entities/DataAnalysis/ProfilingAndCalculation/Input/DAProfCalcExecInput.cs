﻿using System;
using System.Collections.Generic;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcExecInput
    {
        public Guid OutputItemDefinitionId { get; set; }

        public Dictionary<Guid, dynamic> FilterParameterValues { get; set; }

        public DAProfCalcExecPayload DAProfCalcPayload { get; set; }
    }


    public abstract class DAProfCalcExecPayload
    {
 
    }


    public class DAProfCalcExecInputDetail
    {
        public string DataAnalysisUniqueName { get; set; }

        public DAProfCalcExecInput DAProfCalcExecInput { get; set; }
    }
}