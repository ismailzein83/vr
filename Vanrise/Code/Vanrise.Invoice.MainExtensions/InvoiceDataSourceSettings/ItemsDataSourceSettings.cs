﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class ItemsDataSourceSettings : InvoiceDataSourceSettings
    {
        public override Guid ConfigId { get { return new Guid("6721BF29-D257-47D9-8D56-4EE10538BFDC"); } }
        public List<string> ItemSetNames { get; set; }
        public CompareOperator CompareOperator { get; set; }
        public override IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context)
        {
            InvoiceItemManager invoiceItemManager = new InvoiceItemManager();
            var invoiceItems = context.InvoiceActionContext.GetInvoiceItems(this.ItemSetNames, this.CompareOperator);// invoiceItemManager.GetInvoiceItemsByItemSetNames(context.InvoiceId, this.ItemSetNames);
            List<dynamic> invoiceItemsDetails = new List<dynamic>();
            foreach(var invoiceItem in invoiceItems)
            {
                invoiceItemsDetails.Add(invoiceItem.Details);
            }
            return invoiceItemsDetails;
           
        }
    }
}
