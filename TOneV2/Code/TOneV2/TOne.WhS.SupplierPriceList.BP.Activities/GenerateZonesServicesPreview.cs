using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Business;
using Vanrise.Queueing;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class GenerateZonesServicesPreview : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NotImportedZone>> NotImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<ZoneServicePreview>>> PreviewZonesServicesQueue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            BaseQueue<IEnumerable<ZoneServicePreview>> previewZonesServicesQueue = this.PreviewZonesServicesQueue.Get(context);
            IEnumerable<ImportedZone> importedZones = this.ImportedZones.Get(context);
            IEnumerable<NotImportedZone> notImportedZones = this.NotImportedZones.Get(context);

            List<ZoneServicePreview> zonesServicesPreview = new List<ZoneServicePreview>();


            if (importedZones != null)
            {
                foreach (ImportedZone importedZone in importedZones)
                {
                    ImportedZoneService importedZoneService = importedZone.ImportedZoneService;

                    if (importedZoneService != null)
                    {
                        ZoneServicePreview importedZonePreview = new ZoneServicePreview()
                        {
                            ZoneName = importedZone.ZoneName,
                            ImportedServiceIds = importedZoneService.ServiceIds,
                            ImportedServicesBED = importedZoneService.BED,
                            ZoneServicesChangeType = importedZoneService.ChangeType
                        };

                        if (importedZoneService.SystemZoneService != null)
                        {
                            importedZonePreview.SystemServiceIds = importedZoneService.SystemZoneService.ZoneServiceEntity.ReceivedServices.Select(item => item.ServiceId).ToList();
                            importedZonePreview.SystemServicesBED = importedZoneService.SystemZoneService.BED;
                            importedZonePreview.SystemServicesEED = importedZoneService.SystemZoneService.EED;
                        }

                        zonesServicesPreview.Add(importedZonePreview);
                    }
                    

                    NotImportedZoneService notImportedZoneService = importedZone.NotImportedZoneService;
                    if (notImportedZoneService != null)
                    {
                        zonesServicesPreview.Add(new ZoneServicePreview
                        {
                            ZoneName = importedZone.ZoneName,
                            SystemServiceIds = notImportedZoneService.ZoneServicesIds,
                            SystemServicesBED = notImportedZoneService.BED,
                            SystemServicesEED = notImportedZoneService.EED,
                            ZoneServicesChangeType = notImportedZoneService.HasChanged ? ZoneServiceChangeType.Deleted : ZoneServiceChangeType.NotChanged
                        });
                    }
                    
                }
            }


            if (notImportedZones != null)
            {
                foreach (NotImportedZone notImportedZone in notImportedZones)
                {
                    NotImportedZoneService notImportedZoneService = notImportedZone.NotImportedZoneService;
                    ZoneServicePreview zoneServicePreview = new ZoneServicePreview()
                    {
                        ZoneName = notImportedZone.ZoneName,
                        ZoneServicesChangeType = notImportedZone.HasChanged ? ZoneServiceChangeType.Deleted : ZoneServiceChangeType.NotChanged,
                        SystemServicesBED = notImportedZoneService.BED,
                        SystemServicesEED = notImportedZoneService.EED,
                        SystemServiceIds = notImportedZoneService.ZoneServicesIds
                    };
                    zonesServicesPreview.Add(zoneServicePreview);
                }
            }

            previewZonesServicesQueue.Enqueue(zonesServicesPreview);
        }
    }
}
