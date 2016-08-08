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
    public class BalanceAlertRuleDataManager : BaseSQLDataManager, IBalanceAlertRuleDataManager
    {

        #region ctor/Local Variables
        public BalanceAlertRuleDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
        }
        #endregion

        public void GetLiveBalancesToAlert(Action<AccountBalanceForAlertRule> onAccountBalanceForAlertRuleReady)
        {
            ExecuteReaderSP("[VR_AccountBalance].[sp_LiveBalance_GetBalancesForAlert]",
               (reader) =>
               {
                   while (reader.Read())
                   {
                       onAccountBalanceForAlertRuleReady(AccountBalanceForAlertRuleMapper(reader));
                   }
               });
        }


        #region Mappers

        private AccountBalanceForAlertRule AccountBalanceForAlertRuleMapper(IDataReader reader)
        {
            return new AccountBalanceForAlertRule
            {
                AlertRuleId = GetReaderValue<int>(reader, "AlertRuleID"),
                AccountId = (long)reader["AccountId"],
                Threshold = GetReaderValue<Decimal>(reader, "Threshold"),
                ThresholdActionIndex = GetReaderValue<int>(reader, "ThresholdActionIndex"),
            };
        }
        #endregion

    }
}
