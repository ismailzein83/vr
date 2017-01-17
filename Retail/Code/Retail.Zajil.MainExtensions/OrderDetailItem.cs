using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Zajil.MainExtensions
{
    public class OrderDetailItem
    {
        public string Charges { get; set; }

        public string Payment { get; set; }

        public decimal ContractPeriod { get; set; }

        public decimal ContractRemain { get; set; }

        public decimal ContractDays { get; set; }

        public decimal TotalContract { get; set; }

        public decimal ChargesYear1 { get; set; }

        public decimal ChargesYear2 { get; set; }

        public decimal ChargesYear3 { get; set; }

        public decimal Installation { get; set; }

        public decimal ThirdParty { get; set; }

        public string Discount { get; set; }

        public string Achievement { get; set; }
    }
}
