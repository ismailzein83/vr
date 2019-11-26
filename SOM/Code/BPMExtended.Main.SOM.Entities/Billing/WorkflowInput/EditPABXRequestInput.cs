using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class EditPABXRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public SubmitPabxInput SubmitPabxInput { get; set; }
        public PaymentData PaymentData { get; set; }

    }
}
