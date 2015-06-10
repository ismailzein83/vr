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

        public bool IsCost { get; set; }

        public string CurrencyId { get; set; }

        public string SupplierGroup { get; set; }

        public string CustomerGroup { get; set; }

        public bool GroupBySupplier { get; set; }

        public decimal NormalDuration {get; set;}

        public decimal OffPeakDuration { get; set; }

        public double NormalNet { get; set; }

        public double OffPeakNet { get; set; }

        public double TotalAmount { get; set; }

        public bool IsService { get; set; }

        public bool IsCommission { get; set; }

        public decimal ServicesForCustomer { get; set; }

        public int Margin { get; set; }
    }
}
