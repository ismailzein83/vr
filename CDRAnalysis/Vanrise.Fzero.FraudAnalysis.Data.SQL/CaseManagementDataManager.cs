using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class CaseManagementDataManager : BaseSQLDataManager, ICaseManagementDataManager
    {
        public CaseManagementDataManager()
            : base("CDRDBConnectionString")
        {

        }


        public bool SaveAccountCase(AccountCase accountCaseObject)
        {
            int recordesEffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_Insert",  accountCaseObject.AccountNumber, accountCaseObject.StatusID, accountCaseObject.ValidTill , accountCaseObject.UserId  );
            if (recordesEffected > 0)
                return true;
            return false;
        }

    }
}
