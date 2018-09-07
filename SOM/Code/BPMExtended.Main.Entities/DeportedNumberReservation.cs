using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class DeportedNumberReservation
    {
        public string LinePathID { get; set; }

        public string MDF { get; set; }
        public string MDFPort { get; set; }
        public string Cabinet { get; set; }
        public string CabinetPort { get; set; }
        public string DP { get; set; }
        public string DPPort { get; set; }
    }
}
