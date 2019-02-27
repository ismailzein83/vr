using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class BillingCommonInputArgument
    {
        public Guid? ContactId { get; set; }

        public Guid? AccountId { get; set; }

        public string ContractId { get; set; }
    }
}
