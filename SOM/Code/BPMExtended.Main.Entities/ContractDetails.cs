using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class ContractDetails
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string ContractTypeId { get; set; }
        public string RateplanId { get; set; }
        public int StatusId { get; set; }
        public DateTime ActivationDate { get; set; }
        public string LastChangeReason { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }
    }
}
