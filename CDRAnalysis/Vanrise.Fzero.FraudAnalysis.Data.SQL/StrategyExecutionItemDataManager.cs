using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class StrategyExecutionItemDataManager : BaseSQLDataManager, IStrategyExecutionItemDataManager
    {

        #region ctor
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        public StrategyExecutionItemDataManager()
            : base("CDRDBConnectionString")
        {

        }
        static StrategyExecutionItemDataManager()
        {
            _columnMapper.Add("SuspicionLevelDescription", "SuspicionLevelID");
            _columnMapper.Add("UserName", "UserID");
            _columnMapper.Add("SuspicionOccuranceStatusDescription", "SuspicionOccuranceStatus");
            _columnMapper.Add("AccountCaseStatusDescription", "AccountCaseStatusID");
        }
        #endregion

        #region  Public Methods
        public bool LinkDetailToCase(string accountNumber, int caseID, CaseStatus caseStatus)
        {
            SuspicionOccuranceStatus occuranceStatus = (caseStatus.CompareTo(CaseStatus.Open) == 0 || caseStatus.CompareTo(CaseStatus.Pending) == 0) ? SuspicionOccuranceStatus.Open : SuspicionOccuranceStatus.Closed;

            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionItem_UpdateStatus", accountNumber, caseID, occuranceStatus);
            return (recordsAffected > 0);
        }
        public BigResult<AccountSuspicionDetail> GetFilteredDetailsByCaseID(Vanrise.Entities.DataRetrievalInput<CaseDetailQuery> input)
        {


            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionItem_CreateTempByCaseID", tempTableName, input.Query.CaseID);
            };

            return RetrieveData(input, createTempTableAction, AccountSuspicionDetailMapper, _columnMapper);
        }


        public void GetStrategyExecutionbyExecutionId(long ExecutionId, out List<StrategyExecutionItem> outAllStrategyExecutionItems, out List<AccountCase> outAccountCases, out List<StrategyExecutionItem> outStrategyExecutionItemRelatedtoCases)
        {
            AccountCaseDataManager accountCaseDataManager = new AccountCaseDataManager();
            List<StrategyExecutionItem> allStrategyExecutionItems = new List<StrategyExecutionItem>();
            List<AccountCase> accountCases = new List<AccountCase>();
            List<StrategyExecutionItem> strategyExecutionItemRelatedtoCases = new List<StrategyExecutionItem>();

            ExecuteReaderSP("FraudAnalysis.sp_StrategyExecutionItem_GetItemsAndCasesByExecutionID", (reader) =>
            {
                while (reader.Read())
                {
                    allStrategyExecutionItems.Add(StrategyExecutionItemMapper(reader));
                }
                reader.NextResult();



                while (reader.Read())
                {
                    accountCases.Add(accountCaseDataManager.AccountCaseMapper(reader));
                }
                reader.NextResult();



                while (reader.Read())
                {
                    strategyExecutionItemRelatedtoCases.Add(StrategyExecutionItemMapper(reader));
                }

            }, ExecutionId);

            outAllStrategyExecutionItems = allStrategyExecutionItems;
            outAccountCases = accountCases;
            outStrategyExecutionItemRelatedtoCases = strategyExecutionItemRelatedtoCases;
        }


        #endregion

        #region  Mappers


        private StrategyExecutionItem StrategyExecutionItemMapper(IDataReader reader)
        {
            var item = new StrategyExecutionItem();
            item.ID = (long)reader["ID"];
            item.AggregateValues = Vanrise.Common.Serializer.Deserialize<Dictionary<string, decimal>>(GetReaderValue<string>(reader, "AggregateValues"));
            item.FilterValues = Vanrise.Common.Serializer.Deserialize<Dictionary<int, decimal>>(GetReaderValue<string>(reader, "FilterValues"));
            item.IMEIs = new HashSet<string>((GetReaderValue<string>(reader, "IMEIs")).Split(','));
            item.StrategyExecutionID = (int)reader["StrategyExecutionID"];
            item.AccountNumber = reader["AccountNumber"] as string;
            item.SuspicionLevelID = (int)reader["SuspicionLevelID"];
            item.CaseID = GetReaderValue<int?>(reader, "CaseID");
            item.SuspicionOccuranceStatus = (SuspicionOccuranceStatus)reader["SuspicionOccuranceStatus"];
            return item;
        }


        private AccountSuspicionDetail AccountSuspicionDetailMapper(IDataReader reader)
        {
            var detail = new AccountSuspicionDetail(); // a detail is a fraud result instance

            detail.DetailID = (long)reader["DetailID"];
            detail.SuspicionLevelID = (SuspicionLevel)reader["SuspicionLevelID"];
            detail.StrategyName = reader["StrategyName"] as string;
            detail.SuspicionOccuranceStatus = (SuspicionOccuranceStatus)reader["SuspicionOccuranceStatus"];
            detail.FromDate = (DateTime)reader["FromDate"];
            detail.ToDate = (DateTime)reader["ToDate"];
            detail.ExecutionDate = (DateTime)reader["ExecutionDate"];
            detail.AggregateValues = Vanrise.Common.Serializer.Deserialize<Dictionary<string, decimal>>(GetReaderValue<string>(reader, "AggregateValues"));

            return detail;
        }
        #endregion

    }
}
