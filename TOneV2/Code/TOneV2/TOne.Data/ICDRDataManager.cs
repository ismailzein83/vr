using System;
namespace TOne.Data
{
    public interface ICDRDataManager
    {
        void LoadCDRRange(DateTime from, DateTime to, int? batchSize, Action<System.Collections.Generic.List<TABS.CDR>> onBatchReady);
    }
}
