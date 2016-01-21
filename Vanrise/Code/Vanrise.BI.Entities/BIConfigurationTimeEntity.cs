using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
    public class BIConfigurationTimeEntity
    {
        public string Date { get; set; }
        public string Year { get; set; }
        public string MonthOfYear { get; set; }
        public string WeekOfMonth { get; set; }
        public string DayOfMonth { get; set; }
        public string Hour { get; set; }
        public bool IsDefault { get; set; }
    }
}
