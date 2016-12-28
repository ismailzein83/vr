using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class AccountBEDataManager : BaseSQLDataManager, IAccountBEDataManager
    {
        #region Constructors

        public AccountBEDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<Account> GetAccounts(Guid accountBEDefinitionId)
        {
            return GetItemsSP("Retail.sp_Account_GetByDefinition", AccountMapper, accountBEDefinitionId);
        }

        public bool Insert(Account account, out long insertedId)
        {
            object accountId;
            string serializedSettings = account.Settings != null ? Vanrise.Common.Serializer.Serialize(account.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Account_Insert", out accountId, account.Name, account.TypeId, serializedSettings, account.ParentAccountId, account.StatusId, account.SourceId);

            if (affectedRecords > 0)
            {
                insertedId = (int)accountId;
                return true;
            }

            insertedId = -1;
            return false;
        }

        public bool Update(AccountToEdit account, long? parentId)
        {
            string serializedSettings = account.Settings != null ? Vanrise.Common.Serializer.Serialize(account.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Account_Update", account.AccountId, account.Name, account.TypeId, serializedSettings, parentId, account.SourceId);
            return (affectedRecords > 0);
        }

        public bool AreAccountsUpdated(Guid accountBEDefinitionId, ref object updateHandle)
        {
            string query = String.Format("SELECT MAX(timestamp) " +
                                         "FROM [Retail].[Account] a with(nolock) " + 
                                         "Join [Retail_BE].[AccountType] t with(nolock) " + 
                                         "on a.[TypeID] = t.[ID] " +
                                         "where t.[AccountBEDefinitionID] = {0}", accountBEDefinitionId);

            var newReceivedDataInfo = ExecuteScalarText(query, null);
            return base.IsDataUpdated(ref updateHandle, newReceivedDataInfo);
        }

        #endregion

        #region Mappers

        private Account AccountMapper(IDataReader reader)
        {
            return new Account()
            {
                AccountId = (long)reader["ID"],
                Name = reader["Name"] as string,
                TypeId = GetReaderValue<Guid>(reader,"TypeID"),
                Settings = Vanrise.Common.Serializer.Deserialize<AccountSettings>(reader["Settings"] as string),
                ParentAccountId = GetReaderValue<long?>(reader, "ParentID"),
                ExecutedActions = reader["ExecutedActionsData"] as string != null?Vanrise.Common.Serializer.Deserialize<ExecutedActions>(reader["ExecutedActionsData"] as string) :null,
                StatusId = GetReaderValue<Guid>(reader, "StatusID"),
                SourceId = reader["SourceID"] as string
            };
        }

        #endregion
    }
}
