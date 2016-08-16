using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    #region Public Classes

    public class PrepareRatePreviewsInput
    {
        public IEnumerable<RateToChange> RatesToChange { get; set; }

        public IEnumerable<RateToClose> RatesToClose { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public DateTime MinimumDate { get; set; }
    }

    public class PrepareRatePreviewsOutput
    {
        public IEnumerable<RatePreview> RatePreviews { get; set; }
    }

    #endregion

    public class PrepareRatePreviews : BaseAsyncActivity<PrepareRatePreviewsInput, PrepareRatePreviewsOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<RateToChange>> RatesToChange { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RateToClose>> RatesToClose { get; set; }

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<RatePreview>> RatePreviews { get; set; }

        #endregion

        protected override PrepareRatePreviewsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareRatePreviewsInput()
            {
                RatesToChange = this.RatesToChange.Get(context),
                RatesToClose = this.RatesToClose.Get(context),
                OwnerType = this.OwnerType.Get(context),
                OwnerId = this.OwnerId.Get(context),
                MinimumDate = this.MinimumDate.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.RatePreviews.Get(context) == null)
                this.RatePreviews.Set(context, new List<RatePreview>());
            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareRatePreviewsOutput DoWorkWithResult(PrepareRatePreviewsInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<RateToChange> ratesToChange = inputArgument.RatesToChange;
            IEnumerable<RateToClose> ratesToClose = inputArgument.RatesToClose;

            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            int ownerId = inputArgument.OwnerId;
            DateTime minimumDate = inputArgument.MinimumDate;

            var ratePreviews = new List<RatePreview>();
            var rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(minimumDate));
            var customerSellingProductManager = new CustomerSellingProductManager();
            var saleZoneManager = new SaleZoneManager();

            RateStateByZone stateByZone = StructureRateStateByZone(ratesToChange, ratesToClose);

            foreach (KeyValuePair<long, RateStateData> kvp in stateByZone)
            {
                bool normalRateExists = false;

                foreach (RateToChange rateToChange in kvp.Value.RatesToChange)
                {
                    if (!rateToChange.RateTypeId.HasValue)
                        normalRateExists = true;

                    var ratePreview = new RatePreview()
                    {
                        ZoneName = rateToChange.ZoneName,
                        RateTypeId = rateToChange.RateTypeId,
                        NewRate = rateToChange.NormalRate,
                        ChangeType = rateToChange.ChangeType,
                        EffectiveOn = rateToChange.BED,
                        EffectiveUntil = rateToChange.EED
                    };

                    SetCurrentRateProperties(ratePreview, ownerType, ownerId, rateToChange.RateTypeId, rateLocator, kvp.Key, customerSellingProductManager, rateToChange.BED);
                    ratePreviews.Add(ratePreview);
                }

                foreach (RateToClose rateToClose in ratesToClose)
                {
                    if (!rateToClose.RateTypeId.HasValue)
                        normalRateExists = true;

                    ratePreviews.Add(new RatePreview()
                    {
                        ZoneName = rateToClose.ZoneName,
                        RateTypeId = rateToClose.RateTypeId,
                        CurrentRate = rateToClose.Rate,
                        IsCurrentRateInherited = false, // The user can't close an inherited rate
                        ChangeType = RateChangeType.Deleted,
                        EffectiveOn = rateToClose.CloseEffectiveDate
                    });
                }

                if (!normalRateExists)
                {
                    var normalRatePreview = new RatePreview()
                    {
                        ZoneName = saleZoneManager.GetSaleZoneName(kvp.Key),
                        RateTypeId = null,
                        ChangeType = RateChangeType.NotChanged
                    };

                    SetCurrentRateProperties(normalRatePreview, ownerType, ownerId, null, rateLocator, kvp.Key, customerSellingProductManager, DateTime.Now);
                    ratePreviews.Add(normalRatePreview);
                }
            }

            return new PrepareRatePreviewsOutput()
            {
                RatePreviews = ratePreviews
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareRatePreviewsOutput result)
        {
            this.RatePreviews.Set(context, result.RatePreviews);
        }

        #region Private Methods

        private RateStateByZone StructureRateStateByZone(IEnumerable<RateToChange> ratesToChange, IEnumerable<RateToClose> ratesToClose)
        {
            var stateByZone = new RateStateByZone();
            RateStateData stateData;

            foreach (RateToChange rateToChange in ratesToChange)
            {
                if (!stateByZone.TryGetValue(rateToChange.ZoneId, out stateData))
                {
                    stateData = new RateStateData();
                    stateData.RatesToChange = new List<RateToChange>();
                    stateData.RatesToClose = new List<RateToClose>();
                    stateByZone.Add(rateToChange.ZoneId, stateData);
                }
                stateData.RatesToChange.Add(rateToChange);
            }

            foreach (RateToClose rateToClose in ratesToClose)
            {
                if (!stateByZone.TryGetValue(rateToClose.ZoneId, out stateData))
                {
                    stateData = new RateStateData();
                    stateData.RatesToChange = new List<RateToChange>();
                    stateData.RatesToClose = new List<RateToClose>();
                    stateByZone.Add(rateToClose.ZoneId, stateData);
                }
                stateData.RatesToClose.Add(rateToClose);
            }

            return stateByZone;
        }

        private void SetCurrentRateProperties(RatePreview ratePreview, SalePriceListOwnerType ownerType, int ownerId, int? rateTypeId, SaleEntityZoneRateLocator rateLocator, long zoneId, CustomerSellingProductManager customerSellingProductManager, DateTime effectiveOn)
        {
            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                SaleEntityZoneRate currentRate = rateLocator.GetSellingProductZoneRate(ownerId, zoneId);

                if (currentRate != null)
                {
                    ratePreview.CurrentRate = GetRateValue(rateTypeId, currentRate);
                    ratePreview.IsCurrentRateInherited = false;
                }
            }
            else if (ownerType == SalePriceListOwnerType.Customer)
            {
                int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(ownerId, effectiveOn, false);
                if (!sellingProductId.HasValue)
                    throw new NullReferenceException("sellingProductId");

                SaleEntityZoneRate currentRate = rateLocator.GetCustomerZoneRate(ownerId, sellingProductId.Value, zoneId);
                if (currentRate != null)
                {
                    ratePreview.CurrentRate = GetRateValue(rateTypeId, currentRate);
                    ratePreview.IsCurrentRateInherited = (currentRate.Source != SalePriceListOwnerType.Customer);
                }
            }
        }

        private decimal? GetRateValue(int? rateTypeId, SaleEntityZoneRate rate)
        {
            if (!rateTypeId.HasValue)
                return rate.Rate.NormalRate; // The rate locator ensures that rate.Rate != null

            if (rate.RatesByRateType != null)
            {
                SaleRate otherRate;
                if (rate.RatesByRateType.TryGetValue(rateTypeId.Value, out otherRate))
                    return otherRate.NormalRate;
            }

            return null;
        }
        
        #endregion

        #region Private Classes

        private class RateStateByZone : Dictionary<long, RateStateData>
        {

        }

        private class RateStateData
        {
            public List<RateToChange> RatesToChange { get; set; }
            public List<RateToClose> RatesToClose { get; set; }
        }

        #endregion
    }
}
