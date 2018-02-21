using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class LeasedLineContractInfo
    {
        public string LeasedLineContractId { get; set; }

        public string PhoneNumber { get; set; }

        public string Status { get; set; }

        public DateTime ContractTime { get; set; }
    }
}
