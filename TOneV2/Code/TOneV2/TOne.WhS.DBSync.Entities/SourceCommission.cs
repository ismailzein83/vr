using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceCommission
    {
        public int CommissionId { get; set; }
        public string SupplierId { get; set; }
        public string CustomerId { get; set; }
        public int ZoneId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public decimal FromRate { get; set; }
        public decimal ToRate { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? Amount { get; set; }
        public bool IsExtraCharge { get; set; }

    }
}
