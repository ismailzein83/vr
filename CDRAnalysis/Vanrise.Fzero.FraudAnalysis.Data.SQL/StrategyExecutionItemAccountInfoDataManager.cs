using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class StrategyExecutionItemAccountInfoDataManager : BaseSQLDataManager, IStrategyExecutionItemAccountInfoDataManager
    {

        #region ctor
        public StrategyExecutionItemAccountInfoDataManager()
            : base("CDRDBConnectionString")
        {

        }
        #endregion

        #region Private Methods

        internal StrategyExecutionItemAccountInfo StrategyExecutionItemAccountInfoMapper(IDataReader reader)
        {
            var strategyExecutionItemAccountInfo = new StrategyExecutionItemAccountInfo();
            strategyExecutionItemAccountInfo.AccountNumber = reader["AccountNumber"] as string;
            string imeis = reader["IMEIs"] as string;
            if (imeis != null && imeis != string.Empty)
                strategyExecutionItemAccountInfo.IMEIs = new HashSet<string>(imeis.Split(','));

            return strategyExecutionItemAccountInfo;
        }
        # endregion

    }
}
