using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class RDLCReportDataSourceSettings
    {
        public int ConfigId { get; set; }
        public abstract IEnumerable<dynamic> GetDataSourceItems(IRDLCReportDataSourceSettingsContext context);
    }
    public interface IRDLCReportDataSourceSettingsContext
    {
        IInvoiceActionContext InvoiceActionContext { get;}
    }
}
