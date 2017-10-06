using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class PartnerGroupConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Invoice_Partnergroup";
        public string Editor { get; set; }
        public string BehaviorFQTN { get; set; }
    }

    public abstract class PartnerGroup
    {
        public abstract Guid ConfigId { get; }

        public abstract List<string> GetPartnerIds(IPartnerGroupContext context);
    }

    public interface IPartnerGroupContext
    {
        Guid InvoiceTypeId { get; }
        DateTime? EffectiveDate { get; }
        bool? IsEffectiveInFuture { get; }
        VRAccountStatus Status { get; }
    }

    public class PartnerGroupContext : IPartnerGroupContext
    {
        public Guid InvoiceTypeId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsEffectiveInFuture { get; set; }
        public VRAccountStatus Status { get; set; }
    }
}