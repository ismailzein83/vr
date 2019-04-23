using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public enum ContractDetailStatus { Undefined, OnHold,Active, Suspended, Inactive, Invisible };

    public class ContractDetail
    {
        public string ContractId { get; set; }

        public string RatePlanName { get; set; }

        public string ContractStatusId { get; set; }

        public string Status { get; set; }

        public DateTime? ActivationDate { get; set; }

        public DateTime? StatusDate { get; set; }
    }
}
