using System;
using System.Collections.Generic;
using System.Data;
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

        static string[] s_Columns = new string[] {
            "ID"
            ,"StrategyExecutionID"  
            ,"AccountNumber"
            ,"SuspicionLevelID"
            ,"FilterValues"
            ,"AggregateValues" 
            ,"CaseID" 
            ,"SuspicionOccuranceStatus"  
            ,"IMEIs"  
        };
        #endregion

        #region  Public Methods
        public bool LinkItemToCase(string accountNumber, int accountCaseId, CaseStatus caseStatus)
        {
            SuspicionOccuranceStatus occuranceStatus = (caseStatus.CompareTo(CaseStatus.Open) == 0 || caseStatus.CompareTo(CaseStatus.Pending) == 0) ? SuspicionOccuranceStatus.Open : SuspicionOccuranceStatus.Closed;

            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionItem_UpdateStatus", accountNumber, accountCaseId, occuranceStatus);
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

        public void GetStrategyExecutionItemswithCasesbyExecutionId(long ExecutionId, out List<StrategyExecutionItem> outAllStrategyExecutionItems, out List<AccountCase> outAccountCases, out List<StrategyExecutionItem> outStrategyExecutionItemRelatedtoCases)
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

        DataTable GetStrategyExecutionItemTable()
        {
            DataTable dt = new DataTable("FraudAnalysis.StrategyExecutionItem");
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("SuspicionOccuranceStatus", typeof(int));
            dt.Columns.Add("CaseId", typeof(int));
            dt.Columns.Add("AccountNumber", typeof(string));
            return dt;
        }

        public bool CancelStrategyExecutionItemBatch(List<long> itemIds, int userId, SuspicionOccuranceStatus status)
        {
            DataTable dtStrategyExecutionItemsToUpdate = GetStrategyExecutionItemTable();
            DataRow dr;

            foreach (var item in itemIds)
            {
                dr = dtStrategyExecutionItemsToUpdate.NewRow();
                dr["ID"] = item;
                dr["SuspicionOccuranceStatus"] = (int)status;
                dr["AccountNumber"] = string.Empty;
                dr["CaseId"] = 0;
                dtStrategyExecutionItemsToUpdate.Rows.Add(dr);
            }

            int recordsAffected = 0;
            if (dtStrategyExecutionItemsToUpdate.Rows.Count > 0)
            {
                recordsAffected = ExecuteNonQuerySPCmd("[FraudAnalysis].[sp_StrategyExecutionItem_BulkCancel]",
                       (cmd) =>
                       {
                           var dtPrm = new System.Data.SqlClient.SqlParameter("@StrategyExecutionItem", SqlDbType.Structured);
                           dtPrm.Value = dtStrategyExecutionItemsToUpdate;
                           cmd.Parameters.Add(dtPrm);
                       });
            }

            return (recordsAffected > 0);


        }

        public bool UpdateStrategyExecutionItemBatch(Dictionary<string, int> accountNumberCaseIds)
        {
            DataTable dtStrategyExecutionItemsToUpdate = GetStrategyExecutionItemTable();
            DataRow dr;

            foreach (var item in accountNumberCaseIds)
            {
                dr = dtStrategyExecutionItemsToUpdate.NewRow();
                dr["AccountNumber"] = item.Key;
                dr["CaseId"] = item.Value;
                dr["ID"] = 0;
                dr["SuspicionOccuranceStatus"] = 0;
                dtStrategyExecutionItemsToUpdate.Rows.Add(dr);
            }

            int recordsAffected = 0;
            if (dtStrategyExecutionItemsToUpdate.Rows.Count > 0)
            {
                recordsAffected = ExecuteNonQuerySPCmd("[FraudAnalysis].[sp_StrategyExecutionItem_BulkUpdate]",
                       (cmd) =>
                       {
                           var dtPrm = new System.Data.SqlClient.SqlParameter("@StrategyExecutionItem", SqlDbType.Structured);
                           dtPrm.Value = dtStrategyExecutionItemsToUpdate;
                           cmd.Parameters.Add(dtPrm);
                       });
            }

            return (recordsAffected > 0);


        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[FraudAnalysis].[StrategyExecutionItem]",
                ColumnNames = s_Columns,
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^'
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(StrategyExecutionItem record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("0^{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
                                 record.StrategyExecutionID,
                                 record.AccountNumber,
                                 record.SuspicionLevelID,
                                 Vanrise.Common.Serializer.Serialize(record.FilterValues, true),
                                 Vanrise.Common.Serializer.Serialize(record.AggregateValues, true),
                                 null,
                                 (int)record.SuspicionOccuranceStatus,
                                 string.Join<string>(",", record.IMEIs)
                                 );
        }

        public void ApplyStrategyExecutionItemsToDB(object preparedStrategyExecutionItem)
        {
            InsertBulkToTable(preparedStrategyExecutionItem as BaseBulkInsertInfo);
        }

        public void GetStrategyExecutionItemsbyNULLCaseId(out List<StrategyExecutionItemAccountInfo> outItems, out List<AccountCase> outCases, out List<AccountInfo> outInfos)
        {
            AccountCaseDataManager accountCaseDataManager = new AccountCaseDataManager();
            StrategyExecutionItemAccountInfoDataManager strategyExecutionItemAccountInfoDataManager = new StrategyExecutionItemAccountInfoDataManager();
            AccountInfoDataManager accountInfoDataManager = new AccountInfoDataManager();
            List<StrategyExecutionItemAccountInfo> itemsInfo = new List<StrategyExecutionItemAccountInfo>();
            List<AccountCase> cases = new List<AccountCase>();
            List<AccountInfo> infos = new List<AccountInfo>();

            ExecuteReaderSP("FraudAnalysis.sp_StrategyExecutionItem_GetItemsAndCasesAndInfoByNULLCaseID", (reader) =>
            {
                while (reader.Read())
                {
                    itemsInfo.Add(strategyExecutionItemAccountInfoDataManager.StrategyExecutionItemAccountInfoMapper(reader));
                }
                reader.NextResult();



                while (reader.Read())
                {
                    cases.Add(accountCaseDataManager.AccountCaseMapper(reader));
                }
                reader.NextResult();



                while (reader.Read())
                {
                    infos.Add(accountInfoDataManager.AccountInfoMapper(reader));
                }

            });

            outItems = itemsInfo;
            outCases = cases;
            outInfos = infos;
        }

        #endregion

        #region  Mappers


        private StrategyExecutionItem StrategyExecutionItemMapper(IDataReader reader)
        {
            var item = new StrategyExecutionItem();
            item.ID = (long)reader["ID"];
            item.AggregateValues = Vanrise.Common.Serializer.Deserialize<Dictionary<string, decimal>>(GetReaderValue<string>(reader, "AggregateValues"));
            item.FilterValues = Vanrise.Common.Serializer.Deserialize<Dictionary<int, decimal?>>(GetReaderValue<string>(reader, "FilterValues"));
            string imies = GetReaderValue<string>(reader, "IMEIs");
            if (imies != null)
                item.IMEIs = new HashSet<string>((imies).Split(','));
            item.StrategyExecutionID = Convert.ToInt64(reader["StrategyExecutionID"]);
            item.AccountNumber = reader["AccountNumber"] as string;
            item.SuspicionLevelID = (int)reader["SuspicionLevelID"];
            item.CaseID = GetReaderValue<int?>(reader, "CaseID");
            item.SuspicionOccuranceStatus = (SuspicionOccuranceStatus)reader["SuspicionOccuranceStatus"];
            return item;
        }


        private AccountSuspicionDetail AccountSuspicionDetailMapper(IDataReader reader)
        {
            var detail = new AccountSuspicionDetail(); // a detail is a fraud result instance
            detail.StrategyExecutionId = Convert.ToInt64(reader["StrategyExecutionID"]);
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
