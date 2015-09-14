using System;
namespace TOne.CDR.Data
{
    public interface ICDRTargetDataManager : IDataManager
    {
     
        void DeleteDailyTrafficStats(DateTime date);
        //long GetMinCDRInvalidID();
        //long GetMinCDRMainID();
        void UpdateDailyPostpaid(DateTime date);
        void UpdateDailyPrepaid(DateTime date);
        void UpdateDailyBillingStatistics(DateTime date);
    }
}
