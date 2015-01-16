using System;

namespace TABS.Addons.Utilities
{
    public class RatePoolChecker
    {
        internal static log4net.ILog log = log4net.LogManager.GetLogger(typeof(RatePoolChecker));

        protected static DateTime LastRatePoolAffectorsChange
        {
            get
            {
                return SystemParameter.sys_RateChangeLastDate.DateTimeValue.Value;
            }
        }
        protected static DateTime LastRatePoolCacheClear
        {
            get
            {
                return SystemParameter.sys_RatePoolLastClear.DateTimeValue.Value;
            }
        }

        public static void ChangeLastRateCheck()
        {
            SystemParameter.sys_RateChangeLastDate.DateTimeValue = DateTime.Now;
            Exception ex;
            TABS.ObjectAssembler.SaveOrUpdate(SystemParameter.sys_RateChangeLastDate, out ex);
            if (ex != null)
                log.Error("Error while saving last rate pool affector done on system on a date", ex);
            ex = null;
        }
        public static void ChangeLastRoutePoolCachclear()
        {
            SystemParameter.sys_RatePoolLastClear.DateTimeValue = DateTime.Now;
            Exception ex;
            TABS.ObjectAssembler.SaveOrUpdate(SystemParameter.sys_RatePoolLastClear, out ex);
            if (ex != null)
                log.Error("Error while saving last rate pool cache clear date", ex);
            ex = null;
        }

        public static void CheckAndClear()
        {
            if (LastRatePoolCacheClear < LastRatePoolAffectorsChange)
            {
                log.Info("Clearing cache for Rate Pool due to Rate/Zone/Code changes");
                TABS.Components.RoutePool.ClearCachedCollections();
            }
        }

    }
}
