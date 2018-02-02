using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Queueing;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class GenerateZonesRoutingProductsPreview : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ZoneToProcess>> ZonesToProcess { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NotImportedZone>> NotImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<ZoneRoutingProductPreview>>> PreviewZonesRoutingProductsQueue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ZoneToProcess> zonesToProcess = this.ZonesToProcess.Get(context);
            BaseQueue<IEnumerable<ZoneRoutingProductPreview>> previewZonesRoutingProductsQueue = this.PreviewZonesRoutingProductsQueue.Get(context);
            IEnumerable<NotImportedZone> notImportedZones = this.NotImportedZones.Get(context);

            List<ZoneRoutingProductPreview> zoneRoutingProductPreview = new List<ZoneRoutingProductPreview>();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            foreach (ZoneToProcess zoneToProcess in zonesToProcess)
            {
                zoneRoutingProductPreview.AddRange(zoneToProcess.ZonesRoutingProductsToAdd.MapRecords(ZoneRoutingProductToAddPreviewMapper));
                foreach (var notImportedZoneRoutingProduct in zoneToProcess.NotImportedZoneRoutingProduct)
                {
                    if (notImportedZoneRoutingProduct.OwnerType == SalePriceListOwnerType.Customer)
                    {
                        if (!carrierAccountManager.IsCarrierAccountActive(notImportedZoneRoutingProduct.OwnerId))
                            continue;
                    }
                    zoneRoutingProductPreview.Add(DeletedZoneRoutingProductPreviewMapper(notImportedZoneRoutingProduct));
                }
            }
            foreach (NotImportedZone notImportedZone in notImportedZones)
            {
                zoneRoutingProductPreview.AddRange(notImportedZone.NotImportedZoneRoutingProduct.MapRecords(NotImportedZoneRoutingProductPreviewMapper));
            }
            

            previewZonesRoutingProductsQueue.Enqueue(zoneRoutingProductPreview);
        }
        private ZoneRoutingProductPreview ZoneRoutingProductToAddPreviewMapper(ZoneRoutingProductToAdd zoneRoutingProductToAdd)
        {
            return new ZoneRoutingProductPreview()
            {
                ZoneName = zoneRoutingProductToAdd.ZoneName,
                OwnerType = zoneRoutingProductToAdd.OwnerType,
                OwnerId = zoneRoutingProductToAdd.OwnerId,
                RoutingProductId = zoneRoutingProductToAdd.RoutingProductId,
                BED = GetZoneRoutingProductToAddBED(zoneRoutingProductToAdd.AddedZonesRoutingProducts),
                EED = GetZoneRoutingProductToAddEED(zoneRoutingProductToAdd.AddedZonesRoutingProducts),
                ChangeType = ZoneRoutingProductChangeType.New,
            };
        }

        private ZoneRoutingProductPreview NotImportedZoneRoutingProductPreviewMapper(NotImportedZoneRoutingProduct notImportedZoneRoutingProduct)
        {
            return new ZoneRoutingProductPreview()
            {
                ZoneName = notImportedZoneRoutingProduct.ZoneName,
                OwnerType=notImportedZoneRoutingProduct.OwnerType,
                OwnerId=notImportedZoneRoutingProduct.OwnerId,
                RoutingProductId = notImportedZoneRoutingProduct.RoutingProductId,
                BED = notImportedZoneRoutingProduct.BED,
                EED = notImportedZoneRoutingProduct.EED,
                ChangeType = ZoneRoutingProductChangeType.NotChanged,
            };
        }

        private ZoneRoutingProductPreview DeletedZoneRoutingProductPreviewMapper(NotImportedZoneRoutingProduct notImportedZoneRoutingProduct)
        {
            return new ZoneRoutingProductPreview()
            {
                ZoneName = notImportedZoneRoutingProduct.ZoneName,
                OwnerType = notImportedZoneRoutingProduct.OwnerType,
                OwnerId = notImportedZoneRoutingProduct.OwnerId,
                RoutingProductId = notImportedZoneRoutingProduct.RoutingProductId,
                BED = notImportedZoneRoutingProduct.BED,
                EED = notImportedZoneRoutingProduct.EED,
                ChangeType = ZoneRoutingProductChangeType.Deleted,
            };
        }

        private DateTime GetZoneRoutingProductToAddBED(IEnumerable<AddedZoneRoutingProduct> addedZoneRoutingProduct)
        {
            return addedZoneRoutingProduct.Select(item => item.BED).Min();
        }
        private DateTime? GetZoneRoutingProductToAddEED(IEnumerable<AddedZoneRoutingProduct> addedZoneRoutingProduct)
        {
            return addedZoneRoutingProduct.Select(item => item.EED).VRMaximumDate();
        }
    }
}
