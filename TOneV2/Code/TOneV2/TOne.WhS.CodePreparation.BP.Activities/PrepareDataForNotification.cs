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
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    
    #region InArguments
    public class PrepareDataForNotificationInput
    {
        public IEnumerable<ExistingZone> ExistingZones { get; set; }

        public IEnumerable<AddedZone> AddedZones { get; set; }

        public IEnumerable<RatePreview> RatesPreview { get; set; }

        public List<SalePLZoneChange> SalePLZonesChanges { get; set; }
    }

    #endregion

     public sealed class PrepareDataForNotification : BaseAsyncActivity<PrepareDataForNotificationInput>
     {
        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<AddedZone>> AddedZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RatePreview>> RatesPreview { get; set; }

        [RequiredArgument]
        public InArgument<List<SalePLZoneChange>> SalePLZonesChanges { get; set; }

        protected override void DoWork(PrepareDataForNotificationInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<SalePLZoneChange> zoneChanges = this.GetZoneChanges(inputArgument.RatesPreview, inputArgument.ExistingZones, inputArgument.AddedZones);
            if(zoneChanges.Count() > 0)
            {
                lock (inputArgument.SalePLZonesChanges)
                {
                    inputArgument.SalePLZonesChanges.AddRange(zoneChanges);
                }
            }
        }

        protected override PrepareDataForNotificationInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareDataForNotificationInput()
            {
                ExistingZones = this.ExistingZones.Get(context),
                AddedZones = this.AddedZones.Get(context),
                RatesPreview = this.RatesPreview.Get(context),
                SalePLZonesChanges = this.SalePLZonesChanges.Get(context)
            };
        }

        #region Private Methods

        private IEnumerable<SalePLZoneChange> GetZoneChanges(IEnumerable<RatePreview> ratesPreview, IEnumerable<ExistingZone> existingZones, IEnumerable<AddedZone> addedZones)
        {
            Dictionary<string, List<RatePreview>> ratesPreviewByZoneName = new Dictionary<string, List<RatePreview>>();

            if (ratesPreview != null)
            {
                IEnumerable<RatePreview> newRatesPreview = ratesPreview.FindAllRecords(item => item.ChangeType == RateChangeType.New);
                ratesPreviewByZoneName = StructureRatesPreviewByZoneName(newRatesPreview);
            }

            List<SalePLZoneChange> zonesChanges = new List<SalePLZoneChange>();
            SaleZoneManager zoneManager = new SaleZoneManager();

            if (existingZones != null)
            {
                foreach (ExistingZone existingZone in existingZones)
                {
                    if (existingZone.AddedCodes.Count > 0 || existingZone.ExistingCodes.Any(item => item.ChangedEntity != null))
                    {

                        SalePLZoneChange zoneChange = new SalePLZoneChange()
                        {
                            ZoneName = existingZone.Name,
                            CountryId = existingZone.CountryId,
                            HasCodeChange = true,
                            CustomersHavingRateChange = this.GetCustomersHavingRateChange(ratesPreviewByZoneName, existingZone.Name)
                        };
                        zonesChanges.Add(zoneChange);
                    }
                }
            }

            if (addedZones != null)
            {
                foreach (AddedZone addedZone in addedZones)
                {
                    SalePLZoneChange zoneChange = new SalePLZoneChange()
                    {
                        ZoneName = addedZone.Name,
                        CountryId = addedZone.CountryId,
                        HasCodeChange = true,
                        CustomersHavingRateChange = this.GetCustomersHavingRateChange(ratesPreviewByZoneName, addedZone.Name)
                    };
                    zonesChanges.Add(zoneChange);
                }
            }

            return zonesChanges;
        }

        private IEnumerable<int> GetCustomersHavingRateChange(Dictionary<string, List<RatePreview>> ratesPreviewByZoneName, string zoneName)
        {
            HashSet<int> customersIds = new HashSet<int>();
            IEnumerable<RatePreview> newRates = ratesPreviewByZoneName.GetRecord(zoneName);

            if (newRates != null)
            {
                foreach (RatePreview rate in newRates)
                {
                    if (rate.OnwerType == SalePriceListOwnerType.Customer)
                        customersIds.Add(rate.OwnerId);
                    else
                    {
                        CustomerSellingProductManager manager = new CustomerSellingProductManager();
                        IEnumerable<CarrierAccountInfo> customersAssignedToSellingProduct = manager.GetCustomersBySellingProductId(rate.OwnerId, DateTime.Today);
                        if (customersAssignedToSellingProduct != null)
                        {
                            IEnumerable<int> ids = customersAssignedToSellingProduct.Select(itm => itm.CarrierAccountId);
                            customersIds.UnionWith(ids);
                        }
                    }
                }
            }


            return customersIds;
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
