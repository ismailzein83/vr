using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CDRProcessing.Business;
using TOne.WhS.CDRProcessing.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.WhS.CDRProcessing.QueueActivators
{
    public class GenerateBillingCDRActivator : QueueActivator
    {
        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
             CDRBatch cdrBatch = item as CDRBatch;
             CDRBillingBatch billingCDRBatch = new CDRBillingBatch();
             billingCDRBatch.CDRs = new List<BillingCDRBase>();
             CDRMainBatch cdrMainBatch = new CDRMainBatch();
             CDRFailedBatch cdrFailedBatch = new CDRFailedBatch();
             CDRInvalidBatch cdrInvalidBatch = new CDRInvalidBatch();

            foreach(CDR cdr in cdrBatch.CDRs){
                  DateTime StartIdentification = DateTime.Now;
                  BillingCDRBase baseCDR = GenerateBillingCdr(cdr);

                  billingCDRBatch.CDRs.Add(baseCDR);
                 if (baseCDR.CustomerId == 0 || baseCDR.SupplierId == 0 || baseCDR.SaleCode == null || baseCDR.SaleZoneID == 0 || baseCDR.SupplierCode == null || baseCDR.SupplierZoneID == 0)
                      {
                          if (cdrInvalidBatch.InvalidCDRs == null)
                              cdrInvalidBatch.InvalidCDRs = new List<BillingInvalidCDR>();
                          BillingInvalidCDR invalid = new BillingInvalidCDR();
                          invalid.BillingCDR = baseCDR;
                          cdrInvalidBatch.InvalidCDRs.Add(invalid);     
                  }
                  else
                  {
                      if (baseCDR.DurationInSeconds > 0)
                      {
                          if (cdrMainBatch.MainCDRs == null)
                              cdrMainBatch.MainCDRs = new List<BillingMainCDR>();
                          BillingMainCDR main = new BillingMainCDR();
                          main.BillingCDR = baseCDR;
                          cdrMainBatch.MainCDRs.Add(main);
                      }
                      else
                      {
                          if (cdrFailedBatch.FailedCDRs == null)
                              cdrFailedBatch.FailedCDRs = new List<BillingFailedCDR>();

                          BillingFailedCDR failed = new BillingFailedCDR();
                          failed.BillingCDR = baseCDR;
                          cdrFailedBatch.FailedCDRs.Add(failed);
                      }
                  }   
                 
                  

                
            }
            if (cdrMainBatch.MainCDRs != null && cdrMainBatch.MainCDRs.Count()>0)
              outputItems.Add("Generate CDR Prices", cdrMainBatch);
            if (billingCDRBatch.CDRs != null && billingCDRBatch.CDRs.Count() > 0)
                outputItems.Add("Generate Stats", billingCDRBatch);
            if (cdrInvalidBatch.InvalidCDRs != null && cdrInvalidBatch.InvalidCDRs.Count() > 0)
             outputItems.Add("Store Invalid CDRs", cdrInvalidBatch);
            if (cdrFailedBatch.FailedCDRs != null && cdrFailedBatch.FailedCDRs.Count() > 0)
               outputItems.Add("Store Failed CDRs", cdrFailedBatch);

        }

        private BillingCDRBase GenerateBillingCdr(CDR cdr)
        {
            BillingCDRBase billingCDRMapped = new BillingCDRBase();
            CustomerIdentificationRuleTarget customerIdentificationRuleTarget = new Entities.CustomerIdentificationRuleTarget
            {
                EffectiveOn = cdr.Attempt,
                CDPNPrefix = cdr.CDPN,
                InCarrier = cdr.InCarrier,
                InTrunk = cdr.InTrunk
            };
            SupplierIdentificationRuleTarget supplierIdentificationRuleTarget = new Entities.SupplierIdentificationRuleTarget
            {
                EffectiveOn = cdr.Attempt,
                CDPNPrefix = cdr.CDPN,
                OutCarrier = cdr.OutCarrier,
                OutTrunk = cdr.OutTrunk
            };
            billingCDRMapped = GetBillingCDRBase(cdr, billingCDRMapped);
           
            CustomerIdentificationRuleManager customerManager = new CustomerIdentificationRuleManager();
            SupplierIdentificationRuleManager supplierManager = new SupplierIdentificationRuleManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CodeMatchBuilder codeMatchBuilder = new CodeMatchBuilder();
            CustomerIdentificationRule customerIdentificationRule = customerManager.GetMatchRule(customerIdentificationRuleTarget);
            SupplierIdentificationRule supplierIdentificationRule = supplierManager.GetMatchRule(supplierIdentificationRuleTarget);
            SaleCodeMatch saleCodeMatch = null;
            CarrierAccount customer = null;
            if (customerIdentificationRule!=null)
            {
                billingCDRMapped.CustomerId = customerIdentificationRule.Settings.CustomerId;
                saleCodeMatch = codeMatchBuilder.GetSaleCodeMatch(cdr.CDPN, customerIdentificationRule.Settings.CustomerId, cdr.Attempt);
                customer = carrierAccountManager.GetCarrierAccount(customerIdentificationRule.Settings.CustomerId);
            }

            SupplierCodeMatch supplierCodeMatch = null;
            CarrierAccount supplier = null;
            if (supplierIdentificationRule != null)
            {
                billingCDRMapped.SupplierId = supplierIdentificationRule.Settings.SupplierId;
                supplierCodeMatch = codeMatchBuilder.GetSupplierCodeMatch(cdr.CDPN, supplierIdentificationRule.Settings.SupplierId, cdr.Attempt);
                supplier = carrierAccountManager.GetCarrierAccount(supplierIdentificationRule.Settings.SupplierId);
            }
         
           
            if (saleCodeMatch != null)
            {
                billingCDRMapped.SaleCode = saleCodeMatch.SaleCode;
                billingCDRMapped.SaleZoneID = saleCodeMatch.SaleZoneId;
            }

            if (supplierCodeMatch != null)
            {
                billingCDRMapped.SupplierCode = supplierCodeMatch.SupplierCode;
                billingCDRMapped.SupplierZoneID = supplierCodeMatch.SupplierZoneId;

            }
            return billingCDRMapped;
        }
        private BillingCDRBase GetBillingCDRBase(CDR cdrBase, BillingCDRBase cdr)
        {
            cdr.Alert = cdrBase.Alert;
            cdr.Attempt = cdrBase.Attempt;
            cdr.CDPN = cdrBase.CDPN;
            cdr.CGPN = cdrBase.CGPN;
            cdr.Connect = cdrBase.Connect;
            cdr.Disconnect = cdrBase.Disconnect;
            cdr.DurationInSeconds = cdrBase.DurationInSeconds;
            cdr.ID = cdrBase.ID;
            cdr.PortIn = cdrBase.PortIn;
            cdr.PortOut = cdrBase.PortOut;
            cdr.ReleaseCode = cdrBase.ReleaseCode;
            cdr.ReleaseSource = cdrBase.ReleaseSource;
            return cdr;
        }
      
    }
}
