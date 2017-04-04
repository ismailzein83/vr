using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Invoice.Entities;

namespace TOne.WhS.Invoice.Data
{
    public interface IInvoiceAccountDataManager:IDataManager
    {
        List<InvoiceAccount> GetInvoiceAccounts();
        bool Update(InvoiceAccount InvoiceAccount);
        bool Insert(InvoiceAccount InvoiceAccount, out int insertedId);
        bool AreInvoiceAccountsUpdated(ref object updateHandle);
    }
}
