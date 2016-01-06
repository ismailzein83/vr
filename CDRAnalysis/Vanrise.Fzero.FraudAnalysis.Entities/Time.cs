using System;
using System.Collections.Generic;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class Time
    {
        public DateTime DateInstance { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Week { get; set; }
        public int? Day { get; set; }
        public int? Hour { get; set; }
        public string MonthName { get; set; }
        public string DayName { get; set; }

    }


}