using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class ZoneInfo
    {
        public int ZoneID { get; set; }
        public string ZoneName { get; set; }
        public List<decimal> Values { get; set; }

        public List<string> Time { get; set; }
        


    }
}
