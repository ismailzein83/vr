using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
    public class ReportParameters
    {
        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public string CustomerId { get; set; }

        public string SupplierId { get; set; }

        public bool GroupByCustomer { get; set; }

        public int CustomerAMUId { get; set; }

        public int SupplierAMUId { get; set; }   

    }
}
