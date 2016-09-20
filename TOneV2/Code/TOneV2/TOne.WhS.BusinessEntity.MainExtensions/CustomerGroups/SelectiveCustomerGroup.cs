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
        public override Guid ConfigId { get { return new Guid("967c9c1f-0a9f-41ca-8a19-3a70338fe853"); } }

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
