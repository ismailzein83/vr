using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class PrepareRatePreviews : CodeActivity
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

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<RateToChange> ratesToChange = RatesToChange.Get(context);
            IEnumerable<RateToClose> ratesToClose = RatesToClose.Get(context);

            SalePriceListOwnerType ownerType = OwnerType.Get(context);
            int ownerId = OwnerId.Get(context);
            DateTime minimumDate = MinimumDate.Get(context);

            var ratePreviews = new List<RatePreview>();
            var rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(minimumDate));
            var customerSellingProductManager = new CustomerSellingProductManager();

            if (ratesToChange != null)
            {
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
            }

            if (ratesToClose != null)
            {
                foreach (RateToClose rateToClose in ratesToClose)
                {
                    ratePreviews.Add(new RatePreview()
                    {
                        ZoneName = rateToClose.ZoneName,
                        CurrentRate = rateToClose.Rate,
                        IsCurrentRateInherited = false, // The user can't close an inherited rate
                        EffectiveOn = rateToClose.CloseEffectiveDate
                    });
                }
            }

            RatePreviews.Set(context, (ratePreviews.Count > 0) ? ratePreviews : null);
        }
    }
}
