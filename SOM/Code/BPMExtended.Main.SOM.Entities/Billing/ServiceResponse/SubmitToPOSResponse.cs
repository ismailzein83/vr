using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class SubmitToPOSResponse
    {
        public string ServiceName { get; set; }
        public Money Amount { get; set; }
        public Money TaxAmount { get; set; }
        public Money TotalAmount { get; set; }

    }
}
