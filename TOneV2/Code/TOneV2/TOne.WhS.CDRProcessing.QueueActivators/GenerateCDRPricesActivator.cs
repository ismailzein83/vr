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
            SaleRateManager saleRateManager = new SaleRateManager();
            foreach(BillingMainCDR cdr in cdrMainBatch.MainCDRs){
                CallCost callCost = supplierRateManager.GetCallCost(cdr.BillingCDR.SupplierId, cdr.BillingCDR.SupplierZoneID, cdr.BillingCDR.DurationInSeconds, cdr.BillingCDR.Attempt);
                CallSale callSale = saleRateManager.GetCallSale(cdr.BillingCDR.CustomerId, cdr.BillingCDR.SaleZoneID, cdr.BillingCDR.DurationInSeconds, cdr.BillingCDR.Attempt);
                cdr.Cost =new Cost();
                cdr.Sale =new Sale();
                if (callCost != null)
                {
                        cdr.Cost.CurrencyId = callCost.CurrencyId;
                        cdr.Cost.RateValue = callCost.RateValue;
                        cdr.Cost.TotalNet = callCost.TotalNet;
                }
                if (callSale != null)
                {
                    cdr.Sale.CurrencyId = callSale.CurrencyId;
                    cdr.Sale.RateValue = callSale.RateValue;
                    cdr.Sale.TotalNet = callSale.TotalNet;

                }
                
            }
           
            outputItems.Add("Store Main CDRs", cdrMainBatch);

        }
    }
}
