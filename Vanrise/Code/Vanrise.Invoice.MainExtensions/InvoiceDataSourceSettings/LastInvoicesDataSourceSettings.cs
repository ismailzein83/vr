﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Common;
using Vanrise.Common.Business;
namespace Vanrise.Invoice.MainExtensions
{
    public class LastInvoicesDataSourceSettings : InvoiceDataSourceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A06AF611-6996-488D-B6D9-75BFC775947C"); }
        }
        public int LastInvoices { get; set; }
        public override IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context)
        {
            var currentInvoice = context.InvoiceActionContext.GetInvoice();
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(currentInvoice.InvoiceTypeId);
            var invoiceManager = new InvoiceManager();
            DateTime? datetime = currentInvoice.FromDate == DateTime.MinValue ? default(DateTime?) : currentInvoice.FromDate;
            var invoices = invoiceManager.GetLasInvoices(currentInvoice.InvoiceTypeId, currentInvoice.PartnerId, datetime, this.LastInvoices);
            List<InvoiceDataSourceItem> invoiceDataSourceItems = new List<InvoiceDataSourceItem>();
            if (invoices != null)
            {
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
                invoiceType.Settings.CurrencyFieldName.ThrowIfNull("invoiceType.Settings.CurrencyFieldName");
                invoiceType.Settings.AmountFieldName.ThrowIfNull("invoiceType.Settings.AmountFieldName");
                var currencyManager = new CurrencyManager();
                foreach (var invoice in invoices)
                {
                    InvoiceRecordObject invoiceRecordObject = new InvoiceRecordObject(invoice);
                    invoiceRecordObject.ThrowIfNull("invoiceRecordObject");
                    invoiceRecordObject.InvoiceDataRecordObject.ThrowIfNull("invoiceRecordObject.InvoiceDataRecordObject");
                    var currencyId = invoiceRecordObject.InvoiceDataRecordObject.GetFieldValue(invoiceType.Settings.CurrencyFieldName);
                    var amount = invoiceRecordObject.InvoiceDataRecordObject.GetFieldValue(invoiceType.Settings.AmountFieldName);
                    invoiceDataSourceItems.Add(new InvoiceDataSourceItem
                    {
                        Amount = amount,
                        CurrencyName = currencyManager.GetCurrencySymbol(currencyId),
                        CreatedTime = invoice.CreatedTime,
                        SerialNumber = invoice.SerialNumber,
                    });
                }
            }
            return invoiceDataSourceItems;
        }
    }
}
