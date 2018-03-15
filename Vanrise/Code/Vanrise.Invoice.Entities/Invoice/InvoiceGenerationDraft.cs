using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class PartnerInvoiceGenerationDraftItem
    {
        public long InvoiceGenerationDraftId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public dynamic CustomPayload { get; set; }

    }
    public class PartnerInvoiceGenerationDraft
    {
        public List<PartnerInvoiceGenerationDraftItem> Items { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public Guid InvoiceGenerationIdentifier { get; set; }
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
    }
    public class InvoiceGenerationDraft
    {
        public long InvoiceGenerationDraftId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public Guid InvoiceGenerationIdentifier { get; set; }
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public dynamic CustomPayload { get; set; }
    }

    public class InvoiceGenerationDraftDetail
    {
        public long InvoiceGenerationDraftId { get; set; }
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public dynamic CustomPayload { get; set; }
        public Dictionary<VRButtonType, List<InvoiceGenerationDraftActionDetail>> InvoiceGenerationDraftActionDetails { get; set; }
    }

    public class InvoiceGenerationDraftToEdit
    {
        public long InvoiceGenerationDraftId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public dynamic CustomPayload { get; set; }
        public bool IsSelected { get; set; }
    }

    public class InvoiceGenerationDraftActionDetail
    {
        public InvoiceGeneratorAction InvoiceGeneratorAction { get; set; }
        public InvoiceAction InvoiceAction { get; set; }
    }

    public class GenerateInvoicesOutput
    {
        public long? ProcessInstanceId { get; set; }
        public bool Succeed { get; set; }
        public string OutputMessage { get; set; }
    }

    public class InvoiceGenerationDraftSummary
    {
        public int TotalCount { get; set; }
        public DateTime? MinimumFrom { get; set; }
        public DateTime? MaximumTo { get; set; }
    }
}