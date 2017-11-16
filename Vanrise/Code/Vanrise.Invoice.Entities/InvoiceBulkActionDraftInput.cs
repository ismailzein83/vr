using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public enum HandlingErrorOption { Skip = 1, Stop = 2 }
    public class ExecuteMenualInvoiceActionsInput
    {
        public Guid InvoiceTypeId { get; set; }
        public Guid InvoiceBulkActionIdentifier { get; set; }
        public List<InvoiceBulkActionRuntime> InvoiceBulkActions { get; set; }
        public bool IsAllInvoicesSelected { get; set; }
        public List<long> TargetInvoicesIds { get; set; }
        public HandlingErrorOption HandlingErrorOption { get; set; }
    }
    public class ExecuteMenualInvoiceActionsOutput
    {
        public long? ProcessInstanceId { get; set; }
        public bool Succeed { get; set; }
        public string OutputMessage { get; set; }
    }
}
