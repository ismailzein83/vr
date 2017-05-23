using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.SQL
{
    public class InvoiceAccountDataManager : BaseSQLDataManager, IInvoiceAccountDataManager
    {  
      
        #region ctor
        public InvoiceAccountDataManager()
            : base(GetConnectionStringName("InvoiceDBConnStringKey", "InvoiceDBConnString"))
        {

        }

        #endregion

        public bool InsertInvoiceAccount(VRInvoiceAccount invoiceAccount, out long insertedId)
        {
            object insertedID;
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_InvoiceAccount_Insert", out insertedID, invoiceAccount.InvoiceTypeId, invoiceAccount.PartnerId, invoiceAccount.BED, invoiceAccount.EED, invoiceAccount.Status, invoiceAccount.IsDeleted);
            insertedId = Convert.ToInt64(insertedID);
            return (affectedRows > -1);
        }
        public bool UpdateInvoiceAccount(VRInvoiceAccount invoiceAccount)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_InvoiceAccount_Update", invoiceAccount.InvoiceAccountId, invoiceAccount.InvoiceTypeId, invoiceAccount.PartnerId, invoiceAccount.BED, invoiceAccount.EED, invoiceAccount.Status, invoiceAccount.IsDeleted);
            return (affectedRows > -1);
        }
    }
}
