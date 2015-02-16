using System;
namespace TOne.CDR.Data
{
    public interface ICDRDataManager : IDataManager
    {
        void LoadCDRRange(DateTime from, DateTime to, int? batchSize, Action<System.Collections.Generic.List<TABS.CDR>> onBatchReady);

        Object PrepareMainCDRsForDBApply(System.Collections.Generic.List<TABS.Billing_CDR_Main> cdrs);

        void ApplyMainCDRsToDB(Object preparedMainCDRs);

        Object PrepareInvalidCDRsForDBApply(System.Collections.Generic.List<TABS.Billing_CDR_Invalid> cdrs);

        void ApplyInvalidCDRsToDB(Object preparedInvalidCDRs);

        Object PrepareCDRsForDBApply(System.Collections.Generic.List<TABS.CDR> cdrs,int SwitchID);

        void ApplyCDRsToDB(Object preparedCDRs);

    }
}
