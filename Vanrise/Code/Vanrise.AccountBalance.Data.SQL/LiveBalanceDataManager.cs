using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.AccountBalance.Data.SQL
{
    public class LiveBalanceDataManager : BaseSQLDataManager, ILiveBalanceDataManager
    {
         
        #region ctor/Local Variables
        public LiveBalanceDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
        }
        #endregion

        #region Public Methods
        public LiveBalance GetLiveBalance(long accountId)
        {
            return GetItemSP("[VR_AccountBalance].[sp_LiveBalance_GetById]", LiveBalanceMapper, accountId);
        }
        #endregion

        #region Mappers

        private LiveBalance LiveBalanceMapper(IDataReader reader)
        {
            return new LiveBalance
            {
                CurrentBalance = GetReaderValue<Decimal>(reader, "CurrentBalance"),
                AccountId = (long)reader["AccountId"],
                UsageBalance = GetReaderValue<Decimal>(reader, "UsageBalance"),
                CurrencyId = GetReaderValue<int>(reader, "CurrencyID"),
                AlertRuleID = GetReaderValue<long>(reader, "AlertRuleID"),
                CurrentAlertThreshold = GetReaderValue<Decimal>(reader, "CurrentAlertThreshold"),
                InitialBalance = GetReaderValue<Decimal>(reader, "InitialBalance"),
                NextAlertThreshold = GetReaderValue<Decimal>(reader, "NextAlertThreshold"),
            };
        }

        #endregion
    }
}
