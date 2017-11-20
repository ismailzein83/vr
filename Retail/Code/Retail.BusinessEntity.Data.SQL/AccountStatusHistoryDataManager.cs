using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class AccountStatusHistoryDataManager : BaseSQLDataManager, IAccountStatusHistoryDataManager
    {

        #region Constructors
        public AccountStatusHistoryDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public void Insert(Guid accountDefinitionId, long accountId, Guid statusDefinitionId, Guid? previousStatusId, DateTime statusChangedDate)
        {
            ExecuteNonQuerySP("Retail_BE.sp_AccountStatusHistory_Insert", accountDefinitionId, accountId, statusDefinitionId, previousStatusId, statusChangedDate);
        }
        public void DeleteAccountStatusHistories(List<long> accountStatusHistoryIdsToDelete)
        {
            string accountStatusHistoryIdsToDeleteAsString = null;
            if (accountStatusHistoryIdsToDelete != null)
                accountStatusHistoryIdsToDeleteAsString = string.Join(",", accountStatusHistoryIdsToDelete);
            ExecuteNonQuerySP("Retail_BE.[sp_AccountStatusHistory_Delete]", accountStatusHistoryIdsToDeleteAsString);
        }
        public IEnumerable<AccountStatusHistory> GetAccountStatusHistoriesAfterDate(Guid accountBEDefinitionId, List<long> accountIds, DateTime statusChangedDate)
        {
            string accountIdsAsString = null;
            if(accountIds != null)
                accountIdsAsString = string.Join(",",accountIds);
            return GetItemsSP("[Retail_BE].[sp_AccountStatusHistory_GetSpecificAfterDate]", AccountStatusHistoryMapper, accountBEDefinitionId, accountIdsAsString, statusChangedDate);
        }

        public List<AccountStatusHistory> GetAccountStatusHistoryList(HashSet<AccountDefinition> accountDefinitions)
        {
            DataTable dtAccountDefinition = BuildAccountDefinitionTable(accountDefinitions);
            return GetItemsSPCmd("[Retail_BE].[sp_AccountStatusHistory_GetByAccountDefinitions]", AccountStatusHistoryMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@AccountDefinitions", SqlDbType.Structured);
                dtPrm.Value = dtAccountDefinition;
                cmd.Parameters.Add(dtPrm);
            });
        }
        #endregion

        #region Private Methods
        private DataTable BuildAccountDefinitionTable(HashSet<AccountDefinition> accountDefinitions)
        {
            DataTable dtAccountDefinition = GetAccountDefinitionTable();
            dtAccountDefinition.BeginLoadData();
            if (accountDefinitions != null)
            {
                foreach (var accountDefinition in accountDefinitions)
                {
                    DataRow dr = dtAccountDefinition.NewRow();
                    dr["AccountBEDefinitionID"] = accountDefinition.AccountBEDefinitionId;
                    dr["AccountID"] = accountDefinition.AccountId;
                    dtAccountDefinition.Rows.Add(dr);
                }
            }
            dtAccountDefinition.EndLoadData();
            return dtAccountDefinition;
        }

        private DataTable GetAccountDefinitionTable()
        {
            DataTable dtAccountDefinition = new DataTable();
            dtAccountDefinition.Columns.Add("AccountBEDefinitionID", typeof(Guid));
            dtAccountDefinition.Columns.Add("AccountID", typeof(long));
            return dtAccountDefinition;
        }
        #endregion

        #region Mappers
        private AccountStatusHistory AccountStatusHistoryMapper(IDataReader reader)
        {
            return new AccountStatusHistory()
            {
                AccountStatusHistoryId = (long)reader["ID"],
                AccountBEDefinitionId = (Guid)reader["AccountBEDefinitionID"],
                AccountId = (long)reader["AccountID"],
                StatusId = (Guid)reader["StatusID"],
                PreviousStatusId = GetReaderValue<Guid?>(reader, "PreviousStatusID"),
                StatusChangedDate = (DateTime)reader["StatusChangedDate"]
            };
        }
        #endregion



    }
}
