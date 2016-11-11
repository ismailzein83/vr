using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class BuildZonesChanges : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<AddedZone>> AddedZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RatePreview>> RatesPreview { get; set; }

        [RequiredArgument]
        public InArgument<List<SalePLZoneChange>> SalePLZonesChanges { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<RatePreview> ratesPreview = this.RatesPreview.Get(context);
            IEnumerable<ExistingZone> existingZones = this.ExistingZones.Get(context);
            IEnumerable<AddedZone> addedZones = this.AddedZones.Get(context);
            List<SalePLZoneChange> salePLZonesChanges = this.SalePLZonesChanges.Get(context);

            IEnumerable<SalePLZoneChange> zoneChanges = this.GetZoneChanges(ratesPreview, existingZones, addedZones);
           
            salePLZonesChanges.AddRange(zoneChanges);
        }

        #region Private Methods

        private IEnumerable<SalePLZoneChange> GetZoneChanges(IEnumerable<RatePreview> ratesPreview, IEnumerable<ExistingZone> existingZones, IEnumerable<AddedZone> addedZones)
        {
            IEnumerable<RatePreview> customersRatesPreview = ratesPreview != null ? ratesPreview.FindAllRecords(item => item.OnwerType == SalePriceListOwnerType.Customer) : null;
            Dictionary<string, List<RatePreview>> ratesPreviewByZoneName = StructureRatesPreviewByZoneName(customersRatesPreview);

            List<RatePreview> ratesPreviewForZone;
            List<SalePLZoneChange> zonesChanges = new List<SalePLZoneChange>();
            SaleZoneManager zoneManager = new SaleZoneManager();

            if (existingZones != null)
            {
                foreach (ExistingZone existingZone in existingZones)
                {
                    if (existingZone.AddedCodes.Count > 0 || existingZone.ExistingCodes.Any(item => item.ChangedEntity != null))
                    {
                        ratesPreviewForZone = ratesPreviewByZoneName.GetRecord(existingZone.Name);
                        SalePLZoneChange zoneChange = new SalePLZoneChange()
                        {
                            ZoneName = existingZone.Name,
                            CountryId = existingZone.CountryId,
                            HasCodeChange = true,
                            CustomersHavingRateChange = ratesPreviewForZone != null ? ratesPreviewForZone.Select(item => item.OwnerId) : null
                        };
                        zonesChanges.Add(zoneChange);
                    }
                }
            }

            if (addedZones != null)
            {
                foreach (AddedZone addedZone in addedZones)
                {
                    ratesPreviewForZone = ratesPreviewByZoneName.GetRecord(addedZone.Name);
                    SalePLZoneChange zoneChange = new SalePLZoneChange()
                    {
                        ZoneName = addedZone.Name,
                        CountryId = addedZone.CountryId,
                        HasCodeChange = true,
                        CustomersHavingRateChange = ratesPreviewForZone != null ? ratesPreviewForZone.Select(item => item.OwnerId) : null
                    };
                    zonesChanges.Add(zoneChange);
                }
            }

            return zonesChanges;
        }

        private Dictionary<string, List<RatePreview>> StructureRatesPreviewByZoneName(IEnumerable<RatePreview> ratesPreview)
        {
            Dictionary<string, List<RatePreview>> ratesPreviewByZoneName = new Dictionary<string, List<RatePreview>>();
            if (ratesPreview != null)
            {
                List<RatePreview> ratesPreviewList;
                foreach (RatePreview ratePreview in ratesPreview)
                {
                    if (!ratesPreviewByZoneName.TryGetValue(ratePreview.ZoneName, out ratesPreviewList))
                    {
                        ratesPreviewList = new List<RatePreview>();
                        ratesPreviewList.Add(ratePreview);
                        ratesPreviewByZoneName.Add(ratePreview.ZoneName, ratesPreviewList);
                    }
                    else
                        ratesPreviewList.Add(ratePreview);
                }
            }
            return ratesPreviewByZoneName;
        }


        #endregion
    }
}
