using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class CommonInputArgument
    {
        public string ContactId { get; set; }
        public string AccountId { get; set; }
        public string ContractId { get; set; }

        public string CustomerId { get; set; }

        public string RequestId { get; set; }
    }
}
