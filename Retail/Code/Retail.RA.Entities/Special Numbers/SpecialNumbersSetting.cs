using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Entities
{
    public class SpecialNumbersSetting
    {
        public List<int> Numbers { get; set; }
        public List<SpecialNumbersRange> Range { get; set; }
    }
    public class SpecialNumbersRange
    {
        public int From { get; set; }
        public int To { get; set; } 
    }
}
