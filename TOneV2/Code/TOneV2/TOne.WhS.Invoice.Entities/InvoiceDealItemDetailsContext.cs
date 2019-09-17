using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace TOne.WhS.Invoice.Entities
{
    public class InvoiceDealItemDetailsContext
    {
        public List<CustomerInvoiceDealItemDetails> AllCustomerDealItemSetNames { get; set; }
        public List<SupplierInvoiceDealItemDetails> AllSupplierDealItemSetNames { get; set; }
        public int EffectiveDealId { get; set; }
        public string ZoneGroupName { get; set; }
        public int ZoneGroupNumber { get; set; }
        public decimal Rate { get; set; }
        public int Volume { get; set; }
        public int DealCurrencyId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime FromDateBeforeShift { get; set; }
        public DateTime IssueDate { get; set; }
        public int CurrencyId { get; set; }
        public IEnumerable<VRTaxItemDetail> TaxItemDetails { get; set; }
    }
}
