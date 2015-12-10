using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
   
    public class CDRLogDetail
    {
        public CDRLog Entity { get; set; }
        public String SwitchName { get; set; }
        public String SaleZoneName { get; set; }
        public String OriginatingZoneName { get; set; }
        public String CustomerName { get; set; }
        public String SupplierName { get; set; }
        public string SupplierZoneName { get; set; }
    }
}
