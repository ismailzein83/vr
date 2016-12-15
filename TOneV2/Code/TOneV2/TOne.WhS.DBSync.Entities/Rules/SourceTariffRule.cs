using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceTariffRule : SourceBaseRule
    {
        public int ZoneId { get; set; }
        public string CustomerId { get; set; }
        public string SupplierId { get; set; }
        public decimal CallFee { get; set; }
        public decimal FirstPeriodRate { get; set; }
        public int FirstPeriod { get; set; }
        public bool RepeatFirstPeriod { get; set; }
        public int FractionUnit { get; set; }
        
    }
}
