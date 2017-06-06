using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class LiveCdrItemDetail
    {
        public string customerName { get; set; }
        public string sourceIP { get; set; }
        public DateTime attemptDate { get; set; }
        public string cli { get; set; }
        public string destinationCode { get; set; }
        public string destinationName { get; set; }
        public string supplierName { get; set; }
        public string routeIP { get; set; }
        public string supplierCode { get; set; }
        public string supplierZone { get; set; }
        public DateTime alertDate { get; set; }
        public DateTime connectDate { get; set; }

    }
}
