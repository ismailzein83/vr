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
        public const int ExtensionConfigId = 4;
        public List<int> CustomerIds { get; set; } 

        public override IEnumerable<int> GetCustomerIds(ICustomerGroupContext context)
        {
            return this.CustomerIds;
        }

        public override string GetDescription(ICustomerGroupContext context)
        {
            var validCustomerIds = context != null ? context.GetGroupCustomerIds(this) : this.CustomerIds;
            if (validCustomerIds != null)
            {
                CarrierAccountManager manager = new CarrierAccountManager();
                return manager.GetDescription(validCustomerIds, true, false);
            }
            else
                return null;
        }
    }
}
