﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class RDLCSupplierSMSInvoiceItemDetails
    {
        public int NumberOfSMS { get; set; }
        public Double SupplierAmount { get; set; }
        public string DimensionName { get; set; }
        public decimal SupplierRate { get; set; }
		public string SupplierRateDescription { get; set; }

		public string SupplierCurrency { get; set; }
        public decimal CostAmount { get; set; }
        public decimal OriginalCostAmount { get; set; }

        public int SupplierId { get; set; }
		public string SupplierIdDescription { get; set; }

		public long SupplierMobileNetworkId { get; set; }
		public string SupplierMobileNetworkIdDescription { get; set; }

		public long OriginalSupplierCurrencyId { get; set; }
		public string OriginalSupplierCurrencyIdDescription { get; set; }

		public long SupplierCurrencyId { get; set; }
		public string SupplierCurrencyIdDescription { get; set; }

		public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal AmountAfterCommission { get; set; }
        public decimal OriginalAmountAfterCommission { get; set; }
        public decimal AmountAfterCommissionWithTaxes { get; set; }
        public decimal OriginalAmountAfterCommissionWithTaxes { get; set; }
        public decimal OriginalSupplierAmountWithTaxes { get; set; }
        public decimal SupplierAmountWithTaxes { get; set; }
        public IEnumerable<SupplierSMSInvoiceItemDetails> GetSupplierSMSInvoiceItemDetailsRDLCSchema()
        {
            return null;
        }
	}
}
