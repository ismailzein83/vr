using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data
{
    public interface IInvoiceAccountDataManager:IDataManager
    {
        bool InsertInvoiceAccount(VRInvoiceAccount invoiceAccount, out long insertedId);
    }
}
