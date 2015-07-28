using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class TimeValuesRecord
    {
        public List<string> Time { get; set; }
        public List<decimal> Values { get; set; }
    }
}
