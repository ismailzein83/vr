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

        public List<string> GetAccountNumbersByNumberPrefixAndStatuses(List<CaseStatus> caseStatuses, List<string> numberPrefixes)
        {
            return GetItemsSP("[FraudAnalysis].[bp_AccountStatus_GetByNumberPrefixesAndStatuses]", (reader) =>
            {
                return reader["AccountNumber"] as string;
            }, string.Join(",", caseStatuses.Select(itm => (int)itm)), numberPrefixes != null ? String.Join(",", numberPrefixes) : null);
        }
    }
}
