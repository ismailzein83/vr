using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class CustomerRoute
    {
        public string Destination { get; set; }
        public int RouteId { get; set; }
        public decimal Percentage { get; set; }
        public int Preference { get; set; }
    }

    public class ConvertedCustomerRoute
    {
        public string DestinationCode { get; set; }
        public string DestinationName { get; set; }
        public List<CustomerRouteOption> Options { get; set; }
    }
    public class CustomerRouteOption
    {
        public string SupplierName { get; set; }
        public int RouteId { get; set; }
        public decimal Percentage { get; set; }
        public int Priority { get; set; }
        public int SupplierId { get; set; }
        public override string ToString()
        {
            return string.Format("{0} | {1}", SupplierName, Percentage);
        }
    }
}
