﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticReport
    {
        public int AnalyticReportId { get; set; }

        public string Name { get; set; }

        public AnalyticReportSettings Settings { get; set; }
    }
}
