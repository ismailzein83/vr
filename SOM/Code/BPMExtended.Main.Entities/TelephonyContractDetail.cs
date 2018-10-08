using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class TelephonyContractDetail : ContractDetail
    {
    }

    public class ADSLContractDetail : ContractDetail
    {
        public string TelephonyContractId { get; set; }
    }

    public class TelephonyContractInfo : ContractInfo
    {
    }
}
