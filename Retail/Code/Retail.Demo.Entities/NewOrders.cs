using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Demo.Entities
{
    public class NewOrders
    {
        public String ProductName { get; set; }
        public string OrderID { get; set; }
        public string ServiceID { get; set; }
        public int NoOfOrders { get; set; }
        public Decimal TotalTarrif { get; set; } 

        public NewOrders()
        {

        }
        public IEnumerable<NewOrders> GetNewOrdersRDLCSchema()
        {
            return null;
        }
      
    }
}
