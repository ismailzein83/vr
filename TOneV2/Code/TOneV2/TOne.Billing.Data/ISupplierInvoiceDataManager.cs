using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Billing.Entities;

namespace TOne.Billing.Data
{
    public interface ISupplierInvoiceDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<SupplierInvoice> GetFilteredSupplierInvoices(Vanrise.Entities.DataRetrievalInput<SupplierInvoiceQuery> input);
        Vanrise.Entities.BigResult<SupplierInvoiceDetail> GetFilteredSupplierInvoiceDetails(Vanrise.Entities.DataRetrievalInput<SupplierInvoiceDetailQuery> input);
        Vanrise.Entities.BigResult<SupplierInvoiceDetailGroupedByDay> GetFilteredSupplierInvoiceDetailsGroupedByDay(Vanrise.Entities.DataRetrievalInput<SupplierInvoiceDetailGroupedByDayQuery> input);
        bool DeleteInvoice(int invoiceID);
    }
}
