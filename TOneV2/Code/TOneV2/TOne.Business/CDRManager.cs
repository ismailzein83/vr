using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Data;
using TOne.Entities;

namespace TOne.Business
{
    public class CDRManager
    {
        public void LoadCDRRange(DateTime from, DateTime to, int? batchSize, Action<List<TABS.CDR>> onBatchReady)
        {
            ICDRDataManager dataManager = DataManagerFactory.GetDataManager<ICDRDataManager>();
            dataManager.LoadCDRRange(from, to, batchSize, onBatchReady);
        }

        static object s_RepricingMainCDRIDLockObj = new object();
        static long _lastMainCDRId;
        public long ReserveRePricingMainCDRIDs(int nbOfRecords)
        {
            long id;
            lock(s_RepricingMainCDRIDLockObj)
            {
                if (_lastMainCDRId < 0)
                {
                    id = _lastMainCDRId - 1;
                }
                else
                {
                    ICDRTargetDataManager dataManager = DataManagerFactory.GetDataManager<ICDRTargetDataManager>();
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
                    ICDRTargetDataManager dataManager = DataManagerFactory.GetDataManager<ICDRTargetDataManager>();
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
            ICDRTargetDataManager dataManager = DataManagerFactory.GetDataManager<ICDRTargetDataManager>();
                    dataManager.DeleteCDRMain(from, to);
                //});
        }

        public void DeleteCDRInvalid(DateTime from, DateTime to)
        {
            //BulkManager.ExecuteActionWithTableLock(BulkManager.INVALID_TABLE_NAME,
            //    () =>
            //    {
            ICDRTargetDataManager dataManager = DataManagerFactory.GetDataManager<ICDRTargetDataManager>();
                    dataManager.DeleteCDRInvalid(from, to);
                //});
        }

        public void DeleteCDRSale(DateTime from, DateTime to)
        {
            //BulkManager.ExecuteActionWithTableLock(BulkManager.SALE_TABLE_NAME,
            //    () =>
            //    {
            ICDRTargetDataManager dataManager = DataManagerFactory.GetDataManager<ICDRTargetDataManager>();
                    dataManager.DeleteCDRSale(from, to);
                //});
        }

        public void DeleteCDRCost(DateTime from, DateTime to)
        {
            //BulkManager.ExecuteActionWithTableLock(BulkManager.COST_TABLE_NAME,
            //    () =>
            //    {
            ICDRTargetDataManager dataManager = DataManagerFactory.GetDataManager<ICDRTargetDataManager>();
                    dataManager.DeleteCDRCost(from, to);
                //});
        }

        public void DeleteTrafficStats(DateTime from, DateTime to)
        {
            //BulkManager.ExecuteActionWithTableLock(BulkManager.TRAFFICSTATS_TABLE_NAME,
            //    () =>
            //    {
            ICDRTargetDataManager dataManager = DataManagerFactory.GetDataManager<ICDRTargetDataManager>();
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
