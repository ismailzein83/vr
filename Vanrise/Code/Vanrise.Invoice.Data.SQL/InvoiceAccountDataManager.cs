using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;
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
           insertedId = -1;
           int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_InvoiceAccount_Insert", out insertedID, invoiceAccount.InvoiceTypeId, invoiceAccount.PartnerId, invoiceAccount.BED, invoiceAccount.EED, invoiceAccount.Status, invoiceAccount.IsDeleted);
           if(affectedRows > -1)
           {
               insertedId = Convert.ToInt64(insertedID);
           }
           return (affectedRows > -1);
        }

        public bool TryUpdateInvoiceAccountStatus(Guid invoiceTypeId, string partnerId, VRAccountStatus status, bool isDeleted)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_InvoiceAccount_TryUpdateStatus", invoiceTypeId, partnerId, status, isDeleted);
            return (affectedRows > -1);
        }
        public bool TryUpdateInvoiceAccountEffectiveDate(Guid invoiceTypeId, string partnerId, DateTime? bed, DateTime? eed)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_InvoiceAccount_TryUpdateEffectiveDate", invoiceTypeId, partnerId, bed, eed);
            return (affectedRows > -1);
        }
        public List<VRInvoiceAccount> GetInvoiceAccountsByPartnerIds(Guid invoiceTypeId, IEnumerable<string> partnerIds)
        {
            string stringPartnerIds = null;
            if (partnerIds != null)
            {
                stringPartnerIds = String.Join<string>(",", partnerIds);
            }
            return GetItemsSP("VR_Invoice.sp_InvoiceAccount_GetByPartnerIds", VRInvoiceAccountMapper,invoiceTypeId, stringPartnerIds);
        }

        #region Mappers
        VRInvoiceAccount VRInvoiceAccountMapper(IDataReader reader)
        {
            return new VRInvoiceAccount
            {
                Status =  GetReaderValue<VRAccountStatus>(reader,"Status"),
                BED= GetReaderValue<DateTime>(reader,"BED"),
                EED= GetReaderValue<DateTime?>(reader,"EED"),
                InvoiceAccountId= GetReaderValue<long>(reader,"ID"),
                InvoiceTypeId= GetReaderValue<Guid>(reader,"InvoiceTypeId"),
                IsDeleted= GetReaderValue<bool>(reader,"IsDeleted"),
                PartnerId = reader["PartnerId"] as string
            };
        }

        #endregion


        
    }
}
