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

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Speed { get; set; }

        public string DSLAMPort { get; set; }

        public string ProviderOfDSLAMPort { get; set; }



    }

    public class TelephonyContractInfo : ContractInfo
    {
    }
}
