using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class AccountInfoDataManager : BaseSQLDataManager, IAccountInfoDataManager
    {

        #region ctor
        public AccountInfoDataManager()
            : base("CDRDBConnectionString")
        {

        }
        #endregion

        #region Public Methods
        public void LoadAccountInfo(IEnumerable<CaseStatus> caseStatuses, Action<AccountInfo> onBatchReady)
        {

            ExecuteReaderSP("[FraudAnalysis].[sp_AccountInfo_GetByAccountStatuses]", (reader) =>
            {
                while (reader.Read())
                {
                    onBatchReady(AccountInfoMapper(reader));
                }
            }, caseStatuses != null ? String.Join(",", caseStatuses.Select(itm => (int)itm)) : null
            );
        }

        public bool InsertOrUpdateAccountInfo(string accountNumber, InfoDetail infoDetail)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountInfo_InsertOrUpdate", accountNumber, Vanrise.Common.Serializer.Serialize(infoDetail));
            return (recordsAffected > 0);
        }

        public void SavetoDB(List<AccountInfo> records)
        {
            string[] s_Columns = new string[] {
            "AccountNumber"
            ,"InfoDetail"
        };


            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (AccountInfo record in records)
            {
                stream.WriteRecord("{0}^{1}",
                                record.AccountNumber,
                                Vanrise.Common.Serializer.Serialize(record.InfoDetail)
                                );
            }

            stream.Close();

            InsertBulkToTable(
                new StreamBulkInsertInfo
                {
                    TableName = "[FraudAnalysis].[AccountInfo]",
                    Stream = stream,
                    TabLock = true,
                    ColumnNames = s_Columns,
                    KeepIdentity = false,
                    FieldSeparator = '^'
                });
        }

        public bool UpdateAccountInfoBatch(List<AccountInfo> accountInfos)
        {
            DataTable dtAccountInfosToUpdate = GetAccountInfoTable();
            DataRow dr;

            foreach (var item in accountInfos)
            {
                dr = dtAccountInfosToUpdate.NewRow();
                dr["AccountNumber"] = item.AccountNumber;
                dr["InfoDetail"] = item.InfoDetail;
                dtAccountInfosToUpdate.Rows.Add(dr);
            }

            int recordsAffected = 0;
            if (dtAccountInfosToUpdate.Rows.Count > 0)
            {
                recordsAffected = ExecuteNonQuerySPCmd("[FraudAnalysis].[sp_AccountInfo_BulkUpdate]",
                       (cmd) =>
                       {
                           var dtPrm = new System.Data.SqlClient.SqlParameter("@AccountInfo", SqlDbType.Structured);
                           dtPrm.Value = dtAccountInfosToUpdate;
                           cmd.Parameters.Add(dtPrm);
                       });
            }

            return (recordsAffected > 0);
        }

        #endregion

        #region Private Methods

        private DataTable GetAccountInfoTable()
        {
            DataTable dt = new DataTable("FraudAnalysis.AccountInfo");
            dt.Columns.Add("AccountNumber", typeof(string));
            dt.Columns.Add("InfoDetail", typeof(InfoDetail));
            return dt;
        }

        internal AccountInfo AccountInfoMapper(IDataReader reader)
        {
            var accountInfo = new AccountInfo();
            accountInfo.AccountNumber = reader["AccountNumber"] as string;
            string infoDetail = GetReaderValue<string>(reader, "InfoDetail");
            if (infoDetail == null)
                accountInfo.InfoDetail = new InfoDetail() { IMEIs = new HashSet<string>() };
            else
                accountInfo.InfoDetail = Vanrise.Common.Serializer.Deserialize<InfoDetail>(infoDetail);
            return accountInfo;
        }
        # endregion

    }
}
