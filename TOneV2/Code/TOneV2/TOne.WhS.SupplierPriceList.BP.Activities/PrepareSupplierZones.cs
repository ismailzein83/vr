using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.BusinessProcess;
namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class PrepareSupplierZones:CodeActivity
    {
        public InArgument<int> SupplierId { get; set; }
        public InArgument<DateTime> EffectiveDate { get; set; }
        public InArgument<Dictionary<String, List<TOne.WhS.SupplierPriceList.Entities.SupplierPriceList>>> PriceListDictionary { get; set; }
        public OutArgument<List<SupplierZone>> SupplierZones { get; set; }
        protected override void Execute(CodeActivityContext context)

        {
            DateTime startPreparing = DateTime.Now;
            int supplierId=SupplierId.Get(context);
            DateTime effectiveDate=EffectiveDate.Get(context);
            Dictionary<String, List<TOne.WhS.SupplierPriceList.Entities.SupplierPriceList>> priceListDictionary = PriceListDictionary.Get(context);
            List<SupplierZone> supplierZones = new List<SupplierZone>();
            foreach (var obj in priceListDictionary)
            {
                supplierZones.Add(new SupplierZone
                {
                    SupplierId = supplierId,
                    Name = obj.Key,
                    BeginEffectiveDate = effectiveDate,
                    EndEffectiveDate = null

                });
            }
           SupplierZones.Set(context, supplierZones);
           TimeSpan spent = DateTime.Now.Subtract(startPreparing);
           context.WriteTrackingMessage(LogEntryType.Information, "Preparing Supplier Zones  done and takes:{0}", spent);
        }
    }
}
