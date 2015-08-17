using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class InOutVolumeTraffic
    {
        public string TrafficDirection {get; set;}
        public decimal Duration {get; set;}
		public decimal Net {get;set;}
		public string PercDuration { get; set;}
		public string PercNet {get;set;}

        public decimal TotalDuration {get;set;}

        public decimal TotalNet { get; set; }
        public string Date { get; set; }
    }
}
