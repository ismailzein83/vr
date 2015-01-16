﻿using System;
namespace TOne.Data
{
    public interface ICDRTargetDataManager
    {
        void DeleteCDRCost(DateTime from, DateTime to);
        void DeleteCDRInvalid(DateTime from, DateTime to);
        void DeleteCDRMain(DateTime from, DateTime to);
        void DeleteCDRSale(DateTime from, DateTime to);
        void DeleteTrafficStats(DateTime from, DateTime to);
        void DeleteDailyTrafficStats(DateTime date);
        long GetMinCDRInvalidID();
        long GetMinCDRMainID();
        void UpdateDailyPostpaid(DateTime date);
        void UpdateDailyPrepaid(DateTime date);
        void UpdateDailyBillingStatistics(DateTime date);
    }
}
