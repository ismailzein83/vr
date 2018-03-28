using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public enum InvoicePartnerPeriod { FollowBillingCycle = 0, FixedDates = 1 }
    public class InvoiceGenerationDraftQuery
    {
        public Guid InvoiceTypeId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsEffectiveInFuture { get; set; }
        public VRAccountStatus Status { get; set; }
        public PartnerGroup PartnerGroup { get; set; }
        public InvoicePartnerPeriod Period { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? MaximumToDate { get; set; }
        public DateTime IssueDate { get; set; }
        public Guid InvoiceGenerationIdentifier { get; set; }
        public bool IsAutomatic { get; set; }
    }
    public class InvoiceGenerationPartnerDraftInput
    {
        public long InvoiceGenerationDraftId { get; set; }
        public bool IsSelected { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime IssueDate { get; set; }
        public Guid InvoiceTypeId { get; set; }
    }

    public enum InvoiceGenerationDraftResult { Succeeded = 0, Failed = 1 }
    public class InvoiceGenerationDraftOutput
    {
        public InvoiceGenerationDraftResult Result { get; set; }
        public string Message { get; set; }
        public int Count { get; set; }
        public DateTime MinimumFrom { get; set; }
        public DateTime MaximumTo { get; set; }
        public List<string> InvalidPartnerMessages { get; set; }
    }
}