using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface ICustomerGroupContext : IContext
    {
        CustomerFilterSettings FilterSettings { get; set; }

        IEnumerable<int> GetGroupCustomerIds(CustomerGroupSettings group);
    }
}
