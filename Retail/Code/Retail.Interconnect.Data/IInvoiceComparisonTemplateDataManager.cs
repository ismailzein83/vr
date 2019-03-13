using Retail.Interconnect.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Interconnect.Data
{
    public interface IInvoiceComparisonTemplateDataManager : IDataManager
    {
        bool TryAddOrUpdateInvoiceCompareTemplate(InvoiceComparisonTemplate invoiceComparisonTemplate);
        List<InvoiceComparisonTemplate> GetInvoiceCompareTemplates();
        bool AreInvoiceComparisonTemplatesUpdated(ref object updateHandle);
    }
}
