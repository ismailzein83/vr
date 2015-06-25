using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class ProfitByDay
    {
        public DateTime Day { get; set; }
        public string DayOfWeek { get; set; }
        public int DayNumber { get; set; }
        public decimal Profit { get; set; }
    }
    public class ProfitByWeekDayView
    {
        public string DayOfWeek { get; set; }
        public int DayNumber { get; set; }
        public decimal? ProfitWeek1 { get; set; }
        public decimal? ProfitWeek2 { get; set; }
        public decimal? ProfitWeek3 { get; set; }
        public decimal? ProfitWeek4 { get; set; }
    }
}
