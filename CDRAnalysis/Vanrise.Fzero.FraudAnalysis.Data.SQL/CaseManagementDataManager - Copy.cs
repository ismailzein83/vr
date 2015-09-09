using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public partial class CaseManagementDataManager : BaseSQLDataManager, ICaseManagementDataManager
    {
        public bool CancelAccountCases(int strategyID, string accountNumber, DateTime from, DateTime to)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_Cancel", strategyID, accountNumber, from, to);
            return (recordsAffected > 0);
        }
    }
}
