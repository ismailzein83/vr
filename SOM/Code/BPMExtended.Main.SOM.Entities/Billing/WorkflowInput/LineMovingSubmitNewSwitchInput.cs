using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class LineMovingSubmitNewSwitchInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string NewDirectoryNumber { get; set; }
        public string OldDirectoryNumber { get; set; }
        public bool SameSwitch { get; set; }
        public Address Address { get; set; }

        public PaymentData PaymentData { get; set; }

    }
}
