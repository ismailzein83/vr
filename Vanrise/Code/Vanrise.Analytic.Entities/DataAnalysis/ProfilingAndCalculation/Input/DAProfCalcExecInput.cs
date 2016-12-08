using System;
using System.Collections.Generic;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcExecInput
    {
        public Guid OutputItemDefinitionId { get; set; }

        public Dictionary<Guid, dynamic> FilterParameterValues { get; set; }
    }

    public class DAProfCalcExecInputDetail
    {
        public string DataAnalysisUniqueName { get; set; }

        public DAProfCalcExecInput DAProfCalcExecInput { get; set; }
    }
}