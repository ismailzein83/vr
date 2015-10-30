using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Business;
using TOne.WhS.CDRProcessing.Entities;
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
             cdrMainBatch.MainCDRs = new List<BillingMainCDR>();

             CDRFailedBatch cdrFailedBatch = new CDRFailedBatch();
             cdrFailedBatch.FailedCDRs = new List<BillingFailedCDR>();

             CDRInvalidBatch cdrInvalidBatch = new CDRInvalidBatch();
             cdrInvalidBatch.InvalidCDRs = new List<BillingInvalidCDR>();

             CustomerIdentificationRuleManager customerManager = new CustomerIdentificationRuleManager();
             SupplierIdentificationRuleManager supplierManager = new SupplierIdentificationRuleManager();
            foreach(CDR cdr in cdrBatch.CDRs){
                  CustomerIdentificationRuleTarget customerIdentificationRuleTarget=new Entities.CustomerIdentificationRuleTarget{
                         CDPNPrefix=cdr.CDPN,
                         IN_Carrier=cdr.IN_Carrier,
                         IN_Trunk=cdr.IN_Trunk
                    };
                  SupplierIdentificationRuleTarget supplierIdentificationRuleTarget = new Entities.SupplierIdentificationRuleTarget
                  {
                      CDPNPrefix = cdr.CDPN,
                      OUT_Carrier = cdr.OUT_Carrier,
                      OUT_Trunk = cdr.OUT_Trunk
                  };
                  CustomerIdentificationRule customerIdentificationRule = customerManager.GetMatchRule(customerIdentificationRuleTarget);
                  SupplierIdentificationRule supplierIdentificationRule = supplierManager.GetMatchRule(supplierIdentificationRuleTarget);
                  billingCDRBatch.CDRs.Add(FillBillingCDR(customerIdentificationRule, supplierIdentificationRule));
                  cdrMainBatch.MainCDRs.Add(FillMainCDR(customerIdentificationRule, supplierIdentificationRule));
                  cdrFailedBatch.FailedCDRs.Add(FillFailedCDR(customerIdentificationRule, supplierIdentificationRule));
                  cdrInvalidBatch.InvalidCDRs.Add(FillInvalidCDR(customerIdentificationRule, supplierIdentificationRule));
            }

            outputItems.Add("Generate Stats", billingCDRBatch);
            outputItems.Add("Generate CDR Prices", cdrMainBatch);
            outputItems.Add("Store Invalid CDRs", cdrInvalidBatch);
            outputItems.Add("Store Failed CDRs", cdrFailedBatch);

        }

        public BillingMainCDR FillMainCDR(CustomerIdentificationRule customerIdentificationRule, SupplierIdentificationRule supplierIdentificationRule)
        {
            return new BillingMainCDR
            {
                CustomerId = customerIdentificationRule.Settings.CustomerId,
                SupplierId = supplierIdentificationRule.Settings.SupplierId
            };
        }
        public BillingInvalidCDR FillInvalidCDR(CustomerIdentificationRule customerIdentificationRule, SupplierIdentificationRule supplierIdentificationRule)
        {
            return new BillingInvalidCDR
            {
                CustomerId = customerIdentificationRule.Settings.CustomerId,
                SupplierId = supplierIdentificationRule.Settings.SupplierId
            };
        }
        public BillingFailedCDR FillFailedCDR(CustomerIdentificationRule customerIdentificationRule, SupplierIdentificationRule supplierIdentificationRule)
        {
            return new BillingFailedCDR
            {
                CustomerId = customerIdentificationRule.Settings.CustomerId,
                SupplierId = supplierIdentificationRule.Settings.SupplierId
            };
        }
        public BillingCDRBase FillBillingCDR(CustomerIdentificationRule customerIdentificationRule, SupplierIdentificationRule supplierIdentificationRule)
        {
            return new BillingCDRBase
            {
                CustomerId = customerIdentificationRule.Settings.CustomerId,
                SupplierId = supplierIdentificationRule.Settings.SupplierId
            };
        }
    }
}
