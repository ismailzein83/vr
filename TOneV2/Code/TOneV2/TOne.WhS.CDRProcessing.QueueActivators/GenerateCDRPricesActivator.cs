using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CDRProcessing.Business;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.WhS.CDRProcessing.QueueActivators
{
   public class GenerateCDRPricesActivator: QueueActivator
    {
        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            CDRMainBatch cdrMainBatch = item as CDRMainBatch;
            SupplierRateManager supplierRateManager = new SupplierRateManager();
            foreach(BillingMainCDR cdr in cdrMainBatch.MainCDRs){
                CallCost callCost = supplierRateManager.GetCallCost(cdr.BillingCDR.SupplierId, cdr.BillingCDR.SupplierZoneID, cdr.BillingCDR.DurationInSeconds, cdr.BillingCDR.Attempt);
                if (callCost != null)
                {
                    cdr.MainCost = new MainCost
                    {
                        CurrencyId = callCost.CurrencyId,
                        RateValue = callCost.RateValue,
                        TotalNet = callCost.TotalNet
                    };
                    
                }
                
            }
           
            outputItems.Add("Store Main CDRs", cdrMainBatch);

        }
    }
}
