using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ApplyZonePreviewDataToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<NewZone>> NewZones { get; set; }
        
        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public InArgument<int> PriceListId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<NewZone> newZonesList = this.NewZones.Get(context);
            Dictionary<long, ExistingZone> existingZonesList = this.ExistingZonesByZoneId.Get(context);
            int priceListId = this.PriceListId.Get(context);
            
            List<ZonePreview> zonePreviewList = new List<ZonePreview>();

            if(newZonesList != null)
            {
                foreach (NewZone item in newZonesList)
                {
                    zonePreviewList.Add(new ZonePreview()
                    {
                        Name = item.Name,
                        ChangeType = ZoneChangeType.New,
                        BED = item.BED,
                        EED = item.EED
                    });
                }
            }

            if(existingZonesList != null)
            {
                foreach (ExistingZone item in existingZonesList.Values)
                {
                    if(item.ChangedZone != null)
                    {
                        zonePreviewList.Add(new ZonePreview()
                        {
                            Name = item.ZoneEntity.Name,
                            ChangeType = (item.BED == item.ChangedZone.EED) ? ZoneChangeType.Deleted : ZoneChangeType.Closed,
                            BED = item.BED,
                            EED = item.ChangedZone.EED
                        });
                    }
                }
            }

            SupplierZonePreviewManager manager = new SupplierZonePreviewManager();
            manager.Insert(priceListId, zonePreviewList);
        }
    }
}
