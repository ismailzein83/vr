using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class ActivateCptRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        //public string DirectoryNumber { get; set; }
        public string LinePathId { get; set; }
        public string CPTNumber { get; set; }
        public string CPTServiceId { get; set; }
        public string CPTId { get; set; }
        public PaymentData PaymentData { get; set; }

    }
    public class FinalizeActivateCptRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        //public string DirectoryNumber { get; set; }
        public string LinePathId { get; set; }
        public string CPTNumber { get; set; }
        public string CPTServiceId { get; set; }
        public string CPTId { get; set; }
        public PaymentData PaymentData { get; set; }

    }
}
  