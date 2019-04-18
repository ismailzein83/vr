using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class Customer
    {
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string CustomerCode { get; set; }
    }
}
