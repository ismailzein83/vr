using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RateInfo
    {
        public int ZoneId { get; set; }

        public decimal Rate { get; set; }

        public short ServicesFlag { get; set; }
    }
}
