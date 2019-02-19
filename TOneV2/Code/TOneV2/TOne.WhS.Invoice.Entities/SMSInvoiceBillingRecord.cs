using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
	public class SMSInvoiceBillingRecord
	{
		public long SaleZoneId { get; set; }
		public int CustomerId { get; set; }
		public int OriginalSaleCurrencyId { get; set; }
		public Decimal SaleRate { get; set; }
		public int SaleCurrencyId { get; set; }
		public int CountryId { get; set; }
		public int SupplierId { get; set; }
		public int SupplierZoneId { get; set; }
		public SMSInvoiceMeasures InvoiceMeasures { get; set; }
	}
}
