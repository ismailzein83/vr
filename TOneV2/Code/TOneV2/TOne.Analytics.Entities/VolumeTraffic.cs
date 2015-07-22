using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class VolumeTraffic
    {
       // public List<decimal> {get; set;}
        public int Attempts { get; set; }
        public decimal Duration { get; set; }
        public string Date { get; set; }
    }
}
