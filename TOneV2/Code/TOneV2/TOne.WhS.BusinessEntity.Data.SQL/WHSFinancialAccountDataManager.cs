using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class WHSFinancialAccountDataManager : BaseSQLDataManager, IWHSFinancialAccountDataManager
    {
      
        #region ctor/Local Variables
        public WHSFinancialAccountDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }
        #endregion

        #region Public Methods

        public List<WHSFinancialAccount> GetFinancialAccounts()
        {
            return GetItemsSP("[TOneWhS_BE].[sp_FinancialAccount_GetAll]", FinancialAccountMapper);

        }

        public bool Update(Entities.WHSFinancialAccountToEdit financialAccountToEdit)
        {
            string serializedSettings = null;
            if (financialAccountToEdit.Settings != null)
                serializedSettings = Vanrise.Common.Serializer.Serialize(financialAccountToEdit.Settings);
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_FinancialAccount_Update]", financialAccountToEdit.FinancialAccountId, serializedSettings, financialAccountToEdit.BED, financialAccountToEdit.EED, financialAccountToEdit.LastModifiedBy);
            return (recordsEffected > 0);
        }

        public bool Insert(Entities.WHSFinancialAccount financialAccount, out int insertedId)
        {
            object financialAccountId;
            string serializedSettings = null;
            if (financialAccount.Settings != null)
                serializedSettings = Vanrise.Common.Serializer.Serialize(financialAccount.Settings);
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_FinancialAccount_Insert]", out financialAccountId, financialAccount.CarrierProfileId, financialAccount.CarrierAccountId,financialAccount.FinancialAccountDefinitionId, serializedSettings, financialAccount.BED, financialAccount.EED, financialAccount.CreatedBy, financialAccount.LastModifiedBy);
            insertedId = (int)financialAccountId;
            return (recordsEffected > 0);
        }

        public bool AreFinancialAccountsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[TOneWhS_BE].[FinancialAccount]", ref updateHandle);
        }
       
        # endregion

        #region Mappers

        WHSFinancialAccount FinancialAccountMapper(IDataReader reader)
        {
            WHSFinancialAccount financialAccount = new WHSFinancialAccount();
            financialAccount.FinancialAccountId = (int)reader["ID"];
            financialAccount.CarrierAccountId = GetReaderValue<int?>(reader, "CarrierAccountId");
            financialAccount.CarrierProfileId = GetReaderValue<int?>(reader, "CarrierProfileId");
            financialAccount.FinancialAccountDefinitionId = GetReaderValue<Guid>(reader, "FinancialAccountDefinitionId");
            financialAccount.Settings = Vanrise.Common.Serializer.Deserialize<WHSFinancialAccountSettings>(reader["FinancialAccountSettings"] as string);
            financialAccount.BED = GetReaderValue<DateTime>(reader, "BED");
            financialAccount.EED = GetReaderValue<DateTime?>(reader, "EED");
            financialAccount.CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime");
            financialAccount.CreatedBy = GetReaderValue<int?>(reader, "CreatedBy");
            financialAccount.LastModifiedBy = GetReaderValue<int?>(reader, "LastModifiedBy");
            financialAccount.LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime");
            return financialAccount;
        }

        # endregion

    }
}
