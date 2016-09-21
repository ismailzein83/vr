using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace CP.SupplierPricelist.Business
{
    public class CustomerGroup : GroupSettings
    {
        public override Guid ConfigId { get { return new Guid("9a1c1152-b9dc-4e4c-b2ff-181325cd6d9b"); } }
        public override bool IsMember(IGroupSettingsContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            return new CustomerUserManager().IsUserCustomer(context.UserId);

        }
    }
}
