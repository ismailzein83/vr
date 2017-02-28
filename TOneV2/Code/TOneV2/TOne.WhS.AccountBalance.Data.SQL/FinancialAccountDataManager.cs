using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.AccountBalance.Data.SQL
{
    public class FinancialAccountDataManager : BaseSQLDataManager,IFinancialAccountDataManager
    {
        public FinancialAccountDataManager()
            : base(GetConnectionStringName("WhS_AccountBalance_AccountBalanceDBConnStringKey", "WhS_AccountBalance_AccountBalanceDBConnStringKey"))
        {

        }
        public List<FinancialAccount> GetFinancialAccounts()
        {
            return GetItemsSP("[TOneWhS_AccBalance].[sp_CarrierFinancialAccount_GetAll]", FinancialAccountMapper);
        }

        public bool Update(FinancialAccount financialAccount)
        {
            string serializedSettings = null;
            if (financialAccount.Settings != null)
                serializedSettings = Vanrise.Common.Serializer.Serialize(financialAccount.Settings);
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_AccBalance].[sp_CarrierFinancialAccount_Update]", financialAccount.FinancialAccountId, financialAccount.CarrierProfileId, financialAccount.CarrierAccountId, serializedSettings,financialAccount.BED, financialAccount.EED);
            return (recordsEffected > 0);
        }

        public bool Insert(FinancialAccount financialAccount, out int insertedId)
        {
            object financialAccountId;
            
            string serializedSettings = null;
            if (financialAccount.Settings != null)
                serializedSettings = Vanrise.Common.Serializer.Serialize(financialAccount.Settings);

            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_AccBalance].[sp_CarrierFinancialAccount_Insert]", out financialAccountId, financialAccount.CarrierProfileId, financialAccount.CarrierAccountId, serializedSettings, financialAccount.BED, financialAccount.EED);
            insertedId = (int)financialAccountId;
            return (recordsEffected > 0);
        }

        public bool AreFinancialAccountsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[TOneWhS_AccBalance].[CarrierFinancialAccount]", ref updateHandle);
        }


        #region Mappers

        FinancialAccount FinancialAccountMapper(IDataReader reader)
        {
            FinancialAccount financialAccount = new FinancialAccount();
            financialAccount.FinancialAccountId = (int)reader["ID"];
            financialAccount.CarrierAccountId = GetReaderValue<int?>(reader, "CarrierAccountId");
            financialAccount.CarrierProfileId = GetReaderValue<int?>(reader, "CarrierProfileId");
            financialAccount.Settings = Vanrise.Common.Serializer.Deserialize<FinancialAccountSettings>(reader["FinancialAccountSettings"] as string);
            financialAccount.BED = GetReaderValue<DateTime>(reader, "BED");
            financialAccount.EED = GetReaderValue<DateTime?>(reader, "EED");
            return financialAccount;
        }

        # endregion

    }
}
