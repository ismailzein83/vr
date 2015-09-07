using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TABS;
using TOne.Business;
using TOne.Caching;
using TOne.CDR.Entities;
using Vanrise.Caching;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.CDR.Business;
using Vanrise.Common;

namespace TOne.CDRProcess.Activities
{

    #region Arguments Classes

    public class GenerateBillingAndPricingCDRsInput
    {
        public BaseQueue<TOne.CDR.Entities.CDRBatch> InputQueue { get; set; }

        public BaseQueue<TOne.CDR.Entities.CDRBillingBatch> OutputBillingQueue { get; set; }

        public BaseQueue<TOne.CDR.Entities.CDRMainBatch> OutputMainCDRQueue { get; set; }

        public BaseQueue<TOne.CDR.Entities.CDRInvalidBatch> OutputInvalidCDRQueue { get; set; }

    }

    #endregion


    public sealed class GenerateBillingAndPricingCDRs : Vanrise.BusinessProcess.DependentAsyncActivity<GenerateBillingAndPricingCDRsInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<TOne.CDR.Entities.CDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<TOne.CDR.Entities.CDRBillingBatch>> OutputBillingQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<TOne.CDR.Entities.CDRMainBatch>> OutputMainCDRQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<TOne.CDR.Entities.CDRInvalidBatch>> OutputInvalidCDRQueue { get; set; }


        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            if (this.OutputBillingQueue.Get(context) == null)
                this.OutputBillingQueue.Set(context, new MemoryQueue<TOne.CDR.Entities.CDRBillingBatch>());
            if (this.OutputMainCDRQueue.Get(context) == null)
                this.OutputMainCDRQueue.Set(context, new MemoryQueue<TOne.CDR.Entities.CDRMainBatch>());
            if (this.OutputInvalidCDRQueue.Get(context) == null)
                this.OutputInvalidCDRQueue.Set(context, new MemoryQueue<TOne.CDR.Entities.CDRInvalidBatch>());


            var cacheManager = context.GetSharedInstanceData().GetCacheManager<TOneCacheManager>();
            handle.CustomData.Add("CacheManager", cacheManager);
            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(GenerateBillingAndPricingCDRsInput inputArgument, Vanrise.BusinessProcess.AsyncActivityStatus previousActivityStatus, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            TOneCacheManager cacheManager = handle.CustomData["CacheManager"] as TOneCacheManager;



            PricingGenerator generator;
            generator = new PricingGenerator(cacheManager);
            ProtCodeMap codeMap = new ProtCodeMap(cacheManager);
            SalePricing salePricing = new SalePricing(cacheManager);
            CostPricing costPricing = new CostPricing(cacheManager);
            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((cdrBatch) =>
                    {
                        TOne.CDR.Entities.CDRBillingBatch billingCDRs = new TOne.CDR.Entities.CDRBillingBatch();
                        billingCDRs.CDRs = new List<BillingCDRBase>();

                        TOne.CDR.Entities.CDRMainBatch CDRMains = new TOne.CDR.Entities.CDRMainBatch();
                        CDRMains.MainCDRs = new List<BillingCDRMain>();
                        TOne.CDR.Entities.CDRInvalidBatch CDRInvalids = new TOne.CDR.Entities.CDRInvalidBatch();
                        CDRInvalids.InvalidCDRs = new List<BillingCDRInvalid>();


                        TABS.Switch cdrSwitch = null;
                        if (cdrBatch.SwitchId != 0)
                            if (!TABS.Switch.All.TryGetValue(cdrBatch.SwitchId, out cdrSwitch))
                                throw new Exception("Switch Not Exist");

                        DateTime startPricing = DateTime.Now;
                        double totalsecondsforidentifications = 0;
                        double totalsecondsforpricing = 0;
                        foreach (TABS.CDR cdr in cdrBatch.CDRs)
                        {
                            if (cdr == null) continue;
                            if (cdr.Switch == null)
                                cdr.Switch = cdrSwitch;

                            DateTime StartIdentification = DateTime.Now;

                            Billing_CDR_Base cdrBase = GenerateBillingCdr(codeMap, cdr);
                            TimeSpan spentIndentification = DateTime.Now.Subtract(StartIdentification);
                            totalsecondsforidentifications += spentIndentification.TotalSeconds;
                            BillingCDRBase baseCDR = GetBillingCDRBase(cdrBase);

                            billingCDRs.CDRs.Add(baseCDR);

                            if (baseCDR.IsValid)
                            {
                                BillingCDRMain main = new BillingCDRMain(baseCDR);
                                DateTime timePricing = DateTime.Now;
                                main.sale = salePricing.GetRepricing(main);
                                main.cost = costPricing.GetRepricing(main);



                                //main.cost = generator.GetRepricing<BillingCDRCost>(main);
                                //main.sale = generator.GetRepricing<BillingCDRSale>(main);
                                TimeSpan spentPricingPerCDR = DateTime.Now.Subtract(timePricing);
                                totalsecondsforpricing += spentPricingPerCDR.TotalSeconds;
                                HandlePassThrough(main);

                                if (main != null && main.cost != null && main.SupplierCode != null)
                                    main.cost.Code = main.SupplierCode;

                                if (main != null && main.sale != null && main.OurCode != null)
                                    main.sale.Code = main.OurCode;
                                if (main.sale != null)
                                {
                                    main.sale.Attempt = main.Attempt;
                                    main.sale.Updated = DateTime.Now;
                                }
                                if (main.cost != null)
                                {
                                    main.cost.Attempt = main.Attempt;
                                    main.cost.Updated = DateTime.Now;
                                }
                                CDRMains.MainCDRs.Add(main);

                            }
                            else
                                CDRInvalids.InvalidCDRs.Add(new BillingCDRInvalid(baseCDR));

                        }
                        TimeSpan spent = DateTime.Now.Subtract(startPricing);
                        handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Pricing and billings cdrs({0}-Main:{1},Invalid:{2}) done and takes:{3}", cdrBatch.CDRs.Count, CDRMains.MainCDRs.Count, CDRInvalids.InvalidCDRs.Count, spent);
                        handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Pricing and billings cdrs({0}) Identifications:{1},Pricing:{2}", cdrBatch.CDRs.Count, totalsecondsforidentifications, totalsecondsforpricing);
                        inputArgument.OutputBillingQueue.Enqueue(billingCDRs);
                        if (CDRMains.MainCDRs.Count > 0) inputArgument.OutputMainCDRQueue.Enqueue(CDRMains);
                        if (CDRInvalids.InvalidCDRs.Count > 0) inputArgument.OutputInvalidCDRQueue.Enqueue(CDRInvalids);

                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override GenerateBillingAndPricingCDRsInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new GenerateBillingAndPricingCDRsInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputBillingQueue = this.OutputBillingQueue.Get(context),
                OutputMainCDRQueue = this.OutputMainCDRQueue.Get(context),
                OutputInvalidCDRQueue = this.OutputInvalidCDRQueue.Get(context)
            };
        }

        #region private Methods

        private void HandlePassThrough(BillingCDRMain cdr)
        {
            TABS.CarrierAccount Customer = TABS.CarrierAccount.All.ContainsKey(cdr.CustomerID) ? TABS.CarrierAccount.All[cdr.CustomerID] : null;
            TABS.CarrierAccount Supplier = TABS.CarrierAccount.All.ContainsKey(cdr.SupplierID) ? TABS.CarrierAccount.All[cdr.SupplierID] : null;

            if (Customer == null || Supplier == null) return;

            if (Customer.IsPassThroughCustomer && cdr.cost != null)
            {
                var sale = new BillingCDRSale();
                cdr.sale = sale;
                sale.Copy(cdr.cost);
                sale.ZoneID = cdr.OurZoneID;
            }
            if (Supplier.IsPassThroughSupplier && cdr.sale != null)
            {
                var cost = new BillingCDRCost();
                cdr.cost = cost;
                cost.Copy(cdr.sale);
                cost.ZoneID = cdr.SupplierZoneID;
            }
        }

        private BillingCDRBase GetBillingCDRBase(Billing_CDR_Base cdrBase)
        {
            BillingCDRBase cdr = new BillingCDRBase();

            cdr.Alert = cdrBase.Alert;
            cdr.Attempt = cdrBase.Attempt;
            cdr.CDPN = cdrBase.CDPN;
            cdr.CDPNOut = cdrBase.CDPNOut;
            cdr.CGPN = cdrBase.CGPN;
            cdr.Connect = cdrBase.Connect;
            cdr.CustomerID = cdrBase.CustomerID;
            cdr.Disconnect = cdrBase.Disconnect;
            cdr.DurationInSeconds = cdrBase.DurationInSeconds;
            cdr.Extra_Fields = cdrBase.Extra_Fields;
            cdr.ID = cdrBase.ID;
            cdr.IsRerouted = cdrBase.IsRerouted;
            cdr.OriginatingZoneID = cdrBase.OriginatingZone == null ? -1 : cdrBase.OriginatingZone.ZoneID;
            cdr.OurCode = cdrBase.OurCode;
            cdr.OurZoneID = cdrBase.OurZone == null ? -1 : cdrBase.OurZone.ZoneID;
            cdr.Port_IN = cdrBase.Port_IN;
            cdr.Port_OUT = cdrBase.Port_OUT;
            cdr.ReleaseCode = cdrBase.ReleaseCode;
            cdr.ReleaseSource = cdrBase.ReleaseSource;
            cdr.SIP = cdrBase.SIP;
            cdr.SubscriberID = cdrBase.SubscriberID;
            cdr.SupplierCode = cdrBase.SupplierCode;
            cdr.SupplierID = cdrBase.SupplierID;
            cdr.SupplierZoneID = cdrBase.SupplierZone == null ? -1 : cdrBase.SupplierZone.ZoneID;
            cdr.SwitchCdrID = cdrBase.SwitchCdrID;
            cdr.SwitchID = cdrBase.Switch == null ? -1 : cdrBase.Switch.SwitchID;
            cdr.Tag = cdrBase.Tag;
            cdr.IsValid = cdrBase.IsValid;

            return cdr;
        }

        private Billing_CDR_Base GenerateBillingCdr(TOne.Business.ProtCodeMap codeMap, TABS.CDR cdr)
        {
            Billing_CDR_Base billingCDR = null;

            if (cdr.DurationInSeconds > 0)
            {
                billingCDR = new Billing_CDR_Main();
            }
            else
                billingCDR = new Billing_CDR_Invalid();

            bool valid = cdr.Switch.SwitchManager.FillCDRInfo(cdr.Switch, cdr, billingCDR);

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

        private System.Text.RegularExpressions.Regex InvalidCGPNDigits = new System.Text.RegularExpressions.Regex("[^0-9]", System.Text.RegularExpressions.RegexOptions.Compiled);
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
