﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
   public class DataGridDirectiveSetting
    {
        public string OperationType { get; set; }
        public string EntityType { get; set; }
        public List<string> MeasureTypes { get; set; }
        public string TopMeasure { get; set; }
    }
}
