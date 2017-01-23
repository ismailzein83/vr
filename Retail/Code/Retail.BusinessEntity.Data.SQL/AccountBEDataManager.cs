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

        public bool Insert(AccountToInsert accountToInsert, out long insertedId)
        {
            object accountId;
            string serializedSettings = accountToInsert.Settings != null ? Vanrise.Common.Serializer.Serialize(accountToInsert.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Account_Insert", out accountId, accountToInsert.Name, accountToInsert.TypeId, serializedSettings, accountToInsert.ParentAccountId, accountToInsert.StatusId, accountToInsert.SourceId);

            if (affectedRecords > 0)
            {
                insertedId = (int)accountId;
                return true;
            }

            insertedId = -1;
            return false;
        }

        public bool Update(AccountToEdit accountToEdit)
        {
            string serializedSettings = accountToEdit.Settings != null ? Vanrise.Common.Serializer.Serialize(accountToEdit.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Account_Update", accountToEdit.AccountId, accountToEdit.Name, accountToEdit.TypeId, serializedSettings, accountToEdit.SourceId);
            return (affectedRecords > 0);
        }

        public bool AreAccountsUpdated(Guid accountBEDefinitionId, ref object updateHandle)
        {
            string query = String.Format("SELECT MAX(a.timestamp) " +
                                         "FROM [Retail].[Account] a with(nolock) " +
                                         "Join [Retail_BE].[AccountType] t with(nolock) " +
                                         "on a.[TypeID] = t.[ID] " +
                                         "where t.[AccountBEDefinitionID] = '{0}'", accountBEDefinitionId);

            var newReceivedDataInfo = ExecuteScalarText(query, null);
            return base.IsDataUpdated(ref updateHandle, newReceivedDataInfo);
        
        }

        public bool UpdateStatus(long accountId, Guid statusId)
        {
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Account_UpdateStatusID", accountId, statusId);
            return (affectedRecords > 0);
        }

        public bool UpdateExecutedActions(long accountId, ExecutedActions executedAction)
        {
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Account_UpdateExecutedActions", accountId, Vanrise.Common.Serializer.Serialize(executedAction));
            return (affectedRecords > 0);
        }

        #endregion

        #region Mappers

        private Account AccountMapper(IDataReader reader)
        {
            return new Account()
            {
                AccountId = (long)reader["ID"],
                Name = reader["Name"] as string,
                TypeId = GetReaderValue<Guid>(reader, "TypeID"),
                Settings = Vanrise.Common.Serializer.Deserialize<AccountSettings>(reader["Settings"] as string),
                ParentAccountId = GetReaderValue<long?>(reader, "ParentID"),
                ExecutedActions = reader["ExecutedActionsData"] as string != null ? Vanrise.Common.Serializer.Deserialize<ExecutedActions>(reader["ExecutedActionsData"] as string) : null,
                StatusId = GetReaderValue<Guid>(reader, "StatusID"),
                SourceId = reader["SourceID"] as string
            };
        }

        #endregion
    }
}
