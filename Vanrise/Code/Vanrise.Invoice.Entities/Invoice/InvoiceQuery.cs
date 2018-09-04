using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceQuery
    {
        public Guid InvoiceTypeId { get; set; }
        public List<string> PartnerIds { get; set; }
        public string PartnerPrefix { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsEffectiveInFuture { get; set; }
        public VRAccountStatus? Status { get; set; }
        public bool IncludeAllFields { get; set; }

        public bool? IsSelectAll { get; set; }
        public Guid? InvoiceBulkActionIdentifier { get; set; }
        public bool? IsPaid { get; set; }
        public bool? IsSent { get; set; }
        public List<string> InvoiceSettingIds { get; set; }

    }
}
