using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.MultiNet.Entities
{
    public class InvoiceDetails
    {
        public int Quantity { get; set; }
        public Decimal CurrentCharges { get; set; }
        public Decimal TotalCurrentCharges { get; set; }
        public Decimal SalesTaxAmount { get; set; }
        public Decimal SalesTax{ get; set; }
        public Decimal WHTaxAmount { get; set; }
        public Decimal WHTax { get; set; }
        public int CurrencyId { get; set; }
        public Decimal PayableByDueDate { get; set; }
        public Decimal LatePaymentCharges { get; set; }
        public Decimal PayableAfterDueDate { get; set; }
        public Guid AccountTypeId { get; set; }
        public decimal OTC { get; set; } //first Invoice
        public decimal LineRent { get; set; }
        public decimal InComing { get; set; }
        public decimal OutGoing { get; set; }
        public string ContractReferenceNumber { get; set; } //branch only
        public long CompanyId { get; set; }
        public long? BranchId { get; set; }
        public string BranchCode { get; set; }

    }
}
