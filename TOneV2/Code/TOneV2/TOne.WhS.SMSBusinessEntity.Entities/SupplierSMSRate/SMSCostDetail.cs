using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SMSCostDetail
    {
        public string MobileCountryName { get; set; }

        public string MobileNetworkName { get; set; }

        public List<SMSCostOptionDetail> CostOptions { get; set; }
    }

    public class SMSCostOptionDetail
    {
        public string SupplierName { get; set; }

        public decimal SupplierRate { get; set; }
    }
}
