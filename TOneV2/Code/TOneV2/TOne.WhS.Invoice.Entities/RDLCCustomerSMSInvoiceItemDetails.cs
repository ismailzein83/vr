using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
	public class RDLCCustomerSMSInvoiceItemDetails
	{
		public int NumberOfSMS { get; set; }
		public decimal SaleAmount { get; set; }
		public decimal OriginalSaleAmount { get; set; }
		public int CountryId { get; set; }
		public int SupplierId { get; set; }
		public int SupplierZoneId { get; set; }
		public int CustomerId { get; set; }
		public string CustomerIdDescription { get; set; }
		public long SaleZoneId { get; set; }
		public string SaleZoneIdDescription { get; set; }
		public long CustomerMobileNetworkId { get; set; }
		public string CustomerMobileNetworkIdDescription { get; set; }
		public long OriginalSaleCurrencyId { get; set; }
		public string OriginalSaleCurrencyIdDescription { get; set; }
		public string SaleCurrencyIdDescription { get; set; }
		public long SaleCurrencyId { get; set; }
		public string SaleCurrency { get; set; }
		public string SaleRateDescription { get; set; }
		public Decimal SaleRate { get; set; }
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
		public decimal AmountAfterCommission { get; set; }
		public decimal OriginalAmountAfterCommission { get; set; }
		public decimal AmountAfterCommissionWithTaxes { get; set; }
		public decimal OriginalAmountAfterCommissionWithTaxes { get; set; }
		public decimal OriginalSaleAmountWithTaxes { get; set; }
		public decimal SaleAmountWithTaxes { get; set; }
		public RDLCCustomerSMSInvoiceItemDetails() { }
		public IEnumerable<CustomerInvoiceItemDetails> GetCustomerSMSInvoiceItemDetailsRDLCSchema()
		{
			return null;
		}
	}




}
