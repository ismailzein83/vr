﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticAggregate
    {
        public int AnalyticAggregateConfigId { get; set; }
        public string Name { get; set; }
        public AnalyticAggregateConfig Config { get; set; }
    }
}
