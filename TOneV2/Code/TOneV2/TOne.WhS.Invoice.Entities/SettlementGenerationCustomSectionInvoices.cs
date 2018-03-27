using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Invoice.Entities
{
    public class SettlementGenerationCustomSectionInvoices
    {
        public string PartnerName { get; set; }
        public string PartnerType { get; set; }

        public List<CustomerInvoiceDetail> CustomerInvoiceDetails { get; set; }
        public List<SupplierInvoiceDetail> SupplierInvoiceDetails { get; set; }

    }
    public class CustomerInvoiceDetail
    {
        public long InvoiceId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime IssueDate { get; set; }
        public int TotalNumberOfCalls { get; set; }
        public decimal Duration { get; set; }
        public string SaleCurrency { get; set; }
        public int? TimeZoneId { get; set; }
        public Decimal TotalAmountAfterCommission { get; set; }
        public decimal? Commission { get; set; }
        public string Offset { get; set; }
        public string TimeZoneDescription { get; set; }

    }
    public class SupplierInvoiceDetail
    {
        public long InvoiceId { get; set; }

        public string SerialNumber { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime IssueDate { get; set; }
        public int TotalNumberOfCalls { get; set; }
        public decimal Duration { get; set; }
        public string SupplierCurrency { get; set; }
        public int? TimeZoneId { get; set; }
        public string TimeZoneDescription { get; set; }
        public Decimal TotalAmountAfterCommission { get; set; }
        public decimal? Commission { get; set; }
        public string Offset { get; set; }
    }
}
