using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public enum ContractDetailStatus { Active, Inactive };

    public class ContractDetail
    {
        public string ContractId { get; set; }

        public string CustomerId { get; set; }

        public string PhoneNumber { get; set; }

        public string RatePlanId { get; set; }

        public string RatePlanName { get; set; }

        public ContractDetailStatus Status { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastModifiedTime { get; set; }
    }

    public class ContractInfo
    {
        public string ContractId { get; set; }

        public string CustomerId { get; set; }

        public string PhoneNumber { get; set; }

    }

}
