using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class RepeatedNumberDetail
    {
        public RepeatedNumber Entity { get; set; }
        public String CustomerName { get; set; }
        public String SupplierName { get; set; }
        public String SaleZoneName { get; set; }
        public String SwitchName { get; set; }
    }
}
