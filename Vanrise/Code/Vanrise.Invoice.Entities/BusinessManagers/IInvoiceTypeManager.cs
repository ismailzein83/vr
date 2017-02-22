using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public interface IInvoiceTypeManager : IBusinessManager
    {
        bool DoesUserHaveViewAccess(int userId, Guid invoiceTypeId);

        bool DoesUserHaveViewSettingsAccess(int userId);
        

    }
}
