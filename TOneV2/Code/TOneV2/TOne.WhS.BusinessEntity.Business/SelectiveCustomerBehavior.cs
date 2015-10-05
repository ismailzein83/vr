using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SelectiveCustomerBehavior : CustomerGroupBehavior
    {
        public override List<int> GetCustomerIds(CustomerGroupSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
