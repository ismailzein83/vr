using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Demo.Entities
{
    public class ActiveServices
    {
        public String ProductName { get; set; }
        public int NoOfOrders { get; set; }
        public String Cycle { get; set; }
        public Decimal TotalTarrif { get; set; }
        public ActiveServices()
        {

        }
        public IEnumerable<ActiveServices> GetActiveServicesRDLCSchema()
        {
            return null;
        }
      
    }

}
