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

        public void SaveCDRstoDB(CDRBatch cdrs)
        {

            using (TABS.Components.BulkManager BulkManager = new TABS.Components.BulkManager(log))
            {
                BulkManager.Write(cdrs.CDRs);
            }
        }


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

        public CDRBase GenerateBillingCdrs(CDRBatch cdrBatch, ProtCodeMap codeMap)
        {
            CDRBase CdrBillingGenerated = new CDRBase();
            List<Billing_CDR_Base> billingCDRs = new List<Billing_CDR_Base>();

            foreach (TABS.CDR cdr in cdrBatch.CDRs)
            {
                Billing_CDR_Base billingCDR = GenerateBillingCdr(codeMap, cdr);
                billingCDRs.Add(billingCDR);
            }
            CdrBillingGenerated.CDRs = billingCDRs;
            return CdrBillingGenerated;
        }



        static System.Text.RegularExpressions.Regex InvalidCGPNDigits = new System.Text.RegularExpressions.Regex("[^0-9]", System.Text.RegularExpressions.RegexOptions.Compiled);
        public Billing_CDR_Base GenerateBillingCdr(TOne.Business.ProtCodeMap codeMap, TABS.CDR cdr)
        {
            Billing_CDR_Base billingCDR = null;
            try
            {
                if (cdr.DurationInSeconds > 0)
                {
                    billingCDR = new Billing_CDR_Main();
                }
                else
                    billingCDR = new Billing_CDR_Invalid();
                //log.Info("Switch:" + cdr.Switch.SwitchID + "(" + cdr.Switch.Name + ")");

                bool valid = cdr.Switch.SwitchManager.FillCDRInfo(cdr.Switch, cdr, billingCDR);

                //billingCDR.CDPNOut = cdr.CDPNOut;

                GenerateZones(codeMap, billingCDR);

                // If there is a duration and missing supplier (zone) or Customer (zone) info
                // then it is considered invalid
                if (billingCDR is Billing_CDR_Main)
                    if (!valid
                        || billingCDR.Customer.RepresentsASwitch
                        || billingCDR.Supplier.RepresentsASwitch
                        || billingCDR.CustomerID == null
                        || billingCDR.SupplierID == null
                        || billingCDR.OurZone == null
                        || billingCDR.SupplierZone == null
                        || billingCDR.Customer.ActivationStatus == ActivationStatus.Inactive
                        || billingCDR.Supplier.ActivationStatus == ActivationStatus.Inactive)
                    {
                        billingCDR = new Billing_CDR_Invalid(billingCDR);
                    }
                return billingCDR;
            }
            catch (Exception EX)
            {
                log.Error("Error Generating Billing Cdr: " + EX.Message);
            }
            return billingCDR;
        }
        void GenerateZones(TOne.Business.ProtCodeMap codeMap, Billing_CDR_Base cdr)
        {
            // Our Zone
            Code ourCurrentCode = codeMap.Find(cdr.CDPN, CarrierAccount.SYSTEM, cdr.Attempt);
            if (ourCurrentCode != null)
            {
                cdr.OurZone = ourCurrentCode.Zone;
                cdr.OurCode = ourCurrentCode.Value;
            }

            // Originating Zone
            if (cdr.CustomerID != null && CarrierAccount.All.ContainsKey(cdr.CustomerID))
            {
                CarrierAccount customer = CarrierAccount.All[cdr.CustomerID];
                if (customer.IsOriginatingZonesEnabled)
                {
                    if (cdr.CGPN != null && cdr.CGPN.Trim().Length > 0)
                    {
                        string orginatingCode = InvalidCGPNDigits.Replace(cdr.CGPN, "");
                        Code originatingCode = codeMap.Find(orginatingCode, CarrierAccount.SYSTEM, cdr.Attempt);
                        if (originatingCode != null)
                            cdr.OriginatingZone = originatingCode.Zone;
                    }
                }
            }

            // Supplier Zone
            if (cdr.SupplierID != null && CarrierAccount.All.ContainsKey(cdr.SupplierID))
            {
                CarrierAccount supplier = CarrierAccount.All[cdr.SupplierID];
                Code supplierCode = null;

                if (TABS.SystemParameter.AllowCostZoneCalculationFromCDPNOut.BooleanValue.Value)
                {
                    if (string.IsNullOrEmpty(cdr.CDPNOut))
                        supplierCode = codeMap.Find(cdr.CDPN, supplier, cdr.Attempt);
                    else
                        supplierCode = codeMap.Find(cdr.CDPNOut, supplier, cdr.Attempt);
                }
                else
                    supplierCode = codeMap.Find(cdr.CDPN, supplier, cdr.Attempt);

                if (supplierCode != null)
                {
                    cdr.SupplierZone = supplierCode.Zone;
                    cdr.SupplierCode = supplierCode.Value;
                }
            }
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
