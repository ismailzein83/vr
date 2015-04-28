using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TABS;
using TOne.Business;
using TOne.Caching;
using TOne.Entities;
using Vanrise.Caching;

namespace TOne.CDRProcess.Activities
{
    #region Arguments Classes

    public class ProcessCDRsInput
    {
        public int SwitchID { get; set; }

        public TOneQueue<CDRBatch> InputQueue { get; set; }

        public List<TOneQueue<CDRBillingBatch>> OutputQueues { get; set; }


        public Guid CacheManagerId { get; set; }

    }

    #endregion
    public sealed class ProcessCDRs : Vanrise.BusinessProcess.DependentAsyncActivity<ProcessCDRsInput>
    {

        //[RequiredArgument]
        public InArgument<Guid> CacheManagerId { get; set; }

        [RequiredArgument]
        public InArgument<TOneQueue<CDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> SwitchID { get; set; }

        [RequiredArgument]
        public InOutArgument<List<TOneQueue<CDRBillingBatch>>> OutputQueues { get; set; }


        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            if (this.OutputQueues.Get(context) == null)
                this.OutputQueues.Set(context, new List<TOneQueue<CDRBillingBatch>>());

            base.OnBeforeExecute(context, handle);
        }


        protected override void DoWork(ProcessCDRsInput inputArgument, Vanrise.BusinessProcess.AsyncActivityStatus previousActivityStatus, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            TOneCacheManager cacheManager = CacheManagerFactory.GetCacheManager<TOneCacheManager>(inputArgument.CacheManagerId);
            ProtCodeMap codeMap = new ProtCodeMap(cacheManager);
            CDRManager manager = new CDRManager();

            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((cdrBatch) =>
                    {
                        CDRBillingBatch CdrBillingGenerated = new CDRBillingBatch();

                        foreach (TABS.CDR cdr in cdrBatch.CDRs)
                        {
                            CdrBillingGenerated.CDRs.Add(GenerateBillingCdr(codeMap, cdr));
                        }

                        //CDRBase cdrs = manager.GenerateBillingCdrs(cdrBatch, codeMap);

                        foreach (TOneQueue<CDRBillingBatch> outputQueue in inputArgument.OutputQueues)
                            outputQueue.Enqueue(CdrBillingGenerated);


                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }




        protected override ProcessCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ProcessCDRsInput
            {
                InputQueue = this.InputQueue.Get(context),
                CacheManagerId = this.CacheManagerId.Get(context),
                SwitchID = this.SwitchID.Get(context),
                OutputQueues = this.OutputQueues.Get(context)
            };
        }



        #region Private Methods
        private CDRBillingBatch GenerateBillingCdrs(CDRBatch cdrBatch, ProtCodeMap codeMap)
        {
            CDRBillingBatch CdrBillingGenerated = new CDRBillingBatch();
            List<Billing_CDR_Base> billingCDRs = new List<Billing_CDR_Base>();

            foreach (TABS.CDR cdr in cdrBatch.CDRs)
            {
                Billing_CDR_Base billingCDR = GenerateBillingCdr(codeMap, cdr);
                billingCDRs.Add(billingCDR);
            }
            CdrBillingGenerated.CDRs = billingCDRs;
            return CdrBillingGenerated;
        }


        log4net.ILog log = log4net.LogManager.GetLogger("TOne.CDRProcess.Activities.ProcessCDRsInput");
        static System.Text.RegularExpressions.Regex InvalidCGPNDigits = new System.Text.RegularExpressions.Regex("[^0-9]", System.Text.RegularExpressions.RegexOptions.Compiled);
        private Billing_CDR_Base GenerateBillingCdr(TOne.Business.ProtCodeMap codeMap, TABS.CDR cdr)
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
        private void GenerateZones(TOne.Business.ProtCodeMap codeMap, Billing_CDR_Base cdr)
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


    }
}
