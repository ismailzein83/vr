using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class CustomerGroupSettings
    {
        public int ConfigId { get; set; }

        public abstract IEnumerable<int> GetCustomerIds(CustomerGroupContext context);
    }

    public class SelectiveCustomersSettings : CustomerGroupSettings
    {
        public List<int> CustomerIds { get; set; }

        public override IEnumerable<int> GetCustomerIds(CustomerGroupContext context)
        {
            throw new NotImplementedException();
        }
    }
}
