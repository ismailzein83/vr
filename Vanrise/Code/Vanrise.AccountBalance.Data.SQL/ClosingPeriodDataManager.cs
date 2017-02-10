using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.AccountBalance.Data.SQL
{
    public class ClosingPeriodDataManager : BaseSQLDataManager, IClosingPeriodDataManager
    {

        #region ctor/Local Variables
        public ClosingPeriodDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
        }
        #endregion

        #region Public Methods
        public BalanceClosingPeriod GetLastClosingPeriod(Guid accountTypeId)
        {
            return GetItemSP("[VR_AccountBalance].[sp_BalanceClosingPeriod_GetLastClosingPeriod]", BalanceClosingPeriodMapper, accountTypeId);
        }
        #endregion

        #region Private Methods
        private bool Insert(DateTime balanceClosingPeriod, Guid accountTypeId, out long insertedId)
        {
            object closingPeriodID;
            int recordesEffected = ExecuteNonQuerySP("[VR_AccountBalance].[sp_BalanceClosingPeriod_Insert]", out closingPeriodID, balanceClosingPeriod, accountTypeId);
            insertedId = (recordesEffected > 0) ? (long)closingPeriodID : -1;
            return (recordesEffected > 0);
        }

        #endregion

        #region Mappers
        private BalanceClosingPeriod BalanceClosingPeriodMapper(IDataReader reader)
        {
            return new BalanceClosingPeriod
            {
                BalanceClosingPeriodId = (long)reader["ID"],
                ClosingTime = GetReaderValue<DateTime>(reader, "ClosingTime"),
                AccountTypeId = GetReaderValue<Guid>(reader, "AccountTypeID")
            };
        }
        #endregion

    }
}
