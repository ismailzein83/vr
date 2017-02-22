using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceSettingsViewSettings : Vanrise.Security.Entities.ViewSettings
    {
        public override bool DoesUserHaveAccess(Security.Entities.IViewUserAccessContext context)
        {
            return BusinessManagerFactory.GetManager<IInvoiceTypeManager>().DoesUserHaveViewSettingsAccess(context.UserId);
        }
    }
}
