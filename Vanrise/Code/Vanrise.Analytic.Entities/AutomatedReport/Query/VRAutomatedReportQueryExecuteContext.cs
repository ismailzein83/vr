﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportQueryExecuteContext : IVRAutomatedReportQueryExecuteContext
    {
        public Guid QueryDefinitionId { get; set; }
        public VRReportGenerationFilter Filter { get; set; }
        public VRReportGenerationRuntimeFilter RuntimeFilter { get; set; }
    }
}
