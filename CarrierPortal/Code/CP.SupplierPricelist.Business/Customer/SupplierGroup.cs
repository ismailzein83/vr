using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace CP.SupplierPricelist.Business
{
    public class SupplierGroup : GroupSettings
    {
        public override Guid ConfigId { get { return new Guid("6e558c88-31f8-4eb6-a362-58db06ab9cc7"); } }

        public override List<int> GetUserIds(IGroupSettingsGetUserIdsContext context)
        {
            return new CustomerSupplierMappingManager().GetCachedCustomerSupplierMappings().Values.Select(itm => itm.UserId).Distinct().ToList();
        }

        public override bool IsMember(IGroupSettingsContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            return new CustomerSupplierMappingManager().IsUserSupplier(context.UserId);

        }
    }
}
