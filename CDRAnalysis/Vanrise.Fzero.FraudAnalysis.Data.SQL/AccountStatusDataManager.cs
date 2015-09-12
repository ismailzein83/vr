using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class AccountStatusDataManager : BaseSQLDataManager, IAccountStatusDataManager
    {

        public AccountStatusDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public void LoadAccountStatus(Action<AccountStatus> onBatchReady)
        {
            ExecuteReaderSP("sp_AccountStatus_Load", (reader) =>
            {
                while (reader.Read())
                {
                    AccountStatus accountStatus = new AccountStatus();
                    accountStatus.AccountInfo = Vanrise.Common.Serializer.Deserialize<AccountInfo>(GetReaderValue<string>(reader, "AccountInfo"));
                    accountStatus.AccountNumber = reader["AccountNumber"] as string;
                    accountStatus.Status = GetReaderValue<CaseStatus>(reader, "Status");
                    onBatchReady(accountStatus);
                }



            }
               );
        }

    }
}
