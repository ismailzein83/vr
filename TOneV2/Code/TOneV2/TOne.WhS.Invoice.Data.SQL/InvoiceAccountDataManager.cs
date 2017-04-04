using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Invoice.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Invoice.Data.SQL
{
    public class InvoiceAccountDataManager : BaseSQLDataManager,IInvoiceAccountDataManager
    {
        public InvoiceAccountDataManager()
            : base(GetConnectionStringName("WhS_Invoice_InvoiceDBConnStringKey", "WhS_Invoice_InvoiceDBConnStringKey"))
        {

        }
        public List<InvoiceAccount> GetInvoiceAccounts()
        {
            return GetItemsSP("[TOneWhS_Invoice].[sp_CarrierInvoiceAccount_GetAll]", InvoiceAccountMapper);
        }

        public bool Update(InvoiceAccount InvoiceAccount)
        {
            string serializedSettings = null;
            if (InvoiceAccount.Settings != null)
                serializedSettings = Vanrise.Common.Serializer.Serialize(InvoiceAccount.Settings);
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_Invoice].[sp_CarrierInvoiceAccount_Update]", InvoiceAccount.InvoiceAccountId, InvoiceAccount.CarrierProfileId, InvoiceAccount.CarrierAccountId, serializedSettings, InvoiceAccount.BED, InvoiceAccount.EED);
            return (recordsEffected > 0);
        }

        public bool Insert(InvoiceAccount InvoiceAccount, out int insertedId)
        {
            object InvoiceAccountId;
            
            string serializedSettings = null;
            if (InvoiceAccount.Settings != null)
                serializedSettings = Vanrise.Common.Serializer.Serialize(InvoiceAccount.Settings);

            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_Invoice].[sp_CarrierInvoiceAccount_Insert]", out InvoiceAccountId, InvoiceAccount.CarrierProfileId, InvoiceAccount.CarrierAccountId, serializedSettings, InvoiceAccount.BED, InvoiceAccount.EED);
            insertedId = (int)InvoiceAccountId;
            return (recordsEffected > 0);
        }

        public bool AreInvoiceAccountsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[TOneWhS_Invoice].[CarrierInvoiceAccount]", ref updateHandle);
        }


        #region Mappers

        InvoiceAccount InvoiceAccountMapper(IDataReader reader)
        {
            InvoiceAccount InvoiceAccount = new InvoiceAccount();
            InvoiceAccount.InvoiceAccountId = (int)reader["ID"];
            InvoiceAccount.CarrierAccountId = GetReaderValue<int?>(reader, "CarrierAccountId");
            InvoiceAccount.CarrierProfileId = GetReaderValue<int?>(reader, "CarrierProfileId");
            InvoiceAccount.Settings = Vanrise.Common.Serializer.Deserialize<InvoiceAccountSettings>(reader["InvoiceAccountSettings"] as string);
            InvoiceAccount.BED = GetReaderValue<DateTime>(reader, "BED");
            InvoiceAccount.EED = GetReaderValue<DateTime?>(reader, "EED");
            return InvoiceAccount;
        }

        # endregion

    }
}
