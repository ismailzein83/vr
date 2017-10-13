using System;

namespace Vanrise.Invoice.Entities
{
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
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public dynamic CustomPayload { get; set; }
    }

    public class InvoiceGenerationDraftToEdit
    {
        public long InvoiceGenerationDraftId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public dynamic CustomPayload { get; set; }
        public bool IsSelected { get; set; }
    }

    public class GenerateInvoicesOutput
    {

    }
}