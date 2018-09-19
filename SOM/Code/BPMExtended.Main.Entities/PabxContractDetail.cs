using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{

    public enum PabxPhoneNumberOption { MakeCall = '0', ReceiveCall = '1', Both = '2' }

    public class PabxContractDetail
    {

        public string ContractId { get; set; }

        public string CustomerId { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public bool IsPilot { get; set; }

        public PabxPhoneNumberOption PabxOption { get; set; }

    }

}
