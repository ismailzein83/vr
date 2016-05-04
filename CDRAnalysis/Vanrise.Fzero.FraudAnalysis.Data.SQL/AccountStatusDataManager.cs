using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class AccountStatusDataManager : BaseSQLDataManager, IAccountStatusDataManager
    {
        #region ctor
        public AccountStatusDataManager()
            : base("CDRDBConnectionString")
        {

        }
        #endregion

        #region Private
        private AccountStatus AccountStatusMapper(IDataReader reader)
        {
            AccountStatus accountStatus = new AccountStatus
            {
                AccountNumber = reader["AccountNumber"] as string,
                Status = GetReaderValue<CaseStatus>(reader, "Status"),
                ValidTill = GetReaderValue<DateTime?>(reader, "ValidTill"),
                Source = GetReaderValue<AccountStatusSource>(reader, "Source"),
                Reason = reader["Reason"] as string,
                UserId = (int)reader["UserId"]
            };

            return accountStatus;
        }

        private string GetSingleQuery(Vanrise.Entities.DataRetrievalInput<AccountStatusQuery> input)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(String.Format(@"
                    SELECT   [AccountNumber] ,[Status]  ,[ValidTill], [Reason], [Source], [UserId]
                        FROM [FraudAnalysis].[AccountStatus]
                        WITH(NOLOCK) #WHERE_CLAUSE#  
                        "));

            queryBuilder.Replace("#WHERE_CLAUSE#", GetWhereClause(input.Query.AccountNumber, input.Query.Sources, input.Query.UserIds, input.Query.FromDate, input.Query.ToDate, input.Query.Status));
            return queryBuilder.ToString();
        }

        private string GetWhereClause(string accountNumber, List<AccountStatusSource> sources, List<int> userIds, DateTime fromDate, DateTime? toDate, CaseStatus status)
        {
            StringBuilder whereClause = new StringBuilder();

            if (toDate.HasValue)
                whereClause.Append("WHERE ValidTill Is NULL OR ValidTill >= '" + fromDate + "' AND ValidTill < '" + toDate + "'");
            else
                whereClause.Append("WHERE ValidTill Is NULL OR ValidTill >= '" + fromDate + "' ");

            whereClause.Append(" AND Status = " + ((int)status).ToString());

            if (accountNumber != null)
                whereClause.Append(" AND AccountNumber like '%" + accountNumber + "%'");

            if (sources != null && sources.Count > 0)
                whereClause.Append(" AND Source IN (" + string.Join(",", GetAccountStatusSourceListAsIntList(sources)) + ")");

            if (userIds != null && userIds.Count > 0)
                whereClause.Append(" AND UserID IN (" + string.Join(",", userIds) + ")");

            return whereClause.ToString();
        }

        private List<int> GetAccountStatusSourceListAsIntList(List<AccountStatusSource> items)
        {
            List<int> list = new List<int>();

            foreach (AccountStatusSource item in items)
                list.Add((int)item);

            return list;
        }

        #endregion

        #region Public Methods

        public bool ApplyAccountStatuses(DataTable accountStatusDataTables, DateTime validTill, string reason, int userId)
        {
            int recordsAffected = 0;
            if (accountStatusDataTables.Rows.Count > 0)
            {
                recordsAffected = ExecuteNonQuerySPCmd("[FraudAnalysis].[sp_AccountStatus_BulkUpdate]",
                       (cmd) =>
                       {
                           var dtPrm = new System.Data.SqlClient.SqlParameter("@AccountStatus", SqlDbType.Structured);
                           dtPrm.Value = accountStatusDataTables;
                           cmd.Parameters.Add(dtPrm);
                           cmd.Parameters.Add(new SqlParameter("@ValidTill", validTill));
                           cmd.Parameters.Add(new SqlParameter("@Source", (int)AccountStatusSource.ManualUpload));
                           cmd.Parameters.Add(new SqlParameter("@Reason", reason));
                           cmd.Parameters.Add(new SqlParameter("@UserId", userId));
                       });
            }

            return (recordsAffected > 0);
        }

        public List<string> GetAccountNumbersByNumberPrefixAndStatuses(List<CaseStatus> caseStatuses, List<string> numberPrefixes)
        {
            return GetItemsSP("[FraudAnalysis].[bp_AccountStatus_GetByNumberPrefixesAndStatuses]", (reader) =>
            {
                return reader["AccountNumber"] as string;
            }, string.Join(",", caseStatuses.Select(itm => (int)itm)), numberPrefixes != null ? String.Join(",", numberPrefixes) : null);
        }

        public bool InsertOrUpdateAccountStatus(string accountNumber, CaseStatus caseStatus, DateTime? validTill, string reason, int userId)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountStatus_InsertOrUpdate", accountNumber, caseStatus, validTill, reason, AccountStatusSource.CaseUpdate, userId);
            return (recordsAffected > 0);
        }

        public IEnumerable<AccountStatus> GetAccountStatusesData(Vanrise.Entities.DataRetrievalInput<AccountStatusQuery> input)
        {
            return GetItemsText(GetSingleQuery(input), AccountStatusMapper, (cmd) => { });
        }

        public bool Update(AccountStatus accountStatus)
        {
            int recordsEffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountStatus_Update", accountStatus.AccountNumber, accountStatus.ValidTill, (int)accountStatus.Source, accountStatus.Reason, accountStatus.UserId);
            return (recordsEffected > 0);
        }

        public bool Insert(AccountStatus accountStatus)
        {
            int recordsEffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountStatus_Insert", accountStatus.AccountNumber, (int)accountStatus.Status, accountStatus.ValidTill, (int)accountStatus.Source, accountStatus.Reason, accountStatus.UserId);

            return (recordsEffected > 0);
        }

        public bool Delete(string number)
        {
            int recordsEffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountStatus_Delete", number);

            return (recordsEffected > 0);
        }

        public AccountStatus GetAccountStatus(string accountNumber)
        {
            return GetItemSP("FraudAnalysis.sp_AccountStatus_GetByAccountNumber", AccountStatusMapper, accountNumber);
        }

        #endregion
    }
}
