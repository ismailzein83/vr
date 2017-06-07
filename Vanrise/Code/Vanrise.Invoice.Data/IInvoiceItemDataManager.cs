﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data
{
    public interface IInvoiceItemDataManager:IDataManager
    {
        string StorageConnectionStringKey { set; }
        IEnumerable<Entities.InvoiceItem> GetFilteredInvoiceItems(long invoiceId, string itemSetNam);
        IEnumerable<InvoiceItem> GetInvoiceItemsByItemSetNames(long invoiceId, IEnumerable<string> itemSetNames, CompareOperator compareOperator);
    }
}
