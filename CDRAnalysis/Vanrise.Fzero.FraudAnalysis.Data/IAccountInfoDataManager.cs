using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IAccountInfoDataManager : IDataManager 
    {
        void LoadAccountInfo(Action<AccountInfo> onBatchReady);
    }
}
