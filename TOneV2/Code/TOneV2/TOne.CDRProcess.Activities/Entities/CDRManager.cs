using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TABS;
using TOne.Business;
using TOne.CDR.Data;

namespace TOne.CDRProcess.Activities
{
    public class CDRManager
    {
        log4net.ILog log = log4net.LogManager.GetLogger("TOne.CDRProcess.Activities.CDRManager");
     
        #region CDR Import Process

       

        public CDRBatch GetCDRs(int SwitchID)
        {
            List<TABS.CDR> ToneCdrs = new List<TABS.CDR>();
            CDRBatch BatchCdrs = new CDRBatch();

            TABS.Switch CurrentSwitch = null;
            if (TABS.Switch.All.ContainsKey(SwitchID))
                CurrentSwitch = TABS.Switch.All[SwitchID];


            if (CurrentSwitch != null)
            {
                var rawCDRs = CurrentSwitch.SwitchManager.GetCDR(CurrentSwitch);

                // create CDRs from Standard CDRs
                foreach (TABS.Addons.Utilities.Extensibility.CDR rawCDR in rawCDRs)
                    ToneCdrs.Add(new TABS.CDR(CurrentSwitch, rawCDR));
            }
            BatchCdrs.CDRs = ToneCdrs;
            return BatchCdrs;
        }

       


        #endregion

        //static object s_RepricingMainCDRIDLockObj = new object();
        //static long _lastMainCDRId;
        //public long ReserveRePricingMainCDRIDs(int nbOfRecords)
        //{
        //    long id;
        //    lock (s_RepricingMainCDRIDLockObj)
        //    {
        //        if (_lastMainCDRId < 0)
        //        {
        //            id = _lastMainCDRId - 1;
        //        }
        //        else
        //        {
        //            ICDRTargetDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRTargetDataManager>();
        //            id = dataManager.GetMinCDRMainID() - 1;
        //        }
        //        if (id > 0)
        //            id = -1;
        //        _lastMainCDRId = id - nbOfRecords;
        //    }
        //    return id;
        //}

        //static object s_RepricingInvalidCDRIDLockObj = new object();
        //static long _lastInvalidCDRId;
        //public long ReserveRePricingInvalidCDRIDs(int nbOfRecords)
        //{
        //    long id;
        //    lock (s_RepricingInvalidCDRIDLockObj)
        //    {
        //        if (_lastInvalidCDRId < 0)
        //        {
        //            id = _lastInvalidCDRId - 1;
        //        }
        //        else
        //        {
        //            ICDRTargetDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRTargetDataManager>();
        //            id = dataManager.GetMinCDRInvalidID() - 1;
        //        }
        //        if (id > 0)
        //            id = -1;
        //        _lastInvalidCDRId = id - nbOfRecords;
        //    }
        //    return id;
        //}



        //public void DeleteDailyTrafficStats(DateTime from, DateTime to)
        //{
        //    CDRTargetDataManager dataManager = new CDRTargetDataManager();
        //    dataManager.DeleteDailyTrafficStats(from, to);
        //}
    }
}
