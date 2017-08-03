using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data
{
    public interface IInvoiceAccountDataManager:IDataManager
    {
        bool InsertInvoiceAccount(VRInvoiceAccount invoiceAccount, out long insertedId);
        bool TryUpdateInvoiceAccountStatus(Guid invoiceTypeId, string partnerId, VRAccountStatus status, bool isDeleted);
        bool TryUpdateInvoiceAccountEffectiveDate(Guid invoiceTypeId, string partnerId, DateTime? bed, DateTime? eed);
        List<VRInvoiceAccount> GetInvoiceAccountsByPartnerIds(Guid invoiceTypeId, IEnumerable<string> partnerIds);
    }
}
