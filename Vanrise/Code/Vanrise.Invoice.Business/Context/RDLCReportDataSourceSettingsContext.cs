using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.Context
{
    public class RDLCReportDataSourceSettingsContext : IInvoiceDataSourceSettingsContext
    {
        public IInvoiceActionContext InvoiceActionContext { get; set; }
    }
}
