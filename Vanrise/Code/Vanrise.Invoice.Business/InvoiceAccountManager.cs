﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class InvoiceAccountManager
    {
        public bool TryAddInvoiceAccount(VRInvoiceAccount invoiceAccount, out long insertedId)
        {
            IInvoiceAccountDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceAccountDataManager>();
           return dataManager.InsertInvoiceAccount(invoiceAccount, out insertedId);
        }
        public bool TryUpdateInvoiceAccount(VRInvoiceAccount invoiceAccount)
        {
            IInvoiceAccountDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceAccountDataManager>();
            return dataManager.TryUpdateInvoiceAccount(invoiceAccount);
        }
    }
}
