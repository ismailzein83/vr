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
        public SalePriceListOwnerType OwnerType { get; set; }
        public IEnumerable<DataByZone> DataByZone { get; set; }
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
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<DataByZone>> DataByZone { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<RatePreview>> RatePreviews { get; set; }

        #endregion

        protected override PrepareRatePreviewsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareRatePreviewsInput()
            {
                OwnerType = this.OwnerType.Get(context),
                DataByZone = this.DataByZone.Get(context)
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
            var ratePreviews = new List<RatePreview>();

            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            IEnumerable<DataByZone> dataByZone = inputArgument.DataByZone;

            foreach (DataByZone zoneData in dataByZone)
            {
                RatePreview normalRatePreview = GetNormalRatePreview(ownerType, zoneData);
                if (normalRatePreview != null)
                    ratePreviews.Add(normalRatePreview);

                foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
                {
                    var otherRatePreview = GetPreviewFromRateToChange(otherRateToChange, zoneData.ZoneRateGroup, ownerType);
                    ratePreviews.Add(otherRatePreview);
                }

                foreach (RateToClose otherRateToClose in zoneData.OtherRatesToClose)
                {
                    var otherRatePreview = GetPreviewFromRateToClose(otherRateToClose, zoneData.ZoneRateGroup, ownerType);
                    ratePreviews.Add(otherRatePreview);
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

        private RatePreview GetNormalRatePreview(SalePriceListOwnerType ownerType, DataByZone zoneData)
        {
            RatePreview normalRatePreview = null;

            if (zoneData.NormalRateToChange != null)
                normalRatePreview = GetPreviewFromRateToChange(zoneData.NormalRateToChange, zoneData.ZoneRateGroup, ownerType);
            
            else if (zoneData.NormalRateToClose != null)
                normalRatePreview = GetPreviewFromRateToClose(zoneData.NormalRateToClose, zoneData.ZoneRateGroup, ownerType);
            
            else if (zoneData.OtherRatesToChange.Count > 0 || zoneData.OtherRatesToClose.Count > 0)
            {
                normalRatePreview = new RatePreview()
                {
                    ZoneName = zoneData.ZoneName,
                    ChangeType = RateChangeType.NotChanged
                };
                SetCurrentNormalRateProperties(normalRatePreview, zoneData.ZoneRateGroup, ownerType);
            }

            return normalRatePreview;
        }

        private RatePreview GetPreviewFromRateToChange(RateToChange rateToChange, ZoneRateGroup zoneRateGroup, SalePriceListOwnerType ownerType)
        {
            var ratePreview = new RatePreview()
            {
                ZoneName = rateToChange.ZoneName,
                RateTypeId = rateToChange.RateTypeId,
                NewRate = rateToChange.NormalRate,
                ChangeType = rateToChange.ChangeType,
                EffectiveOn = rateToChange.BED,
                EffectiveUntil = rateToChange.EED
            };
            if (rateToChange.RateTypeId.HasValue)
                SetCurrentOtherRateProperties(ratePreview, zoneRateGroup, ownerType);
            else
                SetCurrentNormalRateProperties(ratePreview, zoneRateGroup, ownerType);
            return ratePreview;
        }
        private RatePreview GetPreviewFromRateToClose(RateToClose rateToClose, ZoneRateGroup zoneRateGroup, SalePriceListOwnerType ownerType)
        {
            var ratePreview = new RatePreview()
            {
                ZoneName = rateToClose.ZoneName,
                RateTypeId = rateToClose.RateTypeId,
                ChangeType = RateChangeType.Deleted,
                EffectiveOn = rateToClose.CloseEffectiveDate
            };
            if (rateToClose.RateTypeId.HasValue)
                SetCurrentOtherRateProperties(ratePreview, zoneRateGroup, ownerType);
            else
                SetCurrentNormalRateProperties(ratePreview, zoneRateGroup, ownerType);
            return ratePreview;
        }

        private void SetCurrentNormalRateProperties(RatePreview normalRatePreview, ZoneRateGroup zoneRateGroup, SalePriceListOwnerType ownerType)
        {
            if (zoneRateGroup != null && zoneRateGroup.NormalRate != null)
            {
                normalRatePreview.CurrentRate = zoneRateGroup.NormalRate.Rate;
                normalRatePreview.IsCurrentRateInherited = zoneRateGroup.NormalRate.Source != ownerType;
            }
        }
        private void SetCurrentOtherRateProperties(RatePreview otherRatePreview, ZoneRateGroup zoneRateGroup, SalePriceListOwnerType ownerType)
        {
            if (zoneRateGroup != null && zoneRateGroup.OtherRatesByType != null)
            {
                ZoneRate otherRate;
                zoneRateGroup.OtherRatesByType.TryGetValue(otherRatePreview.RateTypeId.Value, out otherRate);
                if (otherRate != null)
                {
                    otherRatePreview.CurrentRate = otherRate.Rate;
                    otherRatePreview.IsCurrentRateInherited = otherRate.Source != ownerType;
                }
            }
        }

        #endregion
    }
}
