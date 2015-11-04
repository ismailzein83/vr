using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerGroupContext : ICustomerGroupContext
    {
        public CustomerFilterSettings FilterSettings { get; set; }

        public IEnumerable<int> GetGroupCustomerIds(CustomerGroupSettings group)
        {
            var allGroupCustomerIds = group.GetCustomerIds(this);
            if (allGroupCustomerIds == null)
                return null;
            HashSet<int> filteredSupplierIds = GetFilteredCustomerIds(this.FilterSettings);
            if (filteredSupplierIds != null)
                return allGroupCustomerIds.Where(supplierId => filteredSupplierIds.Contains(supplierId));

            return allGroupCustomerIds;
        }
        public static HashSet<int> GetFilteredCustomerIds(CustomerFilterSettings filterSettings)
        {
            return null;
        }
    }
}
