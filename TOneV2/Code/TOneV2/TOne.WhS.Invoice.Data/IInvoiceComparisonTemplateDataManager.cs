using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Invoice.Entities;

namespace TOne.WhS.Invoice.Data
{
    public interface IInvoiceComparisonTemplateDataManager : IDataManager
    {
         bool TryAddOrUpdateInvoiceCompareTemplate(InvoiceComparisonTemplate invoiceComparisonTemplate);
        List<InvoiceComparisonTemplate> GetInvoiceCompareTemplates();
        bool AreInvoiceComparisonTemplatesUpdated(ref object updateHandle);
    }
}
