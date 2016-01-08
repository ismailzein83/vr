using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Data;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class DWAccountCaseManager : BaseSQLDataManager, IDWAccountCaseManager
    {
        public List<DWAccountCase> GetDWAccountCases(DateTime from, DateTime to)
        {
            return GetItemsSP("[FraudAnalysis].[sp_DataWarehouse_GetAccountCases]", DWAccountCaseMapper, from, to);
        }


        #region Mappers
        private DWAccountCase DWAccountCaseMapper(IDataReader reader)
        {
            DWAccountCase dwAccountCase = new DWAccountCase();
            dwAccountCase.AccountNumber = (int)reader["AccountNumber"];
            dwAccountCase.CaseGenerationTime = (DateTime)reader["CaseGenerationTime"];
            dwAccountCase.CaseID = (int)reader["CaseID"];
            dwAccountCase.CaseStatus = reader["CaseStatus"] as string;
            dwAccountCase.CaseUser = GetReaderValue<int?>(reader, "CaseUser");
            dwAccountCase.IsDefault = (bool)reader["IsDefault"];
            dwAccountCase.NetType = (int)reader["NetType"];
            dwAccountCase.PeriodID = (int)reader["PeriodID"];
            dwAccountCase.StrategyName = reader["StrategyName"] as string;
            dwAccountCase.StrategyUser = (int)reader["StrategyUser"];
            dwAccountCase.SuspicionLevelID = (int)reader["SuspicionLevelID"];
            return dwAccountCase;
        }
        #endregion


    }
}
