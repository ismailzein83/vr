using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class PartnerInvoiceSettingQuery
    {
        public Guid InvoiceSettingId { get; set; }
        public List<string> PartnerIds { get; set; }
        public List<Guid> PartsConfigIds { get; set; }
        public Boolean? IsEffectiveInFuture { get; set; }
        public VRAccountStatus? Status { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
