using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Zajil.MainExtensions
{
    public class OrderDetailItem
    {
        public int OrderId { get; set; }
        public string Charges { get; set; }

        public string Payment { get; set; }

        public double ContractPeriod { get; set; }

        public string ContractRemain { get; set; }

        public double ContractDays { get; set; }

        public double TotalContract { get; set; }

        public double ChargesYear1 { get; set; }

        public double ChargesYear2 { get; set; }

        public double ChargesYear3 { get; set; }

        public double Installation { get; set; }

        public double ThirdParty { get; set; }

        public string Discount { get; set; }

        public string Achievement { get; set; }
    }
}
