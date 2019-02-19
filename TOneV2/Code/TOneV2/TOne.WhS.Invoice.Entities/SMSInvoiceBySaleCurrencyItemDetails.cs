using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
	public class SMSInvoiceBySaleCurrencyItemDetails
	{
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
		public decimal AmountAfterCommission { get; set; }
		public decimal AmountAfterCommissionWithTaxes { get; set; }
		public decimal TotalRecurringChargeAmount { get; set; }
		public int NumberOfSMS { get; set; }
		public int CurrencyId { get; set; }
		public decimal Amount { get; set; }
		public string Month { get; set; }
	}
}
