﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BI.Entities
{
    public class BaseTimeDimensionRecord
    {
        public DateTime Time { get; set; }

        public int WeekNumber { get; set; }
    }
}
