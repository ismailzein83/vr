using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.AccountBalance.Data.SQL
{
    public class BalanceHistoryDataManager : BaseSQLDataManager, IBalanceHistoryDataManager
    {
        #region ctor/Local Variables
        public BalanceHistoryDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
        }
        #endregion

        public bool InsertBalanceHistoryFromLiveBalance(long closingPeriodID, Guid accountTypeId)
        {
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_BalanceHistory_InsertFromLiveBalance]", closingPeriodID, accountTypeId) > 0);

        }
    }
}
