using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
	public class SMSInvoiceMeasures
	{
		public decimal SaleNet { get; set; }
		public int NumberOfSMS { get; set; }
		public DateTime BillingPeriodTo { get; set; }
		public DateTime BillingPeriodFrom { get; set; }
		public decimal SaleNetWithTaxes { get; set; }
		public DateTime OriginalBillingPeriodTo { get; set; }
		public DateTime OriginalBillingPeriodFrom { get; set; }
		public decimal SaleNet_OrigCurr { get; set; }
		public decimal SaleNet_OrigCurrWithTaxes { get; set; }
		public decimal AmountAfterCommission { get; set; }
		public decimal OriginalAmountAfterCommission { get; set; }
		public decimal AmountAfterCommissionWithTaxes { get; set; }
		public decimal OriginalAmountAfterCommissionWithTaxes { get; set; }
	}
}
