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
                ValidTill = GetReaderValue<DateTime?>(reader, "ValidTill")

            };

            return accountStatus;
        }

        private string GetSingleQuery()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(String.Format(@"
                    SELECT   [AccountNumber] ,[Status]  ,[ValidTill]
                        FROM [FraudAnalysis].[AccountStatus]
                        WITH(NOLOCK) WHERE (AccountNumber like '%' + @AccountNumber + '%' and Status = @StatusID and ValidTill>= @FromDate AND (ValidTill<= @ToDate OR @ToDate IS NULL))  
                        "));
            return queryBuilder.ToString();
        }

        #endregion

        #region Public Methods
        public List<string> GetAccountNumbersByNumberPrefixAndStatuses(List<CaseStatus> caseStatuses, List<string> numberPrefixes)
        {
            return GetItemsSP("[FraudAnalysis].[bp_AccountStatus_GetByNumberPrefixesAndStatuses]", (reader) =>
            {
                return reader["AccountNumber"] as string;
            }, string.Join(",", caseStatuses.Select(itm => (int)itm)), numberPrefixes != null ? String.Join(",", numberPrefixes) : null);
        }

        public bool InsertOrUpdateAccountStatus(string accountNumber, CaseStatus caseStatus, DateTime? validTill)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountStatus_InsertOrUpdate", accountNumber, caseStatus, validTill);
            return (recordsAffected > 0);
        }

        public IEnumerable<AccountStatus> GetAccountStatusesData(Vanrise.Entities.DataRetrievalInput<AccountStatusQuery> input)
        {
            return GetItemsText(GetSingleQuery(), AccountStatusMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromDate));
                cmd.Parameters.Add(new SqlParameter("@ToDate", ToDBNullIfDefault(input.Query.ToDate)));
                cmd.Parameters.Add(new SqlParameter("@AccountNumber", input.Query.AccountNumber));
                cmd.Parameters.Add(new SqlParameter("@StatusID", (int)input.Query.Status));
            });
        }

        public bool Update(AccountStatus accountStatus)
        {
            int recordsEffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountStatus_Update", accountStatus.AccountNumber, accountStatus.ValidTill);
            return (recordsEffected > 0);
        }

        public bool Insert(AccountStatus accountStatus)
        {
            int recordsEffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountStatus_Insert", accountStatus.AccountNumber, (int)accountStatus.Status, accountStatus.ValidTill);

            return (recordsEffected > 0);
        }

        public AccountStatus GetAccountStatus(string accountNumber)
        {
            return GetItemSP("FraudAnalysis.sp_AccountStatus_GetByAccountNumber", AccountStatusMapper, accountNumber);
        }

        #endregion
    }
}
