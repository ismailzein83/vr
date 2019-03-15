using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
	public class CustomerSMSInvoiceItemDetails
	{
		public int NumberOfSMS { get; set; }
		public decimal SaleAmount { get; set; }
		public decimal OriginalSaleAmount { get; set; }
		public int MobileCountryId { get; set; }
		public int SupplierId { get; set; }
		public int CustomerId { get; set; }
		public long CustomerMobileNetworkId { get; set; }
		public int OriginalSaleCurrencyId { get; set; }
		public int SaleCurrencyId { get; set; }
		public Decimal SaleRate { get; set; }
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
		public decimal AmountAfterCommission { get; set; }
		public decimal OriginalAmountAfterCommission { get; set; }
		public decimal AmountAfterCommissionWithTaxes { get; set; }
		public decimal OriginalAmountAfterCommissionWithTaxes { get; set; }
		public decimal OriginalSaleAmountWithTaxes { get; set; }
		public decimal SaleAmountWithTaxes { get; set; }
		public CustomerSMSInvoiceItemDetails() { }
		public IEnumerable<CustomerInvoiceItemDetails> GetCustomerSMSInvoiceItemDetailsRDLCSchema()
		{
			return null;
		}
	}




}
