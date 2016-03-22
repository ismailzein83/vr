﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticConfiguration<T>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ConfigurationType Type { get; set; }
        public T Configuration { get; set; }
    }
}
