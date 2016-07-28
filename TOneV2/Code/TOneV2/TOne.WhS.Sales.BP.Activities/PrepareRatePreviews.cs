﻿using System;
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

            foreach (RateToChange rateToChange in ratesToChange)
            {
                var ratePreview = new RatePreview()
                {
                    ZoneName = rateToChange.ZoneName,
                    NewRate = rateToChange.NormalRate,
                    ChangeType = rateToChange.ChangeType,
                    EffectiveOn = rateToChange.BED,
                    EffectiveUntil = rateToChange.EED
                };

                // Set the properties of the current rate

                if (rateToChange.RecentExistingRate != null)
                {
                    ratePreview.CurrentRate = rateToChange.RecentExistingRate.RateEntity.NormalRate;
                    ratePreview.IsCurrentRateInherited = false;
                }
                else if (ownerType == SalePriceListOwnerType.Customer) // Check if an inherited rate exists
                {
                    int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(ownerId, rateToChange.BED, false);
                    if (!sellingProductId.HasValue)
                        throw new NullReferenceException("sellingProductId");

                    SaleEntityZoneRate rate = rateLocator.GetCustomerZoneRate(ownerId, sellingProductId.Value, rateToChange.ZoneId);
                    if (rate != null)
                    {
                        ratePreview.CurrentRate = rate.Rate.NormalRate;
                        ratePreview.IsCurrentRateInherited = true;
                    }
                    else
                        ratePreview.IsCurrentRateInherited = false;
                }

                ratePreviews.Add(ratePreview);
            }

            foreach (RateToClose rateToClose in ratesToClose)
            {
                ratePreviews.Add(new RatePreview()
                {
                    ZoneName = rateToClose.ZoneName,
                    CurrentRate = rateToClose.Rate,
                    IsCurrentRateInherited = false, // The user can't close an inherited rate
                    ChangeType = RateChangeType.Deleted,
                    EffectiveOn = rateToClose.CloseEffectiveDate
                });
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
    }
}
