﻿using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IAccountInfoDataManager : IDataManager 
    {
        void LoadAccountInfo(IEnumerable<CaseStatus> caseStatuses, Action<AccountInfo> onBatchReady);
        bool InsertOrUpdateAccountInfo(string accountNumber, InfoDetail infoDetail);
    }
}
