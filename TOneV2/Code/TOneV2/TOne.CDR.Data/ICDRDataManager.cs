using System;
namespace TOne.CDR.Data
{
    public interface ICDRDataManager : IDataManager
    {
        void LoadCDRRange(DateTime from, DateTime to, int? batchSize, Action<System.Collections.Generic.List<TABS.CDR>> onBatchReady);

        Object PrepareCDRsForDBApply(System.Collections.Generic.List<TABS.CDR> cdrs,int SwitchID);

        void ApplyCDRsToDB(Object preparedCDRs);


    }
}
