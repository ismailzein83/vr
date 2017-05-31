using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Entities
{
    public enum CarrierType { Profile = 0, Account = 1 }
    public class InvoiceAccountInfoFilter
    {
        public Guid InvoiceTypeId { get; set; }
        public VRAccountStatus? Status { get; set; }
        public DateTime?EffectiveDate  { get; set; }
        public bool? IsEffectiveInFuture { get; set; }
        public CarrierType? CarrierType { get; set; }
        public ActivationStatus? ActivationStatus { get; set; }
        public List<IInvoicePartnerFilter> Filters { get; set; }
    }
}
