using System;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class CaseManagementDataManager : BaseMySQLDataManager, ICaseManagementDataManager
    {
        public CaseManagementDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public bool SaveAccountCase(AccountCase accountCaseObject)
        {
            throw new NotImplementedException();
        }


        public Vanrise.Entities.BigResult<AccountCase> GetFilteredAccountCases(Vanrise.Entities.DataRetrievalInput<AccountCaseResultQuery> input)
        {
            throw new NotImplementedException();
        }
    }
}
