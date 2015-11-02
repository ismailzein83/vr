using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups
{
    public class SelectiveCustomerGroup : CustomerGroupSettings
    {
        public List<int> CustomerIds { get; set; }

        public override IEnumerable<int> GetCustomerIds(CustomerGroupContext context)
        {
            return this.CustomerIds;
        }

        public override string GetDescription()
        {
            if (this.CustomerIds != null)
            {
                CarrierAccountManager manager = new CarrierAccountManager();
                IEnumerable<CarrierAccountDetail> carrierAccounts = manager.GetCarrierAccountsByIds(this.CustomerIds, true, false);
                return string.Join(", ", carrierAccounts.Select(x => x.Name));
            }

            return string.Empty;
        }
    }
}
