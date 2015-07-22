using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
    public class BaseTimeDimensionRecord
    {
        public DateTime Time { get; set; }

        public int WeekNumber { get; set; }

        public string DateTimeValue { get; set; }
    }
}
