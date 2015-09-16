﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class StrategyResultQuery
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<PeriodEnum> PeriodIDs { get; set; }

        public List<int> UserIDs { get; set; }

        public string IsDefaultList { get; set; }

        public string IsEnabledList { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

    }
}
