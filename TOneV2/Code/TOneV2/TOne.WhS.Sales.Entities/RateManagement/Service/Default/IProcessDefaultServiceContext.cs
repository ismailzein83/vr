using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public interface IProcessDefaultServiceContext
    {
        DefaultServiceToAdd DefaultServiceToAdd { get; }
        DefaultServiceToClose DefaultServiceToClose { get; }
        IEnumerable<ExistingDefaultService> ExistingDefaultServices { get; }
        NewDefaultService NewDefaultService { set; }
        IEnumerable<ChangedDefaultService> ChangedDefaultServices { set; }
    }
}
