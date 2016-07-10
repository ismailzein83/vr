using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ProcessDefaultRoutingProductContext : IProcessDefaultRoutingProductContext
    {
        public DefaultRoutingProductToAdd DefaultRoutingProductToAdd { get; set; }

        public DefaultRoutingProductToClose DefaultRoutingProductToClose { get; set; }

        public IEnumerable<ExistingDefaultRoutingProduct> ExistingDefaultRoutingProducts { get; set; }

        public NewDefaultRoutingProduct NewDefaultRoutingProduct { get; set; }

        public IEnumerable<ChangedDefaultRoutingProduct> ChangedDefaultRoutingProducts { get; set; }
    }
}
