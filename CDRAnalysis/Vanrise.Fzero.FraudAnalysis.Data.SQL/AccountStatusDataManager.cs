using System;
using System.Collections.Generic;
using System.Linq;
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
        #endregion
       
    }
}
