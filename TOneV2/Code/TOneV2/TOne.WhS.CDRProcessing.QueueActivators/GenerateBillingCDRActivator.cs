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
                  if (baseCDR is BillingMainCDR)
                  {
                      if (cdrMainBatch.MainCDRs == null)
                          cdrMainBatch.MainCDRs = new List<BillingMainCDR>();
                      BillingMainCDR main = new BillingMainCDR(baseCDR);
                      cdrMainBatch.MainCDRs.Add(main);
                  }
                  else if (baseCDR is BillingFailedCDR)
                  {
                      if (cdrFailedBatch.FailedCDRs == null)
                          cdrFailedBatch.FailedCDRs = new List<BillingFailedCDR>();

                      BillingFailedCDR failed = new BillingFailedCDR(baseCDR);
                      cdrFailedBatch.FailedCDRs.Add(failed);
                  }
                  else
                  {
                      if (cdrInvalidBatch.InvalidCDRs == null)
                          cdrInvalidBatch.InvalidCDRs = new List<BillingInvalidCDR>();
                      BillingInvalidCDR invalid = new BillingInvalidCDR(baseCDR);
                      cdrInvalidBatch.InvalidCDRs.Add(invalid);
                  }   
            }
            outputItems.Add("Generate CDR Prices", cdrMainBatch);
            outputItems.Add("Generate Stats", billingCDRBatch);
            outputItems.Add("Store Invalid CDRs", cdrInvalidBatch);
            outputItems.Add("Store Failed CDRs", cdrFailedBatch);

        }

        private BillingCDRBase GenerateBillingCdr(CDR cdr)
        {
            BillingCDRBase billingCDRMapped = null;
            if (cdr.DurationInSeconds > 0)
            {
                billingCDRMapped = new BillingMainCDR();
            }
            else
            {
                billingCDRMapped = new BillingFailedCDR();
            }

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
            CodeMatchBuilder codeMatchBuilder = new CodeMatchBuilder();
            CustomerIdentificationRule customerIdentificationRule = customerManager.GetMatchRule(customerIdentificationRuleTarget);
            SupplierIdentificationRule supplierIdentificationRule = supplierManager.GetMatchRule(supplierIdentificationRuleTarget);

            billingCDRMapped.CustomerId=customerIdentificationRule.Settings.CustomerId;
            billingCDRMapped.SupplierId = supplierIdentificationRule.Settings.SupplierId;

            SaleCodeMatch saleCodeMatch = codeMatchBuilder.GetSaleCodeMatch(cdr.CDPN, customerIdentificationRule.Settings.CustomerId, cdr.Attempt);
            SupplierCodeMatch supplierCodeMatch = codeMatchBuilder.GetSupplierCodeMatch(cdr.CDPN, supplierIdentificationRule.Settings.SupplierId, cdr.Attempt);
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
         
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierAccountDetail customer = carrierAccountManager.GetCarrierAccount(customerIdentificationRule.Settings.CustomerId);
            CarrierAccountDetail supplier = carrierAccountManager.GetCarrierAccount(supplierIdentificationRule.Settings.SupplierId);
            if (billingCDRMapped is BillingMainCDR)
                if ( billingCDRMapped.CustomerId == 0
                    || billingCDRMapped.SupplierId == 0
                    || saleCodeMatch.SaleZoneId == 0
                    || supplierCodeMatch.SupplierZoneId == 0)
                {
                    billingCDRMapped = new BillingInvalidCDR(billingCDRMapped);
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
