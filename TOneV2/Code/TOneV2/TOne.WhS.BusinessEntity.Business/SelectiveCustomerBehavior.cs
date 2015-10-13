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
            SelectiveCustomersSettings selectiveCustomersSettings = settings as SelectiveCustomersSettings;
            if (selectiveCustomersSettings == null)
                throw new Exception(String.Format("{0} is not of type  TOne.WhS.BusinessEntity.Entities.SelectiveCustomersSettings", settings));

            return selectiveCustomersSettings.CustomerIds;
        }
    }
}
