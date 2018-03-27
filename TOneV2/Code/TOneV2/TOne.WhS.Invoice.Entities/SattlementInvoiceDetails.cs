using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Invoice.Entities
{
    public class SattlementInvoiceDetails
    {
        public Decimal CustomerDuration { get; set; }
        public int CustomerTotalNumberOfCalls { get; set; }
        public string PartnerType { get; set; }
        public Decimal SupplierDuration { get; set; }
        public int SupplierTotalNumberOfCalls { get; set; }
        public decimal? Commission { get; set; }
        public bool HasComission { get; set; }

        public SattlementInvoiceDetails() { }
        public IEnumerable<SattlementInvoiceDetails> GetSattlementInvoiceDetailsRDLCSchema()
        {
            return null;
        }
    }
}
