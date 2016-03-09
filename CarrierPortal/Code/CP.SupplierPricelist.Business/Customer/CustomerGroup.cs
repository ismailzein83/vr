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
        public override bool IsMember(IGroupSettingsContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            //TODO: write the logic that compares if a user is in the customer user list

            return true;
        }
    }
}
