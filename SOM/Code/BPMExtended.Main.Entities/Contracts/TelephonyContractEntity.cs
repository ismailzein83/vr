using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class TelephonyContractEntity : ContractEntity
    {
        public string PhoneNumber { get; set; }
        public bool IsPABX { get; set; }
    }
}
