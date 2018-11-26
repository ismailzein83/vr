using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Demo.Business
{
    public class ActiveServices
    {
        String ProductName { get; set; }
        int NoOfOrders { get; set; }
        DateTime Cycle { get; set; }
        Decimal TotalTarrif { get; set; }
    }
    
}
