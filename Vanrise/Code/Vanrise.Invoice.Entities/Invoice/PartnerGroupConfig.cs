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
        Func<IPartnerStatusFilterMatchingContext, bool> IsStatusFilterMatching { get; }
    }

    public class PartnerGroupContext : IPartnerGroupContext
    {
        public Guid InvoiceTypeId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsEffectiveInFuture { get; set; }
        public VRAccountStatus Status { get; set; }
        public Func<IPartnerStatusFilterMatchingContext, bool> IsStatusFilterMatching { get; set; }
    }

    public interface IPartnerStatusFilterMatchingContext
    {
        string AccountId { get; }
        Guid InvoiceTypeId { get; }
        DateTime? EffectiveDate { get; }
        bool? IsEffectiveInFuture { get; }
        VRAccountStatus Status { get; }
        DateTime CurrentDate { get; }
    }

    public class PartnerStatusFilterMatchingContext : IPartnerStatusFilterMatchingContext
    {
        public string AccountId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsEffectiveInFuture { get; set; }
        public VRAccountStatus Status { get; set; }
        public DateTime CurrentDate { get; set; }
    }
}