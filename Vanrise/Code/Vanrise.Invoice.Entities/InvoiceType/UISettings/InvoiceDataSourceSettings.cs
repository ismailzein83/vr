using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class InvoiceDataSourceSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context);
    }
    public interface IInvoiceDataSourceSettingsContext
    {
        IInvoiceActionContext InvoiceActionContext { get;}
    }
}
