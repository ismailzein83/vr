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


        //public bool SaveAccountCase(AccountCase accountCaseObject)
        //{
        //    //int recordesEffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_Insert",  accountCaseObject.AccountNumber, accountCaseObject.StatusID, accountCaseObject.ValidTill , accountCaseObject.UserId, accountCaseObject.StrategyId, accountCaseObject.SuspicionLevelID  );
        //    //if (recordesEffected > 0)
        //    //    return true;
        //    //return false;
        //}

        //public BigResult<AccountCase> GetFilteredAccountCases(Vanrise.Entities.DataRetrievalInput<AccountCaseResultQuery> input)
        //{
        //    //Action<string> createTempTableAction = (tempTableName) =>
        //    //{
        //    //    ExecuteNonQuerySP("FraudAnalysis.sp_FraudResult_CreateTempForFilteredAccountCases", tempTableName,  input.Query.AccountNumber);
        //    //};


        //    //return RetrieveData(input, createTempTableAction, AccountCaseMapper);
        //}

        #region Private Methods

        //private AccountCase AccountCaseMapper(IDataReader reader)
        //{
        //    AccountCase accountCase = new AccountCase();
        //    accountCase.AccountNumber = reader["AccountNumber"] as string;
        //    accountCase.LogDate = (DateTime) reader["LogDate"] ;
        //    accountCase.StatusID = (int)reader["StatusId"];
        //    accountCase.StatusName = reader["StatusName"] as string;
        //    accountCase.StrategyId =  GetReaderValue<int?>(reader,"StrategyId");
        //    accountCase.SuspicionLevelID = GetReaderValue<int?>(reader, "SuspicionLevelID");
        //    accountCase.StrategyName = reader["StrategyName"] as string;
        //    accountCase.SuspicionLevelName = reader["SuspicionLevelName"] as string;
        //    accountCase.UserId = GetReaderValue<int?>(reader, "UserId");
        //    accountCase.ValidTill = GetReaderValue<DateTime?>(reader, "ValidTill");

        //    return accountCase;
        //}

        #endregion



        public bool SaveAccountCase(AccountCase accountCaseObject)
        {
            throw new NotImplementedException();
        }

        public BigResult<AccountCase> GetFilteredAccountCases(DataRetrievalInput<AccountCaseResultQuery> input)
        {
            throw new NotImplementedException();
        }
    }
}
