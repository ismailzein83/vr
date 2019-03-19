using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class SupplierInvoiceDealItemDetails
    {
        public long? CostDealZoneGroupNb { get; set; }
        public int? CostDealTierNb { get; set; }
        public int? CostDeal { get; set; }
		public string  CostDealDescription { get; set; }
		public decimal? CostDealRateTierNb { get; set; }
        public decimal CostRate { get; set; }
        public decimal Duration { get; set; }
        public decimal OriginalAmount { get; set; }
        public int NumberOfCalls { get; set; }
        public int CurrencyId { get; set; }
		public string CurrencyIdDescription { get; set; }
        public decimal OriginalAmountAfterTax { get; set; }
        public decimal Amount { get; set; }
		public string AmountDescription { get; set; }
		public DateTime ToDate { get; set; }
		public string ToDateDescription { get; set; }
		public DateTime FromDate { get; set; }
		public string FromDateDescription { get; set; }

	}
}
