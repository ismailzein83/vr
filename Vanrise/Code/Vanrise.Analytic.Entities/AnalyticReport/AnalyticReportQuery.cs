﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticReportQuery
    {
        public string Name { get; set; }
        public List<Guid> DevProjectIds { get; set; }
    }
}
