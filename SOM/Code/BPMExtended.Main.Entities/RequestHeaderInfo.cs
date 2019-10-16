using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class RequestHeaderInfo
    {
        public Guid RequestId { get; set; }
        public string ContactId { get; set; }
        public string AccountId { get; set; }
        public string ContractId { get; set; }
        public string SequenceNumber{ get; set; }

    }
}
