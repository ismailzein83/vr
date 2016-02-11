using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AccountInfoManager
    {
        public void LoadAccountInfo(IEnumerable<CaseStatusEnum> caseStatuses, Action<AccountInfo> onBatchReady)
        {
            IAccountInfoDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountInfoDataManager>();
             dataManager.LoadAccountInfo(caseStatuses, onBatchReady);
        }
        public bool InsertOrUpdateAccountInfo(string accountNumber, InfoDetail infoDetail)
        {
            IAccountInfoDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountInfoDataManager>();
            return dataManager.InsertOrUpdateAccountInfo(accountNumber, infoDetail);
        }
    }
}
