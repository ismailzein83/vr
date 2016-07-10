using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public interface IProcessDefaultRoutingProductContext
    {
        // Input Properties
        DefaultRoutingProductToAdd DefaultRoutingProductToAdd { get; }
        
        DefaultRoutingProductToClose DefaultRoutingProductToClose { get; }

        IEnumerable<ExistingDefaultRoutingProduct> ExistingDefaultRoutingProducts { get; }

        // Output Properties
        NewDefaultRoutingProduct NewDefaultRoutingProduct { set; }

        IEnumerable<ChangedDefaultRoutingProduct> ChangedDefaultRoutingProducts { set; }
    }
}
