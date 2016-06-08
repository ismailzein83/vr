﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class ReleaseCodeQuery
    {
        public ReleaseCodeFilter Filter { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
    }
}
