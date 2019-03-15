using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class CustomerInvoiceDealItemDetails
    {
        public long? SaleDealZoneGroupNb { get; set; }
        public int? SaleDealTierNb { get; set; }
        public int? SaleDeal { get; set; }
		public string SaleDealDescription { get; set; }
		public decimal? SaleDealRateTierNb { get; set; }
        public decimal Duration { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal OriginalAmountAfterTax { get; set; }
        public decimal Amount { get; set; }
		public string AmountDescription { get; set; }
		public int NumberOfCalls { get; set; }
        public int CurrencyId { get; set; }
		public string CurrencyIdDescription { get; set; }
		public DateTime FromDate { get; set; }
		public string FromDateDescription { get; set; }
		public DateTime ToDate { get; set; }
		public string ToDateDescription { get; set; }


	}
}
