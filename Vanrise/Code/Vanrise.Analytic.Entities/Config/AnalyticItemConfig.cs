﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public enum AnalyticItemType { Dimension = 1, Measure = 2, Join = 3 }
    
    public class AnalyticItemConfig<T>
    {
        public int AnalyticItemConfigId { get; set; }

        public int TableId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public AnalyticItemType ItemType { get; set; }

        public T Config { get; set; }
    }
}
