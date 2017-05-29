using System;
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

        public bool TryAddInvoiceAccount(Guid invoiceTypeId, string partnerId, DateTime? bed, DateTime? eed, VRInvoiceAccountStatus status, bool isDeleted)
        {
            IInvoiceAccountDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceAccountDataManager>();
            long insertedId = -1;
            return TryAddInvoiceAccount(new VRInvoiceAccount
            {
                BED = bed,
                Status = status,
                EED = eed,
                InvoiceTypeId = invoiceTypeId,
                IsDeleted = isDeleted,
                PartnerId = partnerId
            }, out insertedId);
        }
        public bool TryUpdateInvoiceAccount(Guid invoiceTypeId, string partnerId, DateTime? bed, DateTime? eed, VRInvoiceAccountStatus status, bool isDeleted)
        {
            IInvoiceAccountDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceAccountDataManager>();
            return TryUpdateInvoiceAccount(new VRInvoiceAccount
            {
                BED = bed,
                Status = status,
                EED = eed,
                InvoiceTypeId = invoiceTypeId,
                IsDeleted = isDeleted,
                PartnerId = partnerId
            });
        }
    }
}
