using System;
namespace TOne.CDR.Data
{
    public interface ICDRDataManager : IDataManager
    {
        void LoadCDRRange(DateTime from, DateTime to, int? batchSize, Action<System.Collections.Generic.List<TABS.CDR>> onBatchReady);

        void ApplyMainCDRsToDB(Object preparedMainCDRs);

        Object PrepareInvalidCDRsForDBApply(System.Collections.Generic.List<TOne.CDR.Entities.BillingCDRInvalid> cdrs);

        void ApplyInvalidCDRsToDB(Object preparedInvalidCDRs);

        void ApplyTrafficStatsToDB(Object preparedTrafficStats);

        Object PrepareCDRsForDBApply(System.Collections.Generic.List<TABS.CDR> cdrs,int SwitchID);

        void ApplyCDRsToDB(Object preparedCDRs);

        Object PrepareMainCDRsForDBApply(System.Collections.Generic.List<TOne.CDR.Entities.BillingCDRMain> cdrs);

        Object PrepareTrafficStatsForDBApply(System.Collections.Generic.List<TOne.CDR.Entities.TrafficStatistic> trafficStatistics);

    }
}
