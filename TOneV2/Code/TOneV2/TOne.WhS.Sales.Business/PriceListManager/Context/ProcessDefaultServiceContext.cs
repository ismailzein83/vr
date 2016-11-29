using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ProcessDefaultServiceContext : IProcessDefaultServiceContext
    {
        public DefaultServiceToAdd DefaultServiceToAdd { get; set; }
        public DefaultServiceToClose DefaultServiceToClose { get; set; }
        public IEnumerable<ExistingDefaultService> ExistingDefaultServices { get; set; }
        public NewDefaultService NewDefaultService { get; set; }
        public IEnumerable<ChangedDefaultService> ChangedDefaultServices { get; set; }
    }
}
