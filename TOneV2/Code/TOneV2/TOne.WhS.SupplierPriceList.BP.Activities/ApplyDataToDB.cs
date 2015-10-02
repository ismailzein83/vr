using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public class ApplyDataToDB : CodeActivity
    {
        public InArgument<List<Zone>> Zones { get; set; }
        public InArgument<List<Code>> CodesToBeDeleted { get; set; }
        public InArgument<int> SupplierId { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            DateTime startApplying = DateTime.Now;
            List<Zone> zones = Zones.Get(context);
            SupplierPriceListManager supplierPriceListManager = new SupplierPriceListManager();
            int priceListId;
            if (!supplierPriceListManager.AddSupplierPriceList(SupplierId.Get(context), out priceListId))
            {
                TimeSpan stop = DateTime.Now.Subtract(startApplying);
                context.WriteTrackingMessage(LogEntryType.Information, "Failed to insert supplier price list and takes:{0}", stop);
                return;
            }
            
            ImportPriceListManager importPriceListManager = new ImportPriceListManager();
            importPriceListManager.InsertPriceListObject(zones, CodesToBeDeleted.Get(context), SupplierId.Get(context), priceListId);

            TimeSpan spent = DateTime.Now.Subtract(startApplying);
            context.WriteTrackingMessage(LogEntryType.Information, "Apply Date To DB  done and takes:{0}", spent);
        }
    }
}
