using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
   public class InstallmentTemplate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal InterestRatePerPeriod { get; set; }
        public long PaymentInterval { get; set; }
        public long PeriodForFirstPayment { get; set; }
    }
    public class InstallmentTemplateInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
