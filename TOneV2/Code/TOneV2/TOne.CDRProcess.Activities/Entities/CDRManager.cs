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
        public void LoadCDRRange(DateTime from, DateTime to, int? batchSize, Action<List<TABS.CDR>> onBatchReady)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            dataManager.LoadCDRRange(from, to, batchSize, onBatchReady);
        }

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

        static object s_RepricingMainCDRIDLockObj = new object();
        static long _lastMainCDRId;
        public long ReserveRePricingMainCDRIDs(int nbOfRecords)
        {
            long id;
            lock (s_RepricingMainCDRIDLockObj)
            {
                if (_lastMainCDRId < 0)
                {
                    id = _lastMainCDRId - 1;
                }
                else
                {
                    ICDRTargetDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRTargetDataManager>();
                    id = dataManager.GetMinCDRMainID() - 1;
                }
                if (id > 0)
                    id = -1;
                _lastMainCDRId = id - nbOfRecords;
            }
            return id;
        }

        static object s_RepricingInvalidCDRIDLockObj = new object();
        static long _lastInvalidCDRId;
        public long ReserveRePricingInvalidCDRIDs(int nbOfRecords)
        {
            long id;
            lock (s_RepricingInvalidCDRIDLockObj)
            {
                if (_lastInvalidCDRId < 0)
                {
                    id = _lastInvalidCDRId - 1;
                }
                else
                {
                    ICDRTargetDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRTargetDataManager>();
                    id = dataManager.GetMinCDRInvalidID() - 1;
                }
                if (id > 0)
                    id = -1;
                _lastInvalidCDRId = id - nbOfRecords;
            }
            return id;
        }

        public void DeleteCDRMain(DateTime from, DateTime to)
        {
            //BulkManager.ExecuteActionWithTableLock(BulkManager.MAIN_TABLE_NAME,
            //    () =>
            //    {
            ICDRTargetDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRTargetDataManager>();
            dataManager.DeleteCDRMain(from, to);
            //});
        }

        public void DeleteCDRInvalid(DateTime from, DateTime to)
        {
            //BulkManager.ExecuteActionWithTableLock(BulkManager.INVALID_TABLE_NAME,
            //    () =>
            //    {
            ICDRTargetDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRTargetDataManager>();
            dataManager.DeleteCDRInvalid(from, to);
            //});
        }

        public void DeleteCDRSale(DateTime from, DateTime to)
        {
            //BulkManager.ExecuteActionWithTableLock(BulkManager.SALE_TABLE_NAME,
            //    () =>
            //    {
            ICDRTargetDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRTargetDataManager>();
            dataManager.DeleteCDRSale(from, to);
            //});
        }

        public void DeleteCDRCost(DateTime from, DateTime to)
        {
            //BulkManager.ExecuteActionWithTableLock(BulkManager.COST_TABLE_NAME,
            //    () =>
            //    {
            ICDRTargetDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRTargetDataManager>();
            dataManager.DeleteCDRCost(from, to);
            //});
        }

        public void DeleteTrafficStats(DateTime from, DateTime to)
        {
            //BulkManager.ExecuteActionWithTableLock(BulkManager.TRAFFICSTATS_TABLE_NAME,
            //    () =>
            //    {
            ICDRTargetDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRTargetDataManager>();
            dataManager.DeleteTrafficStats(from, to);
            //});
        }

        //public void DeleteDailyTrafficStats(DateTime from, DateTime to)
        //{
        //    CDRTargetDataManager dataManager = new CDRTargetDataManager();
        //    dataManager.DeleteDailyTrafficStats(from, to);
        //}
    }
}
