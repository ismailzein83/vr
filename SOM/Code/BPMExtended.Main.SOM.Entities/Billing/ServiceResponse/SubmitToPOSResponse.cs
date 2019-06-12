using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class SubmitToPOSResponse
    {
        public decimal? Amount { get; set; }
        public decimal? DepositAmount { get; set; }
        public decimal? FinancialStampAmount { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
