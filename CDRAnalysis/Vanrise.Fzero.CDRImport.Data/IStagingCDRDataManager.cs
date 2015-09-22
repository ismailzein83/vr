using System;
using System.Collections.Generic;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Data
{
    public interface IStagingCDRDataManager : IDataManager
    {
        void SaveStagingCDRsToDB(List<StagingCDR> cdrs);

        void LoadStagingCDR(DateTime from, DateTime to, int? batchSize, Action<StagingCDR> onBatchReady);
    }
}
