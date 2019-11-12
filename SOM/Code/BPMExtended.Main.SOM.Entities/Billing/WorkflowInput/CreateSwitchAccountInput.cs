using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class CreateSwitchAccountInput
    {
        public string ContractId { get; set; }
        public string RequestId { get; set; }
        public string LinePathId { get; set; }
    }
}
