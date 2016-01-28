using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Linq;

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
                    AccountInfo accountInfo = new AccountInfo();

                    string infoDetail = GetReaderValue<string>(reader, "InfoDetail");
                    if (infoDetail == null)
                        accountInfo.InfoDetail = new InfoDetail() { IMEIs = new HashSet<string>() };
                    else
                        accountInfo.InfoDetail = Vanrise.Common.Serializer.Deserialize<InfoDetail>(infoDetail);

                    accountInfo.AccountNumber = reader["AccountNumber"] as string;
                    onBatchReady(accountInfo);
                }
            }, caseStatuses != null ? String.Join(",", caseStatuses.Select(itm => (int)itm)) : null
            );
        }
        public bool InsertOrUpdateAccountInfo(string accountNumber, InfoDetail infoDetail)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountInfo_InsertOrUpdate", accountNumber, Vanrise.Common.Serializer.Serialize(infoDetail));
            return (recordsAffected > 0);
        }
      
        #endregion

    }
}
