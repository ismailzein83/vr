﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SMSFutureRate
    {
        public decimal? Rate { get; set; }

        public DateTime? BED { get; set; }

        public DateTime? EED { get; set; }
    }
}