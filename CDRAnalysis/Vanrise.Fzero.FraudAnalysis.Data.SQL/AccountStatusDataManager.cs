using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Linq;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class AccountStatusDataManager : BaseSQLDataManager, IAccountStatusDataManager
    {

        public AccountStatusDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public void LoadAccountStatus(Action<AccountStatus> onBatchReady, List<CaseStatus> caseStatuses)
        {
            List<int> selectedCasesStatusIds = new List<int>();
            foreach (var i in caseStatuses)
            {
                selectedCasesStatusIds.Add((int)i);
            }


            ExecuteReaderSP("FraudAnalysis.sp_AccountStatus_GetByStatuses", (reader) =>
            {
                while (reader.Read())
                {
                    AccountStatus accountStatus = new AccountStatus();
                    accountStatus.AccountNumber = reader["AccountNumber"] as string;
                    accountStatus.Status = GetReaderValue<CaseStatus>(reader, "Status");
                    onBatchReady(accountStatus);
                }



            }
            , string.Join(",", selectedCasesStatusIds));
        }


        public List<string> GetAccountNumberByStatus(List<CaseStatus> caseStatuses, string fromAccountNumber, int nbOfRecords)
        {
            List<int> selectedCasesStatusIds = new List<int>();
            foreach (var i in caseStatuses)
            {
                selectedCasesStatusIds.Add((int)i);
            }

            return GetItemsSP("FraudAnalysis.bp_AccountStatus_GetNumbersByStatuses", (reader) =>
            {
                return reader["AccountNumber"] as string;
            }, string.Join(",", selectedCasesStatusIds), fromAccountNumber, nbOfRecords);
        }

        public List<string> GetAccountNumbersByNumberPrefixAndStatuses(List<CaseStatus> caseStatuses, IEnumerable<string> numberPrefixes)
        {
            return GetItemsSP("[FraudAnalysis].[bp_AccountStatus_GetByNumberPrefixesAndStatuses]", (reader) =>
            {
                return reader["AccountNumber"] as string;
            }, string.Join(",", caseStatuses.Select(itm => (int)itm)), numberPrefixes != null ? String.Join(",", numberPrefixes) : null);
        }

    }
}
