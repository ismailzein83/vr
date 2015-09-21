using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Billing.Entities;

namespace TOne.Billing.Data
{
    public interface ICustomerInvoiceDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<CustomerInvoice> GetFilteredCustomerInvoices(Vanrise.Entities.DataRetrievalInput<CustomerInvoiceQuery> input);
    }
}
