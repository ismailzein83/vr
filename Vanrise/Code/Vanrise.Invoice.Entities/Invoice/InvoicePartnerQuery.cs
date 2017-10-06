using System;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public enum InvoicePartnerPeriod { FollowBillingCycle = 0, FixedDates = 1 }
    public class InvoicePartnerQuery
    {
        public Guid InvoiceTypeId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsEffectiveInFuture { get; set; }
        public VRAccountStatus Status { get; set; }
        public PartnerGroup PartnerGroup { get; set; }
        public InvoicePartnerPeriod Period { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime IssueDate { get; set; }
    }
}