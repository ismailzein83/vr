﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class SubscriberThresholdResultQuery
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string SubscriberNumber { get; set; }

    }
}
