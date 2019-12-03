using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class ContractEntity
    {
        public string ContractId { get; set; }

        public string CustomerId { get; set; }

        public string RatePlanId { get; set; }

        public string RatePlanName { get; set; }

        public Address ContractAddress { get; set; }

        public string ContractStatusId { get; set; }

        public decimal ContractBalance { get; set; }

        public string Promotions { get; set; }

        public string FreeUnit { get; set; }

        public string PathId { get; set; }
        public string StatusChangeDate { get; set; }

        public bool IsBlocked { get; set; }
        public string WholeSale { get; set; }
    }
}
